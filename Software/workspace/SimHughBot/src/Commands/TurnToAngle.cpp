#include "TurnToAngle.h"


#define P 0.1
#define I 0.005
#define D 0.0

#define WIDTH 25
#define LENGTH 25


TurnToAngle::TurnToAngle(double a) : CommandBase("DriveStraight"),
	pid(P,I,D,this,this)
{
	Requires(driveTrain.get());
	angle = a;
	std::cout << "new TurnToAngle("<<a<<")"<< std::endl;
}

// Called just before this Command runs the first time
void TurnToAngle::Initialize() {
	double d = driveTrain->GetDistance();
	driveTrain->Reset();
	driveTrain->SetDistance(d);
  	pid.Reset();
  	radius = 0.5*sqrt(WIDTH*WIDTH+LENGTH*LENGTH);// radius of robot bounding circle
	double a = 2*angle*M_PI*radius/360; // arc length given radius and turn angle
  	pid.SetSetpoint(a);
	pid.SetAbsoluteTolerance(0.5);
	pid.Enable();
	driveTrain->Enable();
	std::cout << "TurnToAngle Started: "<< a <<std::endl;
}

// Called repeatedly when this Command is scheduled to run
void TurnToAngle::Execute() {

}

// Make this return true when this Command no longer needs to run execute()
bool TurnToAngle::IsFinished() {
	return pid.OnTarget();
}

// Called once after isFinished returns true
void TurnToAngle::End() {
	pid.Disable();
    std::cout << "TurnToAngle End" << std::endl;
}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void TurnToAngle::Interrupted() {
	End();
}
double TurnToAngle::PIDGet() {
	double l=driveTrain->GetLeftDistance();
	double r=driveTrain->GetRightDistance();
	double d = 0.5*sqrt(l*l+r*r);

    d= angle <0? - d: d;
    double a = d*360/M_PI/radius/2;

    driveTrain->SetAngle(a);

	std::cout << "TurnToAngle::PIDGet("<<d<<","<<a<<")"<<std::endl;
	return d;
}

void TurnToAngle::PIDWrite(double a) {

    std::cout << "TurnToAngle::PIDWrite("<<a<<")"<< std::endl;

	driveTrain->TankDrive(a,-a);
}
