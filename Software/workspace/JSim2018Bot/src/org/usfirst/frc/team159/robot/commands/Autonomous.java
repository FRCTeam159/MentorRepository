package org.usfirst.frc.team159.robot.commands;

import org.usfirst.frc.team159.robot.Robot;
import edu.wpi.first.wpilibj.command.CommandGroup;

/**
 *
 */
public class Autonomous extends CommandGroup {

    public Autonomous() {
		requires(Robot.driveTrain);
    	//addSequential(new DriveStraight(5.0));
    //	addSequential(new DrivePath());

    }
}
