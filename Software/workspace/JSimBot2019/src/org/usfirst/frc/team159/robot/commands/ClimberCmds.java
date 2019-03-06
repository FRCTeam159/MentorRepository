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
	public static double MIN_VALUE=-0.1;
	public static double MAX_VALUE=1.0;
	public static double DELTA_VALUE=0.05;
	
	static double set_value=0;

	public ClimberCmds() {
		// Use requires() here to declare subsystem dependencies
		requires(Robot.climber);
	}

	// Called just before this Command runs the first time
	@Override
	protected void initialize() {
		System.out.println("ClimberCmds.initialize()");
		set_value=MIN_VALUE;
	}

	// Called repeatedly when this Command is scheduled to run
	@Override
	protected void execute() {
		Joystick stick = OI.stick;
		boolean deployPressed = stick.getRawButton(RobotMap.DEPLOY_CLIMBER_BUTTON);
		double value=0;
		
		if(deployPressed)
			set_value+=DELTA_VALUE;
		else
			set_value=MIN_VALUE;
		set_value=set_value>MAX_VALUE?MAX_VALUE:set_value;
		//System.out.println(set_value);
		Robot.climber.set(set_value);
	}

	// Make this return true when this Command no longer needs to run execute()
	@Override
	protected boolean isFinished() {
		return false;
	}

	// Called once after isFinished returns true
	@Override
	protected void end() {
		System.out.println("ClimberCmds::end()");
	}

	// Called when another command which requires one or more of the same
	// subsystems is scheduled to run
	@Override
	protected void interrupted() {
		System.out.println("ClimberCmds::interrupted()");
	}
	

}
