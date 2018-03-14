package org.usfirst.frc.team159.robot.commands;

import edu.wpi.first.wpilibj.command.Command;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;

import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.OI;
import edu.wpi.first.wpilibj.Joystick;
import org.usfirst.frc.team159.robot.RobotMap;

/**
 *
 */
public class DriveWithJoystick extends Command implements RobotMap {
  public static double MINTHRESHOLD = 0.3;
  public static double MINOUTPUT = 0;
  boolean gearButtonReleased;
  public static double turnScale = 0.8;

  public DriveWithJoystick() {
    // Use requires() here to declare subsystem dependencies
    requires(Robot.driveTrain);
    SmartDashboard.putNumber("Turn Scale", turnScale);
  }

  // Called just before this Command runs the first time
  @Override
  protected void initialize() {
    System.out.println("DriveWithJoystick::initialize()");
    // Robot.driveTrain.setHighGear();
  }

  // Called repeatedly when this Command is scheduled to run
  @Override
  protected void execute() {
    // Get axis values
    Joystick stick = OI.stick;
    double move = 0; // left stick - drive
    double rotate = 0; // right stick - rotate
    move = -stick.getRawAxis(1); // left stick - drive
    rotate = -stick.getRawAxis(3); // right stick - rotate
    turnScale = SmartDashboard.getNumber("Turn Scale", turnScale);
    rotate *= Math.abs(move) * (1 - turnScale) + turnScale;

    Robot.driveTrain.arcadeDrive(move, rotate, SQUARE_INPUTS);
  }

  // Make this return true when this Command no longer needs to run execute()
  @Override
  protected boolean isFinished() {
    return false;
  }

  // Called once after isFinished returns true
  @Override
  protected void end() {
    System.out.println("DriveWithJoystick::end()");
  }

  // Called when another command which requires one or more of the same
  // subsystems is scheduled to run
  @Override
  protected void interrupted() {
    System.out.println("DriveWithJoystick::interrupted()");
  }

  void toggleGear() {
    Joystick stick = OI.stick;
    boolean gearButtonPressed = stick.getRawButton(RobotMap.GEAR_TOGGLE_BUTTON);
    boolean inLow = Robot.driveTrain.inLowGear();
    if (!gearButtonPressed)
      gearButtonReleased = true;
    else if (gearButtonReleased && gearButtonPressed && inLow) {
      Robot.driveTrain.setHighGear();
      gearButtonReleased = false;
    } else if (gearButtonReleased && gearButtonPressed && !inLow) {
      Robot.driveTrain.setLowGear();
      gearButtonReleased = false;
    }
  }

  /**
   * @param minThreshold
   * @param minOutput
   * @param input
   * @return
   */
  protected double quadDeadband(double minThreshold, double minOutput, double input) {
    if (input > minThreshold) {
      return ((((1 - minOutput) / ((1 - minThreshold) * (1 - minThreshold)))
          * ((input - minThreshold) * (input - minThreshold))) + minOutput);
    } else {
      if (input < (-1 * minThreshold)) {
        return (((minOutput - 1) / ((minThreshold - 1) * (minThreshold - 1)))
            * ((minThreshold + input) * (minThreshold + input))) - minOutput;
      } else {
        return 0;
      }
    }
  }
}
