/*----------------------------------------------------------------------------*/
/* Copyright (c) 2018 FIRST. All Rights Reserved.                             */
/* Open Source Software - may be modified and shared by FRC teams. The code   */
/* must be accompanied by the FIRST BSD license file in the root directory of */
/* the project.                                                               */
/*----------------------------------------------------------------------------*/

package org.usfirst.frc.team159.robot.commands;

import edu.wpi.first.wpilibj.command.Command;
import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.subsystems.Elevator;

public class InitElevator extends Command {
  private final int UNINITIALIZED = 0;
  private final int ELEVATOR_UPRIGHT = 1;
  private final int ELEVATOR_AT_ZERO = 2;
  private final int ELEVATOR_INITIALIZED = 3;
  private final int ELEVATOR_AT_TARGET = 4;
  private int state = UNINITIALIZED;
  private boolean done = false;
  edu.wpi.first.wpilibj.Timer timer = new edu.wpi.first.wpilibj.Timer();

  public InitElevator() {
    requires(Robot.elevator);
  }

  // Called just before this Command runs the first time
  @Override
  protected void initialize() {
    state = UNINITIALIZED;
    timer.start();
    timer.reset();
    //Robot.elevator.enable();
    System.out.println("InitElevator initialized");
    goToHatchHeight();
  }

  // Called repeatedly when this Command is scheduled to run
  @Override
  protected void execute() {
    switch (state) {
    case UNINITIALIZED:
      double tm = timer.get();
      if (tm > 2.0) {
        timer.reset();
        state = ELEVATOR_AT_TARGET;
        Robot.elevator.tiltElevator(true);
      }
      break;
    case ELEVATOR_AT_TARGET:
      if (timer.get() > 1.0) {
        state = ELEVATOR_INITIALIZED;
      }
      break;
    case ELEVATOR_INITIALIZED:
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
    System.out.println("InitElevator end");
  }

  // Called when another command which requires one or more of the same
  // subsystems is scheduled to run
  @Override
  protected void interrupted() {
    System.out.println("InitElevator interrupted");
    end();
  }

  public void goToHatchHeight() {
    Robot.elevator.setPosition(Elevator.CARGO_HATCH_HEIGHT);
  }
}
