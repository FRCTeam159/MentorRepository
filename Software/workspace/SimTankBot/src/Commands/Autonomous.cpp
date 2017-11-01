#include "Autonomous.h"
#include "Commands/DriveStraight.h"
#include "Commands/DrivePath.h"


Autonomous::Autonomous() {
	AddSequential(new DrivePath());
}
