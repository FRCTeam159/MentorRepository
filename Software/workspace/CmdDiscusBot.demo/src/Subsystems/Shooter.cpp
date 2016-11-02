/*
 * Shooter.cpp
 *
 *  Created on: Oct 28, 2016
 *      Author: dean
 */

#include <Subsystems/Shooter.h>
#include <Commands/ControlShooter.h>
#include "RobotMap.h"

#define DEBUG_SHOOTER

Shooter::Shooter() : Subsystem("Shooter") {
	std::cout<<"New Shooter()"<<std::endl;
	flywheel=new CANTalon(FLYWHEEL_MOTOR);
	piston=new DoubleSolenoid(PISTON,0,1);
	flywheel_speed=0;
}

void Shooter::InitDefaultCommand() {
	SetDefaultCommand(new ControlShooter());
}

void Shooter::PistonIn() {
#ifdef DEBUG_SHOOTER
	if(!piston_in)
		std::cout<<"setting piston in"<<std::endl;
#endif
	piston->Set(DoubleSolenoid::kReverse);
	piston_in=true;
}

void Shooter::PistonOut() {
#ifdef DEBUG_SHOOTER
	if(piston_in)
		std::cout<<"setting piston out"<<std::endl;
#endif
	piston->Set(DoubleSolenoid::kForward);
	piston_in=false;
}

void Shooter::SetFlywheelSpeed(float s) {
#ifdef DEBUG_SHOOTER
	if(s != flywheel_speed)
		std::cout<<"setting flywheel speed = "<<s<<std::endl;
#endif
	flywheel->Set(s);
	flywheel_speed=s;
}
