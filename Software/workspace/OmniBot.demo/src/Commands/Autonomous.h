/*
 * AutoTest.h
 *
 *  Created on: Oct 30, 2016
 *      Author: dean
 */

#ifndef SRC_COMMANDS_AUTONOMOUS_H_
#define SRC_COMMANDS_AUTONOMOUS_H_

#include <CommandBase.h>

class Autonomous: public CommandGroup {
public:
	Autonomous();
	void Interrupted();
	void Cancel();
};

#endif /* SRC_COMMANDS_AUTONOMOUS_H_ */
