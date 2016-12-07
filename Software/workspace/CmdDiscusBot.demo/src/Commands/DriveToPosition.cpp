#include "DriveToPosition.h"

DriveToPosition::DriveToPosition(double p, double s)
{
	// Use Requires() here to declare subsystem dependencies
	// eg. Requires(chassis);
	Requires(drivetrain.get());

	position = p;
	speed=s;

}

// Called just before this Command runs the first time
void DriveToPosition::Initialize()
{
	double ave=drivetrain.get()->GetPosition();
	std::cout << "DriveToPosition::Started initial position="<<ave<<std::endl;
}

// Called repeatedly when this Command is scheduled to run
void DriveToPosition::Execute()
{
	drivetrain.get()->Drive(speed,0,0);
	double ave=drivetrain.get()->GetPosition();
	double l=drivetrain.get()->GetLeftPosition();
	double r=drivetrain.get()->GetRightPosition();

	std::cout<<"position l:"<<l<<" r:"<<r<<" ave:"<<ave<<std::endl;
}

// Make this return true when this Command no longer needs to run execute()
bool DriveToPosition::IsFinished()
{
	//return false;
	double ave=drivetrain.get()->GetPosition();
	return ave >= position ? true:false;
}

// Called once after isFinished returns true
void DriveToPosition::End()
{
	double ave=drivetrain.get()->GetPosition();
	std::cout << "DriveToPosition::Finished pos="<<ave<<" target="<<position<<std::endl;

}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void DriveToPosition::Interrupted()
{

}
