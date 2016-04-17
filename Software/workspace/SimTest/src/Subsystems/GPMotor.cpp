/*
 * SmartMotor.cpp
 *
 *  Created on: Jul 6, 2015
 *      Author: alpiner
 */
#include "WPILib.h"
#include <Subsystems/GPMotor.h>

//#ifdef SIMULATION

#define SIMPIDRATE 0.01

//#define SIMRATE 0.02

GPMotor::GPMotor(int i) : GPMotor(i,true){
}

#if MOTORTYPE == CANTALON
GPMotor::GPMotor(int id, int enc) : CANTalon(id){
	control_mode=SPEED;
	inverted=false;
	syncGroup=0x08;
}
#else
#if MOTORTYPE == VICTOR
GPMotor::GPMotor(int i,bool enc) : Victor(i){
#else
GPMotor::GPMotor(int i,bool enc) : Talon(i){
#endif
	pid=0;
	encoder=0;
	id=i;
	if(enc){
		int ival=(id-1)*2+1; // 1,3,5,..
		encoder=new Encoder(ival,ival+1); // {1,2} {3,4} {5,6} ..
	}
	control_mode=VOLTAGE;
	inverted=false;
	syncGroup=0x08;
	debug=0;
}
#endif

GPMotor::~GPMotor(){
	if(encoder)
		delete encoder;
	if(pid)
		delete pid;
}
//#define DEBUG_OUTPUT
void GPMotor::PIDWrite(float output){
	if (output !=output) {
		std::cout<< GetChannel() << " (NaN)" << std::endl;
		return;
	}
#ifdef DEBUG_OUTPUT
	std::cout << GetChannel()<<" "<<output<<std::endl;
#endif
#if MOTORTYPE == CANTALON
	CANTalon::PIDWrite(output);
#elif MOTORTYPE == VICTOR
	Victor::PIDWrite(output);
#else
	//if(debug)
	//	std::cout<< "GPMotor::PIDWrite:"<<output << std::endl;

	Talon::PIDWrite(output);
#endif
}
void GPMotor::UsePIDOutput(double value){
#if MOTORTYPE == CANTALON
	CANTalon::Set(value,syncGroup);
#elif MOTORTYPE == VICTOR
	Victor::PIDWrite(value);
#else
	Talon::PIDWrite(value);
#endif
}
double GPMotor::ReturnPIDInput(){
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
double GPMotor::PIDGet(){
	return ReturnPIDInput();
}

void GPMotor::SetSetpoint(double value){
#if MOTORTYPE == CANTALON
	CANTalon::Set(value);
#else
	if(pid)
		pid->SetSetpoint(value);
#endif
}
void GPMotor::SetVelocity(double value){
#if MOTORTYPE == CANTALON
	CANTalon::Set(value);
#else
	if(pid)
		pid->SetSetpoint(value);
	Talon::Set(value);
#endif
}

double GPMotor::GetVelocity(){
#if MOTORTYPE == CANTALON
	return CANTalon::GetSpeed();
#else
	return encoder->GetRate();
#endif
}

void GPMotor::SetDistance(double value){
#if MOTORTYPE == CANTALON
	CANTalon::SetPosition(value);
#else
	if(pid)
		pid->SetSetpoint(value);
	else
		std::cout<<"ERROR SetDistance:PID=NULL"<<std::endl;
#endif
}

double GPMotor::GetDistance(){
#if MOTORTYPE == CANTALON
	return CANTalon::GetPosition();
#else
	if(encoder)
		return encoder->GetDistance();
	else{
		std::cout<<"ERROR GetDistance:encoder=NULL"<<std::endl;
		return 0;
	}
#endif
}

void GPMotor::SetVoltage(double value){
#if MOTORTYPE == CANTALON
	CANTalon::Set(value);
#else
	Set(value);
#endif
}
double GPMotor::GetVoltage(){
#if MOTORTYPE == CANTALON
	return CANTalon::Get();
#else
	return Talon::GetSpeed();
#endif
}

void GPMotor::ClearIaccum(){
#ifdef REAL
	CANTalon::ClearIaccum();
#else
	if(pid){
		pid->Reset(); // clears accumulator but also disables
		//pid->Enable();
	}
#endif
}

void GPMotor::EnablePID(){
#if MOTORTYPE == CANTALON
	// TODO: what is the equivalent function for a CANTalon ?
#else
	if(pid)
		pid->Enable();
#endif
}

void GPMotor::DisablePID(){
#if MOTORTYPE == CANTALON
	// TODO: what is the equivalent function for a CANTalon ?
#else
	if(pid)
		pid->Disable();
#endif
}

void GPMotor::Enable(){
#if MOTORTYPE == CANTALON
	CANTalon::Enable();
#else
	if(pid)
		pid->Enable();
#endif
}

bool GPMotor::IsEnabled(){
#if MOTORTYPE == CANTALON
	return CANTalon::IsEnabled();
#else
	if(pid)
		return pid->IsEnabled();
	else
		return false;
#endif
}
void GPMotor::Disable(){
#if MOTORTYPE == CANTALON
	CANTalon::Disable();
#else
	Talon::Disable();
	if(pid)
		pid->Disable();
#endif
}

void GPMotor::SetPID(int mode, double P, double I, double D){
	SetPID(P,I,D);
	SetMode(mode);
}
void GPMotor::SetPID(int mode, double P, double I, double D,PIDSource *s){
	SetPID(P,I,D,s);
	SetMode(mode);
}

void GPMotor::SetInverted(bool t) {
	inverted=t;
	// This causes purposefully thrown exception in simulation
	// if(encoder)
	//	 encoder->SetReverseDirection(t);
}

PIDController *GPMotor::GetPID(){
#if MOTORTYPE == CANTALON
	return 0;
#else
   return pid;
#endif
}

void GPMotor::SetPID(double P, double I, double D){
	SetPID(P,I,D,this);
}

void GPMotor::SetPID(double P, double I, double D, PIDSource *s){
#if MOTORTYPE != CANTALON
	if(pid)
		delete pid;
	pid=new PIDController(P, I, D,s,this,SIMPIDRATE);
#else
	CANTalon::SetPID(P,I,D,s);
#endif
}

void GPMotor::ClearPID(){
	if(pid)
		delete pid;
	pid=0;
}
void GPMotor::SetMode(int m){
	if((m != POSITION) && (m != SPEED) && (m != VOLTAGE)){
		std::cout<<"ERROR unknown mode:"<<m<<std::endl;
		return;
	}
	control_mode=m;
#if MOTORTYPE == CANTALON
	CANTalon::ControlMode mode = (m==POSITION)? CANTalon::ControlMode::kPosition: CANTalon::ControlMode::kSpeed;
	CANTalon::SetControlMode(mode);
#else
	if(control_mode==VOLTAGE){
		ClearPID();
	}
	else{
		PIDSourceType pidMode=(control_mode==POSITION)? PIDSourceType::kDisplacement:PIDSourceType::kRate;
		SetPIDSourceType(pidMode);
		if(encoder)
			encoder->SetPIDSourceType(pidMode);
	}
#endif
}

void GPMotor::SetInputRange(double min, double max){
#if MOTORTYPE == CANTALON
	// TODO: what is the equivalent function for a CANTalon ?
#else
	if(pid)
		pid->SetInputRange(min,max);
#endif

}
void GPMotor::SetOutputRange(double min, double max){
#if MOTORTYPE == CANTALON
	// TODO: what is the equivalent function for a CANTalon ?
#else
	if(pid)
		pid->SetOutputRange(min,max);
#endif
}

void GPMotor::SetTolerance(double tol){
#if MOTORTYPE == CANTALON
	// TODO: implement equivalent function for a CANTalon
#else
	if(pid)
		pid->SetAbsoluteTolerance(tol);
#endif
}
void GPMotor::SetToleranceBuffer(unsigned bufLength){
#if MOTORTYPE == CANTALON
	// TODO: implement equivalent function for a CANTalon
#else
	if(pid)
		pid->SetAbsoluteTolerance(bufLength);
#endif
}

double GPMotor::GetTargetCorrection(){
#if MOTORTYPE == CANTALON
	// TODO: implement equivalent function for a CANTalon
#else
	if(pid)
		return pid->Get();
	else
		return 0;
#endif
}

double GPMotor::GetTargetError(){
#if MOTORTYPE == CANTALON
	// TODO: implement equivalent function for a CANTalon
#else
	if(pid)
		return pid->GetError();
	else
		return 0;
#endif
}

bool GPMotor::OnTarget(){
#if MOTORTYPE == CANTALON
	// TODO: implement equivalent function for a CANTalon
#else
	if(pid)
		return pid->OnTarget();
	else
		return false;
#endif
}

void GPMotor::Reset(){
#if MOTORTYPE == CANTALON
	// TODO: implement equivalent function for a CANTalon
#else
	if(encoder)
		encoder->Reset();
	if(pid)
		pid->Reset();
	ClearIaccum();
#endif
}
void GPMotor::SetDistancePerPulse(double target){
#if MOTORTYPE == CANTALON
	// TODO: implement equivalent function for a CANTalon
#else
	if(encoder)
		encoder->SetDistancePerPulse(target);
#endif
}

void GPMotor::SetDebug(int b){
	debug=b;
}
