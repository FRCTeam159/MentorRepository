#include <Commands/DriveWithJoystick.h>
#include "DriveTrain.h"
#include "RobotMap.h"

DriveTrain::DriveTrain(): Subsystem("DriveTrain") ,
	frontLeft(FRONT_LEFT),
	frontRight(FRONT_RIGHT),
	backLeft(BACK_LEFT),
	backRight(BACK_RIGHT),
	gyro(GYRO)
{
	std::cout<<"New DriveTrain()"<<std::endl;

	frontLeft.SetFeedbackDevice(CANTalon::QuadEncoder);
	frontRight.SetFeedbackDevice(CANTalon::QuadEncoder);
	backLeft.SetFeedbackDevice(CANTalon::QuadEncoder);
	backRight.SetFeedbackDevice(CANTalon::QuadEncoder);


	drive = new RobotDrive(&frontLeft,&backLeft,&frontRight,&backRight);

	drive->SetInvertedMotor(RobotDrive::kFrontRightMotor,true);
	drive->SetInvertedMotor(RobotDrive::kRearRightMotor,true);

}

/**
 * When no other command is running let the operator drive around
 * using the PS3 joystick.
 */
void DriveTrain::InitDefaultCommand() {
	SetDefaultCommand(new DriveWithJoystick());
}

double DriveTrain::GetHeading(){
	return gyro.GetAngle();
}
/**
 * The log method puts interesting information to the SmartDashboard.
 */
void DriveTrain::Log() {
}

void DriveTrain::Drive(double x, double y, double z) {
#ifdef REAL
	drive->MecanumDrive_Cartesian(x, y, z, 0.0);
#else
	drive->TankDrive(x, y, false);
#endif
}


