/*
 * AutoTest.cpp
 *
 *  Created on: Oct 30, 2016
 *      Author: dean
 */

#include <Commands/Autonomous.h>
#include <Commands/AutoDrive.h>

const double DIRECTION=1.0;
const double SPEED=1.0;
const double TIME=3.0;

Autonomous::Autonomous() : CommandGroup("Autonomous"){
	AddSequential(new AutoDrive(DIRECTION,SPEED,TIME));
}

void Autonomous::Interrupted() {
	std::cout << "Autonomous::Interruped"<<std::endl;
	End();
}

void Autonomous::Cancel() {
	std::cout << "Autonomous::Cancel"<<std::endl;
	_End();
	CommandGroup::Cancel();
}
