#include "Autonomous.h"
#include "PrepareToPickup.h"
#include "Pickup.h"
#include "Place.h"
#include "DriveStraight.h"
#include "CloseClaw.h"

#include <iostream>

Autonomous::Autonomous() : CommandGroup("Autonomous") {
	AddSequential(new PrepareToPickup());
    AddSequential(new Pickup());
    AddSequential(new DriveStraight(4)); // Use Encoders if ultrasonic is broken
    AddSequential(new Place());
    AddSequential(new DriveStraight(-2)); // Use Encoders if ultrasonic is broken
    AddSequential(new CloseClaw());
}
