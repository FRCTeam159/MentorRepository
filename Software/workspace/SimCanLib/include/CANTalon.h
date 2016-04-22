/**
 * The simulation CANTalon class extends Talon
 * - Talon extends SpeeedController(PIDOutput) interface
 * - Incorporates a PID controller to emulate built in (limited) CAN features
 * - Provides an interface to incorporate an encoder
 * - Provides an interface to incorporate limit switches
 *
 */
#pragma once

#include "WPILib.h"

#define PI 3.141516
#define RPD(x) (x)*2*PI/360
#define DPR(x) (x)*180.0/PI
#define SIMPIDRATE 0.01
#define SIM_ENCODER_TICKS 360

class CANTalon: public Talon, public PIDSource
{
public:
	enum ControlMode {
		kPercentVbus,
		kSpeed,
		kPosition,
		kMaxControlMode
	};

	enum FeedbackDevice {
		QuadEncoder,
		MaxFeedbackDevice
	};
	enum LimitMode {
		/** Only use switches for limits */
		kLimitMode_SwitchInputsOnly = 0,
		/** Use both switches and soft limits */
		kLimitMode_SoftPositionLimits = 1,
		/* SRX extensions */
		/** Disable switches and disable soft limits */
		kLimitMode_SrxDisableSwitchInputs = 2,
	};
protected:
	class PIDData {
	protected:
		double P=0;
		double I=0;
		double D=0;
		double F=0;
	public:
		bool changed=false;
		PIDController *pid=0;
		PIDData();
		void SetPID(double P, double I, double D, double F, PIDSource *s,PIDOutput *d);
		void SetPID(double P, double I, double D, PIDSource *s,PIDOutput *d);
		void Set(PIDSource *s,PIDOutput *d);
		void Clear();
		void SetP(double d);
		void SetI(double d);
		void SetD(double d);
		void SetF(double d);
		void SetSetpoint(double value);
		bool IsEnabled();
		void Enable();
		void Disable();
		void Reset();
		double GetTargetError();
		double GetSetpoint();
		bool OnTarget();
		double GetP();
		double GetI();
		double GetD();
		double GetF();
	};

	ControlMode control_mode;
	LimitMode limit_mode;
	bool inverted=false;
	int debug=0;
	int id=0;
	PIDData pid_data[2];
	int pid_channel=0;
	Encoder *encoder=0;
	DigitalInput *lowerLimit;
	DigitalInput *upperLimit;
	virtual void ClearPID();
	virtual double ReturnPIDInput();
	virtual void SetVelocity(double value);
	virtual bool OnTarget();
	virtual double GetTargetError();
	virtual double GetSetpoint();

public:
	CANTalon(int id);
	~CANTalon();

	virtual void SelectProfileSlot(int i);
	virtual void SetControlMode(ControlMode mode);
	virtual void SetFeedbackDevice(FeedbackDevice device);
	virtual void SetP(double d);
	virtual void SetI(double d);
	virtual void SetD(double d);
	virtual void SetF(double d);
	virtual void SetPID(double P, double I, double D);
	virtual void SetPID(double P, double I, double D,double F);
	virtual double GetP();
	virtual double GetI();
	virtual double GetD();
	virtual double GetF();

	virtual void ClearIaccum();
	virtual bool IsModePID(ControlMode mode);

	virtual void ConfigEncoderCodesPerRev(uint16_t codesPerRev);
	//virtual void ConfigPotentiometerTurns(uint16_t turns);

	virtual void ConfigLimitMode(LimitMode mode);
	virtual int IsFwdLimitSwitchClosed();
	virtual int IsRevLimitSwitchClosed();
	virtual void ConfigFwdLimitSwitchNormallyOpen(bool normallyOpen);
	virtual void ConfigRevLimitSwitchNormallyOpen(bool normallyOpen);

	virtual double PIDGet();

	virtual double GetSpeed();
	virtual double GetVoltage();

	virtual void SetPosition(double value);
	virtual double GetPosition();

	virtual void Set(double value);
	virtual void SetSetpoint(double value);

	virtual double GetTargetCorrection();

	virtual void SetInverted(bool t);
	virtual bool IsInverted()		 { return inverted;}
	virtual bool IsEnabled();

	virtual void Enable();
	virtual void EnableControl();

	virtual void Disable();

	virtual void SetEncPosition(int);
	virtual int GetEncVel();

	virtual int GetClosedLoopError() const;
    void SetAllowableClosedLoopErr(uint32_t allowableCloseLoopError);
	virtual void Reset();
	virtual ControlMode GetControlMode(){ return control_mode;}

	virtual void PIDWrite(float output);
	virtual void SetDebug(int b);
};
