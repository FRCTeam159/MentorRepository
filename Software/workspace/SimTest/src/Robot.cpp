
#include "Robot.h"
#include "Assignments.h"

std::shared_ptr<DriveTrain> Robot::drivetrain;
std::shared_ptr<Holder> Robot::holder;
std::shared_ptr<Shooter> Robot::shooter;
std::shared_ptr<Loader> Robot::loader;
std::shared_ptr<OI> Robot::oi;

std::unique_ptr<Autonomous> Robot::autonomous;


void Robot::RobotInit() {
	std::cout << "Robot::RobotInit" << std::endl;
	holder.reset(new Holder());
	loader.reset(new Loader());
	shooter.reset(new Shooter());
	drivetrain.reset(new DriveTrain());
	oi.reset(new OI());

	autonomous.reset(new Autonomous());

	//SmartDashboard::PutData(drivetrain.get());
	//SmartDashboard::PutData(holder.get());
	//SmartDashboard::PutData(shooter.get());
	OI::SetMode(SHOOTING);
}

void Robot::AutonomousInit() {
	std::cout << "Robot::AutonomousInit" << std::endl;
	drivetrain->AutonomousInit();
	shooter->AutonomousInit();
	holder->AutonomousInit();
	loader->AutonomousInit();

	autonomous->Start();
}

void Robot::AutonomousPeriodic() {
	Scheduler::GetInstance()->Run();
}

void Robot::DisabledInit(){
	std::cout << "Robot::DisabledInit" << std::endl;
	autonomous->Cancel();
	drivetrain->DisabledInit();
	shooter->DisabledInit();
	holder->DisabledInit();
	loader->DisabledInit();

}
void Robot::DisabledPeriodic(){
}

void Robot::TeleopInit() {
	std::cout << "Robot::TeleopInit" << std::endl;
	// This makes sure that the autonomous stops running when
	// teleop starts running. If you want the autonomous to
	// continue until interrupted by another command, remove
	// this line or comment it out.
	autonomous->Cancel();
	shooter->TeleopInit();
	holder->TeleopInit();
	drivetrain->TeleopInit();
	loader->TeleopInit();
}

void Robot::TeleopPeriodic() {
	// Scheduler runs "default" commands for all subsystems
	// e.g. void DriveTrain::InitDefaultCommand() {
	//	    SetDefaultCommand(new TankDriveWithJoystick()); }
	Scheduler::GetInstance()->Run();
}

void Robot::TestPeriodic() {
	lw->Run();
}

void Robot::SetMode(int m) {
	OI::SetMode(m);
	if(m==LOADING){
		std::cout << "Changing Mode to Loading"<<std::endl;
		loader->SpinRollers(true);
		loader->LoadBall();
		drivetrain->SetReverseDriving(true);
	}
	else{
		std::cout << "Changing Mode to Shooting"<<std::endl;
		drivetrain->SetReverseDriving(false);
	}
}

int Robot::GetMode() {
	return OI::GetMode();
}
START_ROBOT_CLASS(Robot);

