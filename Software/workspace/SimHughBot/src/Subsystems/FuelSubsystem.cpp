#include <Subsystems/FuelSubsystem.h>
#include "RobotMap.h"
#include <Commands/BallPusherToggle.h>

#define PUSHER_SPEED 0.5
FuelSubsystem::FuelSubsystem() : Subsystem("FuelSubsystem"),
	FuelPusherMotor(FUELMOTOR) {
}

void FuelSubsystem::InitDefaultCommand() {
	Enable();
	frc::SmartDashboard::PutBoolean("BallPusher",false);
	SetDefaultCommand(new BallPusherToggle());
}

void FuelSubsystem::PushFuel() {
}

void FuelSubsystem::PusherOff() {
	FuelPusherMotor.Set(-PUSHER_SPEED);
	frc::SmartDashboard::PutBoolean("BallPusher",false);
}

void FuelSubsystem::PusherOn() {
	FuelPusherMotor.Set(PUSHER_SPEED);
	frc::SmartDashboard::PutBoolean("BallPusher",true);
}

void FuelSubsystem::Disable() {
	FuelPusherMotor.Disable();
}

void FuelSubsystem::Enable() {
	FuelPusherMotor.Enable();
}
