#include "Rotate.h"
#include "Robot.h"
#include <math.h>

Rotate::Rotate(double a) {
	Requires(Robot::drivetrain);
	angle=a;
    pid = new PIDController(1, 0.1, 0.001, new RotatePIDSource(),
    		                         new RotatePIDOutput());
    pid->SetAbsoluteTolerance(2.0/360.0);
    pid->SetSetpoint(angle/360.0);

}

// Called just before this Command runs the first time
void Rotate::Initialize() {
	// Get everything in a safe starting state.
    //Robot::drivetrain->Reset();
	pid->Reset();
    pid->Enable();
    std::cout<< "Rotate:"<<angle<<std::endl;
}

// Called repeatedly when this Command is scheduled to run
void Rotate::Execute() {}

// Make this return true when this Command no longer needs to run execute()
bool Rotate::IsFinished() {
	return pid->OnTarget();
}

// Called once after isFinished returns true
void Rotate::End() {
	// Stop PID and the wheels
	pid->Disable();
	Robot::drivetrain->Stop();
	std::cout<< "Rotate:Ontarget:"<<Robot::drivetrain->GetHeading()<<std::endl;
}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void Rotate::Interrupted() {
	End();
}

RotatePIDSource::~RotatePIDSource() {}
double RotatePIDSource::PIDGet() {
	double v=Robot::drivetrain->GetRotation();
	//std::cout<< "Rotate:PIDGet:"<<v<<std::endl;
    return v/360.0;
}

RotatePIDOutput::~RotatePIDOutput() {}
void RotatePIDOutput::PIDWrite(float d) {
	//std::cout<< "Rotate:PIDWrite:"<<d<<std::endl;
    Robot::drivetrain->Rotate(-d);

}
