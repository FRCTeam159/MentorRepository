/*
 * ControlElevator.h
 *
 *  Created on: Oct 28, 2016
 *      Author: dean
 */

#ifndef SRC_COMMANDS_CONTROLLIFTER_H_
#define SRC_COMMANDS_CONTROLLIFTER_H_

#include <CommandBase.h>

class ControlLifter: public CommandBase {
public:
	ControlLifter();
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted() {End();}

};

#endif /* SRC_COMMANDS_CONTROLLIFTER_H_ */
