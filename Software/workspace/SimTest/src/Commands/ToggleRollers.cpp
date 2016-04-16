/*
 * ToggleRollers.cpp
 *
 *  Created on: Mar 27, 2016
 *      Author: dean
 */

#include <Commands/ToggleRollers.h>
#include "Robot.h"

ToggleRollers::ToggleRollers()  : Command("ToggleRollers") {
	Requires(Robot::loader.get());
}

void ToggleRollers::Initialize() {
	initial_state=Robot::loader->RollersAreOn();
	if(initial_state==ROLLERS_ON){
		target_state=ROLLERS_OFF;
		Robot::loader->StopRollers();
		std::cout << "Rollers are currently On: Stopping ..."<< std::endl;
	}
	else{
		target_state=ROLLERS_ON;
		Robot::loader->SpinRollers(true);
		std::cout << "Rollers are currently Off: Starting ..."<< std::endl;
	}
}
bool ToggleRollers::IsFinished() {
	return (Robot::loader->RollersAreOn()==target_state);
}
void ToggleRollers::End() {
	if(target_state==ROLLERS_ON)
		std::cout << "ToggleRollers Rollers are On"<< std::endl;
	else
		std::cout << "ToggleRollers Rollers are Off"<< std::endl;

}

void ToggleRollers::Execute() {
	if(target_state==ROLLERS_ON)
		Robot::loader->SpinRollers(true);
}
