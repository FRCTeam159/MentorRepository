#include <Commands/DriveForTime.h>
#include <Commands/DriveToPosition.h>

#include "Autonomous.h"

const double DURATION=2.0;
const double POSITION=5; // 5 feet
const double SPEED=0.5;
Autonomous::Autonomous()
{
	//AddSequential(new DriveForTime(DURATION,SPEED));
	AddSequential(new DriveToPosition(POSITION,SPEED));
}
