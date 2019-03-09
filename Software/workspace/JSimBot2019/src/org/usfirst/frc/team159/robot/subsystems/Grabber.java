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
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;

import org.usfirst.frc.team159.robot.Robot;
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

  private double spinCW = 1.0;
  private double spinCCW = -1.0;
  private boolean clawOpen = false;
  private boolean inputActive=false;
  private boolean ejectActive=false;

  boolean tilted=false;

  public Grabber(){
   log();
  }
  @Override
  public void initDefaultCommand() {
    setDefaultCommand(new GrabberCommands());
  }
  public void init(){
    tilt(false);
    openClaw();
  }
  public boolean isTilted(){
    return tilted;
  }
  public boolean isClawOpen(){
    return clawOpen;
  }
  public void closeClaw(){
    grabPneumatic.set(DoubleSolenoid.Value.kReverse);
    clawOpen = false;
    log();
  }
  public void openClaw(){
    grabPneumatic.set(DoubleSolenoid.Value.kForward);
    clawOpen = true;
    log();
  }
  public void eject(){
    if(Robot.hatchMode)
      grabberMotor.set(spinCW);
    else
      grabberMotor.set(spinCCW);
    ejectActive=true;
    log();
  }
  public void grab(){
    if(Robot.hatchMode)
      grabberMotor.set(spinCCW);
    else
      grabberMotor.set(spinCW);
    inputActive=true;
    log();
  }
  public void hold(){
    grabberMotor.set(0);
    inputActive=false;
    ejectActive=false;
    log();
  }
  public void dropGrabber(){
    tilt(true);
  }
  public void tilt(boolean forward){
    if(forward){
      System.out.println("Grabber.tilt(forward)");
      tiltPneumatic.set(DoubleSolenoid.Value.kReverse);
      tilted=false;
    }
    else{
      System.out.println("Grabber.tilt(back)");
      tiltPneumatic.set(DoubleSolenoid.Value.kForward);
      tilted=true;
    }
    log();
  }
  void log(){
    SmartDashboard.putBoolean("Input On", inputActive);
    SmartDashboard.putBoolean("Eject On", ejectActive);
    SmartDashboard.putBoolean("Claw Open", clawOpen);
    SmartDashboard.putBoolean("Grabber Tilted", tilted);
    SmartDashboard.putBoolean("Hatch-Cargo", Robot.hatchMode);
  }
}
