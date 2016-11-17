/*
 * ToggleClaw.h
 *
 *  Created on: Feb 4, 2015
 *      Author: dean
 */

#ifndef SRC_COMMANDS_TOGGLECLAW_H_
#define SRC_COMMANDS_TOGGLECLAW_H_

#include "Commands/Command.h"

/**
 * Toggle the claw between Open (default) and Closed state
 */
class ToggleClaw: public Command {
bool clawOpen;
public:
	ToggleClaw();
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
};




#endif /* SRC_COMMANDS_TOGGLECLAW_H_ */
