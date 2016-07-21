/*
 * ToggleClaw.cpp
 *
 *  Created on: Feb 4, 2015
 */

#include "ToggleClaw.h"
#include "Robot.h"

ToggleClaw::ToggleClaw() : Command("ToggleClaw") {
	Requires(Robot::claw);
	std::cout << "new ToggleClaw"<< std::endl;
}

// Called just before this Command runs the first time
void ToggleClaw::Initialize() {
	if (Robot::claw->IsOpen())  //If the clawTrigger button is pressed, close the pincers.
	{
		Robot::claw->Close();
	} else  //Otherwise, keep claws open.
	{
		Robot::claw->Open();
	}
	std::cout << "ToggleClaw Open="<< Robot::claw->IsOpen()<< std::endl;
}

// Called repeatedly when this Command is scheduled to run
void ToggleClaw::Execute() {}

// Make this return true when this Command no longer needs to run execute()
bool ToggleClaw::IsFinished() {
	return true;
}

// Called once after isFinished returns true
void ToggleClaw::End() {
	// NOTE: Doesn't stop in simulation due to lower friction causing the can to fall out
	// + there is no need to worry about stalling the motor or crushing the can.
	#ifdef REAL
		Robot::claw->Stop();
	#endif
}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void ToggleClaw::Interrupted() {
	End();
}


