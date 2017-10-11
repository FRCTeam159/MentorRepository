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

	static double WHEEL_DIAMETER=3.0; // inches
	static int ENDODER_TICKS=900;

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
		
		int ticks_per_foot=(int)(1.0/getFeetPerTick());
		
		frontRight.configEncoderCodesPerRev(ticks_per_foot);
		backLeft.configEncoderCodesPerRev(ticks_per_foot);
		setLowGear();
	    setExpiration(0.1);
	    setSafetyEnabled(true);
	    Publish(true);    
	}
	// feet-per-tick = diameter(in)*pi/12/ENCODER_TICKS_PER_REV
	double getFeetPerTick() {
		if (Robot.isReal()) 
			return Math.PI*WHEEL_DIAMETER/12.0/ENDODER_TICKS; // ~1146 ticks/foot
		else 
			return Math.PI*WHEEL_DIAMETER/12.0/360;	// hard-coded in simulation to 360			
	}
	void Publish(boolean init) {
		if(init){
			SmartDashboard.putBoolean("HighGear", false);
			SmartDashboard.putNumber("Heading", 0);
			SmartDashboard.putNumber("LeftDistance", 0);
			SmartDashboard.putNumber("RightDistance", 0);
			SmartDashboard.putNumber("Travel", 0);
			SmartDashboard.putNumber("LeftWheels", 0);
			SmartDashboard.putNumber("RightWheels", 0);
		}else{
			SmartDashboard.putBoolean("HighGear", !inlowgear);
			SmartDashboard.putNumber("Heading", getHeading());
			SmartDashboard.putNumber("LeftDistance", distance_in_inches(getLeftDistance()));
			SmartDashboard.putNumber("RightDistance", distance_in_inches(getRightDistance()));
			SmartDashboard.putNumber("Travel",distance_in_inches(getDistance()));
			SmartDashboard.putNumber("LeftWheels", backLeft.get());
			SmartDashboard.putNumber("RightWheels", frontRight.get());
		}
	}
	double distance_in_inches(double x) {
		return Math.round(12*x*100.0/100);
	}
	double getRightDistance() {
		return frontRight.getPosition();
	}
	double getLeftDistance() {
		return backLeft.getPosition();
	}
	double getDistance() {
		double d1=getRightDistance();
		double d2=getLeftDistance();
		double x=0.5*(d1+d2);
		return x;
	}
	double getHeading() {
		if (Robot.isReal()) 
			return gyro.getAngle();
		else 
			return -gyro.getAngle();
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
	public void tankDrive(double left, double right) {
		backLeft.set(left);
		frontRight.set(-right);
		backRight.set(RobotMap.FRONTRIGHT);
		frontLeft.set(RobotMap.BACKLEFT);

		Publish(false);
		safetyHelper.feed();
	}

	public void customArcade(double xAxis, double yAxis, double zAxis, boolean squaredInputs) {
		double left=0;
		double right=0;

		if (squaredInputs) {
			if (xAxis >= 0.0) {
				xAxis = (xAxis * xAxis);
			} else {
				xAxis = -(xAxis * xAxis);
			}
			if (yAxis >= 0.0) {
				yAxis = (yAxis * yAxis);
			} else {
				yAxis = -(yAxis * yAxis);
			}
		}

		if (zAxis != 0) {
			xAxis = zAxis;
			yAxis = -zAxis;
		}

		if (xAxis > 0.0) {
			if (yAxis > 0.0) {
				left = xAxis - yAxis;
				right = Math.max(xAxis, yAxis);
			} else {
				left = Math.max(xAxis, -yAxis);
				right = xAxis + yAxis;
			}
		} else {
			if (yAxis > 0.0) {
				left = -Math.max(-xAxis, yAxis);
				right = xAxis + yAxis;
			} else {
				left = xAxis - yAxis;
				right = -Math.max(-xAxis, -yAxis);
			}
		}
		if(inlowgear){
			right*=0.5;
			left*=0.5;
		}
		
		backLeft.set(left);
		frontRight.set(-right);
		backRight.set(RobotMap.FRONTRIGHT);
		frontLeft.set(RobotMap.BACKLEFT);
	
		safetyHelper.feed();
		Publish(false);
	}

	/**
	 * Reset the robots sensors to the zero states.
	 */
	public void reset() {
		gyro.reset();
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
