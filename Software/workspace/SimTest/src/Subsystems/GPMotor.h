#ifndef GP_MOTOR_H
#define GP_MOTOR_H

#include "WPILib.h"

#define SIMRATE 0.01

#define BASE_CONTROLLER PIDController

#define PI 3.141516
#define RPD(x) (x)*2*PI/360
#define DPR(x) (x)*180.0/PI

#define CANTALON 1
#define TALON    2
#define VICTOR   3

#ifndef MOTORTYPE
#ifdef SIMULATION
    #define MOTORTYPE TALON
#else
	#define MOTORTYPE CANTALON
#endif
#endif

/**
 * The GPMotor subsystem extends either a CANTalon, Talon or Victor base class
 * - All base classes extend SpeeedController(->PIDOutput) interface
 * - In non CAN modes GPMotor incorporates a PID controller to emulate built in CAN features
 * - Provides an interface to incorporate an encoder
 * - Can be used to drive wheels, lifters etc.
 * - PID controller can be used to set constant velocity or target position
 */
#if MOTORTYPE == CANTALON
class GPMotor: public CANTalon, public PIDSource
#elif MOTORTYPE == VICTOR
class GPMotor: public Victor, public PIDSource
#else
class GPMotor: public Talon, public PIDSource
#endif
{
public:
	enum { POSITION,SPEED,VOLTAGE};
	uint8_t syncGroup;

protected:
	int control_mode;
	bool inverted;
	int debug;
	int id;

#if MOTORTYPE != CANTALON
	PIDController *pid;
	Encoder *encoder;
#endif

public:
	GPMotor(int id);
	GPMotor(int id, bool enc);

	~GPMotor();

	virtual double ReturnPIDInput();

	virtual void UsePIDOutput(double output);
	virtual double PIDGet();
	virtual void SetVelocity(double value);
	virtual double GetVelocity();
	virtual void SetVoltage(double value);
	virtual double GetVoltage();

	virtual void SetDistance(double value);
	virtual void SetSetpoint(double value);

	virtual double GetDistance();
	virtual double GetPosition() { return GetDistance();}

	virtual double GetTargetError();
	virtual double GetTargetCorrection();

	virtual void SetInverted(bool t);
	virtual bool IsInverted()		 { return inverted;}
	virtual bool IsEnabled();

	virtual void Enable();
	virtual void EnablePID();
	virtual void DisablePID();

	virtual void Disable();
	virtual void SetPID(int mode, double P, double I, double D);
	virtual void SetPID(int mode, double P, double I, double D, PIDSource *s);
	virtual void SetPID(double P, double I, double D);
	virtual void SetPID(double P, double I, double D, PIDSource *s);

	virtual void ClearPID();
	virtual PIDController *GetPID();
	virtual void SetInputRange(double min, double max);
	virtual void SetOutputRange(double min, double max);
	virtual void SetTolerance(double d);
	virtual void SetToleranceBuffer(unsigned bufLength);
	virtual void Reset();
	virtual void ClearIaccum();
	virtual void SetMode(int m);
	virtual int GetMode(){ return control_mode;}
	virtual bool OnTarget();
	virtual void SetDistancePerPulse(double target);
	virtual void PIDWrite(float output);
	virtual void SetDebug(int b);
};
#endif
