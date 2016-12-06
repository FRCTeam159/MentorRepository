#ifndef TankDriveWithJoystick_H
#define TankDriveWithJoystick_H

#include "Commands/Command.h"
#include "CommandBase.h"

/**
 * Have the robot drive tank style using the PS3 Joystick until interrupted.
 */
class      DriveWithJoystick: public CommandBase {
	float quadDeadband(float minThreshold, float minOutput, float input);

public:
	     DriveWithJoystick();
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
};

#endif
