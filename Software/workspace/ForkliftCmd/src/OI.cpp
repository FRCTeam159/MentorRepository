/*
 * OI.cpp
 *
 *  Created on: Jun 3, 2014
 *      Author: alex
 */

#include "OI.h"

#include "Commands/SetElevatorSetpoint.h"
#include "Commands/OpenClaw.h"
#include "Commands/CloseClaw.h"
#include "Commands/PrepareToPickup.h"
#include "Commands/Pickup.h"
#include "Commands/Place.h"
#include "Commands/Autonomous.h"
#include "Commands/ToggleClaw.h"

OI::OI() {
	//SmartDashboard::PutData("Open Claw", new OpenClaw());
	//SmartDashboard::PutData("Close Claw", new CloseClaw());

	joy= new Joystick(0);


    // Create some buttons
    JoystickButton* d_up = new JoystickButton(joy, 2);
    JoystickButton* d_down= new JoystickButton(joy, 3);
    JoystickButton* d_right= new JoystickButton(joy, 1);
    //JoystickButton* d_left = new JoystickButton(joy, 4);

//    JoystickButton* l2 = new JoystickButton(joy, 9);
//    JoystickButton* r2 = new JoystickButton(joy, 10);
//    JoystickButton* l1 = new JoystickButton(joy, 11);
//    JoystickButton* r1 = new JoystickButton(joy, 12);

    // Connect the buttons to commands
    d_up->WhenPressed(new SetElevatorSetpoint(0.8));
    d_down->WhenPressed(new SetElevatorSetpoint(-1.0));

    //d_right->WhenPressed(new CloseClaw());
    //d_left->WhenPressed(new OpenClaw());

    d_right->ToggleWhenPressed(new ToggleClaw());

//    r1->WhenPressed(new PrepareToPickup());
//    r2->WhenPressed(new Pickup());
//    l1->WhenPressed(new Place());
//    l2->WhenPressed(new Autonomous());
}


Joystick* OI::GetJoystick() {
	return joy;
}
