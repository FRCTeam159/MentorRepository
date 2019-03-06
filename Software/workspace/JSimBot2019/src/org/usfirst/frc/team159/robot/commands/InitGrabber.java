/*----------------------------------------------------------------------------*/
/* Copyright (c) 2018 FIRST. All Rights Reserved.                             */
/* Open Source Software - may be modified and shared by FRC teams. The code   */
/* must be accompanied by the FIRST BSD license file in the root directory of */
/* the project.                                                               */
/*----------------------------------------------------------------------------*/

package org.usfirst.frc.team159.robot.commands;

import edu.wpi.first.wpilibj.command.Command;
import org.usfirst.frc.team159.robot.Robot;
import edu.wpi.first.wpilibj.Timer;

public class InitGrabber extends Command {
  Timer timer = new Timer();
  static final int UNINITIALIZED = 0;
  static final int ARMS_OPEN = 1;
  static final int GRABBER_DROPPED = 2;
  int state = UNINITIALIZED;
  boolean done = false;
  public InitGrabber() {
    // Use requires() here to declare subsystem dependencies
    // eg. requires(chassis);
  }

  // Called just before this Command runs the first time
  @Override
  protected void initialize() {
    System.out.println("InitGrabber OpenClaw");
    timer.start();
    timer.reset();
    Robot.grabber.closeClaw();
  }

  // Called repeatedly when this Command is scheduled to run
  @Override
  protected void execute() {
    switch (state) {
      case UNINITIALIZED:
        if (timer.get() > 0.5){
          state = ARMS_OPEN;
          System.out.println("InitGrabber dropGrabber");
          Robot.grabber.dropGrabber();
          timer.reset();
        }
        break;
      case ARMS_OPEN:
        if (timer.get() > 0.75)
          state = GRABBER_DROPPED;
        break;
      case GRABBER_DROPPED:
        done = true;
        break;
      }
  }

  // Make this return true when this Command no longer needs to run execute()
  @Override
  protected boolean isFinished() {
    return done;
  }

  // Called once after isFinished returns true
  @Override
  protected void end() {
    System.out.println("InitGrabber end");
  }

  // Called when another command which requires one or more of the same
  // subsystems is scheduled to run
  @Override
  protected void interrupted() {
    System.out.println("InitGrabber interrupted");
  }
}
