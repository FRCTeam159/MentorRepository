#include "Pickup.h"
#include "CloseClaw.h"
#include "SetElevatorSetpoint.h"

#include <iostream>

Pickup::Pickup() : CommandGroup("Pickup") {
	AddSequential(new CloseClaw());
	AddSequential(new SetElevatorSetpoint(0.25));
}
