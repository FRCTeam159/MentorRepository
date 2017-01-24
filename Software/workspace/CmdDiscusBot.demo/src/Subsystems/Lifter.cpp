#include <Commands/ControlLifter.h>
#include <Subsystems/Lifter.h>
#include "RobotMap.h"

#ifdef REAL
#define FORWARDLIMIT_INCHES 47 // 1.2 meters
#define ENCODER_TICKS 64       // encoder ticks per revolution
#define WHEEL_DIAMETER 2.0     // inches belt moves per revolution = pi*diameter
#define FORWARDLIMIT_ROTATIONS (FORWARDLIMIT_INCHES/WHEEL_DIAMETER/M_PI)
#else
#define ENCODER_TICKS 360.0/2/M_PI  // seems to convert ticks to meters for sim-encoders (need to understand this better)
#define FORWARDLIMIT_ROTATIONS 1.2  // from sdf upper travel limit (in meters)
#endif

#define DEBUG_LIFTER

Lifter::Lifter() : Subsystem("Lifter"), motor(LIFTER)
{
	std::cout<<"New Elevator("<<LIFTER<<")"<<std::endl;
	motor.ConfigRevLimitSwitchNormallyOpen(false);
	motor.ConfigFwdLimitSwitchNormallyOpen(false);
	motor.ConfigEncoderCodesPerRev(ENCODER_TICKS); // converts all units to rotations
	motor.ConfigLimitMode(CANTalon::kLimitMode_SwitchInputsOnly);
	//motor.SetDebug(2);
 }

void Lifter::InitDefaultCommand(){
	SetDefaultCommand(new ControlLifter());
}
void Lifter::Log() {
}
void Lifter::SetSpeed(double f){
	motor.SetSpeed(f);
#ifdef DEBUG_LIFTER
	double p=motor.GetPosition();
	if(motor.IsFwdLimitSwitchClosed()){
		if(!at_fwd_limit)
			std::cout<<"Lifter - At Forward Limit"<<std::endl;
		at_fwd_limit=true;
		at_rev_limit=false;
	}
	else if(motor.IsRevLimitSwitchClosed()){
		if(!at_rev_limit)
			std::cout<<"Lifter - At Reverse Limit"<<std::endl;
		at_rev_limit=true;
		at_fwd_limit=false;
	}
	if(p>0.1)
		at_rev_limit=false;
	if(p< FORWARDLIMIT_ROTATIONS-0.1)
		at_fwd_limit=false;
//    if(!at_rev_limit && !at_fwd_limit)
//	    std::cout<<"position="<<p<<std::endl;
#endif
}

bool Lifter::FindZero() {
	if(found_zero)
		return true;
	else{
		std::cout<<"Elevator - searching for Reverse limit"<<std::endl;
		motor.Set(-0.5);
		if(motor.IsRevLimitSwitchClosed()){
			found_zero=true;
			//at_rev_limit=true;
			motor.ConfigLimitMode(CANTalon::kLimitMode_SoftPositionLimits);
			motor.SetPosition(0);
			motor.ConfigSoftPositionLimits(FORWARDLIMIT_ROTATIONS,0);
			return true;
		}
	}
	return false;
}

void Lifter::Reset() {
	found_zero=false;
	motor.ConfigLimitMode(CANTalon::kLimitMode_SwitchInputsOnly);
}

double Lifter::GetPosition(){
	double x = motor.GetPosition();
	return x;
}

