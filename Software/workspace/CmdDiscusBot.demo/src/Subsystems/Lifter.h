#ifndef Lifter_H
#define Lifter_H

#include "WPILib.h"

/**
 * The elevator subsystem uses PID to go to a given height. Unfortunately, in it's current
 * state PID values for simulation are different than in the real world do to minor differences.
 */
class Lifter : public Subsystem {
private:
   CANTalon motor;
   double speed=1.0;
   bool found_zero=false;
   bool at_rev_limit=false;
   bool at_fwd_limit=false;

public:
    Lifter();
    void InitDefaultCommand();

    void Log();
    void SetSpeed(double f);
    bool FindZero();
    bool FoundZero() { return found_zero;}
    void Reset();
    double GetPosition();
};

#endif
