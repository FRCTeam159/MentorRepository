#include "Elevator.h"
#include "RobotMap.h"
#include "Commands/ControlElevator.h"


Elevator::Elevator() : Subsystem("Elevator") {
    motor = new CANTalon(LIFTER);
	std::cout<<"New Elevator("<<LIFTER<<")"<<std::endl;
 }

void Elevator::InitDefaultCommand(){
	SetDefaultCommand(new ControlElevator());
}
void Elevator::Log() {
}
void Elevator::SetSpeed(float f){
	motor->Set(f,0);
}
