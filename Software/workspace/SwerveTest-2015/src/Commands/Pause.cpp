/*
 * Pause.cpp
 *
 *  Created on: Jul 23, 2015
 *      Author: alpiner
 */

#include "Pause.h"

Pause::Pause(double timeout) : Command("Pause") {
	SetTimeout(timeout);
}

// Make this return true when this Command no longer needs to run execute()
bool Pause::IsFinished() {
	return IsTimedOut();
}
void Pause::Initialize() {}
void Pause::Execute(){}
void Pause::End(){}
void Pause::Interrupted(){}


