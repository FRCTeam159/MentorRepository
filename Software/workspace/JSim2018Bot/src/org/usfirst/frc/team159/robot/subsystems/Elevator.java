package org.usfirst.frc.team159.robot.subsystems;

import org.usfirst.frc.team159.robot.RobotMap;
import org.usfirst.frc.team159.robot.commands.ElevatorCommands;

import com.ctre.CANTalon;

import edu.wpi.first.wpilibj.PIDController;
import edu.wpi.first.wpilibj.PIDOutput;
import edu.wpi.first.wpilibj.PIDSource;
import edu.wpi.first.wpilibj.PIDSourceType;
import edu.wpi.first.wpilibj.command.Subsystem;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;

/**
 *
 */
public class Elevator extends Subsystem {
  static double inches_per_meter = 100 / 2.54;

  static public double max_outer_travel = 1.0 * inches_per_meter; // outer
  static public double max_inner_travel = 1.05 * inches_per_meter; // inner

  public static final double MAX_HEIGHT = 78;
  private static final double MIN_HEIGHT = 0;

  public static final double SWITCH_HEIGHT = 24;
  public static final double SCALE_HEIGHT = MAX_HEIGHT;
  public static final double START_HEIGHT = 1;
  double tolerance = 6;

  public static double travel_ratio = max_inner_travel / max_outer_travel;
  double setpoint = 0;

  Inner inner;
  Outer outer;

  public Elevator() {
    super();
    outer = new Outer(RobotMap.ELEVATORMOTOR);
    inner = new Inner(RobotMap.ELEVATORMOTOR + 1);
    SmartDashboard.putNumber("Elevator", Math.round(setpoint));
    reset();
  }

  public void initDefaultCommand() {
    // Set the default command for a subsystem here.
    setDefaultCommand(new ElevatorCommands());
  }

  public void setPosition(double pos) {
    pos /= 2;
    double value = pos > max_outer_travel ? max_outer_travel : pos;
    value = value < 0 ? 0 : value;
    setpoint = value * 2;
    outer.setPosition(value);
    inner.setPosition(travel_ratio * value);
    SmartDashboard.putNumber("Elevator", Math.round(setpoint));
  }

  public void reset() {
    outer.reset();
    inner.reset();
    setpoint = 0;
  }

  public void enable() {
    inner.enable();
    outer.enable();
  }

  public void disable() {
    outer.disable();
    inner.disable();
  }

  public double getPosition() {
    return inner.getPosition() * 2; // return inches
  }

  public double getSetpoint() {
    return setpoint; // return inches
  }

  public boolean atTarget() {
    if (Math.abs(getPosition() - setpoint) < tolerance)
      return true;
    return false;
  }

  // pid controller for elevator
  public class Outer implements PIDSource, PIDOutput {
    double P = 0.4;
    double I = 0.0;
    double D = 2.0;
    private CANTalon motor;
    PIDSourceType type = PIDSourceType.kDisplacement;
    PIDController pid;
    double setpoint = 0;
    boolean debug_in = false;
    boolean debug_out = false;
    int cycle_count = 0;

    public Outer(int i) {
      motor = new CANTalon(i);
      motor.changeControlMode(CANTalon.TalonControlMode.PercentVbus);
      motor.configEncoderCodesPerRev(1); // deprecated in TalonSRX
      pid = new PIDController(P, I, D, 0.0, this, this, 0.01);
      pid.setOutputRange(-1, 1);
      pid.setInputRange(0, max_outer_travel);

      pid.disable();
    }

    public void setPosition(double d) {
      setpoint = d;
      pid.setSetpoint(setpoint);
    }

    public double getPosition() {
      return inches_per_meter * motor.getPosition(); // return inches
    }

    public void disable() {
      pid.disable();
    }

    public void enable() {
      pid.enable();
    }

    public void reset() {
      pid.disable();
      setpoint = 0;
      pid.reset();
      pid.setSetpoint(setpoint);
      pid.disable();
    }

    @Override
    public double pidGet() {
      if (debug_in)
        System.out.println("Outer::pidGet(" + getPosition() + ")");
      return getPosition();
    }

    @Override
    public void pidWrite(double output) {
      cycle_count++;
      if (Double.isNaN(output)) {
        System.out.println("Outer::pidWrite(NAN) - aborting ! " + cycle_count);
        return;
      }
      if (debug_out)
        System.out.println("Outer::pidWrite(" + output + ")");
      motor.set(output);
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

  // pid controller for carriage
  class Inner implements PIDSource, PIDOutput {
    double P = 0.5;
    double I = 0.0;
    double D = 4.0;
    PIDSourceType type = PIDSourceType.kDisplacement;
    private CANTalon motor;
    PIDController pid;
    double setpoint = 0;
    boolean debug_in = false;
    boolean debug_out = false;
    int cycle_count = 0;

    public Inner(int i) {
      motor = new CANTalon(i);
      motor.changeControlMode(CANTalon.TalonControlMode.PercentVbus);
      motor.configEncoderCodesPerRev(1); // deprecated in new CTRE TalonSRX
      pid = new PIDController(P, I, D, 0.0, this, this, 0.01);
      pid.setInputRange(0, max_inner_travel);
      pid.setOutputRange(-1, 1);
      pid.disable();
    }

    public void setPosition(double d) {
      setpoint = d;
      pid.setSetpoint(setpoint);
    }

    public double getPosition() {
      return inches_per_meter * motor.getPosition(); // return inches
    }

    public void disable() {
      pid.disable();
    }

    public void enable() {
      pid.enable();
    }

    public void reset() {
      pid.disable();
      setpoint = 0;
      pid.reset();
      pid.setSetpoint(setpoint);
      pid.disable();
    }

    @Override
    public double pidGet() {
      if (debug_in)
        System.out.println("Inner::pidGet(" + getPosition() + ")");
      return getPosition();
    }

    @Override
    public void pidWrite(double output) {
      cycle_count++;
      if (Double.isNaN(output)) {
        System.out.println("Inner::pidWrite(NAN) - aborting ! " + cycle_count);
        return;
      }
      if (debug_out)
        System.out.println("Inner::pidWrite(" + output + ")");
      motor.set(output);
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
}
