#include "Autonomous.h"
#include "Commands/DriveStraight.h"

const double DURATION=2.0;
const double SPEED=0.5;
Autonomous::Autonomous()
{
	AddSequential(new DriveStraight(DURATION,SPEED));
}
