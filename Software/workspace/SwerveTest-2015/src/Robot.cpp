
#include "Robot.h"
#include "Commands/Autonomous.h"
#include "Commands/Test.h"

DriveTrain* Robot::drivetrain = NULL;
Elevator* Robot::elevator = NULL;
Claw* Robot::claw = NULL;

OI* Robot::oi = NULL;

void Robot::RobotInit() {
	// Wheel Motor ids=1,2,3,4, Wheel encoder ids={1,2} {3,4} {5,6},{7,8}
	// Swerve drive only: Pivot motor ids=5,6,7,8, Pivot encoder ids={9,10} {11,12} {13,14},{15,16}
	drivetrain = new DriveTrain(1);
	drivetrain->SetDeadband(0.25,0.25,0.25);
	drivetrain->SetSquaredInputs(true);
	elevator = new Elevator(9); //motor id=9, encoder id={17,18}
	claw = new Claw();

	oi = new OI();

	autonomousCommand = new Autonomous();
	testCommand = new Test();

	lw = LiveWindow::GetInstance();

    // Show what command your subsystem is running on the SmartDashboard
    SmartDashboard::PutData(drivetrain);
    SmartDashboard::PutData(elevator);
    SmartDashboard::PutData(claw);
    drivetrain->Enable();
	drivetrain->ResetAll();
}

void Robot::DisabledInit() {
	std::cout << "Disabled Init" << std::endl;
	Robot::drivetrain->Log();
	Robot::drivetrain->ResetAll();
	//Robot::drivetrain->Disable();
}

void Robot::TestInit() {
	std::cout << "Starting Test" << std::endl;
}

void Robot::AutonomousInit() {
	drivetrain->ResetAll();
	drivetrain->Enable();
	elevator->SetElevatorLevel(0);
	autonomousCommand->Cancel();
	autonomousCommand->Start();
	std::cout << "Starting Auto" << std::endl;
}

void Robot::AutonomousPeriodic() {
	Scheduler::GetInstance()->Run();
}

void Robot::TeleopInit() {
	// This makes sure that the autonomous stops running when
	// teleop starts running. If you want the autonomous to
	// continue until interrupted by another command, remove
	// this line or comment it out.
	autonomousCommand->Cancel();
	//drivetrain->ResetWheels();
	drivetrain->ResetAll();
	drivetrain->Enable();

	claw->Open();
	elevator->SetElevatorLevel(0);
	std::cout << "Starting Teleop" << std::endl;
}

void Robot::TeleopPeriodic() {
	Scheduler::GetInstance()->Run();
}

void Robot::TestPeriodic() {
	autonomousCommand->Cancel();
	lw->Run();
}

START_ROBOT_CLASS(Robot);
