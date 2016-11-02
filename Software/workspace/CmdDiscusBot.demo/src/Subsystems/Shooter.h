/*
 * Shooter.h
 *
 *  Created on: Oct 28, 2016
 *      Author: dean
 */

#ifndef SRC_SUBSYSTEMS_SHOOTER_H_
#define SRC_SUBSYSTEMS_SHOOTER_H_

#include "WPILib.h"

#include <Commands/Subsystem.h>

class Shooter: public Subsystem {
	CANTalon *flywheel;
	DoubleSolenoid *piston;
	bool flywheel_on=false;
	bool piston_in=false;
	float flywheel_speed;
public:
	Shooter();
	void InitDefaultCommand();
	void PistonIn();
	void PistonOut();
	void SetFlywheelSpeed(float s);
	bool IsPistonIn() { return piston_in;}
	float GetFlywheelSpeed() { return flywheel_speed;}
};

#endif /* SRC_SUBSYSTEMS_SHOOTER_H_ */
