#ifndef ROBOTMAP_H
#define ROBOTMAP_H

#include "WPILib.h"
using namespace std;
/**
 * The RobotMap is a mapping from the ports sensors and actuators are wired into
 * to a variable name. This provides flexibility changing wiring, makes checking
 * the wiring easier and significantly reduces the number of magic numbers
 * floating around.
 */
 
// For example to map the left and right motors, you could define the
// following variables to use with your drivetrain subsystem.
const int FRONTLEFT = 1;
const int FRONTRIGHT = 4;
const int BACKLEFT = 2;
const int BACKRIGHT = 3;

const int STICK = 0;

const int LOWGEAR_BUTTON = 4;
const int HIGHGEAR_BUTTON = 3;

const int GEARSHIFTID=0;

#define TANK       1  // 2-axis left side, right side
#define ARCADE2    2  // 2-axis throttle,turn
#define ARCADE3    3  // 3-axis throttle,turn,twist
#define DRIVETYPE  ARCADE2

#define APPLY_DEADBAND
#define SQUARE_INPUTS  true


// If you are using multiple modules, make sure to define both the port
// number and the module. For example you with a rangefinder:
//const int RANGE_FINDER_PORT = 1;
//const int RANGE_FINDER_MODULE = 1;

#endif
