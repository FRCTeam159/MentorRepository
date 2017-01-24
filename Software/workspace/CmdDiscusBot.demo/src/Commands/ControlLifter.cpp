/*
 * ControlElevator.cpp
 *
 *  Created on: Oct 28, 2016
 *      Author: dean
 */

#include <Commands/ControlLifter.h>
#include "RobotMap.h"

ControlLifter::ControlLifter() : CommandBase("ControlElevator") {
	Requires(lifter.get());
}

void ControlLifter::Initialize() {
}

void ControlLifter::Execute() {
	while(!lifter->FindZero()){
		return;
	}
	Joystick *stick=oi->GetJoystick();
	if (stick->GetRawButton(LIFTER_UP_BUTTON)) {
		lifter->SetSpeed(LIFTER_UP_SPEED);
	}
	else if (stick->GetRawButton(LIFTER_DOWN_BUTTON)) {
		lifter->SetSpeed(LIFTER_DOWN_SPEED);
	} else {
		lifter->SetSpeed(0);
	}
}

bool ControlLifter::IsFinished() {
	return false;
}

void ControlLifter::End() {
}
