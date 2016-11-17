#ifndef DriveWithJoystick_H
#define DriveWithJoystick_H

#include "Commands/Command.h"

/**
 * Have the robot drive using the PS3 Joystick until interrupted.
 */
class DriveWithJoystick: public Command {
public:
	DriveWithJoystick();
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
};

#endif
