/*
 * Loader.h
 *
 *  Created on: Mar 21, 2016
 *      Author: alpiner
 */

#ifndef SRC_SUBSYSTEMS_LOADER_H_
#define SRC_SUBSYSTEMS_LOADER_H_

#include "WPILib.h"
#include "Subsystems/GPMotor.h"

class Loader: public Subsystem, public PIDSource {
	GPMotor liftMotor;
	GPMotor rollerMotor;
	AnalogGyro accel;
	DigitalInput lowerLimit;

	double angle=0;
	double max_angle=80;
	double min_angle=0;
	double roller_speed=0;

	bool rollers_on=false;
	bool initialized=false;
	bool loading=false;
	bool cancelling=false;

	void Init();
	void Disable();

	double PIDGet();
	void InitDefaultCommand();
public:
	Loader();
	void Log();

	void SetLifterAngle(double a);
	double GetLifterAngle();
	bool LifterIsAtTargetAngle();
	bool LifterAtLowerLimit();

	void SpinRollers(bool b);
	void StopRollers();
	bool RollersAreOn();

	void AutonomousInit();
	void TeleopInit();
	void DisabledInit();

	void GoToZeroLimitSwitch();
	void SetLow();
	void LoadBall();
	bool IsInitialized();
	void SetInitialized();
	void Initialize();
	void Execute();
	void CancelLoad();
	void ExecLoad();

	bool Loading();
	void SetLoading(bool b);

};

#endif /* SRC_SUBSYSTEMS_LOADER_H_ */
