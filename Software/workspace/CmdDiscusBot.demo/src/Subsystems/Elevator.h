#ifndef Elevator_H
#define Elevator_H

#include "WPILib.h"

/**
 * The elevator subsystem uses PID to go to a given height. Unfortunately, in it's current
 * state PID values for simulation are different than in the real world do to minor differences.
 */
class Elevator : public Subsystem {
private:
   CANTalon *motor;
   double speed;
   double setpoint;
public:
    Elevator();
    void InitDefaultCommand();

	/**
	 * The log method puts interesting information to the SmartDashboard.
	 */
    void Log();
    void SetSpeed(double f);
    void SetSetpoint(double s);

};

#endif
