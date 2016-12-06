#ifndef DriveTrain_H
#define DriveTrain_H

#include "Commands/Subsystem.h"
#include "WPILib.h"

class DriveTrain: public Subsystem
{
private:
	// It's desirable that everything possible under private except
	// for methods that implement subsystem capabilities
	CANTalon northMotor;
	CANTalon southMotor;
	CANTalon eastMotor;
	CANTalon westMotor;

public:
	DriveTrain();
	void InitDefaultCommand();
	void Drive(double east, double west, double north, double south);
};

#endif
