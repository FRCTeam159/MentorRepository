#include "SetElevatorSetpoint.h"
#include "Robot.h"
#include <math.h>

SetLifterPosition::SetLifterPosition(double setpoint) : Command("SetElevatorSetpoint") {
	this->setpoint = setpoint;
	Requires(Robot::elevator);
}

// Called just before this Command runs the first time
void SetLifterPosition::Initialize() {
	Robot::elevator->SetSetpoint(setpoint);
	std::cout << "SetElevatorSetpoint:"<< this->setpoint <<std::endl;

	Robot::elevator->Enable();
}

// Called repeatedly when this Command is scheduled to run
void SetLifterPosition::Execute() {}

// Make this return true when this Command no longer needs to run execute()
bool SetLifterPosition::IsFinished() {
	return Robot::elevator->OnTarget();
}

// Called once after isFinished returns true
void SetLifterPosition::End() {}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void SetLifterPosition::Interrupted() {}
