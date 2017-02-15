#ifndef Fuel_H
#define Fuel_H

#include <Commands/Subsystem.h>
#include "CANTalon.h"

class FuelSubsystem : public Subsystem {
private:
	CANTalon FuelPusherMotor;

public:
	FuelSubsystem();
	void InitDefaultCommand();
	void PushFuel();
	void PusherOff();
	void PusherOn();

	void Enable();
	void Disable();

};

#endif  // Fuel_H
