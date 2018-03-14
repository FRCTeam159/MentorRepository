
package org.usfirst.frc.team159.robot;

import edu.wpi.first.wpilibj.IterativeRobot;
import edu.wpi.first.wpilibj.Sendable;
import edu.wpi.first.wpilibj.command.Command;
import edu.wpi.first.wpilibj.command.CommandGroup;
import edu.wpi.first.wpilibj.command.Scheduler;
import edu.wpi.first.wpilibj.livewindow.LiveWindow;
import edu.wpi.first.wpilibj.smartdashboard.SendableChooser;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;
import jaci.pathfinder.Waypoint;

import java.util.Random;

import org.usfirst.frc.team159.robot.commands.Autonomous;
import org.usfirst.frc.team159.robot.commands.Calibrate;
import org.usfirst.frc.team159.robot.commands.DrivePath;
import org.usfirst.frc.team159.robot.commands.SetElevator;
import org.usfirst.frc.team159.robot.commands.SetGrabberState;
import org.usfirst.frc.team159.robot.commands.TurnToAngle;
import org.usfirst.frc.team159.robot.subsystems.CubeHandler;
import org.usfirst.frc.team159.robot.subsystems.DriveTrain;
import org.usfirst.frc.team159.robot.subsystems.Elevator;;

/**
 * The VM is configured to automatically run this class, and to call the
 * functions corresponding to each mode, as described in the IterativeRobot
 * documentation. If you change the name of this class or the package after
 * creating this project, you must also update the manifest file in the resource
 * directory.
 */
public class Robot extends IterativeRobot implements RobotMap, PhysicalConstants, Constants {

  public static DriveTrain driveTrain;
  public static Elevator elevator;
  public static CubeHandler cubeHandler;

  public static OI oi;
  SendableChooser<Integer> position_chooser = new SendableChooser<Integer>();
  SendableChooser<Integer> allGoodChooser = new SendableChooser<Integer>();
  SendableChooser<Integer> allBadChooser = new SendableChooser<Integer>();

  CommandGroup autonomousCommand;
  public static double auto_scale = 0.7;
  public static boolean calibrate = false;
  public static boolean test = false;

  public static boolean useGyro = true;

  public static double MAX_VEL = 1.0;
  public static double MAX_ACC = 2.5; // slower but more accurate 0.4
  public static double MAX_JRK = 1.0; // slower but more accurate 0.1
  public static double KP = 2.0;
  public static double KD = 0.0;
  public static double GFACT = 10.0;

  Random random = new Random();

  public static Integer robotPosition = POSITION_RIGHT;
  public static Integer fms_pattern = robotPosition;
  public static String fms_string = "RRR";
  public static Integer allBadOption = OTHER_SCALE;
  public static Integer allGoodOption = SAME_SCALE;

  public static int targetObject = OBJECT_SCALE;
  public static int targetSide = POSITION_LEFT;
  public static boolean allgood = false;

  /**
   * This function is run when the robot is first started up and should be used
   * for any initialization code.
   */
  @Override
  public void robotInit() {
    String pos = System.getenv("POSITION");
    oi = new OI();
    driveTrain = new DriveTrain();
    elevator = new Elevator();
    cubeHandler = new CubeHandler();
    try {
      robotPosition = Integer.parseInt(pos);
    } catch (NumberFormatException e) {
      robotPosition = POSITION_CENTER;
    }
    // position=p.intValue();
    // System.out.println("POSITION="+position+" POS="+pos);

    switch (robotPosition) {
    case POSITION_CENTER:
      position_chooser.addObject("Right", new Integer(POSITION_RIGHT));
      position_chooser.addObject("Left", new Integer(POSITION_LEFT));
      position_chooser.addDefault("Center", new Integer(POSITION_CENTER));
      System.out.println("CENTER");
      break;
    case POSITION_RIGHT:
      position_chooser.addObject("Center", new Integer(POSITION_CENTER));
      position_chooser.addObject("Left", new Integer(POSITION_LEFT));
      position_chooser.addDefault("Right", new Integer(POSITION_RIGHT));
      System.out.println("RIGHT");
      break;
    case POSITION_LEFT:
      position_chooser.addDefault("Left", new Integer(POSITION_LEFT));
      position_chooser.addObject("Center", new Integer(POSITION_CENTER));
      position_chooser.addObject("Right", new Integer(POSITION_RIGHT));
      System.out.println("LEFT");
      break;
    }

    allBadChooser.addDefault("Opposite Scale", new Integer(OTHER_SCALE));
    allBadChooser.addObject("Opposite Switch", new Integer(OTHER_SWITCH));
    allBadChooser.addObject("Go Straight", new Integer(GO_STRAIGHT));

    allGoodChooser.addDefault("Prefer Scale", new Integer(SAME_SCALE));
    allGoodChooser.addObject("Prefer Switch", new Integer(SAME_SWITCH));
    allGoodChooser.addObject("TwoCubeAuto", new Integer(TWO_CUBE_AUTO));

    SmartDashboard.putBoolean("Calibrate", calibrate);
    SmartDashboard.putBoolean("UseGyro", useGyro);
    SmartDashboard.putBoolean("Error", false);

    SmartDashboard.putNumber("MAX_VEL", MAX_VEL);
    SmartDashboard.putNumber("MAX_ACC", MAX_ACC);
    SmartDashboard.putNumber("MAX_JRK", MAX_JRK);
    SmartDashboard.putNumber("KP", KP);
    SmartDashboard.putNumber("KD", KD);
    SmartDashboard.putNumber("GFACT", GFACT);
    SmartDashboard.putString("FMS-STR", "RRR");

    SmartDashboard.putString("Target", "Calculating");
    SmartDashboard.putBoolean("Test", true);
    SmartDashboard.putBoolean("Plot", false);

    SmartDashboard.putNumber("Auto Scale", auto_scale);
    SmartDashboard.putData("Position", position_chooser);
    SmartDashboard.putData("All Good Strategy", allGoodChooser);
    SmartDashboard.putData("All Bad Strategy", allBadChooser);

    robotPosition = position_chooser.getSelected();

  }

  /**
   * This function is called once each time the robot enters Disabled mode. You
   * can use it to reset any subsystem information you want to clear when the
   * robot is disabled.
   */
  @Override
  public void disabledInit() {
    System.out.println("disabledInit");
    if (autonomousCommand != null)
      autonomousCommand.cancel();
  }

  @Override
  public void disabledPeriodic() {
    // getDataFromDashboard();
    Scheduler.getInstance().run();
  }

  @Override
  public void autonomousInit() {
    System.out.println("autonomousInit");

    reset();
    if (autonomousCommand != null)
      autonomousCommand.cancel();

    SmartDashboard.putBoolean("Error", false);
    MAX_VEL = SmartDashboard.getNumber("MAX_VEL", MAX_VEL);
    MAX_ACC = SmartDashboard.getNumber("MAX_ACC", MAX_ACC);
    MAX_JRK = SmartDashboard.getNumber("MAX_JRK", MAX_JRK);
    GFACT = SmartDashboard.getNumber("GFACT", GFACT);

    KP = SmartDashboard.getNumber("KP", KP);
    KD = SmartDashboard.getNumber("KD", KD);

    auto_scale = SmartDashboard.getNumber("Auto Scale", auto_scale);

    calibrate = SmartDashboard.getBoolean("Calibrate", calibrate);

    setFMS();

    SmartDashboard.putBoolean("Error", false);
    getDataFromDashboard();
    setAutoTarget();

    autonomousCommand = new Autonomous();

    if (calibrate)
      autonomousCommand.addSequential(new Calibrate());
    else {
      autonomousCommand.addSequential(new SetGrabberState(GRAB, 0.5));
      if (targetObject == OBJECT_SCALE)
        autonomousCommand.addSequential(new SetElevator(SCALE_DROP_HEIGHT, 4.0));
      else
        autonomousCommand.addSequential(new SetElevator(SWITCH_DROP_HEIGHT, 2.0));
      autonomousCommand.addSequential(new SetGrabberState(HOLD, 0.25));
      autonomousCommand.addSequential(new DrivePath(0));
      if (targetObject != OBJECT_NONE) {
        autonomousCommand.addSequential(new SetGrabberState(PUSH, 1.0)); // fire away (scale or switch)
        if (doTwoCubeAuto()) { // same side both good : do scale then switch
          autonomousCommand.addSequential(new SetElevator(0, 2.0)); // drop elevator and prepare to grab
          if (robotPosition == POSITION_RIGHT) // note: pathfinder can't turn in place or drive in reverse
            autonomousCommand.addSequential(new TurnToAngle(135.0, 3.0));
          else
            autonomousCommand.addSequential(new TurnToAngle(-135.0, 3.0));
          autonomousCommand.addSequential(new SetGrabberState(GRAB, 0.5));
          autonomousCommand.addSequential(new DrivePath(1)); // second pathfinder pass (resets encoders and gyro)
          autonomousCommand.addSequential(new SetElevator(SWITCH_DROP_HEIGHT, 2.0)); // raise elevator and push
          if (robotPosition == POSITION_RIGHT) // turn more toward center of switch
            autonomousCommand.addSequential(new TurnToAngle(25.0, 3.0));
          else
            autonomousCommand.addSequential(new TurnToAngle(-25.0, 3.0));
          autonomousCommand.addSequential(new SetGrabberState(PUSH, 1.0)); // fire away (scale)
        }
      }
    }

    // schedule the autonomous command (example)
    if (autonomousCommand != null)
      autonomousCommand.start();
  }

  /**
   * This function is called periodically during autonomous
   */
  @Override
  public void autonomousPeriodic() {
    Scheduler.getInstance().run();
  }

  @Override
  public void teleopInit() {
    System.out.println("teleopInit");
    reset();
    // This makes sure that the autonomous stops running when
    // teleop starts running. If you want the autonomous to
    // continue until interrupted by another command, remove
    // this line or comment it out.
    if (autonomousCommand != null)
      autonomousCommand.cancel();
  }

  /**
   * This function is called periodically during operator control
   */
  @Override
  public void teleopPeriodic() {
    Scheduler.getInstance().run();
  }

  /**
   * This function is called periodically during test mode
   */
  @Override
  public void testPeriodic() {
    LiveWindow.run();
  }

  void reset() {
    driveTrain.reset();
    elevator.reset();
    cubeHandler.reset();
    cubeHandler.enable();
    elevator.enable();
    driveTrain.enable();
  }

  public static void showAutoTarget() {
    String which_object[] = { "Switch", "Scale", "Straight" };
    String which_side[] = { "Center", "Left", "Right" };
    String targetString = which_side[targetSide] + "-" + which_object[targetObject];
    SmartDashboard.putString("Target", targetString);
  }

  void setAutoTarget() {
    String gameMessage = Robot.fms_string;
    allgood = false;
    if (robotPosition == POSITION_CENTER) {
      targetObject = OBJECT_SWITCH;
      if (gameMessage.charAt(0) == 'R')
        targetSide = POSITION_RIGHT;
      else
        targetSide = POSITION_LEFT;
    } else if (robotPosition == POSITION_RIGHT) {
      targetSide = POSITION_RIGHT;
      if (!isStraightPathForced()) {
        if (gameMessage.charAt(1) == 'R' && gameMessage.charAt(0) == 'R') {
          allgood = true;
          if (doTwoCubeAuto() || isScalePreferredOverSwitch()) {
            targetObject = OBJECT_SCALE;
          } else {
            targetObject = OBJECT_SWITCH;
          }
        } else if (gameMessage.charAt(0) == 'R') {
          targetObject = OBJECT_SWITCH;
        } else if (gameMessage.charAt(1) == 'R') {
          targetObject = OBJECT_SCALE;
        } else if (isOtherScaleSelected()) { // LL
          targetObject = OBJECT_SCALE;
          targetSide = POSITION_LEFT;
        } else if (isOtherSwitchSelected()) { // LL
          targetObject = OBJECT_SWITCH;
          targetSide = POSITION_LEFT;
        } else {
          targetObject = OBJECT_NONE;
        }
      } else {
        targetObject = OBJECT_NONE;
      }
    } else if (robotPosition == POSITION_LEFT) {
      targetSide = POSITION_LEFT;
      if (!isStraightPathForced()) {
        if (gameMessage.charAt(1) == 'L' && gameMessage.charAt(0) == 'L') { // LL
          allgood = true;
          if (doTwoCubeAuto() || isScalePreferredOverSwitch()) {
            targetObject = OBJECT_SCALE;
          } else {
            targetObject = OBJECT_SWITCH;
          }
        } else if (gameMessage.charAt(0) == 'L') { // LL LR
          targetObject = OBJECT_SWITCH;
        } else if (gameMessage.charAt(1) == 'L') { // RL
          targetObject = OBJECT_SCALE;
        } else if (isOtherScaleSelected()) { // RR
          targetObject = OBJECT_SCALE;
          targetSide = POSITION_RIGHT;
        } else if (isOtherSwitchSelected()) { // LL
          targetObject = OBJECT_SWITCH;
          targetSide = POSITION_RIGHT;
        } else {
          targetObject = OBJECT_NONE;
        }
      } else {
        targetObject = OBJECT_NONE;
      }
    }
    showAutoTarget();
  }

  private boolean isOtherScaleSelected() {
    return allBadOption == OTHER_SCALE;
  }

  private boolean isOtherSwitchSelected() {
    return allBadOption == OTHER_SWITCH;
  }

  private boolean isScalePreferredOverSwitch() {
    return allGoodOption == SAME_SCALE;
  }

  private boolean doTwoCubeAuto() {
    return allgood && (allGoodOption == TWO_CUBE_AUTO);
  }

  private boolean isStraightPathForced() {
    return allBadOption == GO_STRAIGHT;
  }

  void getDataFromDashboard() {
    robotPosition = position_chooser.getSelected();
    allBadOption = allBadChooser.getSelected();
    allGoodOption = allGoodChooser.getSelected();
    useGyro = SmartDashboard.getBoolean("UseGyro", useGyro);
  }

  void setFMS() {
    boolean test = SmartDashboard.getBoolean("Test", false);
    if (test) {
      fms_string = SmartDashboard.getString("FMS-STR", "RRR");
    } else {
      String fms[] = { "LLL", "LLR", "LRL", "LRR", "RLL", "RLR", "RRL", "RRR" };
      double rand = random.nextDouble();

      fms_pattern = (int) (7.99999 * rand); // 0-7
      fms_string = fms[fms_pattern];
      System.out.println("rand=" + rand + " indx=" + fms_pattern);

      SmartDashboard.putString("FMS-STR", fms[fms_pattern]);
    }
  }
}
