#include <Commands/GearPlateToggle.h>
#include <Commands/BallPusherToggle.h>

#include "OI.h"
#include "RobotMap.h"
#include <WPILib.h>


OI::OI() {
	stick = new Joystick(STICK);
	// Process operator interface input here.

	JoystickButton* btn = new JoystickButton(stick, GEARTOGGLEBUTTON);
	btn->WhenPressed(new GearPlateToggle());
	btn = new JoystickButton(stick, FUALTOGGLEBUTTON);
	btn->WhenPressed(new BallPusherToggle());
}

Joystick* OI::GetJoystick() {
	return stick;
}
