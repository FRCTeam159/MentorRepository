package com.ctre;

import edu.wpi.first.wpilibj.livewindow.LiveWindow;
import edu.wpi.first.wpilibj.livewindow.LiveWindowSendable;
import edu.wpi.first.wpilibj.simulation.SimSpeedController;
import edu.wpi.first.wpilibj.tables.ITable;
import edu.wpi.first.wpilibj.tables.ITableListener;

public class CANTalon implements 
	edu.wpi.first.wpilibj.SpeedController, 
	edu.wpi.first.wpilibj.PIDSource, 
	edu.wpi.first.wpilibj.PIDOutput, 
	edu.wpi.first.wpilibj.MotorSafety, 
	LiveWindowSendable, ITableListener {

	public static enum TalonControlMode {
		PercentVbus, Speed, Position, Follower, Current, MotionProfile
	}
	public static enum FeedbackDevice {
		QuadEncoder, AnalogPot, UnsetFeedbackDevice
	}
	public static enum LimitMode {
		/** Only use switches for limits */
		kLimitMode_SwitchInputsOnly,
		/** Use both switches and soft limits */
		kLimitMode_SoftPositionLimits,
		/* SRX extensions */
		/** Disable switches and disable soft limits */
		kLimitMode_SrxDisableSwitchInputs,
	}
	public static enum NeutralMode {
		/** Use the NeutralMode that is set by the jumper wire on the CAN device */
		kNeutralMode_Jumper,
		/** Stop the motor's rotation by applying a force. */
		kNeutralMode_Brake,
		/** Do not attempt to stop the motor. Instead allow it to coast to a stop
		 without applying resistance. */
		kNeutralMode_Coast
	}
	public static final int MAXPIDCHNLS=2;
	public static final int MAXCANCHNLS=12;

	FeedbackDevice feedback_device;
	TalonControlMode control_mode;
	LimitMode limit_mode;
	boolean inverted = false;
	int debug = 0;
	int id = 0;
	static double SIMPIDRATE=0.01;
	edu.wpi.first.wpilibj.PIDSourceType pidMode;
	
	private static CANTalon channels[]=null;
	
	PIDData[] pid_data=new PIDData[MAXPIDCHNLS];
	int pid_channel = 0;
	edu.wpi.first.wpilibj.Encoder encoder = null;
	Limit lowerLimit = null;
	Limit upperLimit = null;
	
	edu.wpi.first.wpilibj.MotorSafetyHelper safetyHelper;
	SimSpeedController impl;
	ITable  table;
	int ID1,ID2;

	public CANTalon(final int channel) {
		id=channel;
		safetyHelper = new edu.wpi.first.wpilibj.MotorSafetyHelper(this);

		for (int i = 0; i < MAXPIDCHNLS; i++) {
			pid_data[i] = new PIDData();
			pid_data[i].pid = null;
		}
		if(channels==null) {
			channels=new CANTalon[MAXCANCHNLS];
			for (int i = 0; i < MAXCANCHNLS; i++)
				channels[i]=null;
		};
		limit_mode = LimitMode.kLimitMode_SrxDisableSwitchInputs;
		feedback_device = FeedbackDevice.UnsetFeedbackDevice;

		debug = 0;
		control_mode = TalonControlMode.PercentVbus;
		inverted = false;
		LiveWindow.addActuator("CANTalon", channel, this);
		// note: when can devices are supported in SW exporter can change this to "can/%d"
		impl = new SimSpeedController("simulator/pwm/" + id);
		ID1=((id-1)*2+1); // these map to assumed encoder channels in sdf file
		ID2=((id-1)*2+2);
		channels[channel]=this;
	}
	public Object getChannel() {
		return id;
	}
	public void setDebug(int b) {
		debug = b;
	}

	public double getTargetError() {
		return pid_data[pid_channel].GetTargetError();
	}

	public void enable() {
		enableControl();
	}
	public boolean getInverted() {
		// TODO Auto-generated method stub
		return inverted;
	}
	public void setInverted(boolean t) {
		inverted = t;
		// This causes purposefully thrown exception in simulation
		// if(encoder)
		//	 encoder.SetReverseDirection(t);
	}

	//========= SpeedController Interface functions ===================

	public void set(double val) {
		switch (control_mode) {
		default:
		case Follower:
			if(channels[(int)val]!=null) {
				double mval=channels[(int)val].get();
				impl.set(mval);
			}
			break;
		case PercentVbus:
			val=inverted?-val:val;
			impl.set(val);
			break;
		case Speed:
			val=inverted?-val:val;
			setSetpoint(val);
			break;
		case Position:
			setSetpoint(val);
			break;
		}
	}
	public double get() {
		double val=impl.get();
		switch (control_mode) {
		default:
		case Follower:
			val=0;
			break;
		case PercentVbus:
			val=impl.get();
			break;
		case Speed:
			val=getSpeed();
			break;
		case Position:
			val=getPosition();
			break;
		}
		return val;
	}
	public void disable() {
		setRaw(0);
		pid_data[pid_channel].disable();
	}

	//========= MotorSafety Interface functions ===================

	public void setExpiration(double timeout) {
		safetyHelper.setExpiration(timeout);
	}
	public double getExpiration() {
		return safetyHelper.getExpiration();
	}
	public boolean isAlive(){
		return safetyHelper.isAlive();
	}

	public void stopMotor() {
		setRaw(0);
	}
	public void setSafetyEnabled(boolean enabled) {
		safetyHelper.setSafetyEnabled(enabled);
	}
	public boolean isSafetyEnabled() {
		return safetyHelper.isSafetyEnabled();
	}
	public String getDescription() {
		return  "CAN "+getChannel();
	}
	
	//========= PWM functions ===================
	public void setVoltage(double val) {
		impl.set(val);
		safetyHelper.feed();
	}
	public void setRaw(int value) {
		//wpi_assert(value == 0);
		impl.set(value);
	}
	
	//========== ITableListener Interface functions =========================
	public void valueChanged(ITable tbl, String key, Object value, boolean isNew) {
		if (value instanceof Double)
			setVoltage((Double)value);	
	}
	public String getSmartDashboardType() {
		return "Speed Controller";
	}
	public ITable getTable() {
		return table;
	}
	public void updateTable() {
		if (table != null)
			table.putNumber("Value", getSpeed());		
	}
	
	//========== LiveWindowSendable Interface functions ===========
	public void startLiveWindowMode() {
		setVoltage(0);
		if (table != null)
			table.addTableListener("Value", this, true);
	}
	public void stopLiveWindowMode() {
		setVoltage(0);
		if (table != null)
			table.removeTableListener(this);
	}
	public void initTable(ITable subTable) {
		table = subTable;
		updateTable();
	}
	
	//=========== CANTalon PID control functions ==================
	public void selectProfileSlot(int i) {
		pid_channel = i >= 1 ? 1 : 0;
	}
	public void clearPID() {
		for (int i = 0; i < MAXPIDCHNLS; i++) {
			pid_data[i].clear();
		}
	}
	public void setP(double d) {
		pid_data[pid_channel].setP(d);
	}
	public void setI(double d) {
		pid_data[pid_channel].setI(d);
	}
	public void setD(double d) {
		pid_data[pid_channel].setD(d);
	}
	public void setF(double d) {
		pid_data[pid_channel].setF(d);
	}
	public void setPID(double P, double I, double D, double F) {
		pid_data[pid_channel].setPID(P, I, D, F, this, this);
	}
	public boolean onTarget() {
		return pid_data[pid_channel].onTarget();
	}
	public void clearIaccum() {
		pid_data[pid_channel].reset(); // clears accumulator but also disables
	}
	public double getF() {
		return pid_data[pid_channel].getF();
	}
	
	//========= PIDInterface Interface functions =================
	public void setPID(double P, double I, double D) {
		pid_data[pid_channel].setPID(P, I, D, this, this);
	}
	public double getP() {
		return pid_data[pid_channel].getP();
	}
	public double getI() {
		return pid_data[pid_channel].getI();
	}
	public double getD() {
		return pid_data[pid_channel].getD();
	}
	public double getSetpoint() {
		return pid_data[pid_channel].getSetpoint();
	}
	public void setSetpoint(double value) {
		pid_data[pid_channel].setSetpoint(value);
	}
	
	//========= PIDSource Interface function =====================
	public double pidGet() {
		double d = returnPIDInput();
		if (isEnabled() && ((debug & 2)>0))
			System.out.println(""+id+" pidGet:"+d+" setpoint:"+getSetpoint());
		return d;
	}
	@Override
	public edu.wpi.first.wpilibj.PIDSourceType getPIDSourceType() {
		return pidMode;
	}
	@Override
	public void setPIDSourceType(edu.wpi.first.wpilibj.PIDSourceType m) {
		pidMode=m;
	}

	//========= PIDOutput Interface function ========================
	public void pidWrite(double output) {
		if (isEnabled() && ((debug & 1)>0))
			System.out.println(""+id+" PIDWrite: target:"+" error:"+getTargetError()+" correction:"+output);
		setVoltage(output);
		safetyHelper.feed();
	}
	//==============================================================

	public void changeControlMode(TalonControlMode m) {
		control_mode = m;
		if (m == TalonControlMode.Current ||m == TalonControlMode.MotionProfile) {
			System.out.println("ERROR mode unsupported in simulation:"+m);
			return;
		}
		if ((m != TalonControlMode.Position) && (m != TalonControlMode.Speed)) {
			clearPID();
		} else {
			pidMode = (control_mode == TalonControlMode.Position) ?
					edu.wpi.first.wpilibj.PIDSourceType.kDisplacement : edu.wpi.first.wpilibj.PIDSourceType.kRate;
			if (encoder!=null)
				encoder.setPIDSourceType(pidMode);
		}
	}
	public boolean isModePID(TalonControlMode mode) {
		return (mode == TalonControlMode.Speed) || (mode == TalonControlMode.Position);
	}

	public void setFeedbackDevice(FeedbackDevice device) {
		feedback_device = device;
		if (device == FeedbackDevice.AnalogPot || device == FeedbackDevice.UnsetFeedbackDevice) {
			System.out.println("ERROR feedback device unsupported in simulation:"+device);
			return;
		}
		addEncoder();
	}
	void addEncoder() {
		if (encoder == null)
			encoder = new edu.wpi.first.wpilibj.Encoder(ID1, ID2); // {1,2} {3,4} {5,6} ..
	}
	void addLowerLimit() {
		if (lowerLimit==null)
			lowerLimit = new Limit(ID1, false);
	}
	void addUpperLimit() {
		if (upperLimit==null)
			upperLimit = new Limit(ID2, true);
	}

	double returnPIDInput() {
		switch (control_mode) {
		default:
			return 0;
		case Position:
			return getPosition();
		case Speed:
			return getSpeed();
		case PercentVbus:
			return getOutputVoltage();
		}
	}

	public void setVelocity(double value) {
		pid_data[pid_channel].setSetpoint(value);
		if (debug>0)
			System.out.println(id+" SetVelocity:"+value);
	}
	public void setPosition(double value) {
		if (debug>0)
			System.out.println(id+" SetPosition:"+value);
		pid_data[pid_channel].setSetpoint(value);
	}

	// in simulation rate is in degrees/second
	public double getSpeed() {
		if (encoder!=null)
			return encoder.getRate();
		else
			return 0;
	}
	public double getPosition() {
		if (encoder!=null)
			return encoder.getDistance();
			else {
				System.out.println("ERROR GetPosition() encoder=NULL");
				return 0;
			}
	}
	public float getOutputVoltage() {
		return (float)impl.get();
	}
	public float getOutputCurrent() {
		return (float)(10*impl.get());
	}
	public void enableControl() {
		if (isModePID(control_mode))
			pid_data[pid_channel].set(this, this);
		pid_data[pid_channel].enable();
	}
	public boolean isEnabled() {
		return pid_data[pid_channel].isEnabled();
	}
	public double getTargetCorrection() {
		if (pid_data[pid_channel].pid!=null)
			return pid_data[pid_channel].pid.get();
		else
			return 0;
	}
	public void reset() {
		if (encoder!=null)
			encoder.reset();
			pid_data[pid_channel].reset();
	}
	public void configEncoderCodesPerRev(int codesPerRev) {
		addEncoder();
		encoder.setDistancePerPulse((double) (1.0 / codesPerRev));
	}
	/**
	 * Athena: Configures the soft limit enable (wear leveled persistent memory).
	 *   - Also sets the limit switch overrides.
	 * Simulation: Currently supports hard and soft limits only
	 *   - Hard limit switches also require a (simulated) DigitalInput channel
	 *   - Soft limit switches also require a Encoder
	 */
	public void configLimitMode(LimitMode mode) {
		switch (mode) {
		case kLimitMode_SoftPositionLimits:
			addEncoder();
			addUpperLimit();
			addLowerLimit();
			break;
		case kLimitMode_SwitchInputsOnly:
			if (lowerLimit!=null)
				lowerLimit.setSoftLimitEnabled(false);
				if (upperLimit!=null)
					upperLimit.setSoftLimitEnabled(false);
					break;
		case kLimitMode_SrxDisableSwitchInputs:
			if (lowerLimit!=null)
				lowerLimit.disable();
				if (upperLimit!=null)
					upperLimit.disable();
					break;
		}
		limit_mode = mode;
	}
	/**
	 * Athena: Change the fwd limit switch setting to normally open or closed.
	 * @param normallyOpen true for normally open.  false for normally closed.
	 *
	 * Simulation: This function is not directly supported
	 *   - DigitalInput "Get" function will always return true if the joint is in a position that meets
	 *     the Joint limit properties set in the Solidworks exporter (or .sdf file)
	 *   - DigitalInput ids that emulate CAN limit switches are assigned as follows:
	 *     DIO channel: reverse=(id-1)*2+1 forward=(id-1)*2+2 {1,2},{2,3} ...
	 *   - note: Simulated DigitalInput ids for switches are the same as PWM ids for simulated encoders
	 */
	public void configRevLimitSwitchNormallyOpen(boolean normallyOpen) {
		addLowerLimit();
	}
	/**
	 * API is the same as for ConfigRevLimitSwitchNormallyOpen
	 */
	public void configFwdLimitSwitchNormallyOpen(boolean normallyOpen) {
		addUpperLimit();
	}
	/**
	 * Hard Limit: Return true if joint is in predefined range (using DIO channel)
	 * Soft Limit: Return true if encoder <= preset position
	 */
	public int isRevLimitSwitchClosed() {
		switch (limit_mode) {
		case kLimitMode_SwitchInputsOnly:
			addLowerLimit();
			return lowerLimit.atHardlimit() ? 1 : 0;
		case kLimitMode_SoftPositionLimits:
			addLowerLimit();
			if (lowerLimit.atSoftlimit())
				return 1;
			return lowerLimit.atHardlimit() ? 1 : 0;
		default:
		case kLimitMode_SrxDisableSwitchInputs:
			return 0;
		}
	}
	/**
	 * Hard Limit: Return true if joint is in predefined range
	 * Soft Limit: Return true if encoder >= preset position
	 */
	public int isFwdLimitSwitchClosed() {
		switch (limit_mode) {
		case kLimitMode_SwitchInputsOnly:
			addUpperLimit();
			return upperLimit.atHardlimit() ? 1 : 0;
		case kLimitMode_SoftPositionLimits:
			addUpperLimit();
			if (upperLimit.atSoftlimit())
				return 1;
			return upperLimit.atHardlimit() ? 1 : 0;
		default:
		case kLimitMode_SrxDisableSwitchInputs:
			return 0;
		}
	}
	public void disableSoftPositionLimits() {
		configForwardSoftLimitEnable(false);
		configReverseSoftLimitEnable(false);
	}
	public void configForwardLimit(double forwardLimitPosition) {
		addUpperLimit();
		addEncoder();
		upperLimit.setSoftLimit(encoder, forwardLimitPosition);
	}
	public void configReverseLimit(double reverseLimitPosition) {
		addLowerLimit();
		addEncoder();
		lowerLimit.setSoftLimit(encoder, reverseLimitPosition);
	}
	public void configSoftPositionLimits(double forward, double reverse) {
		configForwardLimit(forward);
		configReverseLimit(reverse);
	}
	public void configForwardSoftLimitEnable(boolean bForwardSoftLimitEn) {
		if (upperLimit!=null)
			upperLimit.setSoftLimitEnabled(bForwardSoftLimitEn);
	}
	public void configReverseSoftLimitEnable(boolean bReverseSoftLimitEn) {
		if (lowerLimit!=null)
			lowerLimit.setSoftLimitEnabled(bReverseSoftLimitEn);
	}
	public boolean getForwardLimitOK() {
		if (upperLimit!=null)
			return upperLimit.isLimitOK();
			return true;
	}
	public boolean getReverseLimitOK() {
		if (lowerLimit!=null)
			return lowerLimit.isLimitOK();
			return true;
	}
	public void setSensorDirection(boolean reverseSensor) {
		// TODO: need to reverse soft limit switches ?
	}
	int getEncVel() {
		// TODO: return raw encoder ticks ?
		return 0;
	}
	public void setEncPosition(int int1) {
		// TODO: set raw encoder ticks ?
	}
	public int getClosedLoopError() {
		// TODO: Use this to mimic PIDController GetError ?
		return 0;
	}
	public void setAllowableClosedLoopErr(int allowableCloseLoopError) {
		// TODO: Use this to mimic PIDController OnTarget ?
	}
	void configNeutralMode(NeutralMode mode) {
		// nothing to do in simulation
	}

	//=========================  private PIDdata Subclass =================
	class PIDData {
		double P = 0;
		double I = 0;
		double D = 0;
		double F = 0;
		boolean changed = false;
		boolean initialized = false;

		edu.wpi.first.wpilibj.PIDController pid ;

		PIDData() {
			P = I = D = F = 0;
			changed = false;
			initialized = false;
		}

		void setPID(double P, double I, double D, double F,
				edu.wpi.first.wpilibj.PIDSource s, edu.wpi.first.wpilibj.PIDOutput d) {
			setF(F);
			setPID(P, I, D, s, d);
		}
		void setPID(double P, double I, double D, edu.wpi.first.wpilibj.PIDSource s,
				edu.wpi.first.wpilibj.PIDOutput d) {
			setP(P);
			setI(I);
			setD(D);
			set(s, d);
		}
		void set(edu.wpi.first.wpilibj.PIDSource s, edu.wpi.first.wpilibj.PIDOutput d) {
			if (changed)
				clear();
			if (!initialized)
				pid = new edu.wpi.first.wpilibj.PIDController(P, I, D, F, s, d, SIMPIDRATE);
			changed = false;
		}
		void clear() {
			pid = null;
		}
		void setP(double d) {
			if (d != P)
				changed = true;
			P = d;
		}
		void setI(double d) {
			if (d != I)
				changed = true;
			I = d;
		}
		void setD(double d) {
			if (d != D)
				changed = true;
			D = d;
		}
		void setF(double d) {
			if (d != F)
				changed = true;
			F = d;
		}
		void setSetpoint(double value) {
			if (pid!=null)
				pid.setSetpoint(value);
		}
		boolean isEnabled() {
			if (initialized)
				return pid.isEnabled();
			else
				return false;
		}
		void enable() {
			if (initialized)
				pid.enable();
		}
		void disable() {
			if (initialized)
				pid.disable();
		}
		void reset() {
			if (initialized)
				pid.reset();
		}
		double GetTargetError() {
			if (initialized)
				return pid.getError();
			else
				return 0;
		}
		double getSetpoint() {
			if (pid!=null)
				return pid.getSetpoint();
			else
				return 0;
		}
		boolean onTarget() {
			if (initialized)
				return pid.onTarget();
			else
				return false;
		}
		double getP() {
			return P;
		}
		double getI() {
			return I;
		}
		double getD() {
			return D;
		}
		double getF() {
			return F;
		}
	}
	//=========================  Limit  ==================
	class Limit {
		edu.wpi.first.wpilibj.DigitalInput dio;
		edu.wpi.first.wpilibj.Encoder enc;
		double soft_limit;
		boolean forward;
		boolean hard_limit_enabled;
		boolean soft_limit_enabled;
		boolean initialized = false;
		boolean test=false;

		Limit(int i, boolean isFwrd) {
			id = i;
			soft_limit = 0;
			forward = isFwrd;
			hard_limit_enabled = true;
			soft_limit_enabled = false;
			dio = new edu.wpi.first.wpilibj.DigitalInput(id);
			initialized = false;
		}

		void setSoftLimit(edu.wpi.first.wpilibj.Encoder encoder, double value) {
			enc = encoder;
			soft_limit = value;
			soft_limit_enabled = true;
			initialized = true;
		}

		boolean atHardlimit() {
			if (!initialized)
				return false;
			if (!hard_limit_enabled)
				return false;
			return dio.get();
		}

		boolean atSoftlimit() {
			if (!initialized)
				return false;
			if (!soft_limit_enabled)
				return false;
			if (forward && enc.getDistance() >= soft_limit) {
				if(test)
					System.out.println(":At Forward Soft Limit:"+enc.getDistance()+":"+soft_limit);
				return true;
			}
			if (!forward && enc.getDistance() <= soft_limit) {
				if(test)
					System.out.println(":At Reverse Soft Limit:"+enc.getDistance()+":"+soft_limit);
				return true;
			}
			return false;
		}
		boolean isLimitOK() {
			if (atHardlimit() || atSoftlimit())
				return false;
			return true;
		}

		void setHardLimitEnabled(boolean b) {
			hard_limit_enabled = b;
		}

		void setSoftLimitEnabled(boolean b) {
			soft_limit_enabled = b;

		}
		void disable() {
			setHardLimitEnabled(false);
			setSoftLimitEnabled(false);
		}
	}
}
