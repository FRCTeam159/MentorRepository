
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
	std::cout << "Starting Teleop" << std::endl;
}

void Robot::TeleopPeriodic() {
	autonomousCommand->Cancel();
	Scheduler::GetInstance()->Run();
}

void Robot::TestPeriodic() {
	lw->Run();
}

void Robot::DisabledInit(){
	std::cout << "DisabledInit" << std::endl;
	CommandBase::drivetrain.get()->Reset();
}

START_ROBOT_CLASS(Robot)
