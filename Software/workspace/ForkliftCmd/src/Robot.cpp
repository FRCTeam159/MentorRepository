
#include "Robot.h"
#include "Commands/Autonomous.h"
#include "Commands/Test.h"

DriveTrain* Robot::drivetrain = NULL;
Elevator* Robot::elevator = NULL;
Claw* Robot::claw = NULL;

OI* Robot::oi = NULL;

void Robot::RobotInit() {

	drivetrain = new DriveTrain();
	elevator = new Elevator();
	claw = new Claw();

	oi = new OI();

	autonomousCommand = new Autonomous();
	testCommand = new Test();

	lw = LiveWindow::GetInstance();

    // Show what command your subsystem is running on the SmartDashboard
    SmartDashboard::PutData(drivetrain);
    SmartDashboard::PutData(elevator);
    SmartDashboard::PutData(claw);
}

void Robot::DisabledInit() {
	std::cout << "Disabled Init" << std::endl;
	Robot::drivetrain->Log();
}

void Robot::TestInit() {
	std::cout << "Starting Test" << std::endl;
}


void Robot::AutonomousInit() {
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
