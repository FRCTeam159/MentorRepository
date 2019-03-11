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

	private CANTalon front_motor;
	private CANTalon back_motor;

	
	public void initDefaultCommand() {
		 setDefaultCommand(new ClimberCmds());
	}
	
	public Climber() {
		super();
		front_motor = new CANTalon(RobotMap.FRONT_CLIMBER_MOTOR);
		back_motor = new CANTalon(RobotMap.BACK_CLIMBER_MOTOR);
	}
	public void init() {
		enable();
		setFront(-0.1);
		setBack(-0.1);
	}

	public void enable() {
		front_motor.enable();
		back_motor.enable();
	}
	
	public void setFront(double v) {
		front_motor.set(v);
	}
	public void setBack(double v) {
		back_motor.set(v);
	}
}
