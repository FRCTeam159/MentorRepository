package org.usfirst.frc.team159.robot.subsystems;

import edu.wpi.first.wpilibj.command.Subsystem;
import edu.wpi.first.wpilibj.livewindow.LiveWindow;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;

import org.usfirst.frc.team159.robot.commands.DriveWithJoystick;

import edu.wpi.first.wpilibj.AnalogGyro;
import edu.wpi.first.wpilibj.Encoder;
import edu.wpi.first.wpilibj.SpeedController;
import edu.wpi.first.wpilibj.Talon;
import edu.wpi.first.wpilibj.DoubleSolenoid;
import edu.wpi.first.wpilibj.MotorSafetyHelper;
import edu.wpi.first.wpilibj.MotorSafety;



import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.RobotMap;

/**
 *
 */
public class DriveTrain extends Subsystem implements MotorSafety{

	private SpeedController frontLeft = new Talon(RobotMap.FRONTLEFT);
	private SpeedController frontRight = new Talon(RobotMap.FRONTRIGHT);
	private SpeedController backLeft = new Talon(RobotMap.BACKLEFT);
	private SpeedController backRight = new Talon(RobotMap.BACKRIGHT);
	
	private Encoder leftEncoder = new Encoder(3, 4);
	private Encoder rightEncoder = new Encoder(7, 8);
	
	private AnalogGyro gyro = new AnalogGyro(3);
	
	private DoubleSolenoid gearPneumatic = new DoubleSolenoid(0,1);
	private MotorSafetyHelper safetyHelper = new MotorSafetyHelper(this);

	
	boolean inlowgear=false;

	// Put methods for controlling this subsystem
	// here. Call these from Commands.

	public void initDefaultCommand() {
		// Set the default command for a subsystem here.
		 setDefaultCommand(new DriveWithJoystick());
	}
	
	public DriveTrain() {
		super();

		// Encoders may measure differently in the real world and in
		// simulation. In this example the robot moves 0.042 barleycorns
		// per tick in the real world, but the simulated encoders
		// simulate 360 tick encoders. This if statement allows for the
		// real robot to handle this difference in devices.
		if (Robot.isReal()) {
			leftEncoder.setDistancePerPulse(0.042);
			rightEncoder.setDistancePerPulse(0.042);
		} else {
			// Circumference in ft = 3in/12(in/ft)*PI
			leftEncoder.setDistancePerPulse((3.0 / 12.0 * Math.PI) / 360.0);
			rightEncoder.setDistancePerPulse((3.0 / 12.0 * Math.PI) / 360.0);
		}
		setLowGear();
	    safetyHelper.setExpiration(0.1);
	    safetyHelper.setSafetyEnabled(true);
	    Publish(true);
	    
	}
	void Publish(boolean init) {
		if(init){
			SmartDashboard.putBoolean("HighGear", false);
			SmartDashboard.putNumber("Heading", 0);
			SmartDashboard.putNumber("LeftDistance", 0);
			SmartDashboard.putNumber("RightDistance", 0);
			SmartDashboard.putNumber("Travel", 0);

		}else{
			SmartDashboard.putBoolean("HighGear", !inlowgear);
			SmartDashboard.putNumber("Heading", getHeading());
			SmartDashboard.putNumber("LeftDistance", round(getLeftDistance()));
			SmartDashboard.putNumber("RightDistance", round(getRightDistance()));
			SmartDashboard.putNumber("Travel",round(getDistance()));
		}
	}
	double round(double x) {
		return Math.round(x*100/100);
	}

	double getRightDistance() {
		return -rightEncoder.getDistance();
	}
	double getLeftDistance() {
		return -leftEncoder.getDistance(); // inverted in simulation for some reason (sdf file axis direction problem ?)
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
	public void TankDrive(double left, double right) {
		frontLeft.set(left);
		backLeft.set(left);
		frontRight.set(-right);
		backRight.set(-right);

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
		
		frontLeft.set(left);
		backLeft.set(left);
		frontRight.set(-right);
		backRight.set(-right);
	
		
		safetyHelper.feed();
		Publish(false);
	}

	/**
	 * Reset the robots sensors to the zero states.
	 */
	public void reset() {
		gyro.reset();
		leftEncoder.reset();
		rightEncoder.reset();
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
