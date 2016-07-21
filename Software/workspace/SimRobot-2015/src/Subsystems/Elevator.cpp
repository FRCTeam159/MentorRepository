#include "Elevator.h"
#include "SmartDashboard/SmartDashboard.h"
#include "LiveWindow/LiveWindow.h"

Elevator::Elevator(int i) : SmartMotor(i), Subsystem("Elevator") {
	SetPID(POSITION, 4.0, 0.05, 1.0);
	SetRange(0,1.4,0.2); // set-up for 7 levels
    //SetDistancePerPulse(1.0/360);
    SetDistancePerPulse(6.0/360);
    SetPercentTolerance(5.0);
    Reset();
}

void Elevator::Log() {
    // TODO: SmartDashboard::PutData(..);
}

void Elevator::SetRange(double mn, double mx, double stp) {
	min=mn;max=mx;step=stp;
	nsteps=(int)((max-min)/stp);
	SetInputRange(min,max);
}

void Elevator::SetElevatorLevel(double n){
	double d=min+n*step;
	SetDistance(d);
}
double Elevator::GetElevatorLevel(){
	double d=GetDistance();
	return (d-min)/step;
}
