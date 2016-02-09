/*
FIRST FRC Team 4237, Sir Lance-A-Bot, Stevensville, Michigan
Minimal example of acquiring distance with PulsedLight LIDAR Lite v2.
Using FIRST FRC C++ WPILib from early 2015 for the NI roboRIO.
Our interpretation of the LIDAR Lite v2 documentation as of 12/2015.
We are not responsible for your usage or if we didn't understand correctly or if this is incomplete.
*/

#include <wpilib.h>

class Lidar
{
public:
	Lidar();
	void AquireDistance(Timer*);
};

class Robot: public SampleRobot
{
public:
	Robot();
	void OperatorControl();
	Timer* mTimer;
	Lidar* mLidar;
};

Robot::Robot()
{
	mTimer = new Timer();
	mLidar = new Lidar;
	Wait(1.);
}

void Robot::OperatorControl()
{
	mTimer->Start();
	mTimer->Reset();

	while (IsOperatorControl())
	{
		mLidar->AquireDistance(mTimer);
	}
}

START_ROBOT_CLASS(Robot);
