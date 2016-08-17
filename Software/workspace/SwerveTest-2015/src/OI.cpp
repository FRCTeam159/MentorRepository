/*
 * OI.cpp
 *
 *  Created on: Jun 3, 2014
 *      Author: alex
 */

#include <Commands/StepElevatorLevel.h>
#include "OI.h"

#include "Commands/OpenClaw.h"
#include "Commands/CloseClaw.h"
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

    // Connect the buttons to commands
    // NOTE:
    d_up->WhenPressed(new StepElevatorLevel(1));
    d_down->WhenPressed(new StepElevatorLevel(-1));

    //d_right->WhenPressed(new CloseClaw());
    //d_left->WhenPressed(new ToggleRotate());
    d_right->ToggleWhenPressed(new ToggleClaw());

}


Joystick* OI::GetJoystick() {
	return joy;
}
