#include "DriveWithJoystick.h"

DriveWithJoystick::DriveWithJoystick() : CommandBase("DriveWithJoystick") {
	Requires(drivetrain.get());
}

// Called just before this Command runs the first time
void DriveWithJoystick::Initialize()
{

}

// Called repeatedly when this Command is scheduled to run
void DriveWithJoystick::Execute()
{
	double x=quadDeadband(0.3, 0.3, oi->GetX());
	double y=quadDeadband(0.3, 0.3, oi->GetY());
	double spin=quadDeadband(0.3, 0.3, oi->GetZ()); // spin
	double n=x+spin;
	double s=-x+spin;
	double e=y+spin;
	double w=-y+spin;
	drivetrain->Drive(n,s,e,w);
}

// Make this return true when this Command no longer needs to run execute()
bool DriveWithJoystick::IsFinished()
{
	return false;
}

// Called once after isFinished returns true
void DriveWithJoystick::End()
{

}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void DriveWithJoystick::Interrupted()
{

}
float DriveWithJoystick::quadDeadband(float minThreshold, float minOutput, float input) {
	if (input > minThreshold) {
		return ((((1 - minOutput)
				/ ((1 - minThreshold) * (1 - minThreshold)))
				* ((input - minThreshold) * (input - minThreshold)))
				+ minOutput);
	} else {
		if (input < (-1 * minThreshold)) {
			return (((minOutput - 1)
					/ ((minThreshold - 1) * (minThreshold - 1)))
					* ((minThreshold + input) * (minThreshold + input)))
					- minOutput;
		}
		else {
			return 0;
		}
	}
}

