#include <Commands/DriveForTime.h>
#include <Commands/DriveToPosition.h>
#include <Commands/SetLifterPosition.h>
#include <Commands/InitAuto.h>

#include "Autonomous.h"

const double DURATION=2.0;
const double POSITION=5; // 5 feet
const double SPEED=0.5;

Autonomous::Autonomous()
{
	//AddSequential(new DriveForTime(DURATION,SPEED));
	//AddSequential(new DriveToPosition(POSITION,SPEED));
	AddSequential(new InitAuto());
	AddSequential(new SetLifterPosition(0.5));
	AddSequential(new SetLifterPosition(0.2));

	SetInterruptible(true);

}
