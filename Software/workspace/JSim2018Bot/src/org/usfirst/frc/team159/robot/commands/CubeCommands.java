package org.usfirst.frc.team159.robot.commands;

import org.usfirst.frc.team159.robot.OI;
import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.RobotMap;

import edu.wpi.first.wpilibj.Joystick;
import edu.wpi.first.wpilibj.command.Command;

/**
 *
 */
public class CubeCommands extends Command implements RobotMap {
  boolean armsButtonReleased;
  boolean cubePresent;
  boolean armsOpen = false;

  int state = HOLD;

  public CubeCommands() {
    // Use requires() here to declare subsystem dependencies
    // eg. requires(chassis);
    requires(Robot.cubeHandler);
  }

  // Called just before this Command runs the first time
  protected void initialize() {
    armsButtonReleased = false;
    cubePresent = Robot.cubeHandler.cubeDetected();
    if (cubePresent)
      Robot.cubeHandler.hold();
    else
      Robot.cubeHandler.grab();
  }

  // Called repeatedly when this Command is scheduled to run
  protected void execute() {
    Joystick stick = OI.stick;
    cubePresent = Robot.cubeHandler.cubeDetected();
    toggleArms();
    if (stick.getRawButton(RobotMap.CUBE_PUSH_BUTTON))
      Robot.cubeHandler.push();
    else if (stick.getRawButton(RobotMap.CUBE_GRAB_BUTTON))
      Robot.cubeHandler.grab();
    else
      Robot.cubeHandler.hold();
//    switch (Robot.cubeHandler.getState()) {
//    case HOLD:
//      if (stick.getRawButton(RobotMap.CUBE_PUSH_BUTTON))
//        Robot.cubeHandler.push();
//      else if (!cubePresent)
//        Robot.cubeHandler.grab();
//      break;
//    case GRAB:
//      if (cubePresent)
//        Robot.cubeHandler.hold();
//      break;
//    case PUSH:
//      if (!cubePresent)
//        Robot.cubeHandler.grab();
//      break;
//    case DROP:
//      if (!cubePresent)
//        Robot.cubeHandler.grab();
//      break;
//    }
    Robot.cubeHandler.spinWheels();
  }

  void toggleArms() {
    Joystick stick = OI.stick;
    boolean armsButtonPressed = stick.getRawButton(RobotMap.ARM_TOGGLE_BUTTON);
    armsOpen = Robot.cubeHandler.armsOpen();
    if (!armsButtonPressed)
      armsButtonReleased = true;
    else if (armsButtonReleased && armsButtonPressed && armsOpen) {
      Robot.cubeHandler.closeArms();
      armsButtonReleased = false;
    } else if (armsButtonReleased && armsButtonPressed && !armsOpen) {
      Robot.cubeHandler.openArms();
      armsButtonReleased = false;
    }
  }

  // Make this return true when this Command no longer needs to run execute()
  protected boolean isFinished() {
    return false;
  }

  // Called once after isFinished returns true
  protected void end() {
  }

  // Called when another command which requires one or more of the same
  // subsystems is scheduled to run
  protected void interrupted() {
    end();
  }
}
