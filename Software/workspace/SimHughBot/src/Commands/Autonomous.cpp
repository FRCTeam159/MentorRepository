#include "Autonomous.h"
#include "Commands/DriveToTarget.h"
#include "Commands/VisionUpdate.h"

Autonomous::Autonomous() {
	// Add Commands here:
	// e.g. AddSequential(new Command1());
	//      AddSequential(new Command2());
	// these will run in order.

	// To run multiple commands at the same time,
	// use AddParallel()
	AddParallel(new VisionUpdate());
	AddSequential(new DriveToTarget());
}
