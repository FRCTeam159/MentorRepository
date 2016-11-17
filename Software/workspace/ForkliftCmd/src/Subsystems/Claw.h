#ifndef Claw_H
#define Claw_H

#include "Commands/Subsystem.h"
#include "WPILib.h"

/**
 * The claw subsystem is a simple system with a motor for opening and closing.
 * If using stronger motors, you should probably use a sensor so that the
 * motors don't stall.
 */
class Claw: public Subsystem {
private:
#ifdef REAL
	DoubleSolenoid *claw;    // use pnuematics to open and close the claw
#else
	SpeedController* motor;  // GearsBot model doesn't know what the solenoid does (uses victor motor)
#endif
	DigitalInput* contact;

public:
	Claw();
	void InitDefaultCommand() {}

    /**
     * Open the claw
     */
	void Open();

    /**
     * Close the claw
     */
	void Close();

    /**
     * Reset
     */
	void Stop();

    /**
     * Return true when the robot is grabbing an object hard enough
     * to trigger the limit switch.
     */
	bool IsGripping();

    /**
     * Open or Close the claw
     */
	bool Toggle();

	void Log() {}
};

#endif
