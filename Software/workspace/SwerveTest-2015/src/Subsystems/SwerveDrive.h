/*
 * SwerveDrive.h
 *
 *  Created on: Jun 24, 2015
 *      Author: alpiner
 */

#ifndef SRC_SUBSYSTEMS_SWERVEDRIVE_H_
#define SRC_SUBSYSTEMS_SWERVEDRIVE_H_

#include "PivotWheel.h"

class SwerveDrive : public RobotDrive
{
	PivotWheel *m_frontLeftWheel;
	PivotWheel *m_frontRightWheel;
	PivotWheel *m_rearLeftWheel;
	PivotWheel *m_rearRightWheel;

	double angle;
	bool squared_inputs;

public:
	SwerveDrive(SpeedController *frontLeftMotor, SpeedController *rearLeftMotor,
				SpeedController *frontRightMotor, SpeedController *rearRightMotor);
	void PivotDrive(float wheelspeed, float angle, float rotation, float gyro);
	void DriveAtAngle(float wheelspeed, float angle);
	void Drive(float wheelspeed);
	void SetAngle(float angle);
	void Rotate(float speed,float angle);
	void Rotate(float speed);
	void SetSquaredInputs(bool b) { squared_inputs=b;}
	void Enable();
	void Disable();
	void SetDriveStraight();
	void SetDriveSideways();
	double GetDistance();
};



#endif /* SRC_SUBSYSTEMS_SWERVEDRIVE_H_ */
