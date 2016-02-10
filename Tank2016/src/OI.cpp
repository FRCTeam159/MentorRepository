/*
 * OI.cpp
 *
 *  Created on: Jun 3, 2014
 *      Author: alex
 */

#include "OI.h"

OI::OI() {

	joy= new Joystick(0);


    // Create some buttons

//    JoystickButton* d_up = new JoystickButton(joy, 2);
//    JoystickButton* d_down= new JoystickButton(joy, 3);
//    JoystickButton* d_right= new JoystickButton(joy, 1);
//    JoystickButton* d_left = new JoystickButton(joy, 4);
//
//
//    JoystickButton* l2 = new JoystickButton(joy, 9);
//    JoystickButton* r2 = new JoystickButton(joy, 10);
//    JoystickButton* l1 = new JoystickButton(joy, 11);
//    JoystickButton* r1 = new JoystickButton(joy, 12);

}


Joystick* OI::GetJoystick() {
	return joy;
}
