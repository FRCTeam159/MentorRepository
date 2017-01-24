
#include "Robot.h"
#include "CommandBase.h"
#include "Commands/Autonomous.h"


void Robot::RobotInit() {
	CommandBase::init();
	autonomousCommand = new Autonomous();
}

void Robot::AutonomousInit() {
	std::cout << "Starting Auto" << std::endl;
	autonomousCommand->Start();
}

void Robot::AutonomousPeriodic() {
	Scheduler::GetInstance()->Run();
}

void Robot::TeleopInit() {
	autonomousCommand->Cancel();
	std::cout << "Starting Teleop" << std::endl;
}

void Robot::TeleopPeriodic() {
	Scheduler::GetInstance()->Run();
}

void Robot::TestPeriodic() {
	lw->Run();
}

void Robot::DisabledInit(){
	std::cout << "DisabledInit" << std::endl;
	autonomousCommand->Cancel();
	CommandBase::drivetrain.get()->Reset();
	CommandBase::lifter.get()->Reset();

}

START_ROBOT_CLASS(Robot)
