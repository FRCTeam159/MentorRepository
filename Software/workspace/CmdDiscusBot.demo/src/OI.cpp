/*
 * OI.cpp
 *
 *  Created on: Jun 3, 2014
 *      Author: alex
 */


#include "OI.h"
#include "RobotMap.h"


OI::OI() {

	joy= new Joystick(0);

}


Joystick* OI::GetJoystick() {
	return joy;
}
