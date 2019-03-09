/*----------------------------------------------------------------------------*/
/* Copyright (c) 2018 FIRST. All Rights Reserved.                             */
/* Open Source Software - may be modified and shared by FRC teams. The code   */
/* must be accompanied by the FIRST BSD license file in the root directory of */
/* the project.                                                               */
/*----------------------------------------------------------------------------*/

package org.usfirst.frc.team159.robot.commands;

import edu.wpi.first.wpilibj.PIDController;
import edu.wpi.first.wpilibj.PIDOutput;
import edu.wpi.first.wpilibj.PIDSource;
import edu.wpi.first.wpilibj.PIDSourceType;
import edu.wpi.first.wpilibj.command.Command;
import org.usfirst.frc.team159.robot.subsystems.GripPipeline;
import org.usfirst.frc.team159.robot.subsystems.VisionProcess;
import edu.wpi.first.networktables.NetworkTableInstance;
import edu.wpi.first.networktables.NetworkTable;
import edu.wpi.first.networktables.NetworkTableEntry;
import org.usfirst.frc.team159.robot.Robot;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;

public class DriveToTarget extends Command implements PIDSource, PIDOutput {
  NetworkTable table;
  double targetDistance = 12.0;
  //private static final double kP = 0.02;
  //private static final double minError = kP * 3;
  private double error = 0;
  private PIDController pid;

  private static final double P = 0.1;
	private static final double I = 0.02;
	private static final double D = 0.0;
  private static final double TOL = 0.05;
  private static PIDSourceType type = PIDSourceType.kDisplacement;

  public DriveToTarget(double d) {
    pid = new PIDController(P, I, D, this, this, 0.02);
    targetDistance = d;

  }

 
  // Called just before this Command runs the first time
  @Override
  protected void initialize() {
    NetworkTableInstance inst = NetworkTableInstance.getDefault();
    table = inst.getTable("TargetData");
    System.out.println("DriveToTarget.initialize");
    pid.setAbsoluteTolerance(1.0);
    pid.reset();
    pid.setSetpoint(targetDistance);
    pid.enable();
    //SmartDashboard.putNumber("RangeError", 0);
  }

  // Called repeatedly when this Command is scheduled to run
  @Override
  protected void execute() {
    double range = table.getEntry("Range").getDouble(0.0);
    //error = kP * (range - targetDistance); 
    //if (error < 0) {
     // error = 0;
    //}
    //SmartDashboard.putNumber("RangeError", error / kP);
    //Robot.m_drivetrain.arcadeDrive(error, 0);
    
    //System.out.println("Range = " + range);

  }

  // Make this return true when this Command no longer needs to run execute()
  @Override
  protected boolean isFinished() {
return pid.onTarget();
    //return error < minError? true : false;
  }

  // Called once after isFinished returns true
  @Override
  protected void end() {
    System.out.println("DriveToTarget.end");
    pid.disable();
  }

  // Called when another command which requires one or more of the same
  // subsystems is scheduled to run
  @Override
  protected void interrupted() {
    end();
  }

  @Override
  public void pidWrite(double output) {
    Robot.driveTrain.set(output, output);
  //  SmartDashboard.putNumber("pidWrite", output);
    System.out.println("pidWrite is: " + output);
  }

  @Override
  public void setPIDSourceType(PIDSourceType pidSource) {
    type = pidSource;
  }

  @Override
  public PIDSourceType getPIDSourceType() {
    return  type;
  }

  @Override
  public double pidGet() {
    //double range = table.getEntry("Range").getDouble(0.0);
    double range = Robot.driveTrain.getDistance();
    return range;
}
}
