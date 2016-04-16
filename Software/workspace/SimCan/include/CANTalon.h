#ifndef GP_MOTOR_H
#define GP_MOTOR_H

#include "WPILib.h"

#define SIMRATE 0.01

#define BASE_CONTROLLER PIDController

#define PI 3.141516
#define RPD(x) (x)*2*PI/360
#define DPR(x) (x)*180.0/PI

/**
 * The simulation CANTalon class extends Talon
 * - base class extends SpeeedController(->PIDOutput) interface
 * - Incorporates a PID controller to emulate built in CAN features
 * - Provides an interface to incorporate an encoder
 */
class CANTalon: public Talon, public PIDSource
{
public:
	enum { POSITION,SPEED,VOLTAGE};
	uint8_t syncGroup;
protected:
	int control_mode;
	bool inverted;
	int debug;
	int id;

	PIDController *pid;
	Encoder *encoder;

public:
	CANTalon(int id);
	CANTalon(int id, bool enc);

	~CANTalon();

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
