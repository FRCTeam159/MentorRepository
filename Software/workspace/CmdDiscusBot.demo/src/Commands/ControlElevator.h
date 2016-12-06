/*
 * ControlElevator.h
 *
 *  Created on: Oct 28, 2016
 *      Author: dean
 */

#ifndef SRC_COMMANDS_CONTROLELEVATOR_H_
#define SRC_COMMANDS_CONTROLELEVATOR_H_

#include <CommandBase.h>

class ControlElevator: public CommandBase {
public:
	ControlElevator();
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted() {End();}

};

#endif /* SRC_COMMANDS_CONTROLELEVATOR_H_ */
