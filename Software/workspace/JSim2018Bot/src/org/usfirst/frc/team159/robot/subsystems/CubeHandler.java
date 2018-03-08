package org.usfirst.frc.team159.robot.subsystems;

import java.util.LinkedList;

import org.usfirst.frc.team159.robot.RobotMap;
import org.usfirst.frc.team159.robot.commands.CubeCommands;

import com.ctre.CANTalon;

import edu.wpi.first.wpilibj.AnalogInput;
import edu.wpi.first.wpilibj.Solenoid;
import edu.wpi.first.wpilibj.command.Subsystem;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;

/**
 *
 */
public class CubeHandler extends Subsystem implements RobotMap {

  // Put methods for controlling this subsystem
  // here. Call these from Commands.
  private CANTalon wheels;
  // private static final double intakePower = 0.5;
  // private static final double outputPower = 0.25;

  boolean outputStarted = false;
  boolean intakeStarted = false;
  boolean armsOpen = false;
  double distance;
  double speed = 0;

  double cubeMaxDetectValue = 0.25;
  double cubeMinDetectValue = 0.1;

  boolean cube_obtained = false;
  private LinkedList<Double> vals = null;
  private double total = 0;
  int aves = 4;

  Solenoid piston;
  //AnalogInput cubeSensor;
  int state = HOLD;
  private String which_state[] = { "Drop", "Hold", "Push", "Grab" };

  public CubeHandler() {
    super();
    wheels = new CANTalon(RobotMap.CUBEWHEELS);
    piston = new Solenoid(1, 1);
    //cubeSensor = new AnalogInput(2);
    reset();
  }

  public void initDefaultCommand() {
    setDefaultCommand(new CubeCommands());
  }

  public void reset() {
    vals = new LinkedList<Double>();
    total = 0;
    disable();
  }

  public void disable() {
    openArms();
    hold();
    logStatus();
  }

  public void enable() {
    total = 0;
    closeArms();
    grab();
    logStatus();
  }

  public void push() {
    speed = 1.0;
    wheels.set(speed);
    state = PUSH;
  }

  public void grab() {
    speed = -1.0;
    wheels.set(speed);
    state = GRAB;
  }

  public void hold() {
    speed = -0.1;
    wheels.set(speed);
    state = HOLD;
  }

  public void drop() {
    speed = 0.0;
    openArms();
    state = DROP;
  }

  public int getState() {
    return state;
  }

  public void spinWheels() {
    wheels.set(speed);
  }

  public void setState(int state) {
    switch(state) {
    case PUSH: push();break;
    case GRAB: grab();break;
    case HOLD: hold();break;
    case DROP: drop();break;
    }
    logStatus();
  }

  public boolean cubeDetected() {
    getStatus();
    return cube_obtained;
  }

  public boolean armsOpen() {
    return armsOpen;
  }

  public void openArms() {
    piston.set(true);
    armsOpen = true;
  }

  public void closeArms() {
    piston.set(false);
    armsOpen = false;
  }

//  double rangefinderDistance() {
//    double d = cubeSensor.getAverageVoltage();
//    return d;
//  }

  void getStatus() {
    //double d = rollingAverage(rangefinderDistance());
    //cube_obtained = (d >= 0.5) ? true : false;
    logStatus();
    // System.out.println("d="+rangefinderDistance()+" got="+cube_obtained);
  }

  void logStatus() {
   // SmartDashboard.putNumber("Range", 0.01 * Math.round(distance * 100));
    //SmartDashboard.putBoolean("Cube", cube_obtained);
    SmartDashboard.putBoolean("ArmsOpen", armsOpen);
    SmartDashboard.putString("State", which_state[state]);
  }

//  private double rollingAverage(double d) {
//    double havecube = (d >= cubeMinDetectValue && d <= cubeMaxDetectValue) ? 1.0 : 0.0;
//    if (vals.size() == aves)
//      total -= vals.removeFirst().doubleValue();
//    vals.addLast(havecube);
//    total += havecube;
//    return total / vals.size();
//  }

}
