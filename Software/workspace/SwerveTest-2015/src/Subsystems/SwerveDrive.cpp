#include "SwerveDrive.h"

#include "Joystick.h"
#include "Utility.h"
#include "WPIErrors.h"
#include <math.h>

SwerveDrive::SwerveDrive(SpeedController *FL, SpeedController *RL, SpeedController *FR, SpeedController *RR) : RobotDrive(FL,RL,FR,RR)
{
	m_frontLeftWheel = (PivotWheel*)FL;
	m_rearLeftWheel = (PivotWheel*)RL;
	m_frontRightWheel = (PivotWheel*)FR;
	m_rearRightWheel = (PivotWheel*)RR;
	angle=0;
	squared_inputs=true;

}

void SwerveDrive::PivotDrive(float x, float y, float r, float g)
{
	if(fabs(r)<0.1){
		DriveAtAngle(x,y);
		return;
	}
	Rotate(r,45.0);
}

void SwerveDrive::Rotate(float speed)
{
	Rotate(speed,45.0);
}

void SwerveDrive::Rotate(float speed, float angle)
{
    m_frontLeftWheel->SetAngle(-angle);
	m_rearLeftWheel->SetAngle(angle);

	m_frontRightWheel->SetAngle(angle);
	m_rearRightWheel->SetAngle(-angle);

    m_frontLeftWheel->SetMotorSpeed(-speed);
	m_rearLeftWheel->SetMotorSpeed(-speed);

	m_frontRightWheel->SetMotorSpeed(-speed);
	m_rearRightWheel->SetMotorSpeed(-speed);

}
void SwerveDrive::DriveAtAngle(float x, float y)
{
	double speed=sqrt(x*x+y*y);
	speed=x<0?-speed:speed;
	double dir = 90*y;
	SetAngle(dir);
	Drive(speed);
}
void SwerveDrive::Enable()
{
    m_frontLeftWheel->Enable();
	m_rearLeftWheel->Enable();
	m_frontRightWheel->Enable();
	m_rearRightWheel->Enable();
}
void SwerveDrive::Disable()
{
    m_frontLeftWheel->Disable();
	m_rearLeftWheel->Disable();
	m_frontRightWheel->Disable();
	m_rearRightWheel->Disable();
}

void SwerveDrive::SetAngle(float dir)
{
	angle=dir;
    m_frontLeftWheel->SetAngle(angle);
	m_rearLeftWheel->SetAngle(angle);
	m_frontRightWheel->SetAngle(angle);
	m_rearRightWheel->SetAngle(angle);
}

void SwerveDrive::Drive(float speed)
{
    m_frontLeftWheel->SetMotorSpeed(speed* m_invertedMotors[kFrontLeftMotor]);
	m_rearLeftWheel->SetMotorSpeed(speed*m_invertedMotors[kRearLeftMotor]) ;
	m_frontRightWheel->SetMotorSpeed(speed* m_invertedMotors[kFrontRightMotor]);
	m_rearRightWheel->SetMotorSpeed(speed* m_invertedMotors[kRearRightMotor]);
}

void SwerveDrive::SetDriveStraight(){
    m_frontLeftWheel->SetAngle(0);
	m_frontRightWheel->SetAngle(0);
	m_rearLeftWheel->SetAngle(0);
	m_rearRightWheel->SetAngle(0);
}
void SwerveDrive::SetDriveSideways(){
    m_frontLeftWheel->SetAngle(90);
	m_frontRightWheel->SetAngle(90);
	m_rearLeftWheel->SetAngle(90);
	m_rearRightWheel->SetAngle(90);
}

double SwerveDrive::GetDistance(){
	double distance=0;
	distance+=m_frontLeftWheel->GetDistance();
	distance+=m_frontRightWheel->GetDistance();
	distance+=m_rearLeftWheel->GetDistance();
	distance+=m_rearRightWheel->GetDistance();
	return distance/4;
}
