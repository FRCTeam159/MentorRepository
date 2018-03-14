package org.usfirst.frc.team159.robot.subsystems;

import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.RobotMap;
import org.usfirst.frc.team159.robot.commands.DriveWithJoystick;

import com.ctre.CANTalon;

import edu.wpi.first.wpilibj.AnalogGyro;
import edu.wpi.first.wpilibj.DoubleSolenoid;
import edu.wpi.first.wpilibj.MotorSafety;
import edu.wpi.first.wpilibj.MotorSafetyHelper;
import edu.wpi.first.wpilibj.command.Subsystem;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;

/**
 *
 */
public class DriveTrain extends Subsystem implements MotorSafety{

	private CANTalon frontLeft;
	private CANTalon frontRight;
	private CANTalon backLeft;
	private CANTalon backRight;
		
	private AnalogGyro gyro = new AnalogGyro(3);
	
	private DoubleSolenoid gearPneumatic = new DoubleSolenoid(0,1);
	private MotorSafetyHelper safetyHelper = new MotorSafetyHelper(this);

	static double WHEEL_DIAMETER=6.1; // inches
	static int ENCODER_TICKS=Robot.isReal()?1024:360;
	//static int ticks_per_foot=(int)(1.0/Math.PI*WHEEL_DIAMETER/12/ENCODER_TICKS); // ~1146 ticks/foot
	static double ticks_per_foot=ENCODER_TICKS*12.0/Math.PI/WHEEL_DIAMETER; // ~1146 ticks/foot
	boolean inlowgear=false;
	
	
	public void initDefaultCommand() {
		 setDefaultCommand(new DriveWithJoystick());
	}
	
	public DriveTrain() {
		super();
		frontLeft = new CANTalon(RobotMap.FRONTLEFT);
		frontRight = new CANTalon(RobotMap.FRONTRIGHT);
		backLeft = new CANTalon(RobotMap.BACKLEFT);
		backRight = new CANTalon(RobotMap.BACKRIGHT);

		frontRight.changeControlMode(CANTalon.TalonControlMode.PercentVbus);
		backLeft.changeControlMode(CANTalon.TalonControlMode.PercentVbus);

		frontLeft.changeControlMode(CANTalon.TalonControlMode.Follower);
		backRight.changeControlMode(CANTalon.TalonControlMode.Follower);
		
		//int ticks_per_foot=(int)(1.0/getFeetPerTick());
		
		frontRight.configEncoderCodesPerRev((int)ticks_per_foot);
		backLeft.configEncoderCodesPerRev((int)ticks_per_foot);
		setLowGear();
	    setExpiration(0.1);
	    setSafetyEnabled(true);
	    Publish(true);    
	}
	double getFeetPerTick() {
		return Math.PI*WHEEL_DIAMETER/12.0/ENCODER_TICKS; // ~1146 ticks/foot		
	}
	void Publish(boolean init) {
		if(init){
			//SmartDashboard.putBoolean("HighGear", false);
			SmartDashboard.putNumber("Heading", 0);
			SmartDashboard.putNumber("LeftDistance", 0);
			SmartDashboard.putNumber("RightDistance", 0);
			SmartDashboard.putNumber("Travel", 0);
			SmartDashboard.putNumber("LeftWheels", 0);
			SmartDashboard.putNumber("RightWheels", 0);
		}else{
			//SmartDashboard.putBoolean("HighGear", !inlowgear);
			SmartDashboard.putNumber("Heading", getHeading());
			SmartDashboard.putNumber("LeftDistance", feet_to_inches(getLeftDistance()));
			SmartDashboard.putNumber("RightDistance", feet_to_inches(getRightDistance()));
			SmartDashboard.putNumber("Travel",feet_to_inches(getDistance()));
			SmartDashboard.putNumber("LeftWheels", round(getLeft()));
			SmartDashboard.putNumber("RightWheels", round(getRight()));
		}
	}
	public static double feet_to_inches(double x) {
		return Math.round(12*x*100.0/100);
	}
	public double getRight() {
		return frontRight.get();
	}
	public double getLeft() {
		return backLeft.get();
	}

	public double getRightDistance() {
		return frontRight.getPosition();
	}
	public double getLeftDistance() {
		return backLeft.getPosition();
	}
	public double getDistance() {
		double d1=getRightDistance();
		double d2=getLeftDistance();
		return 0.5*(d1+d2);
	}
	public double getLeftVelocity() {
		return backLeft.getSpeed();
	}
	public double getRightVelocity() {
		return frontRight.getSpeed();
	}
	public double getVelocity() {
		double d1=getLeftVelocity();
		double d2=getRightVelocity();
		return 0.5*(d1+d2);
	}
	double round(double x) {
		return 0.001*Math.round(x*1000);
	}
	public double getHeading() {
		//if (Robot.isReal()) 
		return gyro.getAngle();
	}
	public boolean inLowGear() {
		return inlowgear;
	}
	public void setLowGear() {
		if(!inlowgear){
			gearPneumatic.set(DoubleSolenoid.Value.kReverse);
			System.out.println("Setting Low Gear");
			inlowgear=true;
		}		
	}
	public void setHighGear() {
		if(inlowgear){
			gearPneumatic.set(DoubleSolenoid.Value.kForward);
			System.out.println("Setting High Gear");
			inlowgear=false;
		}	
	}
	double coerce(double min, double max, double x) {
		if (x < min)
			x = min;
		else if (x > max)
			x = max;
		return x;
	}

	public void arcadeDrive(double moveValue, double rotateValue,
			boolean squaredInputs) {

		// local variables to hold the computed PWM values for the motors
		double leftMotorOutput;
		double rightMotorOutput;

		if (squaredInputs) {
			// square the inputs (while preserving the sign) to increase fine control
			// while permitting full power
			if (moveValue >= 0.0) {
				moveValue = (moveValue * moveValue);
			} else {
				moveValue = -(moveValue * moveValue);
			}
			if (rotateValue >= 0.0) {
				rotateValue = (rotateValue * rotateValue);
			} else {
				rotateValue = -(rotateValue * rotateValue);
			}
		}

		if (moveValue > 0.0) {
			if (rotateValue > 0.0) {
				leftMotorOutput = moveValue - rotateValue;
				rightMotorOutput = Math.max(moveValue, rotateValue);
			} else {
				leftMotorOutput = Math.max(moveValue, -rotateValue);
				rightMotorOutput = moveValue + rotateValue;
			}
		} else {
			if (rotateValue > 0.0) {
				leftMotorOutput = -Math.max(-moveValue, rotateValue);
				rightMotorOutput = moveValue + rotateValue;
			} else {
				leftMotorOutput = moveValue - rotateValue;
				rightMotorOutput = -Math.max(-moveValue, -rotateValue);
			}
		}
		set(leftMotorOutput,rightMotorOutput);
	}

	/**
	 * Reset the robots sensors to the zero states.
	 */
	public void reset() {
	  resetEncoders();
	  resetGyro();
	}
  public void resetEncoders() {
    frontRight.reset();
    backLeft.reset();
  }
  public void resetGyro() {
    gyro.reset(); 
  }

	public void enable() {
		frontRight.enable();
		backLeft.enable();
		Publish(true);
	}
	public void disable() {
		frontRight.disable();
		backLeft.disable();
		Publish(true);
	}

	public void set(double left,double right) {
		left  = coerce(-1.0, 1.0, left);
		right = coerce(-1.0, 1.0, right);
		backLeft.set(left);
		frontLeft.set(RobotMap.BACKLEFT);
		frontRight.set(right);
		frontLeft.set(RobotMap.FRONTRIGHT);
		Publish(false);
		safetyHelper.feed();
	}

	// MotorSafty methods
	@Override
	public void setExpiration(double timeout) {
	    safetyHelper.setExpiration(timeout);
	}

	@Override
	public double getExpiration() {
	    return safetyHelper.getExpiration();
	}

	@Override
	public boolean isAlive() {
	    return safetyHelper.isAlive();
	}

	@Override
	public void stopMotor() {
        safetyHelper.feed();	    
	}

	@Override
	public void setSafetyEnabled(boolean enabled) {
	    safetyHelper.setSafetyEnabled(enabled);		
	}

	@Override
	public boolean isSafetyEnabled() {
	    return safetyHelper.isSafetyEnabled();
	}

	@Override
	public String getDescription() {
	    return "Robot Drive";
	}

}
