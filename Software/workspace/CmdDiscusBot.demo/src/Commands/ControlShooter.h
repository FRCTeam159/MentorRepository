/*
 * ControlShooter.h
 *
 *  Created on: Oct 28, 2016
 *      Author: dean
 */

#ifndef SRC_COMMANDS_CONTROLSHOOTER_H_
#define SRC_COMMANDS_CONTROLSHOOTER_H_

#include <CommandBase.h>

class ControlShooter: public CommandBase {
	Toggle flywheelBtn;
public:
	ControlShooter();
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted() {End();}
};

#endif /* SRC_COMMANDS_CONTROLSHOOTER_H_ */
