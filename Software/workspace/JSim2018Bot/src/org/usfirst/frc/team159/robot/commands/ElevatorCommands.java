package org.usfirst.frc.team159.robot.commands;

import org.usfirst.frc.team159.robot.OI;
import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.RobotMap;

import edu.wpi.first.wpilibj.Joystick;
import edu.wpi.first.wpilibj.command.Command;

/**
 *
 */
public class ElevatorCommands extends Command /*implements PIDSource, PIDOutput*/{


	static public double max_travel=40; // 40 inches ?
	static public double max_speed=40; // inches per second
	static public double cycle_time=0.02; // assumed
	static public double rate_scale=cycle_time*max_speed; // assumed

	double setpoint=0;
	static boolean debug=false;
	
	public ElevatorCommands() {
		requires(Robot.elevator);
	}

	// Called just before this Command runs the first time
	protected void initialize() {
		System.out.println("Elevator.initialize");

		//Robot.elevator.reset();
		setpoint=0;
		//Robot.elevator.disable();

		Robot.elevator.enable();

	}
	// Called repeatedly when this Command is scheduled to run
	protected void execute() {
		Joystick stick = OI.stick;
		double left  = 0.5*(1+stick.getRawAxis(RobotMap.LEFTTRIGGER));
		double right = 0.5*(1+stick.getRawAxis(RobotMap.RIGHTTRIGGER));
		if(left>0)
			decrement(left);
		else if (right>0)
			increment(right);
		setpoint=Robot.elevator.getSetpoint();
	}
	void increment(double v) {
		setpoint += v*rate_scale;
		Robot.elevator.setPosition(setpoint);
	}
	void decrement(double v) {
		setpoint -= v*rate_scale;
		Robot.elevator.setPosition(setpoint);
	}
	// Make this return true when this Command no longer needs to run execute()
	protected boolean isFinished() { // keep going while in teleop
		return false;
	}

	// Called once after isFinished returns true
	protected void end() {
		System.out.println("Elevator.end");
		setpoint=0;
		Robot.elevator.reset();
		Robot.elevator.disable();
	}

	// Called when another command which requires one or more of the same
	// subsystems is scheduled to run
	protected void interrupted() {
		System.out.println("Elevator.interrupted");
		end();
	}

}