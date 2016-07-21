#ifndef Elevator_H
#define Elevator_H

#include "Commands/PIDSubsystem.h"
#include "Subsystems/SmartMotor.h"

#include "WPILib.h"

class Elevator : public SmartMotor, public Subsystem {
private:
    double min,max,step;
    int nsteps;

public:
    Elevator(int i);
    void SetRange(double mn, double mx, double stp);
    double GetMin() { return min;}
    double GetMax() { return max;}
    double GetStep() { return step;}
    int GetNumSteps() {return nsteps;}

    void SetElevatorLevel(double n);
    double GetElevatorLevel();

    void Log();
};

#endif
