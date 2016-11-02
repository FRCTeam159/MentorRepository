#include <Commands/DriveWithJoystick.h>
#include "Robot.h"

DriveWithJoystick::DriveWithJoystick() : CommandBase("DriveWithJoystick") {
	Requires(drivetrain.get());
}

// Called just before this Command runs the first time
void DriveWithJoystick::Initialize() {

}

// Called repeatedly when this Command is scheduled to run
void      DriveWithJoystick::Execute() {
	Joystick *stick=oi->GetJoystick();
#ifdef REAL
		double x = quadDeadband(0.6, 0.3, stick-> GetX());
		double y = quadDeadband(0.6, 0.3, stick-> GetY());
		double z = quadDeadband(0.6, 0.3, stick-> GetZ());
#else
		double x = quadDeadband(0.6, 0.3, -stick->GetRawAxis(1));
		double y = quadDeadband(0.6, 0.3, -stick->GetRawAxis(4));
		double z=0;
#endif
		drivetrain->Drive(x, y, z);
}

// Make this return true when this Command no longer needs to run execute()
bool      DriveWithJoystick::IsFinished() {
	return false;
}

// Called once after isFinished returns true
void      DriveWithJoystick::End() {
	drivetrain->Drive(0, 0, 0);
}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void      DriveWithJoystick::Interrupted() {
	End();
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


