/*----------------------------------------------------------------------------*/
/* Copyright (c) 2018 FIRST. All Rights Reserved.                             */
/* Open Source Software - may be modified and shared by FRC teams. The code   */
/* must be accompanied by the FIRST BSD license file in the root directory of */
/* the project.                                                               */
/*----------------------------------------------------------------------------*/

package org.usfirst.frc.team159.robot.commands;

import edu.wpi.first.wpilibj.command.TimedCommand;
import org.usfirst.frc.team159.robot.Robot;
import edu.wpi.first.wpilibj.Timer;

public class DropGrabber extends TimedCommand {
  public DropGrabber(double delay) {
    super(delay);
    requires(Robot.grabber);
  }

  // Called just before this Command runs the first time
  @Override
  protected void initialize() {
    System.out.println("DropGrabber initialize");
    Robot.grabber.dropGrabber();
  }

  // Called repeatedly when this Command is scheduled to run
  @Override
  protected void execute() {  
  }

  // Make this return true when this Command no longer needs to run execute()
  @Override
  protected boolean isFinished() {
    return super.isFinished();
  }

  // Called once after isFinished returns true
  @Override
  protected void end() {
    System.out.println("DropGrabber end");
  }

  // Called when another command which requires one or more of the same
  // subsystems is scheduled to run
  @Override
  protected void interrupted() {
    System.out.println("DropGrabber interrupted");
    end();
  }
}
