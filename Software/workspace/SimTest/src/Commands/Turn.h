/*
 * Turn.h
 *
 *  Created on: Mar 13, 2016
 *      Author: dean
 */

#ifndef SRC_COMMANDS_TURN_H_
#define SRC_COMMANDS_TURN_H_

#include "Subsystems/GPMotor.h"
#include <Commands/Command.h>

class Turn: public Command, public PIDSource, public PIDOutput{
	PIDController pid;
	double target;
public:
	Turn(double a);
	void Initialize();
	bool IsFinished();
	void Execute() {}
	void End();
	void Interrupted() { End();}

	double PIDGet();
	void PIDWrite(float d);

};

#endif /* SRC_COMMANDS_TURN_H_ */
