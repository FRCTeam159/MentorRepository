#ifndef Rotate_H
#define Rotate_H

#include "WPILib.h"
#include "Commands/Command.h"

/**
 * Drive the given distance straight (negative values go backwards).
 * Uses a local PID controller to run a simple PID loop that is only
 * enabled while this command is running. The input is the averaged
 * values of the left and right encoders.
 */
class Rotate: public Command {
	double angle;
public:
	Rotate(double distance);
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
private:
	PIDController* pid;
};

class RotatePIDSource: public PIDSource {
public:
	virtual ~RotatePIDSource();
	double PIDGet();
};

class RotatePIDOutput: public PIDOutput {
public:
	virtual ~RotatePIDOutput();
	void PIDWrite(float d);
};

#endif
