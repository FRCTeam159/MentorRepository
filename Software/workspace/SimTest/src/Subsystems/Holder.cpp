/*
 * Holder.cpp
 *
 *  Created on: Feb 19, 2016
 *      Author: alpiner
 */
#include <Commands/ExecHolder.h>
#include <Subsystems/Holder.h>
#include "Assignments.h"

#define GATE_OPEN_SPEED 0.25
#define GATE_REMOVE_SPEED -0.5
#define GATE_CLOSE_SPEED -0.3

#define PUSH_SPEED 1.0
#define PUSH_HOLD_SPEED 0.02
#define PUSH_REMOVE_SPEED -0.25

#define BALL_DETECT_VALUE 0.5

Holder::Holder() : Subsystem("Holder"),
	gateMotor(HOLDER_GATE,false),pushMotor(HOLDER_PUSH,false),
	lowerLimit(GATE_MIN),upperLimit(GATE_MAX),ballSensor(BALL_SENSOR)
{
	std::cout<<"New BallHolder("<<HOLDER_GATE<<","<<HOLDER_PUSH<<")"<<std::endl;
	push_hold_speed=0;
	Log();
}

void Holder::Log() {
	SmartDashboard::PutBoolean("Holder Initialized", IsInitialized());
	SmartDashboard::PutBoolean("Gate Open", IsGateOpen());
	SmartDashboard::PutBoolean("Gate Closed", IsGateClosed());
	SmartDashboard::PutBoolean("Ball present", IsBallPresent());
}

void Holder::InitDefaultCommand() {
	SetDefaultCommand(new ExecHolder());
}

void Holder::Init(){
	initialized=false;
	pushMotor.Set(0);
	gateMotor.Set(0);
}

void Holder::AutonomousInit(){
	Init();
}
void Holder::TeleopInit(){
	Init();
}
void Holder::DisabledInit(){
	Init();
}

bool Holder::IsGateOpen(){
	return upperLimit.Get();
}

bool Holder::IsGateClosed(){
	return lowerLimit.Get();
}
// ===========================================================================================================
// Holder::IsBallPresent
// - return true if the ball is in the holder
// - note: using sonar in simulation
// ===========================================================================================================
bool Holder::IsBallPresent(){
	double distance=ballSensor.GetAverageVoltage();
	//std::cout<<"ballSensor:"<<distance<<std::endl;
	return distance<BALL_DETECT_VALUE?true:false;
}

void Holder::OpenGate(){
	gateMotor.Set(GATE_OPEN_SPEED);
}
void Holder::CloseGate(){
	if(!IsGateClosed())
		gateMotor.Set(GATE_CLOSE_SPEED);
	else
		gateMotor.Set(0);
}

void Holder::PushBall(){
	pushMotor.Set(PUSH_SPEED);
	pushRequested=true;
}
void Holder::HoldBall(){
	pushRequested=false;
	gateMotor.Set(GATE_OPEN_SPEED);
	pushMotor.Set(push_hold_speed);
}
void Holder::RemoveBall(){
	gateMotor.Set(GATE_REMOVE_SPEED);
	pushMotor.Set(PUSH_REMOVE_SPEED);
}
bool Holder::IsInitialized() {
	return initialized;
}

void Holder::SetInitialized() {
	initialized=true;
	gateMotor.Set(0);
}

void Holder::Initialize() {
	if(!IsGateClosed())
		gateMotor.Set(GATE_CLOSE_SPEED);
	else
		gateMotor.Set(0);
}

void Holder::Reset(){
	initialized=false;
	gateMotor.Set(0);
	pushMotor.Set(0);
}
void Holder::SetPushHoldSpeed(double d) {
	push_hold_speed=d;
}
// ===========================================================================================================
// Holder::Execute
// ===========================================================================================================
// Called repeatedly from Default Command (ExecHolder) while in Teleop Mode
// - if !initialized, goto lower limit switch (close gate)
// - else if ball is present pinch ball (open gate)
// ===========================================================================================================
void Holder::Execute() {
	Log();
	if(!initialized){
		if(IsGateClosed())
			SetInitialized();
		else
			gateMotor.Set(GATE_CLOSE_SPEED);
	}
}


