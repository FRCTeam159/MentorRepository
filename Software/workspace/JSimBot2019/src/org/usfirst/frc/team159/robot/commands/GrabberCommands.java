/*----------------------------------------------------------------------------*/
/* Copyright (c) 2018 FIRST. All Rights Reserved.                             */
/* Open Source Software - may be modified and shared by FRC teams. The code   */
/* must be accompanied by the FIRST BSD license file in the root directory of */
/* the project.                                                               */
/*----------------------------------------------------------------------------*/

package org.usfirst.frc.team159.robot.commands;

import edu.wpi.first.wpilibj.command.Command;
import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.RobotMap;
import org.usfirst.frc.team159.robot.Button;
import edu.wpi.first.wpilibj.Joystick;
import org.usfirst.frc.team159.robot.OI;

public class GrabberCommands extends Command implements RobotMap{
  Button clawButton=new Button(ARMS_TOGGLE_BUTTON);
  Button tiltButton = new Button(GRABBER_TILT_BUTTON);

  public GrabberCommands() {
    // Use requires() here to declare subsystem dependencies
    requires(Robot.grabber);
  }

  // Called just before this Command runs the first time
  @Override
  protected void initialize() {
    System.out.println("GrabberCommands.initialize()");
    System.out.println("GRABBER_TILT_BUTTON="+GRABBER_TILT_BUTTON+" id="+tiltButton.getID());

    //Robot.grabber.closeClaw();
  }

  // Called repeatedly when this Command is scheduled to run
  @Override
  protected void execute() {
    Joystick stick = OI.stick;
    boolean intakeButtonPressed = stick.getRawButton(INTAKE_BUTTON);
    boolean outputButtonPressed = stick.getRawButton(OUTPUT_BUTTON);
    
    if (clawButton.isPressed()){
      if (Robot.grabber.isClawOpen())
        Robot.grabber.closeClaw();
      else 
        Robot.grabber.openClaw();
    }
    if (tiltButton.isPressed()){
      if(Robot.grabber.isTilted())
        Robot.grabber.tilt(false);
      else
        Robot.grabber.tilt(true);
    }
    if (intakeButtonPressed)
      Robot.grabber.grab();
    else if (outputButtonPressed)
      Robot.grabber.eject();
    else 
      Robot.grabber.hold();
  }

  // Make this return true when this Command no longer needs to run execute()
  @Override
  protected boolean isFinished() {
    return false;
  }

  // Called once after isFinished returns true
  @Override
  protected void end() {
    System.out.println("GrabberCommands.end()");
  }

  // Called when another command which requires one or more of the same
  // subsystems is scheduled to run
  @Override
  protected void interrupted() {
    System.out.println("GrabberCommands.interrupted()");
    end();
  }
}
