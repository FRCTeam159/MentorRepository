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
	SpeedController *frontLeft,  *backLeft,
					*frontRight, *backRight;
	RobotDrive* drive;
	Encoder *left_encoder, *right_encoder;
	AnalogInput* rangefinder;
	Gyro* gyro;

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
	void Drive(double x, double y, double z, double g);

	/**
	 * @param joy The ps3 style joystick to use to drive tank style.
	 */
	void Drive(Joystick* joy);

	/**
	 * @return The robots heading in degrees.
	 */
	double GetHeading();

	/**
	 * Reset the robots sensors to the zero states.
	 */
	void Reset();

	/**
	 * @return The distance driven (average of left and right encoders).
	 */
	double GetDistance();

	/**
	 * @return The distance to the obstacle detected by the rangefinder.
	 */
	double GetDistanceToObstacle();

	double GetRotation();

};

#endif
