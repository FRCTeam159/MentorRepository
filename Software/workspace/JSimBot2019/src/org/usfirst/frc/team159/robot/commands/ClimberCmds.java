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
	public static double DELTA_VALUE=0.04;
	
	static double front_value=0;
	static double back_value=0;


	public ClimberCmds() {
		// Use requires() here to declare subsystem dependencies
		requires(Robot.climber);
	}

	// Called just before this Command runs the first time
	@Override
	protected void initialize() {
		System.out.println("ClimberCmds.initialize()");
		front_value=MIN_VALUE;
	}

	// Called repeatedly when this Command is scheduled to run
	@Override
	protected void execute() {
		Joystick stick = OI.stick;
		boolean frontPressed = stick.getRawButton(RobotMap.FRONT_CLIMBER_BUTTON);
		boolean backPressed = stick.getRawButton(RobotMap.BACK_CLIMBER_BUTTON);
		
		if(frontPressed)
			front_value+=DELTA_VALUE;
		else
			front_value=MIN_VALUE;
		if(backPressed)
			back_value+=DELTA_VALUE;
		else
			back_value=MIN_VALUE;
		front_value=front_value>MAX_VALUE?MAX_VALUE:front_value;
		front_value=front_value<MIN_VALUE?MIN_VALUE:front_value;

		back_value=back_value>MAX_VALUE?MAX_VALUE:back_value;
		back_value=back_value<MIN_VALUE?MIN_VALUE:back_value;


		System.out.println("Climber front:"+front_value+" back:"+back_value);
		Robot.climber.setFront(front_value);
		Robot.climber.setBack(back_value);
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
