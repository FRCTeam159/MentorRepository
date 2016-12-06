/*
 * ControlElevator.cpp
 *
 *  Created on: Oct 28, 2016
 *      Author: dean
 */

#include <Commands/ControlElevator.h>
#include "RobotMap.h"

ControlElevator::ControlElevator() : CommandBase("ControlElevator") {
	Requires(elevator.get());
}

void ControlElevator::Initialize() {
}

void ControlElevator::Execute() {
	Joystick *stick=oi->GetJoystick();
	if (stick->GetRawButton(LIFTER_UP_BUTTON)) {
		elevator->SetSpeed(LIFTER_UP_SPEED);
	}
	else if (stick->GetRawButton(LIFTER_DOWN_BUTTON)) {
		elevator->SetSpeed(LIFTER_DOWN_SPEED);
	} else {
		elevator->SetSpeed(0);
	}
}

bool ControlElevator::IsFinished() {
	return false;
}

void ControlElevator::End() {
}
