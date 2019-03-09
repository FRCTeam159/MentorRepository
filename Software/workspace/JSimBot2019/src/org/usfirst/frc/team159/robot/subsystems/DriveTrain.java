package org.usfirst.frc.team159.robot.subsystems;

import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.RobotMap;
import org.usfirst.frc.team159.robot.commands.DriveWithGamepad;

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
public class DriveTrain extends Subsystem implements MotorSafety, RobotMap {

	private CANTalon leftMotor;
	private CANTalon rightMotor;

	private AnalogGyro gyro = new AnalogGyro(3);

	// private DoubleSolenoid gearPneumatic = new DoubleSolenoid(0,1);
	private MotorSafetyHelper safetyHelper = new MotorSafetyHelper(this);

	static double WHEEL_DIAMETER = 8; // inches
	static int ENDODER_TICKS = 900;

	boolean inlowgear = false;

	public void initDefaultCommand() {
		setDefaultCommand(new DriveWithGamepad());
	}

	public DriveTrain() {
		super();
		leftMotor = new CANTalon(LEFTWHEELS);
		rightMotor = new CANTalon(RIGHTWHEELS);
		
		rightMotor.changeControlMode(CANTalon.TalonControlMode.PercentVbus);
		leftMotor.changeControlMode(CANTalon.TalonControlMode.PercentVbus);

		int ticks_per_foot = (int) (1.0 / getFeetPerTick());

		rightMotor.configEncoderCodesPerRev(ticks_per_foot);
		leftMotor.configEncoderCodesPerRev(ticks_per_foot);
		setLowGear();
		setExpiration(0.1);
		setSafetyEnabled(true);
		Publish(true);
		reset();
	}

	// feet-per-tick = diameter(in)*pi/12/ENCODER_TICKS_PER_REV
	double getFeetPerTick() {
		if (Robot.isReal())
			return Math.PI * WHEEL_DIAMETER / 12.0 / ENDODER_TICKS; // ~1146 ticks/foot
		else
			return Math.PI * WHEEL_DIAMETER / 12.0 / 360; // hard-coded in simulation to 360
	}

	static double metersPerFoot(double x) {
		return x * 0.0254 * x / 12.0;
	}

	public void enable() {
		rightMotor.enable();
		leftMotor.enable();
		Publish(true);
	}

	public void disable() {
		rightMotor.disable();
		leftMotor.disable();
		Publish(true);
	}

	void Publish(boolean init) {
		if (init) {
			SmartDashboard.putBoolean("HighGear", false);
			SmartDashboard.putNumber("Heading", 0);
			SmartDashboard.putNumber("LeftDistance", 0);
			SmartDashboard.putNumber("RightDistance", 0);
			SmartDashboard.putNumber("Travel", 0);
			SmartDashboard.putNumber("LeftWheels", 0);
			SmartDashboard.putNumber("RightWheels", 0);
		} else {
			SmartDashboard.putBoolean("HighGear", !inlowgear);
			SmartDashboard.putNumber("Heading", getHeading());
			SmartDashboard.putNumber("LeftDistance", feet_to_inches(getLeftDistance()));
			SmartDashboard.putNumber("RightDistance", feet_to_inches(getRightDistance()));
			SmartDashboard.putNumber("Travel", feet_to_inches(getDistance()));
			SmartDashboard.putNumber("LeftWheels", round(leftMotor.get()));
			SmartDashboard.putNumber("RightWheels", round(rightMotor.get()));
		}
	}

	public static double feet_to_inches(double x) {
		return Math.round(12 * x * 100.0 / 100);
	}

	public double getRightDistance() {
		return rightMotor.getPosition();
	}

	public double getLeftDistance() {
		return leftMotor.getPosition();
	}

	public double getDistance() {
		double d1 = getRightDistance();
		double d2 = getLeftDistance();
		double x = 0.5 * (d1 + d2);
		return x;
	}

	public double getLeftVelocity() {
		return leftMotor.getSpeed();
	}

	public double getRightVelocity() {
		return rightMotor.getSpeed();
	}

	public double getVelocity() {
		double d1 = getLeftVelocity();
		double d2 = getRightVelocity();
		return 0.5 * (d1 + d2);
	}

	double round(double x) {
		return 0.001 * Math.round(x * 1000);
	}

	public double getHeading() {
		if (Robot.isReal())
			return gyro.getAngle();
		else
			return -gyro.getAngle();
	}

	public void setLowGear() {
		if (!inlowgear) {
			// gearPneumaticgearPneumaticgearPneumatic.set(DoubleSolenoid.Value.kReverse);
			System.out.println("Setting Low Gear");
			inlowgear = true;
		}
	}

	public void setHighGear() {
		if (inlowgear) {
			// gearPneumatic.set(DoubleSolenoid.Value.kForward);
			System.out.println("Setting High Gear");
			inlowgear = false;
		}
	}

	double coerce(double min, double max, double x) {
		if (x < min)
			x = min;
		else if (x > max)
			x = max;
		return x;
	}

	public void tankDrive(double left, double right) {
		leftMotor.set(left);
		rightMotor.set(right);
		Publish(false);
		safetyHelper.feed();
	}

	public void set(double left, double right) {
		tankDrive(left, right);
	}

	public void arcadeDrive(double moveValue, double rotateValue) {
		// local variables to hold the computed PWM values for the motors
		double leftMotorOutput;
		double rightMotorOutput;

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
		leftMotorOutput = coerce(-1.0, 1.0, leftMotorOutput);
		rightMotorOutput = coerce(-1.0, 1.0, rightMotorOutput);

		set(leftMotorOutput,rightMotorOutput);	
	}

	/**
	 * Reset the robots sensors to the zero states.
	 */
	public void reset() {
		gyro.reset();
		rightMotor.reset();
		leftMotor.reset();
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
		leftMotor.stopMotor();
		rightMotor.stopMotor();
		
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
