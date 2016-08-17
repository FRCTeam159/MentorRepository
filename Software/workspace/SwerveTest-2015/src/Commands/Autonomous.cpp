#include <Commands/DriveStraight.h>
#include <Commands/DriveToRangedTarget.h>

#include <Commands/Rotate.h>
#include <Commands/OpenClaw.h>
#include <Commands/CloseClaw.h>
#include <Commands/SetElevatorLevel.h>

#include "Autonomous.h"
#include <iostream>

#define ROTATE_TIMEOUT 4.0
#define DRIVE_TIMEOUT 2.0
Autonomous::Autonomous() : CommandGroup("Autonomous") {
    AddSequential(new OpenClaw());
    AddSequential(new DriveToRangedTarget(0.8),3.0);

    //AddSequential(new DriveStraight(5.0),DRIVE_TIMEOUT);
	AddSequential(new CloseClaw());
	AddSequential(new SetElevatorLevel(1));
    AddSequential(new Rotate(90.0),ROTATE_TIMEOUT);
    AddSequential(new DriveStraight(5.0),DRIVE_TIMEOUT);
    AddSequential(new Rotate(180.0),ROTATE_TIMEOUT);
    AddSequential(new DriveStraight(5.0),DRIVE_TIMEOUT);
    AddSequential(new Rotate(-90.0),ROTATE_TIMEOUT);
    AddSequential(new DriveStraight(5.0),DRIVE_TIMEOUT);
    AddSequential(new Rotate(0.0),ROTATE_TIMEOUT);
	AddSequential(new SetElevatorLevel(0));
    AddSequential(new OpenClaw());
}
