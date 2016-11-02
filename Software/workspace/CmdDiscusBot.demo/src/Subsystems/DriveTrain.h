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

  CANTalon *frontLeft;
  CANTalon *frontRight;
  CANTalon *backLeft;
  CANTalon *backRight;

public:
	DriveTrain();

	/**
	 * When no other command is running let the operator drive around
	 * using the PS3 joystick.
	 */
	void InitDefaultCommand();

	/**
	 * The log method puts interesting information to the SmartDashboard.
	 */
	void Log();

	/**
	 * Tank style driving for the DriveTrain.
	 * @param left Speed in range [-1,1]
	 * @param right Speed in range [-1,1]
	 */
	void Drive(double x, double y, double z);


};

#endif
