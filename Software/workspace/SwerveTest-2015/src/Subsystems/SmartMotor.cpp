/*
 * SmartMotor.cpp
 *
 *  Created on: Jul 6, 2015
 *      Author: alpiner
 */

#include "SmartMotor.h"

#ifdef REAL
SmartMotor::SmartMotor(int id) : CANTalon(id){
	control_mode=SPEED;
	inverted=false;
	syncGroup=0x08;
}
#else
SmartMotor::SmartMotor(int id) : Talon(id){
	pid=0;
	int ival=(id-1)*2+1; // 1,3,5,..
	encoder=new Encoder(ival,ival+1); // {1,2} {3,4} {5,6} ..
	control_mode=SPEED;
	inverted=false;
	syncGroup=0x08;
}
#endif


SmartMotor::~SmartMotor(){
#ifndef REAL
	if(encoder)
		delete encoder;
	if(pid)
		delete pid;
#endif
}

void SmartMotor::UsePIDOutput(double value){
#ifdef REAL
	CANTalon::Set(value,syncGroup);
#else
	Talon::PIDWrite(value);
#endif

}
double SmartMotor::ReturnPIDInput(){
	if(control_mode==SPEED)
		return GetVelocity();
	else
		return GetDistance();
}
double SmartMotor::PIDGet(){
	return ReturnPIDInput();
}

void SmartMotor::SetVelocity(double value){
#ifdef REAL
	CANTalon::Set(value);
#else
	Talon::Set(value);
#endif
}

double SmartMotor::GetVelocity(){
#ifdef REAL
	return CANTalon::GetSpeed();
#else
	return encoder->GetRate();
#endif

}

void SmartMotor::SetDistance(double value){
#ifdef REAL
	CANTalon::SetPosition(value);
#else
	if(pid)
		pid->SetSetpoint(value);
	else
		std::cout<<"ERROR SetDistance:PID=NULL"<<std::endl;
#endif
}

double SmartMotor::GetDistance(){
#ifdef REAL
	return CANTalon::GetPosition();
#else
	return encoder->GetDistance();
#endif
}

void SmartMotor::ClearIaccum(){
#ifdef REAL
	CANTalon::ClearIaccum();
#else
	if(pid){
		pid->Reset(); // clears accumulator but also disables
		pid->Enable();
	}
#endif

}

void SmartMotor::Enable(){
#ifdef REAL
	CANTalon::EnableControl();
#else
	if(pid)
		pid->Enable();
	else
		std::cout<<"ERROR Enable:PID=NULL"<<std::endl;
#endif

}
void SmartMotor::Disable(){
#ifdef REAL
	CANTalon::Disable();
#else
	Talon::Disable();
	if(pid)
		pid->Disable();
#endif
}

void SmartMotor::SetPID(int mode, double P, double I, double D){
	SetPID(P,I,D);
	SetMode(mode);
}
void SmartMotor::SetInverted(bool t) {
	inverted=t;
}

void SmartMotor::SetPID(double P, double I, double D){
#ifndef REAL
	if(pid)
		delete pid;
	pid=new PIDController(P, I, D,this,this);
#else
	CANTalon::SetPID(P,I,D);
#endif
}
void SmartMotor::SetMode(int m){
	if((m & POSITION) && (m != SPEED) )
		m=SPEED;
#ifdef REAL
	CANTalon::ControlMode mode = (m==POSITION)? CANTalon::ControlMode::kPosition: CANTalon::ControlMode::kSpeed;
	CANTalon::SetControlMode(mode);
#else
	encoder->SetPIDSourceType((m==POSITION)? PIDSourceType::kDisplacement : PIDSourceType::kRate);
#endif
	control_mode=m;
}

void SmartMotor::SetSetpoint(double value){
#ifdef REAL
	if(control_mode==SPEED)
		CANTalon::Set(value);
	else
		CANTalon::SetPosition(value);
#else
	if(pid)
		pid->SetSetpoint(value);
	else
		std::cout<<"ERROR SetSetpoint:PID=NULL"<<std::endl;

#endif
}

void SmartMotor::SetInputRange(double min, double max){
#ifdef REAL
	// TODO: what is the equivalent function for a CANTalon ?
#else
	if(pid)
		pid->SetInputRange(min,max);
#endif

}
void SmartMotor::SetOutputRange(double min, double max){
#ifdef REAL
	// TODO: what is the equivalent function for a CANTalon ?
#else
	if(pid)
		pid->SetOutputRange(min,max);
#endif
}

void SmartMotor::SetPercentTolerance(double tol){
#ifdef REAL
	// TODO: implement equivalent function for a CANTalon
#else
	if(pid)
		pid->SetPercentTolerance(tol);
#endif
}

bool SmartMotor::OnTarget(){
#ifdef REAL
	// TODO: implement equivalent function for a CANTalon
#else
	if(pid)
		return pid->OnTarget();
	else
		return false;
#endif
}

void SmartMotor::Reset(){
#ifdef REAL
	// TODO: implement equivalent function for a CANTalon
#else
	if(encoder)
		encoder->Reset();
#endif
}
void SmartMotor::SetDistancePerPulse(double target){
#ifdef REAL
	// TODO: implement equivalent function for a CANTalon
#else
	if(encoder)
		encoder->SetDistancePerPulse(target);
#endif
}
