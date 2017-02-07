#include "OI.h"
#include "RobotMap.h"
#include "Commands/ToggleGearPlate.h"

#include <WPILib.h>


OI::OI() {
	stick = new Joystick(STICK);
	// Process operator interface input here.

	JoystickButton* d_up = new JoystickButton(stick, 2);
	d_up->WhenPressed(new ToggleGearPlate());
}

Joystick* OI::GetJoystick() {
	return stick;
}
