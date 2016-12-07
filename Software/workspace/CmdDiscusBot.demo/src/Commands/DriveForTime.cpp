#include <Commands/DriveForTime.h>

DriveForTime::DriveForTime(double t,double s)
{
	duration=t;
	speed=s;
	// Use Requires() here to declare subsystem dependencies
	Requires(drivetrain.get());
}

// Called just before this Command runs the first time
void DriveForTime::Initialize()
{
	double start_time=Timer::GetFPGATimestamp();
	stop_time=start_time+duration;
	std::cout << "DriveForTime::Started"<<std::endl;
}

// Called repeatedly when this Command is scheduled to run
void DriveForTime::Execute()
{
	drivetrain.get()->Drive(speed,0,0);
}

// Make this return true when this Command no longer needs to run execute()
bool DriveForTime::IsFinished()
{
	if(Timer::GetFPGATimestamp()>=stop_time)
		return true;
	return false;
}

// Called once after isFinished returns true
void DriveForTime::End()
{
	std::cout << "DriveForTime::End"<<std::endl;
}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void DriveForTime::Interrupted()
{

}
