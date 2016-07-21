#include <Commands/StepElevatorLevel.h>
#include "Place.h"
#include "OpenClaw.h"
#include <iostream>

Place::Place() : CommandGroup("Place") {
	//AddSequential(new SetElevatorSetpoint(0.25));
    //AddSequential(new OpenClaw());
}
