/*
 * OI.cpp
 *
 *  Created on: Jun 3, 2014
 *      Author: alex
 */

#include "OI.h"
#include "RobotMap.h"

#define GAMEPAD  1
#define JOYSTICK 2
//#define DEVICE JOYSTICK
#ifndef DEVICE
#define DEVICE GAMEPAD
#endif

OI::OI() {
	stick= new Joystick(0);
}


Joystick* OI::GetJoystick() {
	return stick;
}

float OI::GetX() {
#if DEVICE == JOYSTICK
	return stick->GetX();
#else
	return -stick->GetRawAxis(1); // right knob range=-1.. +1 (off=0)
#endif
}

float OI::GetY() {
#if DEVICE == JOYSTICK
	return stick->GetY();
#else
	return -stick->GetRawAxis(4); // left knob range=-1.. +1 (off=0)
#endif

}

float OI::GetZ() {
#if DEVICE == JOYSTICK
	return stick->GetZ(); // returns "twist" of 3d joystick
#else
	// On the x-box gamepad the two "triggers" map to axis 2 (left) and 5 (right)
	// values range from -1 (off) to +1 (fully pressed)
	// mapped behavior uses left trigger to spin left and right trigger to spin right
	double ccw=0.25*(stick->GetRawAxis(2)+1.0);  // 0 .. 0.5
	double cw=0.25*(stick->GetRawAxis(5)+1.0);   // 0 .. 0.5
	return ccw-cw;
#endif

}
