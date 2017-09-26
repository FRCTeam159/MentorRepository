
#include "OI.h"
#include "RobotMap.h"
#include <WPILib.h>


OI::OI() {
	stick = new Joystick(0);

}

Joystick* OI::GetJoystick() {
	return stick;
}
