#ifndef DriveWithJoystick_H
#define DriveWithJoystick_H

#include "../CommandBase.h"
#include "WPILib.h"

class DriveWithJoystick: public CommandBase
{
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
