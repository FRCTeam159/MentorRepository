#include "DriveStraight.h"

#define P 1.0
#define I 0.0
#define D 0.0
#define TOL 0.05

//#define DEBUG_COMMAND
#define TIME_COMMAND

static int cycle_count=0;

DriveStraight::DriveStraight(double d)  : CommandBase("DriveStraight"),
	pid(P,I,D,this,this)
{
	Requires(driveTrain.get());
	distance=d;
	tolerance=TOL;
    std::cout << "new DriveStraight("<<d<<")"<< std::endl;
    frc::SmartDashboard::PutNumber("P",P);
    frc::SmartDashboard::PutNumber("I",I);
    frc::SmartDashboard::PutNumber("D",D);
    frc::SmartDashboard::PutNumber("TOL",TOL);
    frc::SmartDashboard::PutNumber("DIST",distance);
}

// Called just before this Command runs the first time
void DriveStraight::Initialize() {
	double p = frc::SmartDashboard::GetNumber("P",P);
	double i = frc::SmartDashboard::GetNumber("I",I);
	double d = frc::SmartDashboard::GetNumber("D",D);
	tolerance=frc::SmartDashboard::GetNumber("TOL",TOL);
	distance=frc::SmartDashboard::GetNumber("DIST",distance);
	last_target=false;
	pid.SetPID(p,i,d);
	driveTrain->Reset();
  	pid.Reset();
	pid.SetSetpoint(distance);
	pid.SetAbsoluteTolerance(tolerance);
	pid.Enable();
	//pid.SetToleranceBuffer(5);
	driveTrain->Enable();
	std::cout << "DriveStraight Started .."<< std::endl;
	cycle_count=0;
}

// Called repeatedly when this Command is scheduled to run
void DriveStraight::Execute() {
}

// Make this return true when this Command n o longer needs to run execute()
bool DriveStraight::IsFinished() {
	bool new_target=pid.OnTarget();
	if(new_target && last_target)
		return true;
	last_target=new_target;
	return false;
}

// Called once after isFinished returns true
void DriveStraight::End() {
	pid.Disable();
    std::cout << "DriveStraight End" << std::endl;
}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void DriveStraight::Interrupted() {
    std::cout << "DriveStraight Interrupted" << std::endl;
	End();
}

double DriveStraight::PIDGet() {
	double s=driveTrain->GetDistance();
#ifdef DEBUG_COMMAND
    std::cout << "DriveStraight::PIDGet("<<s<<")"<< std::endl;
#endif
	return s;
}

void DriveStraight::PIDWrite(double d) {
#ifdef DEBUG_COMMAND
    std::cout << "DriveStraight::PIDWrite("<<d<<")"<< std::endl;
#endif
#ifdef TIME_COMMAND
    if(cycle_count>0){
     	std::cout <<cycle_count<<" "<<d<<endl;
    }
    cycle_count++;
#endif
	driveTrain->TankDrive(d,d);
}
