#ifndef DriveToPosition_H
#define DriveToPosition_H

#include "../CommandBase.h"
#include "WPILib.h"

class DriveToPosition: public CommandBase
{
	double position;
	double speed;
public:
	DriveToPosition(double p, double s);
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
};

#endif
