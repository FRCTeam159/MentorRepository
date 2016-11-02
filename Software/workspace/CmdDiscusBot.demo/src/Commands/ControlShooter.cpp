/*
 * ControlShooter.cpp
 *
 *  Created on: Oct 28, 2016
 *      Author: dean
 */

#include <Commands/ControlShooter.h>
#include "RobotMap.h"

ControlShooter::ControlShooter() : CommandBase("ControlShooter") {
	Requires(shooter.get());
}

void ControlShooter::Initialize() {
	flywheelBtn.reset();
}

void ControlShooter::Execute() {
	Joystick *stick=oi->GetJoystick();
	if(flywheelBtn.toggle(stick->GetRawButton(FLYWHEEL_BUTTON)))
		shooter->SetFlywheelSpeed(FLYWHEEL_SPEED);
	else
		shooter->SetFlywheelSpeed(0);
	if(stick->GetRawButton(SHOOTER_BUTTON))
		shooter->PistonIn();
	else
		shooter->PistonOut();
}

bool ControlShooter::IsFinished() {
	return false;
}

void ControlShooter::End() {
}
