package org.usfirst.frc.team159.robot.commands;

import org.usfirst.frc.team159.robot.OI;
import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.RobotMap;
import org.usfirst.frc.team159.robot.subsystems.Elevator;

import javax.lang.model.util.ElementScanner6;

import org.usfirst.frc.team159.robot.Button;

import edu.wpi.first.wpilibj.Joystick;
import edu.wpi.first.wpilibj.command.Command;

/**
 *
 */
public class ElevatorCommands extends Command implements RobotMap {

  //static public double rate_scale = Elevator.CYCLE_TIME * Elevator.MAX_SPEED; // assumed

  //double setpoint = 0;
  static boolean debug = false;
  Button baseHtButton = new Button(ELEVATOR_RESET_HEIGHT_BUTTON);
  Button incrementButton = new Button(RIGHT_TRIGGER_BUTTON);
  Button decrementButton = new Button(LEFT_TRIGGER_BUTTON);
  Button tiltButton = new Button(ELEVATOR_TILT_BUTTON);

  public ElevatorCommands() {
    requires(Robot.elevator);
  }

  // Called just before this Command runs the first time
  protected void initialize() {
    System.out.println("ElevatorCommands.initialize");
    //setpoint = Robot.elevator.getSetpoint();
  }

  // Called repeatedly when this Command is scheduled to run
  protected void execute() {
    Joystick stick = OI.stick;

    double left = 0.5 * (1 + stick.getRawAxis(LEFT_TRIGGER));
    double right = 0.5 * (1 + stick.getRawAxis(RIGHT_TRIGGER));
    double stopAxis=stick.getRawAxis(EMERGENCY_STOP);
    double modeAxis=stick.getRawAxis(HATCH_CARGO_MODE);
    
    if(modeAxis < -0.99)
      Robot.hatchMode=false;
    else if (modeAxis >0.99)
      Robot.hatchMode=true;

    Robot.elevator.checkMode();

    if (tiltButton.isPressed()){
      if(Robot.elevator.isTilted())
        Robot.elevator.tiltElevator(true);
      else
        Robot.elevator.tiltElevator(false);
    }
    if(stopAxis < -0.99)
      Robot.elevator.stopElevator();
    else if (stopAxis > 0.99)
      Robot.elevator.enableElevator();
    if (Robot.elevator.isEnabled()){
      if (baseHtButton.isPressed())
        Robot.elevator.resetLevel();
      else if (incrementButton.isPressed())
        Robot.elevator.incrLevel();
      else if (decrementButton.isPressed())
        Robot.elevator.decrLevel();
      else if (left > 0)
        Robot.elevator.stepDown(left);
      else if (right > 0)
        Robot.elevator.stepUp(right);
      else
        Robot.elevator.log();
    }
  }

  // Make this return true when this Command no longer needs to run execute()
  protected boolean isFinished() { // keep going while in teleop
    return false;
  }

  // Called once after isFinished returns true
  protected void end() {
    System.out.println("ElevatorCommands.end");
    //setpoint = 0;
    //Robot.elevator.reset();
    //Robot.elevator.disable();
  }

  // Called when another command which requires one or more of the same
  // subsystems is scheduled to run
  protected void interrupted() {
    System.out.println("ElevatorCommands.interrupted");
    end();
  }
}
