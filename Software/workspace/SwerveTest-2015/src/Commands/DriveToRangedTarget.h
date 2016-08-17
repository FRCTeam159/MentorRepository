#ifndef SetDistanceToBox_H
#define SetDistanceToBox_H

#include "WPILib.h"
#include "Commands/Command.h"

/**
 * Drive until the robot is the given distance away from the box. Uses a local
 * PID controller to run a simple PID loop that is only enabled while this
 * command is running. The input is the averaged values of the left and right
 * encoders.
 */
class DriveToRangedTarget: public Command {
public:
	const double pc_tol=5.0;

	double target;
	DriveToRangedTarget(double distance);
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
private:
	PIDController* pid;
};

class DriveTrainPIDSource: public PIDSource {
public:
	virtual ~DriveTrainPIDSource();
	double PIDGet();
};

class DriveTrainPIDOutput: public PIDOutput {
public:
	virtual ~DriveTrainPIDOutput();
	void PIDWrite(float d);
};

#endif
