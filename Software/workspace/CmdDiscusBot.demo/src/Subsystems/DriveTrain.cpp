#include <Commands/DriveWithJoystick.h>
#include "DriveTrain.h"
#include "RobotMap.h"

DriveTrain::DriveTrain(): Subsystem("DriveTrain") {
	std::cout<<"New DriveTrain()"<<std::endl;

	frontLeft = new CANTalon(FRONT_LEFT_MOTOR);
	frontRight = new CANTalon(FRONT_RIGHT_MOTOR);
	backLeft = new CANTalon(BACK_LEFT_MOTOR);
	backRight = new CANTalon(BACK_RIGHT_MOTOR);

	frontLeft->SetFeedbackDevice(CANTalon::QuadEncoder);
	frontRight->SetFeedbackDevice(CANTalon::QuadEncoder);
	backLeft->SetFeedbackDevice(CANTalon::QuadEncoder);
	backRight->SetFeedbackDevice(CANTalon::QuadEncoder);


	drive = new RobotDrive(frontLeft,backLeft,frontRight,backRight);

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


