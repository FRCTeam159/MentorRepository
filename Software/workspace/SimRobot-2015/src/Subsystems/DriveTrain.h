#ifndef DriveTrain_H
#define DriveTrain_H
#include "WPILib.h"
#include "Subsystems/SmartMotor.h"
#include "Subsystems/SmartDrive.h"
#include "Subsystems/SwerveDrive.h"

#define MECHANUM 1
#define TANK     2
#define ARCADE   3
#define SWERVE   4

#define DRIVETYPE SWERVE

/**
 * The DriveTrain subsystem incorporates the sensors and actuators attached to
 * the robots chassis. These include four drive motors, a left and right encoder
 * and a gyro.
 */
class DriveTrain : public Subsystem {
private:
#if DRIVETYPE == SWERVE
	SwerveDrive* drive;
#else
	RobotDrive* drive;
#endif
	SmartMotor *frontLeft,  *backLeft, *frontRight, *backRight;
	AnalogInput* rangefinder;
	AnalogGyro* gyro;
	double ftpertick,distance_traveled;
	double x_deadband,y_deadband,z_deadband;
	bool squared_inputs;
	bool field_centric;
public:
	DriveTrain(int id);

	/**
	 * When no other command is running let the operator drive around
	 * using the PS3 joystick.
	 */
	virtual void InitDefaultCommand();
	void SetFtPerTick(double wheel_diam, double encoder_ticks);
	double GetFtperTick() { return ftpertick;}

	/**
	 * The log method puts interesting information to the SmartDashboard.
	 */
	void Log();

	virtual void Drive(double x, double y, double z, double g);

	/**
	 * @param joy The ps3 style joystick to use to drive tank style.
	 */
	virtual void Drive(Joystick* joy);

	/**
	 * @return The robots heading in degrees.
	 */
	virtual double GetHeading();

	/**
	 * Reset the robots sensors to the zero states.
	 */
	virtual void ResetWheels();
	virtual void ResetGyro();
	virtual void ResetTotalDistance();
	virtual void ResetAll();

	virtual void Enable();
	virtual void Disable();
	virtual void Stop();
	virtual void Rotate(double angle);
	virtual void DriveStraight(double distance);

	/**
	 * @return The distance driven (average of encoders).
	 */
	virtual double GetDistance();
	virtual double GetTotalDistance();

	/**
	 * @return The distance to the obstacle detected by the rangefinder.
	 */
	virtual double GetDistanceToObstacle();

	virtual double GetRotation();

	virtual double Deadband(double x, double ignore) { return fabs(x)>=ignore ? x: 0.0;}
	virtual void SetDeadband(double x, double y, double z) { x_deadband=x;y_deadband=y;z_deadband=z;}
	virtual void SetSquaredInputs(bool b) {squared_inputs=b;}
	virtual double SquareValue(double v){ return v<0? -v*v:v*v;}
	virtual void SetFieldCentric(bool b) {field_centric=b;}
};

#endif
