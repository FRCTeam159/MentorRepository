/*
 * Pause.h
 *
 *  Created on: Jul 23, 2015
 *      Author: alpiner
 */

#ifndef SRC_COMMANDS_PAUSE_H_
#define SRC_COMMANDS_PAUSE_H_

#include "Commands/Command.h"

class Pause: public Command {
public:
	Pause(double timeout);
	bool IsFinished();
	void Initialize();
	void Execute();
	void End();
	void Interrupted();
};

#endif /* SRC_COMMANDS_PAUSE_H_ */
