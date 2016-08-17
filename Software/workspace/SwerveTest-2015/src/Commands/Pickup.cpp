#include <Commands/StepElevatorLevel.h>
#include "Pickup.h"
#include "CloseClaw.h"
#include <iostream>

Pickup::Pickup() : CommandGroup("Pickup") {
	AddSequential(new CloseClaw());
	AddSequential(new StepElevatorLevel(1));
}
