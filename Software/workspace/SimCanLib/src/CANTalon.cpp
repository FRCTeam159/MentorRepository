/*
 * CANTalon.cpp
 *
 *  Created on: April 16, 2016
 *      Author: Dean Sindorf
 *  Adds simulation support for CANTalon motor controllers
 */
#include "WPILib.h"
#include <CANTalon.h>


CANTalon::PIDData::PIDData(){
	P=I=D=F=0; changed=false;pid=0;
}
void CANTalon::PIDData::SetPID(double P, double I, double D, double F, PIDSource *s,PIDOutput *d){
	SetF(F);
	SetPID(P,I,D,s,d);
}
void CANTalon::PIDData::SetPID(double P, double I, double D, PIDSource *s,PIDOutput *d){
	SetP(P);
	SetI(I);
	SetD(D);
	Set(s,d);
}
void CANTalon::PIDData::Set(PIDSource *s,PIDOutput *d){
	if(changed && pid)
		Clear();
	if(!pid)
		pid=new PIDController(P,I,D,F,s,d,SIMPIDRATE);
	changed=false;
}
void CANTalon::PIDData::Clear(){
	if(pid)
		delete pid;
	pid=0;
}
void CANTalon::PIDData::SetP(double d){
	if(d!=P)
		changed=true;
	P=d;
}
void CANTalon::PIDData::SetI(double d){
	if(d!=I)
		changed=true;
	I=d;
}
void CANTalon::PIDData::SetD(double d){
	if(d!=D)
		changed=true;
	D=d;
}
void CANTalon::PIDData::SetF(double d){
	if(d!=F)
		changed=true;
	F=d;
}
void CANTalon::PIDData::SetSetpoint(double value){
	if(pid)
		pid->SetSetpoint(value);
}
bool CANTalon::PIDData::IsEnabled(){
	if(pid)
		return pid->IsEnabled();
	else
		return false;
}
void CANTalon::PIDData::Enable(){
	if(pid)
		pid->Enable();
}
void CANTalon::PIDData::Disable(){
	if(pid)
		pid->Disable();
}
void CANTalon::PIDData::Reset(){
	if(pid)
		pid->Reset();
}
double CANTalon::PIDData::GetTargetError(){
	if(pid)
		return pid->GetError();
	else
		return 0;
}
double CANTalon::PIDData::GetSetpoint(){
	if(pid)
		return pid->GetSetpoint();
	else
		return 0;
}
bool CANTalon::PIDData::OnTarget(){
	if(pid)
		return pid->OnTarget();
	else
		return false;
}
double CANTalon::PIDData::GetP() { return P;}
double CANTalon::PIDData::GetI() { return I;}
double CANTalon::PIDData::GetD() { return D;}
double CANTalon::PIDData::GetF() { return F;}

CANTalon::CANTalon(int i) : Talon(i){
	pid_data[0].pid=0;
	pid_data[1].pid=0;
	encoder=0;
	id=i;
	debug=0;
	control_mode=kPercentVbus;
	inverted=false;
}

CANTalon::~CANTalon(){
	if(encoder)
		delete encoder;
	ClearPID();
}
void CANTalon::SelectProfileSlot(int i) {
	pid_channel=i>=1?1:0;
}

void CANTalon::ClearPID(){
	for(int i=0;i<2;i++){
		pid_data[i].Clear();
	}
}
void CANTalon::SetP(double d){
	pid_data[pid_channel].SetP(d);
}
void CANTalon::SetI(double d){
	pid_data[pid_channel].SetI(d);
}
void CANTalon::SetD(double d){
	pid_data[pid_channel].SetD(d);
}
void CANTalon::SetF(double d){
	pid_data[pid_channel].SetF(d);
}
void CANTalon::SetPID(double P, double I, double D){
	pid_data[pid_channel].SetPID(P,I,D,this,this);
}
void CANTalon::SetPID(double P, double I, double D,double F){
	pid_data[pid_channel].SetPID(P,I,D,F,this,this);
}
bool CANTalon::OnTarget() {
	return pid_data[pid_channel].OnTarget();
}
double CANTalon::GetTargetError() {
	return pid_data[pid_channel].GetTargetError();
}
double CANTalon::GetSetpoint() {
	return pid_data[pid_channel].GetSetpoint();
}
void CANTalon::ClearIaccum(){
	pid_data[pid_channel].Reset(); // clears accumulator but also disables
}
double CANTalon::GetP(){
	return pid_data[pid_channel].GetP();
}
double CANTalon::GetI(){
	return pid_data[pid_channel].GetI();
}
double CANTalon::GetD(){
	return pid_data[pid_channel].GetD();
}
double CANTalon::GetF(){
	return pid_data[pid_channel].GetF();
}

void CANTalon::SetControlMode(ControlMode m) {
	if(m>=kMaxControlMode){
		std::cout<<"ERROR mode unsupported in simulation:"<<m<<std::endl;
		return;
	}
	control_mode=m;
	if((m!=kPosition) && (m!=kSpeed)){
		ClearPID();
	}
	else{
		PIDSourceType pidMode=(control_mode==kPosition)? PIDSourceType::kDisplacement:PIDSourceType::kRate;
		SetPIDSourceType(pidMode);
		if(encoder)
			encoder->SetPIDSourceType(pidMode);
	}
}
bool CANTalon::IsModePID(ControlMode mode) {
  return (mode == kSpeed) || (mode == kPosition);
}

void CANTalon::SetFeedbackDevice(FeedbackDevice device){
	if(encoder==0 && device==QuadEncoder){
		int ival=(id-1)*2+1; // 1,3,5,..
		encoder=new Encoder(ival,ival+1); // {1,2} {3,4} {5,6} ..
		//encoder->SetDistancePerPulse(1.0/SIM_ENCODER_TICKS);
	}
	else if(encoder){
		delete encoder;
		encoder=0;
	}
}
double CANTalon::ReturnPIDInput(){
	switch(control_mode){
	case kPosition:
		return GetPosition();
	case kSpeed:
		return GetSpeed();
	default:
	case kPercentVbus:
		return GetVoltage();
	}
}
double CANTalon::PIDGet(){
	double d= ReturnPIDInput();
	if(IsEnabled() && (debug & 2))
		std::cout<<id<<" PIDGet:"<<d<<" setpoint:"<<GetSetpoint()<<std::endl;
	return d;
}

void CANTalon::PIDWrite(float output){
	if(IsEnabled() && (debug & 1))
		std::cout<<id<<" PIDWrite: target:"<<GetSetpoint()<<" error:"<<GetTargetError()<<" correction:"<<output<<std::endl;
	Talon::SetSpeed(output);
}

void CANTalon::SetSetpoint(double value){
	pid_data[pid_channel].SetSetpoint(value);
}
void CANTalon::SetVelocity(double value){
	pid_data[pid_channel].SetSetpoint(value);
	if(debug)
		std::cout<<id<<" CANTalon::SetVelocity:"<<value<<std::endl;
}
void CANTalon::SetPosition(double value){
	if(debug)
		std::cout<<id<<" CANTalon::SetPosition:"<<value<<std::endl;
	pid_data[pid_channel].SetSetpoint(value);
}

// in simulation rate is in degrees/second
double CANTalon::GetSpeed(){
	if(encoder)
		return encoder->GetRate();
	else
		return 0;
}

double CANTalon::GetPosition(){
	if(encoder)
		return encoder->GetDistance();
	else{
		std::cout<<"ERROR CANTalon::GetPosition() encoder=NULL"<<std::endl;
		return 0;
	}
}

void CANTalon::Set(double value){
	Talon::Set(value,0);
}
double CANTalon::GetVoltage(){
	return Talon::Get();
}

void CANTalon::EnableControl(){
	if(IsModePID(control_mode))
		pid_data[pid_channel].Set(this,this);
	pid_data[pid_channel].Enable();
}

void CANTalon::Enable(){
	EnableControl();
}

bool CANTalon::IsEnabled(){
	return pid_data[pid_channel].IsEnabled();
}
void CANTalon::Disable(){
	Talon::Disable();
	pid_data[pid_channel].Disable();
}

void CANTalon::SetInverted(bool t) {
	inverted=t;
	// This causes purposefully thrown exception in simulation
	// if(encoder)
	//	 encoder->SetReverseDirection(t);
}

double CANTalon::GetTargetCorrection(){
	if(pid_data[pid_channel].pid)
		return pid_data[pid_channel].pid->Get();
	else
		return 0;
}

void CANTalon::Reset(){
	if(encoder)
		encoder->Reset();
	pid_data[pid_channel].Reset();
}
void CANTalon::ConfigEncoderCodesPerRev(uint16_t codesPerRev){
	if(encoder)
		encoder->SetDistancePerPulse((double)(1.0/codesPerRev));
}

int CANTalon::GetEncVel() {
	return 0;
}

void CANTalon::SetEncPosition(int int1) {
}

int CANTalon::GetClosedLoopError() const {
	return 0;
}

void CANTalon::SetAllowableClosedLoopErr(uint32_t allowableCloseLoopError) {
}

void CANTalon::SetDebug(int b){
	debug=b;
}

