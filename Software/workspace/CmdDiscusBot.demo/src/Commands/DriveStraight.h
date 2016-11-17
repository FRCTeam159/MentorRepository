#ifndef DriveStraight_H
#define DriveStraight_H

#include "CommandBase.h"
#include "WPILib.h"

class DriveStraight: public CommandBase
{
	double duration;
	double speed;
	double stop_time;
public:
	DriveStraight(double t,double s);
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
};

#endif
