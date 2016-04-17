/*
 * Loader.cpp
 *
 *  Created on: Mar 21, 2016
 *      Author: alpiner
 */
#include <Commands/ExecLoader.h>
#include "Assignments.h"
#include <Subsystems/Loader.h>

#define LOAD_ROLLER_SPEED 0.9
#define SETZEROSPEED -0.2
#define LIFT_ASSIST_SPEED 0.1

#define MED_ANGLE 6
#define HIGH_ANGLE 12
#define LOW_ANGLE 0.0
#define MAX_ANGLE 50

#define MAX_ANGLE_ERROR 1
#define P 0.3
#define I 0.002
#define D 0.2

Loader::Loader() : Subsystem("Loader"),
  liftMotor(LOADER_ANGLE,false),
  rollerMotor(LOADER_ROLLERS,false),
  accel(LOADER_PITCH),
  lowerLimit(LOADER_MIN)
{
	liftMotor.SetPID(P,I,D,this);
	liftMotor.SetTolerance(MAX_ANGLE_ERROR);
	liftMotor.Disable();
	initialized=false;
	roller_speed=LOAD_ROLLER_SPEED;
	accel.Reset();
	Log();
}

void Loader::InitDefaultCommand() {
	SetDefaultCommand(new ExecLoader());
}

void Loader::Log() {
	SmartDashboard::PutBoolean("Loading", Loading());
	SmartDashboard::PutNumber("Lifter Angle", -accel.GetAngle());
	SmartDashboard::PutNumber("Rollers", roller_state);
}

// ===========================================================================================================
// Loader::Execute
// ===========================================================================================================
// Called repeatedly in Teleop Mode
// - if not initialized, goto lower limit switch
// ===========================================================================================================
void Loader::Execute() {
	Log();
	SpinRollers();
	if(!initialized){
		if(!LifterAtLowerLimit())
			GoToZeroLimitSwitch();
		else
			SetInitialized();
	}
}

// ===========================================================================================================
// Loader::SetMin
// - Set lifter to low position
// ===========================================================================================================
void Loader::SetLow() {
	SetRollerState(ROLLERS_OFF);
	liftMotor.Disable(); // disable PID control
	GoToZeroLimitSwitch();
	loading=false;
}

// ===========================================================================================================
// Loader::SetMed
// - Set lifter to grab position
// ===========================================================================================================
void Loader::LoadBall() {
	liftMotor.Disable();
	liftMotor.Set(LIFT_ASSIST_SPEED);
	roller_speed=LOAD_ROLLER_SPEED;
	loading=true;
	ExecLoad();
}

void Loader::ExecLoad() {
	liftMotor.Set(LIFT_ASSIST_SPEED);
	SetRollerState(ROLLERS_FORWARD);
}

bool Loader::LifterAtLowerLimit() {
	return lowerLimit.Get();
}

bool Loader::Loading() {
	return loading;
}
void Loader::SetLoading(bool b) {
	loading=b;
}

void Loader::Disable(){
	liftMotor.Reset();
	liftMotor.Disable();
	rollerMotor.Disable();
	initialized=false;
	roller_speed=0;
	loading=false;
	SetRollerState(ROLLERS_OFF);
	Log();
}

// Initialize
void Loader::Init(){
	initialized=false;
	roller_speed=LOAD_ROLLER_SPEED;
	Log();
}

void Loader::AutonomousInit(){
	std::cout << "Loader::AutonomousInit"<<std::endl;
	Init();
}
void Loader::TeleopInit(){
	std::cout << "Loader::TeleopInit"<<std::endl;
	Init();
}
void Loader::DisabledInit(){
	std::cout << "Loader::DisabledInit"<<std::endl;
	Disable();
}

void Loader::SetLifterAngle(double a){
	a=a>max_angle?max_angle:a;
	a=a<min_angle?min_angle:a;
	angle=a;
	liftMotor.SetSetpoint(angle);
	liftMotor.Enable();
	Log();
}
bool Loader::LifterIsAtTargetAngle(){
	Log();
	return liftMotor.OnTarget();
}

double Loader::GetLifterAngle(){
	return angle;
}

void Loader::SpinRollers() {
	switch(roller_state){
	case ROLLERS_OFF:
		rollerMotor.Set(0);
		break;
	case ROLLERS_FORWARD:
		rollerMotor.Set(roller_speed);
		break;
	case ROLLERS_REVERSE:
		rollerMotor.Set(-roller_speed);
		break;
	}
}
void Loader::SetRollerState(int b) {
	roller_state=b;
}

void Loader::GoToZeroLimitSwitch() {
	if(!LifterAtLowerLimit())
		liftMotor.SetVoltage(SETZEROSPEED);
	else
		liftMotor.SetVoltage(0);
}

void Loader::SetInitialized() {
	initialized=true;
	liftMotor.Set(0);
	accel.Reset();
}

void Loader::Initialize() {
	if(!LifterAtLowerLimit()){
		liftMotor.DisablePID();
		GoToZeroLimitSwitch();
	}
}

bool Loader::IsInitialized() {
	return initialized;
}

double Loader::PIDGet() {
	return -accel.GetAngle();
}


