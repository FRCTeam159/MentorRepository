#include "DriveToRangedTarget.h"
#include "Robot.h"

DriveToRangedTarget::DriveToRangedTarget(double distance) {
	Requires(Robot::drivetrain);
	target=distance;
    pid = new PIDController(-2, 0, 0, new DriveTrainPIDSource(),
    		                          new DriveTrainPIDOutput());
    pid->SetPercentTolerance(pc_tol);
    pid->SetSetpoint(distance);
}

// Called just before this Command runs the first time
void DriveToRangedTarget::Initialize() {
	// Get everything in a safe starting state.
    Robot::drivetrain->ResetWheels();
	pid->Reset();
    pid->Enable();
}

// Called repeatedly when this Command is scheduled to run
void DriveToRangedTarget::Execute() {}

// Make this return true when this Command no longer needs to run execute()
bool DriveToRangedTarget::IsFinished() {
	double d=Robot::drivetrain->GetDistanceToObstacle();
	double err=100.0*(d-target)/target;
	if(fabs(err)<pc_tol)
		return true;
	return false;
}

// Called once after isFinished returns true
void DriveToRangedTarget::End() {
	// Stop PID and the wheels
	pid->Disable();
	Robot::drivetrain->Stop();
    Robot::drivetrain->ResetWheels();
	std::cout<< "DriveToRangedTarget:OnTarget:"<<Robot::drivetrain->GetDistanceToObstacle()
			<<" total:"<<Robot::drivetrain->GetTotalDistance()<<std::endl;
}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void DriveToRangedTarget::Interrupted() {
	End();
}


DriveTrainPIDSource::~DriveTrainPIDSource() {}
double DriveTrainPIDSource::PIDGet() {
	double d=Robot::drivetrain->GetDistanceToObstacle();
	 //std::cout<< "DriveToRangedTarget:"<<d<<std::endl;
    return d;
}

DriveTrainPIDOutput::~DriveTrainPIDOutput() {}
void DriveTrainPIDOutput::PIDWrite(float d) {
    Robot::drivetrain->DriveStraight(d);
}
