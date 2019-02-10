package org.usfirst.frc.team159.robot.commands;

import edu.wpi.first.wpilibj.command.Command;

import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.OI;
import edu.wpi.first.wpilibj.Joystick;
import org.usfirst.frc.team159.robot.RobotMap;

/**
 *
 */
public class ClimberCmds extends Command implements RobotMap{
	public static double MINTHRESHOLD=0.3;
	public static double MINOUTPUT=0;

	public ClimberCmds() {
		// Use requires() here to declare subsystem dependencies
		requires(Robot.climber);
	}

	// Called just before this Command runs the first time
	@Override
	protected void initialize() {
		System.out.println("ClimberCmds.initialize()");
	}

	// Called repeatedly when this Command is scheduled to run
	@Override
	protected void execute() {
		Joystick stick = OI.stick;
		double value=stick.getRawAxis(2)+0.9;
		//System.out.println(value);
		Robot.climber.set(value);
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
	protected double quadDeadband(double minThreshold, double minOutput, double input) {
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
