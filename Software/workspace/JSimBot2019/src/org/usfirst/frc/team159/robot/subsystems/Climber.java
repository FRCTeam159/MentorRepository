package org.usfirst.frc.team159.robot.subsystems;

import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.RobotMap;
import org.usfirst.frc.team159.robot.commands.ClimberCmds;

import com.ctre.CANTalon;

import edu.wpi.first.wpilibj.command.Subsystem;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;

/**
 *
 */
public class Climber extends Subsystem{

	private CANTalon motor;
	
	public void initDefaultCommand() {
		 setDefaultCommand(new ClimberCmds());
	}
	
	public Climber() {
		super();
		motor = new CANTalon(RobotMap.CLIMBER);
	}
	
	public void enable() {
		motor.enable();
	}
	
	public void set(double v) {
		motor.set(v);
	}
	
}
