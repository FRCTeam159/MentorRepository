#include "DriveTrain.h"
#include "RobotMap.h"
#include "Commands/DriveWithJoystick.h"
#include "WPILib.h"

#define P 1
#define I 0.0
#define D 0.0

#define SPEED

#ifdef SIMULATION
#define DRIVE_ENCODER_TICKS 360
#else
#define DRIVE_ENCODER_TICKS 900
#endif
DriveTrain::DriveTrain() : Subsystem("DriveTrain"),
		frontLeft(FRONTLEFT),   // slave  1
		frontRight(FRONTRIGHT), // master 4
		backLeft(BACKLEFT),     // master 2
		backRight(BACKRIGHT)    // slave  3
{
	InitDrive();

	frontRight.ConfigLimitMode(CANTalon::kLimitMode_SrxDisableSwitchInputs);
	backLeft.ConfigLimitMode(CANTalon::kLimitMode_SrxDisableSwitchInputs);
	frontRight.SetFeedbackDevice(CANTalon::QuadEncoder);
	backLeft.SetFeedbackDevice(CANTalon::QuadEncoder);

#ifdef SPEED
	SetControlMode(CANTalon::kSpeed);
#else
	SetControlMode(CANTalon::kPercentVbus);
#endif
	frontRight.SetPID(P,I,D);
	backLeft.SetPID(P,I,D);

	//frontRight.SetDebug(2);
	frontRight.ConfigEncoderCodesPerRev(DRIVE_ENCODER_TICKS);
	backLeft.ConfigEncoderCodesPerRev(DRIVE_ENCODER_TICKS);

	gearPneumatic = new DoubleSolenoid(GEARSHIFTID,0,1);
	SetLowGear();

	SetExpiration(0.2);
}
void DriveTrain::InitDefaultCommand()
{
	// Set the default command for a subsystem here.
	SetDefaultCommand(new DriveWithJoystick());
}
void DriveTrain::TankDrive(float left, float right) {
	frontLeft.Set(BACKLEFT);
	backLeft.Set(left);
	frontRight.Set(-right);
	backRight.Set(FRONTRIGHT);

	m_safetyHelper->Feed();
}

// Put methods for controlling this subsystem
// here. Call these from Commands.
void DriveTrain::CustomArcade(float xAxis, float yAxis, float zAxis, bool squaredInputs) {
	//cout << "xAxis:" << xAxis << " yAxis:" << yAxis << endl;
	double left=0;
	double right=0;

	if (squaredInputs) {
		if (xAxis >= 0.0) {
			xAxis = (xAxis * xAxis);
		} else {
			xAxis = -(xAxis * xAxis);
		}
		if (yAxis >= 0.0) {
			yAxis = (yAxis * yAxis);
		} else {
			yAxis = -(yAxis * yAxis);
		}
	}

	if (zAxis != 0) {
		xAxis = zAxis;
		yAxis = -zAxis;
	}

	if (xAxis > 0.0) {
		if (yAxis > 0.0) {
			left = xAxis - yAxis;
			right = std::max(xAxis, yAxis);
		} else {
			left = std::max(xAxis, -yAxis);
			right = xAxis + yAxis;
		}
	} else {
		if (yAxis > 0.0) {
			left = -std::max(-xAxis, yAxis);
			right = xAxis + yAxis;
		} else {
			left = xAxis - yAxis;
			right = -std::max(-xAxis, -yAxis);
		}
	}

	// Make sure values are between -1 and 1
	left = coerce(-1, 1, left);
	right = coerce(-1, 1, right);
	frontLeft.Set(BACKLEFT);
	backLeft.Set(left);
	frontRight.Set(-right);
	backRight.Set(FRONTRIGHT);

	m_safetyHelper->Feed();
}

float DriveTrain::coerce(float min, float max, float x) {
	if (x < min) {
		x = min;
	}

	else if (x > max) {
		x = max;
	}
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

void DriveTrain::GetDescription(std::ostringstream& desc) const {
  desc << "DriveTrain";
}

void DriveTrain::StopMotor() {
  //backRight.StopMotor();
  backLeft.StopMotor();
  frontRight.StopMotor();
  //frontLeft.StopMotor();
  m_safetyHelper->Feed();
}

void DriveTrain::Enable() {
	//backRight.Enable();
	//frontLeft.Enable();
	frontRight.Enable();
	backLeft.Enable();
}
void DriveTrain::Disable() {
	//backRight.Disable();
	//frontLeft.Disable();
	frontRight.Disable();
	backLeft.Disable();
}

void DriveTrain::SetControlMode(CANTalon::ControlMode controlMode) {
	mode=controlMode;
	frontRight.SetControlMode(controlMode);
	backLeft.SetControlMode(controlMode);
#ifdef SIMULATION
	//frontLeft.SetControlMode(controlMode);
	//backRight.SetControlMode(controlMode);
#else
	frontLeft.SetControlMode(CANTalon::kFollower);
	backRight.SetControlMode(CANTalon::kFollower);
#endif
}

