#include <Commands/GearPlateToggle.h>

#include "OI.h"
#include "RobotMap.h"
#include <WPILib.h>


OI::OI() {
	stick = new Joystick(STICK);
	// Process operator interface input here.

	JoystickButton* btn = new JoystickButton(stick, GEARTOGGLEBUTTON);
	btn->WhenPressed(new GearPlateToggle());
}

Joystick* OI::GetJoystick() {
	return stick;
}
