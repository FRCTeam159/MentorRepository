#include "Commands/DriveForTime.h"

#include "../CommandBase.h"
#include "WPILib.h"

DriveForTime::DriveForTime(double t, double s)
{
	// Use Requires() here to declare subsystem dependencies
	// eg. Requires(chassis);
	Requires(driveTrain.get());
	time = t;
	speed = s;
}

// Called just before this Command runs the first time
void DriveForTime::Initialize()
{
	targetTime = Timer::GetFPGATimestamp() + time;
}

// Called repeatedly when this Command is scheduled to run
void DriveForTime::Execute()
{
	driveTrain->TankDrive(speed, speed);
//	Vision::TargetInfo targetInfo =visionSubsystem->targetInfo;
//	cout<<"target angle error:"<<targetInfo.HorizontalError<<" distance:"<< targetInfo.Distance<<endl;
}

// Make this return true when this Command no longer needs to run execute()
bool DriveForTime::IsFinished()
{
//	currentTime = Timer::GetFPGATimestamp();
//	if(currentTime >= targetTime)
//	{
//		return true;
//	}
//	else
//	{
//	return false;
//	}
	return false;
}

// Called once after isFinished returns true
void DriveForTime::End()
{
	//driveTrain->StopMotor();

}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void DriveForTime::Interrupted()
{

}
