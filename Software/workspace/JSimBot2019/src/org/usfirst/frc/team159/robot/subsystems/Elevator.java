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
import org.usfirst.frc.team159.robot.Robot;
/**
 *
 */
public class Elevator extends Subsystem {
  // simulation only
  static double inches_per_meter = 100 / 2.54;
  //static public double max_stage_travel = 0.85 * inches_per_meter; // stage1
  static public double max_stage_travel = inches_per_meter; // stage1


  // field heights from floor to center of gripper
  public static final double BASE_HEIGHT = 6; // minimum offset when fully lowered
  public static final double HATCH_HEIGHT = 19;
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
  public static int level=0;
  public static int min_level=1;
  public static int max_level=3;
  public static double min_height=1;
  public static double max_height=3;
  public static boolean enabled = true;

  static int elevator_stage=1;
  double tolerance = 0.2;
  double P = 0.5;
  double I = 0.01;
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
    stage1 = new ElevatorStage(RobotMap.ELEVATOR_MOTOR,     0.5,0.001,6);
    stage2 = new ElevatorStage(RobotMap.ELEVATOR_MOTOR + 1, 0.2,0.001,6);
    stage3 = new ElevatorStage(RobotMap.ELEVATOR_MOTOR + 2, 0.1,0.001,8);
   
    //stage1.setDebug(true, false, false);
    //stage2.setDebug(true, false, false);
    //stage3.setDebug(true, false, false);

  }

  public void init(){
    tiltElevator(false);
    checkMode();
    reset();
    checkLevel();
    enable();
    log();
  }
  public void initDefaultCommand() {
    setDefaultCommand(new ElevatorCommands());
  }

  public void setPosition(double pos) {
    pos-=BASE_HEIGHT;

    double value=pos/3;
    value = value > max_stage_travel ? max_stage_travel : value;
    value = value < 0 ? 0 : value;
    
    stage1.setPosition(value);
    stage2.setPosition(value);
    stage3.setPosition(value);

    setpoint = value*3+BASE_HEIGHT;
    log();
  }

  public void reset() {
    stage1.reset();
    stage2.reset();
    stage3.reset();
    setpoint = BASE_HEIGHT;
    log();
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
    double p1=stage1.getPosition();
    double p2=stage2.getPosition();
    double p3=stage3.getPosition();

    return p1+p2+p3+BASE_HEIGHT; // return inches
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
      tiltPneumatic.set(DoubleSolenoid.Value.kReverse);
      tilted = false;
    }
    else{
      tiltPneumatic.set(DoubleSolenoid.Value.kForward);
      tilted = true;
    }
    log();
  }
  public boolean isTilted(){
    return tilted;
  }
  public void checkMode(){
    boolean isHatch=Robot.hatchMode;
    if(isHatch){
      min_level=1;
      max_level=3;
    }
    else{
      min_level=0;
      max_level=4;
    }
  }
  void checkSetpoint(){
    setpoint=setpoint<MIN_HEIGHT?MIN_HEIGHT:setpoint;
    setpoint=setpoint>MAX_HEIGHT?MAX_HEIGHT:setpoint;
  }
  void checkLevel(){
    level=level<min_level?min_level:level;
    level=level>max_level?max_level:level;
  }
  public void stepUp(double v){
    setpoint += v * MOVE_RATE;
  }
  public void stepDown(double v){
    setpoint -= v * MOVE_RATE;
  }
  public void decrLevel(){
    if(Robot.hatchMode && level <= 1)
      return;
    if(!Robot.hatchMode && level == 0)
      return;
    setpoint -=DELTA_TARGET_HEIGHT;
    Elevator.level--;
    checkLevel();
  }
  public void incrLevel(){
    if(!Robot.hatchMode && level==0)
      setpoint=ROCKET_BALL_HEIGHT_LOW;
    else
      setpoint += DELTA_TARGET_HEIGHT;
    level++;
    checkLevel();
  }
  public void resetLevel(){
    if(Robot.hatchMode){
      setpoint = HATCH_HEIGHT;
      level=1;
    }
    else{
      setpoint = BASE_HEIGHT;
      level=0;
    }
  }
  public void setElevator(){
    checkSetpoint();
    setPosition(setpoint);
  }
  public void enableElevator(){
    enabled=true;
    setPosition(setpoint);
  }
  public void stopElevator(){
    enabled=false;
    setpoint = getSetpoint();
    setPosition(setpoint);
  }
  public boolean isEnabled(){
    return enabled;
  }
  
  public void log(){
    SmartDashboard.putBoolean("Elevator Tilted", tilted);
    SmartDashboard.putNumber("Elevator Height", Math.round(setpoint));
    SmartDashboard.putNumber("Elevator Actual", Math.round(getPosition()));

    SmartDashboard.putNumber("Elevator Level", level);
    SmartDashboard.putBoolean("Elevator On", enabled);
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
