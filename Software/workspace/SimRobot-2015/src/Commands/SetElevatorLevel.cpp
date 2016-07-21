#include <Commands/SetElevatorLevel.h>
#include "Robot.h"
#include <math.h>

SetElevatorLevel::SetElevatorLevel(int level) : Command("SetElevatorSetpoint") {
	this->_level = level;
	Requires(Robot::elevator);
	//SetTimeout(1.5);
}

// Called just before this Command runs the first time
void SetElevatorLevel::Initialize() {
	Robot::elevator->SetElevatorLevel(_level);
	Robot::elevator->Enable();
}

// Called repeatedly when this Command is scheduled to run
void SetElevatorLevel::Execute() {}

// Make this return true when this Command no longer needs to run execute()
bool SetElevatorLevel::IsFinished() {
	return Robot::elevator->OnTarget();
}

// Called once after isFinished returns true
void SetElevatorLevel::End() {
	int lvl=Robot::elevator->GetElevatorLevel()+0.5;
	std::cout<< "SetElevatorLevel:OnTarget:"<<lvl<<std::endl;
}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void SetElevatorLevel::Interrupted() {}
