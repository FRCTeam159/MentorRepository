#ifndef DriveTrain_H
#define DriveTrain_H

#include "WPILib.h"

/**
 * The DriveTrain subsystem incorporates the sensors and actuators attached to
 * the robots chassis. These include four drive motors, a left and right encoder
 * and a gyro.
 */
class DriveTrain : public Subsystem {
private:
  RobotDrive* drive;

  CANTalon frontLeft;
  CANTalon frontRight;
  CANTalon backLeft;
  CANTalon backRight;

  AnalogGyro gyro;

public:
	DriveTrain();

	/**
	 * When no other command is running let the operator drive around
	 * using the PS3 joystick.
	 */
	void InitDefaultCommand();

	void Log();

	void Drive(double x, double y, double z);
	double GetHeading();
	double GetPosition();
	double GetLeftPosition();
	double GetRightPosition();
	void Reset();

};

#endif
