/*----------------------------------------------------------------------------*/
/* Copyright (c) 2018 FIRST. All Rights Reserved.                             */
/* Open Source Software - may be modified and shared by FRC teams. The code   */
/* must be accompanied by the FIRST BSD license file in the root directory of */
/* the project.                                                               */
/*----------------------------------------------------------------------------*/

package org.usfirst.frc.team159.robot.commands;

import edu.wpi.first.wpilibj.command.TimedCommand;
import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.subsystems.Elevator;

public class RaiseElevator extends TimedCommand {
  public RaiseElevator(double delay) {
    super(delay);
    requires(Robot.elevator);
  }
  // Called just before this Command runs the first time
  @Override
  protected void initialize() {
    //Robot.elevator.enable();
    System.out.println("RaiseElevator initialized");
    Robot.elevator.resetLevel();
  }

  // Called repeatedly when this Command is scheduled to run
  @Override
  protected void execute() {
    Robot.elevator.log();
  }

  // Make this return true when this Command no longer needs to run execute()
  @Override
  protected boolean isFinished() {
    return super.isFinished();
  }

  // Called once after isFinished returns true
  @Override
  protected void end() {
    System.out.println("RaiseElevator end");
  }

  // Called when another command which requires one or more of the same
  // subsystems is scheduled to run
  @Override
  protected void interrupted() {
    System.out.println("RaiseElevator interrupted");
    end();
  }

}
