/*
 * SmartDrive.h
 *
 *  Created on: Jul 6, 2015
 *      Author: alpiner
 *      extends functionality of built in class "RobotDrive"
 */

#ifndef SRC_SUBSYSTEMS_SMARTDRIVE_H_
#define SRC_SUBSYSTEMS_SMARTDRIVE_H_

#include "RobotDrive.h"
#include "Subsystems/SmartMotor.h"

class SmartDrive : public RobotDrive
{
public:
	SmartDrive(SpeedController *frontLeftMotor, SpeedController *rearLeftMotor,
			SpeedController *frontRightMotor, SpeedController *rearRightMotor);
};

#endif /* SRC_SUBSYSTEMS_SMARTDRIVE_H_ */
