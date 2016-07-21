#ifndef SMART_MOTOR_H
#define SMART_MOTOR_H

#include "WPILib.h"

//#undef REAL
/**
 * The SmartMotor subsystem extends either a CANTalon or Talon base class
 * - Both base classes extend SpeeedController(->PIDOutput) interface
 * - In simulation mode it incorporates a PID controller to emulate built in CAN features
 * - Provides an interface to incorporate an encoder
 * - Can be used to drive wheels, lifters etc.
 * - PID controller can be used to set constant velocity or target position
 */
#ifdef REAL
class SmartMotor: public CANTalon, public PIDSource
#else
class SmartMotor: public Talon, public PIDSource
#endif
{
public:
	enum { POSITION,SPEED};
	uint8_t syncGroup;

protected:
	int control_mode;
	bool inverted;

#ifndef REAL
	PIDController *pid;
	Encoder *encoder;
#endif

public:
	SmartMotor(int id);
	~SmartMotor();

	virtual double ReturnPIDInput();

	virtual void UsePIDOutput(double output);
	virtual double PIDGet();
	virtual void SetVelocity(double value);
	virtual double GetVelocity();
	virtual void SetDistance(double value);
	virtual double GetDistance();
	virtual void SetInverted(bool t);
	virtual bool IsInverted()		 { return inverted;}
	virtual void Enable();
	virtual void Disable();
	virtual void SetPID(int mode, double P, double I, double D);
	virtual void SetPID(double P, double I, double D);
	virtual void SetSetpoint(double target);
	virtual void SetInputRange(double min, double max);
	virtual void SetOutputRange(double min, double max);
	virtual void SetPercentTolerance(double d);
	virtual void Reset();
	virtual void ClearIaccum();
	virtual void SetMode(int m);
	virtual int GetMode(){ return control_mode;}
	virtual bool OnTarget();
	virtual void SetDistancePerPulse(double target);
};
#endif
