#include <Commands/DriveWithJoystick.h>
#include "DriveTrain.h"
#include "RobotMap.h"

#ifdef SIMULATION
#define WHEEL_TICKS 360 // default encoder ticks given in simulation
#else // replace these values for whatever is appropriate for real drive-train
#define WHEEL_TICKS 900
#endif

#define WHEEL_DIAMETER 7.5

DriveTrain::DriveTrain(): Subsystem("DriveTrain") ,
	frontLeft(FRONT_LEFT),
	frontRight(FRONT_RIGHT),
	backLeft(BACK_LEFT),
	backRight(BACK_RIGHT),
	gyro(GYRO)
{
	std::cout<<"New DriveTrain()"<<std::endl;

	backLeft.SetFeedbackDevice(CANTalon::QuadEncoder);
	backRight.SetFeedbackDevice(CANTalon::QuadEncoder);


	drive = new RobotDrive(&frontLeft,&backLeft,&frontRight,&backRight);

	drive->SetInvertedMotor(RobotDrive::kFrontRightMotor,true);
	drive->SetInvertedMotor(RobotDrive::kRearRightMotor,true);

	// cpr=codes-per-revolution
	// - distance-per-code=1/codes-per-revolution
	// -
	const double cpr=12.0*WHEEL_TICKS/M_PI/WHEEL_DIAMETER; // so GetPosition returns units in feet
	std::cout<<"cpr:"<<cpr<<std::endl;

	backLeft.ConfigEncoderCodesPerRev(cpr);
	backRight.ConfigEncoderCodesPerRev(cpr);

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
// returns distance traveled (inches) average from left and right encoders
double DriveTrain::GetPosition() {
	return (backLeft.GetPosition() + backRight.GetPosition())/2;
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

double DriveTrain::GetLeftPosition() {
	return backLeft.GetPosition();
}

double DriveTrain::GetRightPosition() {
	return backRight.GetPosition();
}

void DriveTrain::Reset() {
	backLeft.SetPosition(0.0);
	backRight.SetPosition(0.0);
}
