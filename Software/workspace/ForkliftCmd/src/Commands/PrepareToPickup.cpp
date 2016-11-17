#include "PrepareToPickup.h"
#include "OpenClaw.h"
#include "SetElevatorSetpoint.h"

#include <iostream>

PrepareToPickup::PrepareToPickup() : CommandGroup("PrepareToPickup") {
	AddParallel(new OpenClaw());
	AddSequential(new SetElevatorSetpoint(0));
}
