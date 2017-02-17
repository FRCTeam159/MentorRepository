#include "Turn.h"

static double initTime=0;
#define ENDTIME 2.0
Turn::Turn(double a) {
	angle=a;
	Requires(driveTrain.get());
	std::cout << "new Turn("<<angle<<")"<< std::endl;
}

// Called just before this Command runs the first time
void Turn::Initialize() {
	std::cout << "Turn Started .."<< std::endl;
	initTime = Timer::GetFPGATimestamp();

}

// Called repeatedly when this Command is scheduled to run
void Turn::Execute() {
	driveTrain->TankDrive(angle, -angle);
}

// Make this return true when this Command no longer needs to run execute()
bool Turn::IsFinished() {
	double newtime=Timer::GetFPGATimestamp();
	if(newtime>=initTime+ENDTIME)
		return true;
	return false;
}

// Called once after isFinished returns true
void Turn::End() {
	std::cout << "Turn End"<< std::endl;
}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void Turn::Interrupted() {
	End();
}
