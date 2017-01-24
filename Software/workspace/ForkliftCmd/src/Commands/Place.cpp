#include "Place.h"
#include "OpenClaw.h"
#include "SetElevatorSetpoint.h"

#include <iostream>

Place::Place() : CommandGroup("Place") {
	AddSequential(new SetLifterPosition(0.25));
    AddSequential(new OpenClaw());
}
