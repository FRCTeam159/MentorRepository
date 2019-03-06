/*----------------------------------------------------------------------------*/
/* Copyright (c) 2018 FIRST. All Rights Reserved.                             */
/* Open Source Software - may be modified and shared by FRC teams. The code   */
/* must be accompanied by the FIRST BSD license file in the root directory of */
/* the project.                                                               */
/*----------------------------------------------------------------------------*/

package org.usfirst.frc.team159.robot.subsystems;

import com.ctre.CANTalon;

import edu.wpi.first.wpilibj.DoubleSolenoid;
import edu.wpi.first.wpilibj.Solenoid;
import edu.wpi.first.wpilibj.Servo;
import edu.wpi.first.wpilibj.command.Subsystem;
import org.usfirst.frc.team159.robot.RobotMap;
import org.usfirst.frc.team159.robot.commands.GrabberCommands;

/**
 * Add your docs here.
 */
public class Grabber extends Subsystem {
  //private CANTalon armMover = new CANTalon(RobotMap.ARM_SERVO);
  private CANTalon grabberMotor = new CANTalon(RobotMap.GRABBER_MOTOR);
  DoubleSolenoid grabPneumatic = new DoubleSolenoid(2, 3);
  DoubleSolenoid tiltPneumatic = new DoubleSolenoid(4, 5);

  private double ejectValue = 1.0;
  private double grabValue = -1.0;
  private boolean clawOpen = false;
  boolean tilted=true;

  public Grabber(){
    
  }
  @Override
  public void initDefaultCommand() {
    setDefaultCommand(new GrabberCommands());
  }
  public void init(){
    //tilt(false);
    //closeClaw();
  }
  public boolean isTilted(){
    return tilted;
  }
  public boolean isClawOpen(){
    return clawOpen;
  }

  public void closeClaw(){
    System.out.println("Grabber.closeClaw()");
    grabPneumatic.set(DoubleSolenoid.Value.kReverse);
    clawOpen = false;
  }
  public void openClaw(){
    System.out.println("Grabber.openClaw()");
    grabPneumatic.set(DoubleSolenoid.Value.kForward);
    clawOpen = true;
  }
  public void eject(){
    grabberMotor.set(ejectValue);
  }
  public void grab(){
    grabberMotor.set(grabValue);
  }
  public void hold(){
    grabberMotor.set(0);
  }
  public void dropGrabber(){
    //armMover.set(RobotMap.GRABBER_SERVO_VALUE);
    tilt(true);
  }
  public void tilt(boolean forward){
    if(forward){
      System.out.println("Grabber.tilt(forward)");
      tiltPneumatic.set(DoubleSolenoid.Value.kReverse);
      tilted=true;
    }
    else{
      System.out.println("Grabber.tilt(back)");
      tiltPneumatic.set(DoubleSolenoid.Value.kForward);
      tilted=false;
    }
  }
}
