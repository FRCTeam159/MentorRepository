package org.usfirst.frc.team159.robot.commands;

import org.usfirst.frc.team159.robot.Robot;
import edu.wpi.first.wpilibj.command.CommandGroup;

/**
 *
 */
public class Autonomous extends CommandGroup {

    public Autonomous() {
		//addSequential(new InitGrabber());
		//addSequential(new Calibrate());
		addSequential(new DrivePath());
		//addSequential(new DriveStraight(8.0));
		addSequential(new InitElevator());
		addSequential(new InitGrabber());
		addSequential(new EndAuto());
    }
}
