package org.usfirst.frc.team159.robot.commands;

import org.usfirst.frc.team159.robot.Robot;

import edu.wpi.first.wpilibj.PIDController;
import edu.wpi.first.wpilibj.PIDOutput;
import edu.wpi.first.wpilibj.PIDSource;
import edu.wpi.first.wpilibj.PIDSourceType;
import edu.wpi.first.wpilibj.Timer;
import edu.wpi.first.wpilibj.command.Command;
import edu.wpi.first.wpilibj.command.TimedCommand;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;

/**
 *
 */
public class DriveStraight extends TimedCommand implements PIDSource, PIDOutput {
  Timer mytimer;

  static public double P = 0.5;
  static public double I = 0.0;
  static public double D = 0.0;
  static public double TOL = 0.05;
  PIDController pid;
  double distance = 0;
  double tolerance = TOL;
  boolean last_target = false;
  PIDSourceType type = PIDSourceType.kDisplacement;
  static boolean debug = false;
  double last_time;
  int count = 0;
  double speed=1;
  double heading;
  boolean useGyro=false;

  public DriveStraight(double d, double s, double t) {
    this(d, s, t, 0.0);
    useGyro=false;
  }
  public DriveStraight(double d, double s, double t,double h) {
    super(t);
    requires(Robot.driveTrain);
    pid = new PIDController(P, I, D, this, this, 0.02);
    distance = d;
    speed=s;
    heading=h;
    mytimer = new Timer();
    mytimer.start();
    mytimer.reset();
    count = 0;
    if (debug) {
      SmartDashboard.putNumber("P", P);
      SmartDashboard.putNumber("I", I);
      SmartDashboard.putNumber("D", D);
      SmartDashboard.putNumber("TOL", tolerance);
      SmartDashboard.putNumber("DIST", distance);
    }
    useGyro=Robot.useGyro;
  }

  // Called just before this Command runs the first time
  protected void initialize() {
    System.out.println("DriveStraight::initialize()");
    last_target = false;
    mytimer.start();
    mytimer.reset();
    if (debug) {
      double p = SmartDashboard.getNumber("P", P);
      double i = SmartDashboard.getNumber("I", I);
      double d = SmartDashboard.getNumber("D", D);
      tolerance = SmartDashboard.getNumber("TOL", tolerance);
      distance = SmartDashboard.getNumber("DIST", distance);
      pid.setPID(p, i, d);
    }
    Robot.driveTrain.resetEncoders();
    pid.reset();
    pid.setSetpoint(distance);
    pid.setAbsoluteTolerance(tolerance);
    pid.enable();
    Robot.driveTrain.enable();
    last_time = mytimer.get();
  }

  // Called repeatedly when this Command is scheduled to run
  protected void execute() {
    // double curtime=0.001*(Math.round(mytimer.get()*1000.0));
    // double dt=1000.0*(curtime-last_time);
    // System.out.format("cycle:%1.2f time=%1.2f dt=%2.1f\n",count*0.02,curtime,dt);
    // last_time=curtime;
    // count++;
  }

  // Make this return true when this Command no longer needs to run execute()
  protected boolean isFinished() {
    if(super.isFinished())
      return true;
    boolean new_target = pid.onTarget();
    if (new_target && last_target)
      return true;
    last_target = new_target;
    return false;
  }

  // Called once after isFinished returns true
  protected void end() {
    System.out.println("DriveStraight::end()");
    pid.disable();
  }
  // Called when another command which requires one or more of the same
  // subsystems is scheduled to run
  protected void interrupted() {
    System.out.println("DriveStraight::interrupted()");
    end();
  }

  @Override
  public void pidWrite(double d) {
    double gh = Robot.driveTrain.getHeading();
    double herr=heading-gh;
    double turn=0;
    if (useGyro)
      turn = Robot.GFACT * (-1.0 / 180.0) * herr;

    double lval = d + turn;
    double rval = d - turn;

    lval=clamp(lval);
    rval=clamp(rval);
    if (debug)
      System.out.println("DriveStraight::pidWrite(" + d + ","+turn+")");
    Robot.driveTrain.set(lval, rval);
  }

  double clamp(double d) {
    if(d>speed)
      d=speed;
    else if(d<-speed)
      d=-speed;
    return d;
  }
  @Override
  public double pidGet() {
    double s = 12*Robot.driveTrain.getDistance();
    return s;
  }

  @Override
  public void setPIDSourceType(PIDSourceType pidSource) {
    type = pidSource;
  }

  @Override
  public PIDSourceType getPIDSourceType() {
    return type;
  }
}
