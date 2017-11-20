package org.usfirst.frc.team159.robot.subsystems;

import edu.wpi.first.wpilibj.AnalogGyro;
import edu.wpi.first.wpilibj.MotorSafety;
import edu.wpi.first.wpilibj.MotorSafetyHelper;
import edu.wpi.first.wpilibj.command.Subsystem;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;

import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.RobotMap;
import org.usfirst.frc.team159.robot.commands.DriveWithJoystick;

import com.ctre.CANTalon;

/**
 *
 */
public class DriveTrain extends Subsystem implements MotorSafety{
	private CANTalon frontLeft;
	private CANTalon frontRight;
	private CANTalon backLeft;
	private CANTalon backRight;

	private AnalogGyro gyro = new AnalogGyro(3);
	private MotorSafetyHelper safetyHelper = new MotorSafetyHelper(this);
	
	static double WHEEL_DIAMETER=4.125; // inches

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
		
		int ticks_per_foot=(int)(1.0/getFeetPerTick());
		
		frontRight.configEncoderCodesPerRev(ticks_per_foot);
		backLeft.configEncoderCodesPerRev(ticks_per_foot);
	    setExpiration(0.1);
	    setSafetyEnabled(true);
	    Publish(true);    
	}
	double getFeetPerTick() {
		return Math.PI*WHEEL_DIAMETER/12.0/360;	// hard-coded in simulation to 360			
	}
	public static double feet_to_inches(double x) {
		return Math.round(12*x*100.0/100);
	}

	public double getRightDistance() {
		return -frontRight.getPosition();
	}
	public double getLeftDistance() {
		return -backLeft.getPosition();
	}
	public double getDistance() {
		double d1=getRightDistance();
		double d2=getLeftDistance();
		double x=0.5*(d1+d2);
		return x;
	}
	public double getHeading() {
		return -gyro.getAngle();
	}

	void Publish(boolean init) {
		if(init){
			SmartDashboard.putNumber("Heading", 0);
			SmartDashboard.putNumber("Travel", 0);
		}else{
			SmartDashboard.putNumber("Heading", getHeading());
			SmartDashboard.putNumber("Travel",feet_to_inches(getDistance()));
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
		// Ramp values up
		// Make sure values are between -1 and 1
		leftMotorOutput  = coerce(-1.0, 1.0, leftMotorOutput);
		rightMotorOutput = coerce(-1.0, 1.0, rightMotorOutput);
		
		backLeft.set(leftMotorOutput);
		frontRight.set(-rightMotorOutput);
		backRight.set(RobotMap.FRONTRIGHT);
		frontLeft.set(RobotMap.BACKLEFT);

		Publish(false);

		safetyHelper.feed();
	}
	double round(double x) {
		return 0.001*Math.round(x*1000);
	}

	public void reset() {
		gyro.reset();
		frontRight.reset();
		backLeft.reset();
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
		frontLeft.stopMotor();
		frontRight.stopMotor();
		backLeft.stopMotor();
		backRight.stopMotor();
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

