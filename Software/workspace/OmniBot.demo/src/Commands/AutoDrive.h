/*
 * AutoDrive.h
 *
 *  Created on: Oct 30, 2016
 *      Author: dean
 */

#ifndef SRC_COMMANDS_AUTODRIVE_H_
#define SRC_COMMANDS_AUTODRIVE_H_

#include <CommandBase.h>

class AutoDrive: public CommandBase {
	double direction;
	double speed;
	double duration;
	double stop_time;
	double start_time;
public:
	AutoDrive(double d, double s, double t);
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
};

#endif /* SRC_COMMANDS_AUTODRIVE_H_ */
