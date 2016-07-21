/*
 * SmartDrive.cpp
 *
 *  Created on: Jul 6, 2015
 *      Author: alpiner
 */


#include "SmartDrive.h"

SmartDrive::SmartDrive(SpeedController *FL, SpeedController *RL, SpeedController *FR, SpeedController *RR) :
		RobotDrive(FL,RL,FR,RR)
{

}

