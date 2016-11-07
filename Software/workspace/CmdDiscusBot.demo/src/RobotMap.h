#ifndef ROBOTMAP_H
#define ROBOTMAP_H

#include "WPILib.h"

/**
 * The RobotMap is a mapping from the ports sensors and actuators are wired into
 * to a variable name. This provides flexibility changing wiring, makes checking
 * the wiring easier and significantly reduces the number of magic numbers
 * floating around.
 */
 
const int FRONT_LEFT=1;
const int FRONT_RIGHT=2;
const int BACK_LEFT=3;
const int BACK_RIGHT=4;

const int LIFTER=5;
const int FLYWHEEL=6;

#ifdef REAL
const int LIFTER_UP_BUTTON=5;
const int LIFTER_DOWN_BUTTON=6;
const int FLYWHEEL_BUTTON=2;
const int SHOOTER_BUTTON=1;

#else
const int LIFTER_UP_BUTTON=3;
const int LIFTER_DOWN_BUTTON=2;
const int FLYWHEEL_BUTTON=1;
const int SHOOTER_BUTTON=4;

#endif

const double LIFTER_UP_SPEED=0.5;
const double LIFTER_DOWN_SPEED=-0.5;

const int PISTON=7;
const int GYRO=3;

const double FLYWHEEL_SPEED=1.0;


// For example to map the left and right motors, you could define the
// following variables to use with your drivetrain subsystem.
//const int LEFTMOTOR = 1;
//const int RIGHTMOTOR = 2;

// If you are using multiple modules, make sure to define both the port
// number and the module. For example you with a rangefinder:
//const int RANGE_FINDER_PORT = 1;
//const int RANGE_FINDER_MODULE = 1;

#endif
