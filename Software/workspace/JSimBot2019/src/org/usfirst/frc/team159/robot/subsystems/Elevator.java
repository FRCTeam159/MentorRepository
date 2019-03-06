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
import edu.wpi.first.wpilibj.DoubleSolenoid;

/**
 *
 */
public class Elevator extends Subsystem {
  // simulation only
  static double inches_per_meter = 100 / 2.54;
  static public double max_stage_travel = 0.85 * inches_per_meter; // stage1

  public static final double BASE_HEIGHT = 2; // height of carriage from floor fully lowered
  // field heights from floor
  public static final double HATCH_HEIGHT = 16;
  public static final double CARGO_BALL_HEIGHT = 36;
  public static final double ROCKET_BALL_HEIGHT_LOW = 27.5;
  public static final double CARGO_HATCH_HEIGHT = 16;
  public static final double DELTA_TARGET_HEIGHT = 28;
  public static final double ROCKET_TOP_BALL_HEIGHT = 2*(DELTA_TARGET_HEIGHT)+ ROCKET_BALL_HEIGHT_LOW;

  public static final double MAX_HEIGHT = ROCKET_TOP_BALL_HEIGHT;
  public static final double MIN_HEIGHT = BASE_HEIGHT;
  
  public static final double MAX_SPEED = 60;
  public static final double CYCLE_TIME = 0.01;
  public static final double MOVE_RATE = CYCLE_TIME * MAX_SPEED;

  static int elevator_stage=1;
  double tolerance = 3;
  double P = 0.4;
  double I = 0.0;
  double D = 6.0;

  double setpoint = BASE_HEIGHT;

  ElevatorStage stage1;
  ElevatorStage stage2;
  ElevatorStage stage3;

  DoubleSolenoid tiltPneumatic = new DoubleSolenoid(1, 2);
  boolean tilted=false;

  //private CANTalon pistonMotor = new CANTalon(0);


  public Elevator() {
    super();
    stage1 = new ElevatorStage(RobotMap.ELEVATOR_MOTOR,     0.4,0,6);
    stage2 = new ElevatorStage(RobotMap.ELEVATOR_MOTOR + 1, 0.2,0,6);
    stage3 = new ElevatorStage(RobotMap.ELEVATOR_MOTOR + 2, 0.05,0,8);

    SmartDashboard.putNumber("Elevator", Math.round(setpoint));
    //stage1.setDebug(true, true, true);1973
  }

  public void init(){
    tiltElevator(false);
    reset();
    enable();
  }
  public void initDefaultCommand() {
    setDefaultCommand(new ElevatorCommands());
  }

  public void setPosition(double pos) {
    pos=pos<MIN_HEIGHT?MIN_HEIGHT:pos;
    pos=setpoint>MAX_HEIGHT?MAX_HEIGHT:pos;
    pos-=BASE_HEIGHT;
    double value=pos/3;
    value = value > max_stage_travel ? max_stage_travel : value;
    value = value < 0 ? 0 : value;
    
    stage1.setPosition(value);
    stage2.setPosition(value);
    stage3.setPosition(value);

    setpoint = value*3+BASE_HEIGHT;
    SmartDashboard.putNumber("Elevator", Math.round(setpoint));
  }

  public void reset() {
    stage1.reset();
    stage2.reset();
    stage3.reset();
    setpoint = 0;
  }

  public void enable() {
    stage3.enable();
    stage2.enable();
    stage1.enable();
  }

  public void disable() {
    stage1.disable();
    stage2.disable();
    stage3.disable();
  }

  public double getPosition() {
    return stage3.getPosition() * 3; // return inches
  }

  public double getSetpoint() {
    return setpoint; // return inches
  }

  public boolean atTarget() {
    if (Math.abs(getPosition() - setpoint) < tolerance)
      return true;
    return false;
  }

  public void tiltElevator(boolean forward){
    if(forward){
      System.out.println("Elevator.tilt(forward)");
      //pistonMotor.set(1);
      tiltPneumatic.set(DoubleSolenoid.Value.kReverse);
      tilted = false;
    }
    else{
      System.out.println("Elevator.tilt(back)");
      //pistonMotor.set(-1);
      tiltPneumatic.set(DoubleSolenoid.Value.kForward);
      tilted = true;
    }
  }
  public boolean isTilted(){
    return tilted;
  }
  // pid controller for elevator
  public class ElevatorStage implements PIDSource, PIDOutput {
   
    private CANTalon motor;
    PIDSourceType type = PIDSourceType.kDisplacement;
    PIDController pid;
    double setpoint = 0;
    boolean debug_in = false;
    boolean debug_out = false;
    boolean debug_setpoint = false;
    int stage=0;

    int cycle_count = 0;

    
    public ElevatorStage(int i,double P1,double I1, double D1) {
      motor = new CANTalon(i);
      motor.changeControlMode(CANTalon.TalonControlMode.PercentVbus);
      motor.configEncoderCodesPerRev(1); // deprecated in TalonSRX
      pid = new PIDController(P1, I1, D1, 0.0, this, this, CYCLE_TIME);
      pid.setOutputRange(-1, 1);
      pid.setInputRange(0, max_stage_travel);
      pid.disable();
      stage=elevator_stage;
      elevator_stage=elevator_stage+1;
    }
    public ElevatorStage(int i) {
      this(i,P,I,D);
    }
    public void setDebug(boolean in, boolean out, boolean set){
      debug_in=in;
      debug_out=out;
      debug_setpoint=set;
    }
    public void setPosition(double d) {
      setpoint = d;
      if (debug_setpoint)
        System.out.println("ElevatorStage"+stage+".setPosition(" + setpoint + ")");
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
        System.out.println("ElevatorStage"+stage+".pidGet(" + getPosition() + ")");
      return getPosition();
    }

    @Override
    public void pidWrite(double output) {
      cycle_count++;
      if (Double.isNaN(output)) {
        System.out.println("ElevatorStage::pidWrite(NAN) - aborting ! " + cycle_count);
        return;
      }
      if (debug_out)
        System.out.println("ElevatorStage"+stage+".pidWrite(" + output + ")");
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
