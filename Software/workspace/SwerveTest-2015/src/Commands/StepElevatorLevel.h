#ifndef StepElevatorSetpoint_H
#define StepElevatorSetpoint_H

#include "Commands/Command.h"

/**
 * Move the elevator to a given location. This command finishes when it is within
 * the tolerance, but leaves the PID loop running to maintain the position. Other
 * commands using the elevator should make sure they disable PID!
 */
class StepElevatorLevel: public Command {
private:
	double direction;
public:
	StepElevatorLevel(double setpoint);
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
};

#endif
