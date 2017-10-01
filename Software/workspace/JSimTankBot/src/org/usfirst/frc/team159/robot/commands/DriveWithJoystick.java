package org.usfirst.frc.team159.robot.commands;

import edu.wpi.first.wpilibj.command.Command;

import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.OI;
import edu.wpi.first.wpilibj.Joystick;
import org.usfirst.frc.team159.robot.RobotMap;

/**
 *
 */
public class DriveWithJoystick extends Command {
	public static double MINTHRESHOLD=0.3;
	public static double MINOUTPUT=0;

	public DriveWithJoystick() {
		// Use requires() here to declare subsystem dependencies
		requires(Robot.driveTrain);
	}

	// Called just before this Command runs the first time
	@Override
	protected void initialize() {
		System.out.println("DriveWithJoystick::initialize()");
	}

	// Called repeatedly when this Command is scheduled to run
	@Override
	protected void execute() {
		// Get axis values
		Joystick stick = OI.stick;
		double yAxis=0; // left stick - drive
		double xAxis=0; // right stick - rotate
		double zAxis=0;

		if (stick.getRawButton(RobotMap.LOWGEAR_BUTTON)){
			Robot.driveTrain.setLowGear();
		}
		else if(stick.getRawButton(RobotMap.HIGHGEAR_BUTTON)){
			Robot.driveTrain.setHighGear();
		}
	    if(RobotMap.CONTROL_TYPE == RobotMap.GAMEPAD) {
	    	yAxis=-stick.getRawAxis(4); // left stick - drive
	    	xAxis=-stick.getRawAxis(1); // right stick - rotate
	    	zAxis=0;
	    }
	    else {
	    	yAxis = stick.getY();
	    	xAxis = stick.getX();
	    	zAxis = stick.getZ();
	    }
		yAxis = quadDeadband(MINTHRESHOLD, MINOUTPUT, yAxis);
		xAxis = quadDeadband(MINTHRESHOLD, MINOUTPUT, xAxis);
		zAxis = quadDeadband(MINTHRESHOLD, MINOUTPUT, zAxis);

		Robot.driveTrain.customArcade(xAxis, yAxis, zAxis,true);	    	
	}

	// Make this return true when this Command no longer needs to run execute()
	@Override
	protected boolean isFinished() {
		return false;
	}

	// Called once after isFinished returns true
	@Override
	protected void end() {
		System.out.println("DriveWithJoystick::end()");
	}

	// Called when another command which requires one or more of the same
	// subsystems is scheduled to run
	@Override
	protected void interrupted() {
		System.out.println("DriveWithJoystick::interrupted()");
	}
	
	/**
	 * @param minThreshold
	 * @param minOutput
	 * @param input
	 * @return
	 */
	protected double quadDeadband(double minThreshold, double minOutput, double input)
	{
		if (input > minThreshold) {
			return ((((1 - minOutput)
					/ ((1 - minThreshold)* (1 - minThreshold)))
					* ((input - minThreshold)* (input - minThreshold)))
					+ minOutput);
		} else {
			if (input < (-1 * minThreshold)) {
				return (((minOutput - 1)
						/ ((minThreshold - 1)* (minThreshold - 1)))
						* ((minThreshold + input)* (minThreshold + input)))
						- minOutput;
			}
			else {
				return 0;
			}
		}
	}

}
