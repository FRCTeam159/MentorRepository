/*
 * CANTalon.cpp
 *
 *  Created on: April 16, 2016
 *      Author: Dean Sindorf
 *  Adds simulation support for CANTalon motor controllers
 */
#include "WPILib.h"
#include <CANTalon.h>

#define SIMPIDRATE 0.01

CANTalon::CANTalon(int i,bool enc) : Talon(i){
	pid=0;
	encoder=0;
	id=i;
	debug=0;
	if(enc){
		int ival=(id-1)*2+1; // 1,3,5,..
		encoder=new Encoder(ival,ival+1); // {1,2} {3,4} {5,6} ..
	}
	control_mode=VOLTAGE;
	inverted=false;
	syncGroup=0x08;
}

CANTalon::~CANTalon(){
	if(encoder)
		delete encoder;
	if(pid)
		delete pid;
}
//#define DEBUG_OUTPUT
void CANTalon::PIDWrite(float output){
	if (output !=output) {
		std::cout<< GetChannel() << " (NaN)" << std::endl;
		return;
	}
#ifdef DEBUG_OUTPUT
	std::cout << GetChannel()<<" "<<output<<std::endl;
#endif
	Talon::PIDWrite(output);
}
void CANTalon::UsePIDOutput(double value){
	Talon::PIDWrite(value);
}
double CANTalon::ReturnPIDInput(){
	switch(control_mode){
	case POSITION:
		return GetDistance();
	case SPEED:
		return GetVelocity();
	default:
	case VOLTAGE:
		return GetVoltage();
	}
}
double CANTalon::PIDGet(){
	return ReturnPIDInput();
}

void CANTalon::SetSetpoint(double value){
	if(pid)
		pid->SetSetpoint(value);
}
void CANTalon::SetVelocity(double value){
	if(pid)
		pid->SetSetpoint(value);
	Talon::Set(value);
}

double CANTalon::GetVelocity(){
	return encoder->GetRate();
}

void CANTalon::SetDistance(double value){
	if(pid)
		pid->SetSetpoint(value);
	else
		std::cout<<"ERROR SetDistance:PID=NULL"<<std::endl;
}

double CANTalon::GetDistance(){
	if(encoder)
		return encoder->GetDistance();
	else{
		std::cout<<"ERROR GetDistance:encoder=NULL"<<std::endl;
		return 0;
	}
}

void CANTalon::SetVoltage(double value){
	Set(value);
}
double CANTalon::GetVoltage(){
	return Talon::GetSpeed();
}

void CANTalon::ClearIaccum(){
	if(pid){
		pid->Reset(); // clears accumulator but also disables
	}
}

void CANTalon::EnablePID(){
	if(pid)
		pid->Enable();
}

void CANTalon::DisablePID(){
	if(pid)
		pid->Disable();
}

void CANTalon::Enable(){
	if(pid)
		pid->Enable();
}

bool CANTalon::IsEnabled(){
	if(pid)
		return pid->IsEnabled();
	else
		return false;
}
void CANTalon::Disable(){
	Talon::Disable();
	if(pid)
		pid->Disable();
}

void CANTalon::SetPID(int mode, double P, double I, double D){
	SetPID(P,I,D);
	SetMode(mode);
}
void CANTalon::SetPID(int mode, double P, double I, double D,PIDSource *s){
	SetPID(P,I,D,s);
	SetMode(mode);
}

void CANTalon::SetInverted(bool t) {
	inverted=t;
	// This causes purposefully thrown exception in simulation
	// if(encoder)
	//	 encoder->SetReverseDirection(t);
}

PIDController *CANTalon::GetPID(){
   return pid;
}

void CANTalon::SetPID(double P, double I, double D){
	SetPID(P,I,D,this);
}

void CANTalon::SetPID(double P, double I, double D, PIDSource *s){
	if(pid)
		delete pid;
	pid=new PIDController(P, I, D,s,this);
}

void CANTalon::ClearPID(){
	if(pid)
		delete pid;
	pid=0;
}
void CANTalon::SetMode(int m){
	if((m != POSITION) && (m != SPEED) && (m != VOLTAGE)){
		std::cout<<"ERROR unknown mode:"<<m<<std::endl;
		return;
	}
	control_mode=m;
	if(control_mode==VOLTAGE){
		ClearPID();
	}
	else{
		PIDSourceType pidMode=(control_mode==POSITION)? PIDSourceType::kDisplacement:PIDSourceType::kRate;
		SetPIDSourceType(pidMode);
		if(encoder)
			encoder->SetPIDSourceType(pidMode);
	}
}

void CANTalon::SetInputRange(double min, double max){
	if(pid)
		pid->SetInputRange(min,max);
}
void CANTalon::SetOutputRange(double min, double max){
	if(pid)
		pid->SetOutputRange(min,max);
}

void CANTalon::SetTolerance(double tol){
	if(pid)
		pid->SetAbsoluteTolerance(tol);
}
void CANTalon::SetToleranceBuffer(unsigned bufLength){
	if(pid)
		pid->SetAbsoluteTolerance(bufLength);
}

double CANTalon::GetTargetCorrection(){
	if(pid)
		return pid->Get();
	else
		return 0;
}

double CANTalon::GetTargetError(){
	if(pid)
		return pid->GetError();
	else
		return 0;
}

bool CANTalon::OnTarget(){
	if(pid)
		return pid->OnTarget();
	else
		return false;
}

void CANTalon::Reset(){
	if(encoder)
		encoder->Reset();
	if(pid)
		pid->Reset();
	ClearIaccum();
}
void CANTalon::SetDistancePerPulse(double target){
	if(encoder)
		encoder->SetDistancePerPulse(target);
}

void CANTalon::SetDebug(int b){
	debug=b;
}
