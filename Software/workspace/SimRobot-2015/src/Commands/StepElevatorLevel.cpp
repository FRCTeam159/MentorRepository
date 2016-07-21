#include <Commands/StepElevatorLevel.h>
#include "Robot.h"
#include <math.h>

StepElevatorLevel::StepElevatorLevel(double d) : Command("StepElevatorSetpoint") {
	direction=d;
	std::cout << "new StepElevatorLevel:"<< d <<std::endl;
	Requires(Robot::elevator);
}

// Called just before this Command runs the first time
void StepElevatorLevel::Initialize() {
	int current=Robot::elevator->GetElevatorLevel()+0.5;
	double target=current+direction;
	Robot::elevator->Disable();
	Robot::elevator->SetElevatorLevel(target);
	std::cout << "Changing Elevator SetPoint current:"<< current <<" new:"<<target<<std::endl;
	Robot::elevator->Enable();
}

// Called repeatedly when this Command is scheduled to run
void StepElevatorLevel::Execute() {
}

// Make this return true when this Command no longer needs to run execute()
bool StepElevatorLevel::IsFinished() {
	// note: in teleop sometimes will miss button presses while waiting to elevator to reach target
	//return IsTimedOut();
	return Robot::elevator->OnTarget();
}

// Called once after isFinished returns true
void StepElevatorLevel::End() {
	int lvl=Robot::elevator->GetElevatorLevel()+0.5;
	std::cout<< "StepElevatorLevel:OnTarget lvl:"<<lvl<<" pos:"<<Robot::elevator->GetDistance()<<std::endl;
}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void StepElevatorLevel::Interrupted() {
	std::cout << "SetElevatorSetpoint::Interrupted()"<< std::endl;
}
