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
#define MAXPIDCHNLS 2

class CANTalon: public Talon, public PIDSource
{
public:
	enum ControlMode {
		kPercentVbus,
		kSpeed,
		kPosition,
		UnsetControlMode
	};

	enum FeedbackDevice {
		QuadEncoder,
	    AnalogPot,
		UnsetFeedbackDevice
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
	enum NeutralMode {
		/** Use the NeutralMode that is set by the jumper wire on the CAN device */
		kNeutralMode_Jumper = 0,
		/** Stop the motor's rotation by applying a force. */
		kNeutralMode_Brake = 1,
		/** Do not attempt to stop the motor. Instead allow it to coast to a stop
	       without applying resistance. */
		kNeutralMode_Coast = 2
	};

protected:
	class Limit {
		int id;
		DigitalInput *dio;
		Encoder *enc;
		double soft_limit;
		bool forward;
		bool hard_limit_enabled;
		bool soft_limit_enabled;
	public:
		Limit(int i, bool dir);
		~Limit();
		void SetSoftLimit(Encoder *encoder, double value);
		bool AtHardlimit();
		bool AtSoftlimit();
		bool IsLimitOK();
		void SetHardLimitEnabled(bool b);
		void SetSoftLimitEnabled(bool b);
		void Disable();
	};
	class PIDData {
		double P=0;
		double I=0;
		double D=0;
		double F=0;
	public:
		bool changed=false;
		PIDController *pid=0;
		PIDData();
		~PIDData();
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
	FeedbackDevice feedback_device;
	ControlMode control_mode;
	LimitMode limit_mode;
	bool inverted=false;
	int debug=0;
	int id=0;
	PIDData pid_data[MAXPIDCHNLS];
	int pid_channel=0;
	Encoder *encoder=0;
	Limit *lowerLimit;
	Limit *upperLimit;
	virtual void ClearPID();
	virtual double ReturnPIDInput();
	virtual void SetVelocity(double value);
	virtual bool OnTarget();
	virtual double GetTargetError();
	virtual double GetSetpoint();
	virtual void AddEncoder();
	virtual void AddLowerLimit();
	virtual void AddUpperLimit();

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
	virtual void DisableSoftPositionLimits();
	virtual void ConfigForwardLimit(double forwardLimitPosition);
	virtual void ConfigReverseLimit(double reverseLimitPosition);
	virtual void ConfigSoftPositionLimits(double forwardLimitPosition,double reverseLimitPosition);
	virtual void ConfigForwardSoftLimitEnable(bool bForwardSoftLimitEn);
	virtual void ConfigReverseSoftLimitEnable(bool bReverseSoftLimitEn);
	virtual bool GetForwardLimitOK();
	virtual bool GetReverseLimitOK();
	virtual void SetSensorDirection(bool reverseSensor);
	virtual void ConfigNeutralMode(NeutralMode mode);

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
