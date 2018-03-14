package org.usfirst.frc.team159.robot.commands;

import org.usfirst.frc.team159.robot.Robot;

import edu.wpi.first.wpilibj.PIDController;
import edu.wpi.first.wpilibj.PIDOutput;
import edu.wpi.first.wpilibj.PIDSource;
import edu.wpi.first.wpilibj.PIDSourceType;
import edu.wpi.first.wpilibj.command.TimedCommand;

/**
 *
 */
public class TurnToAngle extends TimedCommand implements PIDSource, PIDOutput{
    double angle;
    PIDController pid;
    PIDSourceType type = PIDSourceType.kDisplacement;

    public TurnToAngle(double angle,double tm) {
        super(tm);
        requires(Robot.driveTrain);
        this.angle=angle;
        pid = new PIDController(0.005, 0.0, 0.0, this, this, 0.01);
    }

    // Called just before this Command runs the first time
    protected void initialize() {
      pid.reset();
      pid.setSetpoint(angle);
      pid.setAbsoluteTolerance(4.0);
      pid.enable();
      System.out.println("TurnToAngle::initialize:"+angle);
    }

    // Called repeatedly when this Command is scheduled to run
    protected void execute() {
    }
    protected boolean isFinished() {
      if (super.isFinished()|| pid.onTarget())
        return true;
      return false;
    }

    // Called once after timeout
    protected void end() {
      System.out.println("TurnToAngle::end()");
      pid.disable();
    }

    // Called when another command which requires one or more of the same
    // subsystems is scheduled to run
    protected void interrupted() {
      System.out.println("TurnToAngle::interrupted()");
      end();
    }

    @Override
    public void pidWrite(double d) {
      Robot.driveTrain.set(-d, d);
    }

    @Override
    public void setPIDSourceType(PIDSourceType pidSource) {
      type = pidSource;      
    }

    @Override
    public PIDSourceType getPIDSourceType() {
      return type;
    }

    @Override
    public double pidGet() {
      return Robot.driveTrain.getHeading();

    }
}
