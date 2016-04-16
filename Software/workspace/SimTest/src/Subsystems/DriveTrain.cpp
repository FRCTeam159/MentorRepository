
#include <math.h>

#include "DriveTrain.h"
#include "Assignments.h"

#include "Commands/TankDriveWithJoystick.h"

#define MP 0.5
#define MI 0.00
#define MD 0.5
//#define MI 0.0
//#define MD 0.0
#define MAX_POS_ERROR 0.1 // tolerance for set position
DriveTrain::DriveTrain() : Subsystem("DriveTrain"),
	left_motor(DRIVE_LEFT),right_motor(DRIVE_RIGHT),gyro(DRIVE_ANGLE)
{
	std::cout<<"New DriveTrain("<<DRIVE_LEFT<<","<<DRIVE_RIGHT<<")"<<std::endl;
	SetDistancePerPulse(WHEEL_DIAMETER,WHEEL_TICKS,INVERT_RIGHT_SIDE);
	SetDeadband(DEADBAND,DEADBAND);
	gyro.Reset();
	Log();
}

// ===========================================================================================================
// DriveTrain::SetDistancePerPulse(double d, double t, bool b)
// ===========================================================================================================
// inputs
//   d    wheel diameter (inches)
//   t    wheel encoder ticks per revolution
//   b    if true reverse encoder direction on right side
// notes:
//   right wheel encoder may need to be reversed but for some reason encoder->SetReverseDirection isn't
//   supported in simulation mode (throws exception) and can only be set in the constructor
//   (which is inconvenient when using "GPMotors", since that would require passing in an additional argument)
//   An alternative solution is to just invert the "DistancePerPulse" value
// ===========================================================================================================
void DriveTrain::SetDistancePerPulse(double d, double t, bool b){
	dpp=(double)M_PI*(d/12.0) / t;
	left_motor.SetDistancePerPulse(dpp);
	//right_motor.SetDistancePerPulse(b?-dpp:dpp);
	right_motor.SetDistancePerPulse(dpp);
	squared_inputs=false;
}
// ===========================================================================================================
// DriveTrain::GetDistancePerPulse()
//  - return Distance per encoder tick (in feet)
// ===========================================================================================================
double DriveTrain::GetDistancePerPulse() {
	return dpp;
}

/**
 * When no other command is running let the operator drive around
 * using the PS3 joystick.
 */
void DriveTrain::InitDefaultCommand() {
	SetDefaultCommand(new TankDriveWithJoystick());
}

/**
 * The log method puts interesting information to the SmartDashboard.
 */
void DriveTrain::Log() {
//	SmartDashboard::PutNumber("Drive Left Distance", left_motor.GetDistance());
//	SmartDashboard::PutNumber("Drive Right Distance", right_motor.GetDistance());
//	SmartDashboard::PutNumber("Drive Left Speed", left_motor.GetVelocity());
	SmartDashboard::PutNumber("Drive Left Speed", GetLeftVoltage());
	SmartDashboard::PutNumber("Drive Right Speed", GetRightVoltage());
	SmartDashboard::PutNumber("Drive Heading", GetHeading());
}

void DriveTrain::Limit(double &num) {
  if (num > 1.0)
    num= 1.0;
  if (num < -1.0)
    num= -1.0;
}
// square the inputs (while preserving the sign) to increase fine control
// while permitting full power
void DriveTrain::SquareInputs(double &left, double &right) {
	if (left >= 0.0)
		left = (left * left);
	else
		left = -(left * left);
	if (right >= 0.0)
		right = (right * right);
	else
		right = -(right * right);
}

void DriveTrain::Drive(Joystick* joy) {
#if JOYTYPE == XBOX_GAMEPAD
	double left=-joy->GetRawAxis(1);
	double right=-joy->GetRawAxis(4);
#else
	double left=-joy->GetY();
	double right=-joy->GetRawAxis(4);
#endif
	if(reverse_driving){
		double r=right;
		double l=left;
		left=-r;
		right=-l;
	}
	//std::cout<<"l:"<<left<<" r:"<<right<<std::endl;
	Limit(left);
	Limit(right);
	if(squared_inputs)
		SquareInputs(left,right);
	left=Deadband(left,x_deadband);
	right=Deadband(right,y_deadband);
	Drive(left,right);
}

double DriveTrain::Deadband(double x, double ignore) {
	return fabs(x)>=ignore ? x: 0.0;
}
void DriveTrain::SetDeadband(double x, double y) {
	x_deadband=x;
	y_deadband=y;
}

void DriveTrain::SetPID(int mode, double P, double I, double D){
	right_motor.SetPID(mode,P,I,D);
	left_motor.SetPID(mode,P,I,D);
	left_motor.SetTolerance(MAX_POS_ERROR);
	right_motor.SetTolerance(MAX_POS_ERROR);
}
void DriveTrain::SetDistance(double d){
	target_distance=d;
	left_motor.SetDistance(d);
	right_motor.SetDistance(d);
}

void DriveTrain::Turn(double d){
	left_motor.Set(d);
	if(inverted)
		right_motor.Set(d);
	else
		right_motor.Set(-d);
	Log();
}

void DriveTrain::Drive(double l,double r){
	left_motor.Set(l);
	if(inverted)
		right_motor.Set(-r);
	else
		right_motor.Set(r);
	Log();
}
void DriveTrain::Reset() {
	right_motor.Reset();
	left_motor.Reset();
}
void DriveTrain::Enable() {
	right_motor.Enable();
	left_motor.Enable();
	disabled=false;
}
void DriveTrain::Disable() {
	right_motor.Disable();
	left_motor.Disable();
	disabled=true;
}
void DriveTrain::EndTravel() {
	Drive(0,0);
}
void DriveTrain::DisablePID() {
	right_motor.DisablePID();
	left_motor.DisablePID();
	pid_disabled=true;
}
void DriveTrain::EnablePID() {
	right_motor.EnablePID();
	left_motor.EnablePID();
	pid_disabled=false;
}

double DriveTrain::GetHeading() {
	return gyro.GetAngle();
}

void DriveTrain::TeleopInit() {
	std::cout << "DriveTrain::TeleopInit"<<std::endl;
	left_motor.SetMode(GPMotor::VOLTAGE);
	right_motor.SetMode(GPMotor::VOLTAGE);
	Reset();
	Enable();
}

void DriveTrain::AutonomousInit() {
	std::cout << "DriveTrain::AutonomousInit"<<std::endl;
	Drive(0,0);
	Reset();
	gyro.Reset();
}

void DriveTrain::DisabledInit() {
	std::cout << "DriveTrain::DisabledInit"<<std::endl;
	Disable();
	Drive(0,0);
	Reset();
}

double DriveTrain::GetDistance() {
	return (left_motor.GetDistance() + right_motor.GetDistance())/2;
}
double DriveTrain::GetSpeed() {
	return (left_motor.GetVelocity() + right_motor.GetVelocity())/2;
}
double DriveTrain::GetLeftSpeed(){
	return left_motor.GetVelocity();
}
double DriveTrain::GetRightSpeed(){
	return right_motor.GetVelocity();
}
double DriveTrain::GetLeftDistance(){
	return left_motor.GetDistance();
}
double DriveTrain::GetRightDistance(){
	return right_motor.GetDistance();
}
double DriveTrain::GetLeftVoltage(){
	return left_motor.GetVoltage();
}
double DriveTrain::GetRightVoltage(){
	return right_motor.GetVoltage();
}

void DriveTrain::SetReverseDriving(bool b) {
	reverse_driving=b;
}

bool DriveTrain::ReverseDriving() {
	return reverse_driving;
}
