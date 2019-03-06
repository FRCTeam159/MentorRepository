package org.usfirst.frc.team159.robot.commands;

import org.usfirst.frc.team159.robot.OI;
import org.usfirst.frc.team159.robot.Robot;
import org.usfirst.frc.team159.robot.RobotMap;
import org.usfirst.frc.team159.robot.subsystems.Elevator;
import org.usfirst.frc.team159.robot.Button;

import edu.wpi.first.wpilibj.Joystick;
import edu.wpi.first.wpilibj.command.Command;

/**
 *
 */
public class ElevatorCommands extends Command implements RobotMap {

  static public double rate_scale = Elevator.CYCLE_TIME * Elevator.MAX_SPEED; // assumed

  double setpoint = 0;
  static boolean debug = false;
  Button hatchButton = new Button(ELEVATOR_GO_TO_HATCH_BUTTON);
  Button incrementButton = new Button(RIGHT_TRIGGER_BUTTON);
  Button decrementButton = new Button(LEFT_TRIGGER_BUTTON);
  Button tiltButton = new Button(ELEVATOR_TILT_BUTTON);

  public ElevatorCommands() {
    requires(Robot.elevator);
  }

  // Called just before this Command runs the first time
  protected void initialize() {
    System.out.println("ElevatorCommands.initialize");
    setpoint = Robot.elevator.getSetpoint();
    //Robot.elevator.enable();
   // System.out.println("ELEVATOR_TILT_BUTTON="+ELEVATOR_TILT_BUTTON+" id="+tiltButton.getID());
  }

  // Called repeatedly when this Command is scheduled to run
  protected void execute() {
    Joystick stick = OI.stick;

    double left = 0.5 * (1 + stick.getRawAxis(LEFT_TRIGGER));
    double right = 0.5 * (1 + stick.getRawAxis(RIGHT_TRIGGER));
    if (tiltButton.isPressed()){
      if(Robot.elevator.isTilted())
        Robot.elevator.tiltElevator(true);
      else
        Robot.elevator.tiltElevator(false);
    }
    else {
      if (hatchButton.isPressed())
        setpoint = Elevator.HATCH_HEIGHT;
      else if (incrementButton.isPressed())
        setpoint += Elevator.DELTA_TARGET_HEIGHT;
      else if (decrementButton.isPressed())
        setpoint -= Elevator.DELTA_TARGET_HEIGHT;
      else if (left > 0)
        setpoint -= left * rate_scale;
      else if (right > 0)
        setpoint += right * rate_scale;
      Robot.elevator.setPosition(setpoint);
      setpoint = Robot.elevator.getSetpoint();
    }
  }

  // Make this return true when this Command no longer needs to run execute()
  protected boolean isFinished() { // keep going while in teleop
    return false;
  }

  // Called once after isFinished returns true
  protected void end() {
    System.out.println("ElevatorCommands.end");
    setpoint = 0;
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
