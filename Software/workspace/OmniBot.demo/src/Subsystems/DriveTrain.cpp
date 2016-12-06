#include "DriveTrain.h"
#include "RobotMap.h"
#include "Commands/DriveWithJoystick.h"

DriveTrain::DriveTrain() : Subsystem("DriveTrain"),
	northMotor(NORTH_MOTOR),
	southMotor(SOUTH_MOTOR),
	eastMotor(EAST_MOTOR),
	westMotor(WEST_MOTOR)
{
	//northMotor.SetInverted(true);
}

void DriveTrain::InitDefaultCommand()
{
	// Set the default command for a subsystem here.
	SetDefaultCommand(new DriveWithJoystick());
}

void DriveTrain::Drive(double north, double south,double east, double west)
{
	northMotor.Set(north);
	southMotor.Set(south);
	eastMotor.Set(east);
	westMotor.Set(west);
}
// Put methods for controlling this subsystem
// here. Call these from Commands.
