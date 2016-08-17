#ifndef PivotWheel_H
#define PivotWheel_H

#include "Commands/PIDSubsystem.h"
#include "WPILib.h"

#include "SmartMotor.h"
/**
 * The PivotWheel subsystem contains pivot and drive motors and the pot for PID control
 */
class PivotWheel: public SmartMotor
{
public:

private:
	// Subsystem devices
	//DigitalInput* upperLimitSwitch;
	//DigitalInput* lowerLimitSwitch;
	SmartMotor *pivot;
	double angle;
	int wchnl,schnl;

public:
	PivotWheel(int,int);


	/**
	 *  No default command, if PID is enabled, the current setpoint will be maintained.
	 */
	void InitDefaultCommand() {}

	/**
	 * @return The angle read in by the potentiometer
	 */
	double ReturnPIDInput();

	/**
	 * Set the motor speed based off of the PID output
	 */
	void UsePIDOutput(double output);

	/**
	 * @return If the pivot is at its upper limit.
	 */
	//bool IsAtUpperLimit();

	/**
	 * @return If the pivot is at its lower limit.
	 */
	//bool IsAtLowerLimit();

	/**
	 * @return The current angle of the pivot.
	 */
	double GetAngle();
	void SetAngle(double value);
	void SetMotorSpeed(double value);
	void Disable();
	void Enable();
	void Reset();
};

#endif
