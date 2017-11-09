#include "DriveTrain.h"
#include "RobotMap.h"
#include "Commands/DriveWithJoystick.h"
#include "WPILib.h"

#define P 1
#define I 0.01
#define D 0.2

//#define SPEED

#ifdef SIMULATION
#define DRIVE_ENCODER_TICKS 360
#else
#define DRIVE_ENCODER_TICKS 900
#endif
#define WHEEL_DIAMETER 4.125 // inches

#define ROUND(x) 0.01*round(x*10000/100)

#define TICKS_PER_INCH (DRIVE_ENCODER_TICKS/M_PI/WHEEL_DIAMETER)
#define TICKS_PER_METER (TICKS_PER_INCH*39.37)

DriveTrain::DriveTrain() : Subsystem("DriveTrain"),
		frontLeft(FRONTLEFT),   // slave  1
		frontRight(FRONTRIGHT), // master 4
		backLeft(BACKLEFT),     // master 2
		backRight(BACKRIGHT),    // slave  3
		gyro(3)
{
	InitDrive();

	frontRight.ConfigLimitMode(CANTalon::kLimitMode_SrxDisableSwitchInputs);
	backLeft.ConfigLimitMode(CANTalon::kLimitMode_SrxDisableSwitchInputs);
	frontRight.SetFeedbackDevice(CANTalon::QuadEncoder);
	backLeft.SetFeedbackDevice(CANTalon::QuadEncoder);
	gyro.Reset();


#ifdef SPEED
	SetControlMode(CANTalon::kSpeed);
	frontRight.SetPID(P,I,D);
	backLeft.SetPID(P,I,D);
#else
	SetControlMode(CANTalon::kPercentVbus);
#endif

	//frontRight.SetDebug(2);
	frontRight.ConfigEncoderCodesPerRev(TICKS_PER_INCH);
	backLeft.ConfigEncoderCodesPerRev(TICKS_PER_INCH);

	gearPneumatic = new DoubleSolenoid(GEARSHIFTID,0,1);
	SetLowGear();

	SetExpiration(0.2);

	Publish(true);
}
void DriveTrain::InitDefaultCommand()
{
	// Set the default command for a subsystem here.
	SetDefaultCommand(new DriveWithJoystick());
}
void DriveTrain::TankDrive(float left, float right) {
	backLeft.Set(-left);
	frontRight.Set(-right);
	backRight.Set(FRONTRIGHT);
	frontLeft.Set(BACKLEFT);

	Publish(false);

	m_safetyHelper->Feed();
}

/**
 * Arcade drive implements single stick driving.
 *
 * This function lets you directly provide joystick values from any source.
 *
 * @param moveValue     The value to use for fowards/backwards
 * @param rotateValue   The value to use for the rotate right/left
 * @param squaredInputs If set, increases the sensitivity at low speeds
 */
void DriveTrain::ArcadeDrive(float moveValue, float rotateValue,
		bool squaredInputs) {

	// local variables to hold the computed PWM values for the motors
	double leftMotorOutput;
	double rightMotorOutput;

	if (squaredInputs) {
		// square the inputs (while preserving the sign) to increase fine control
		// while permitting full power
		if (moveValue >= 0.0) {
			moveValue = (moveValue * moveValue);
		} else {
			moveValue = -(moveValue * moveValue);
		}
		if (rotateValue >= 0.0) {
			rotateValue = (rotateValue * rotateValue);
		} else {
			rotateValue = -(rotateValue * rotateValue);
		}
	}

	if (moveValue > 0.0) {
		if (rotateValue > 0.0) {
			leftMotorOutput = moveValue - rotateValue;
			rightMotorOutput = std::max(moveValue, rotateValue);
		} else {
			leftMotorOutput = std::max(moveValue, -rotateValue);
			rightMotorOutput = moveValue + rotateValue;
		}
	} else {
		if (rotateValue > 0.0) {
			leftMotorOutput = -std::max(-moveValue, rotateValue);
			rightMotorOutput = moveValue + rotateValue;
		} else {
			leftMotorOutput = moveValue - rotateValue;
			rightMotorOutput = -std::max(-moveValue, -rotateValue);
		}
	}
	// Ramp values up
	// Make sure values are between -1 and 1
	leftMotorOutput  = coerce(-1, 1, leftMotorOutput);
	rightMotorOutput = coerce(-1, 1, rightMotorOutput);
	backLeft.Set(leftMotorOutput);
	frontRight.Set(-rightMotorOutput);
	backRight.Set(FRONTRIGHT);
	frontLeft.Set(BACKLEFT);
	Publish(false);

	m_safetyHelper->Feed();
}

float DriveTrain::coerce(float min, float max, float x) {
	if (x < min)
		x = min;
	else if (x > max)
		x = max;
	return x;
}

void DriveTrain::SetLowGear() {
	if(!inlowgear){
		gearPneumatic->Set(DoubleSolenoid::kReverse);
		cout << "Setting Low Gear"<<endl;
		inlowgear=true;
	}
}

void DriveTrain::SetHighGear() {
	if(inlowgear){
		gearPneumatic->Set(DoubleSolenoid::kForward);
		cout << "Setting High Gear"<<endl;
		inlowgear=false;
	}
}
void DriveTrain::InitDrive() {
	m_safetyHelper = std::make_unique<MotorSafetyHelper>(this);
	m_safetyHelper->SetSafetyEnabled(true);
}

void DriveTrain::SetExpiration(double timeout) {
  m_safetyHelper->SetExpiration(timeout);
}

double DriveTrain::GetExpiration() const {
  return m_safetyHelper->GetExpiration();
}

bool DriveTrain::IsAlive() const { return m_safetyHelper->IsAlive(); }

bool DriveTrain::IsSafetyEnabled() const {
  return m_safetyHelper->IsSafetyEnabled();
}

void DriveTrain::SetSafetyEnabled(bool enabled) {
  m_safetyHelper->SetSafetyEnabled(enabled);
}

void DriveTrain::DisableDrive() {
	backRight.Disable();
	backLeft.Disable();
	frontRight.Disable();
	frontLeft.Disable();
}

void DriveTrain::EnableDrive() {
	Reset();
	backRight.Enable();
	backLeft.Enable();
	frontRight.Enable();
	frontLeft.Enable();
}

void DriveTrain::GetDescription(std::ostringstream& desc) const {
  desc << "DriveTrain";
}

void DriveTrain::StopMotor() {
  backLeft.StopMotor();
  frontRight.StopMotor();
  m_safetyHelper->Feed();
}

void DriveTrain::Reset() {
    double d=GetDistance();
    cout << "DriveTrain::Reset previous travel="<<travel_distance<<" current distance="<<d<<" new travel="<<travel_distance+d<<endl;
    travel_distance +=d;
	frontRight.Reset();
	backLeft.Reset();
	//gyro.Reset();
	//gyro.InitGyro();

}
void DriveTrain::Enable() {
	frontRight.Enable();
	backLeft.Enable();
	Publish(true);
}

void DriveTrain::Disable() {
	frontRight.Disable();
	backLeft.Disable();
	frontRight.Reset();
	backLeft.Reset();
	angle=0;
	Publish(true);
}

double DriveTrain::GetHeading() {
#ifdef SIMULATION // need sign change since chassis pose is inverted
	return -gyro.GetAngle();
#else
	return gyro.GetAngle();
#endif
}

void DriveTrain::SetControlMode(CANTalon::ControlMode controlMode) {
	mode=controlMode;
	frontRight.SetControlMode(controlMode);
	backLeft.SetControlMode(controlMode);
	frontLeft.SetControlMode(CANTalon::kFollower);
	backRight.SetControlMode(CANTalon::kFollower);
}

void DriveTrain::Publish(bool init) {
	if(init){
		frc::SmartDashboard::PutNumber("Travel", 0);
		frc::SmartDashboard::PutBoolean("HighGear", false);
		frc::SmartDashboard::PutNumber("Heading", 0);
		frc::SmartDashboard::PutNumber("LeftDistance",0);
		frc::SmartDashboard::PutNumber("RightDistance",0);
		frc::SmartDashboard::PutNumber("LeftWheels",0);
		frc::SmartDashboard::PutNumber("RightWheels", 0);

	}else{
		frc::SmartDashboard::PutNumber("Heading", GetHeading());
		frc::SmartDashboard::PutNumber("Travel", ROUND(GetDistance()));
		frc::SmartDashboard::PutBoolean("HighGear", !inlowgear);
		frc::SmartDashboard::PutNumber("LeftDistance",ROUND(GetLeftDistance()));
		frc::SmartDashboard::PutNumber("RightDistance",ROUND(GetRightDistance()));
		frc::SmartDashboard::PutNumber("LeftWheels", backLeft.Get());
		frc::SmartDashboard::PutNumber("RightWheels", frontRight.Get());
	}
}

double DriveTrain::GetDistance() {
	double d1=GetRightDistance();
	double d2=GetLeftDistance();
	double x=0.5*(d1+d2);
	return x;
}
double DriveTrain::GetVelocity() {
	double d1=GetLeftVelocity();
	double d2=GetRightVelocity();
	double x=0.5*(d1+d2);
	return x;
}

double DriveTrain::GetRightDistance() {
	return -frontRight.GetPosition();
}
double DriveTrain::GetLeftDistance() {
	return -backLeft.GetPosition(); // inverted in simulation for some reason (sdf file axis direction problem ?)
}

double DriveTrain::GetTravelDistance() {
    return travel_distance+GetDistance();
}

double DriveTrain::GetLeftVelocity() {
	return -backLeft.GetSpeed();
}

double DriveTrain::GetRightVelocity() {
	return -frontRight.GetSpeed();
}
