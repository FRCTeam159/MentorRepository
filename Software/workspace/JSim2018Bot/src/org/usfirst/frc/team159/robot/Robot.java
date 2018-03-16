
package org.usfirst.frc.team159.robot;

import edu.wpi.first.wpilibj.IterativeRobot;
import edu.wpi.first.wpilibj.command.CommandGroup;
import edu.wpi.first.wpilibj.command.Scheduler;
import edu.wpi.first.wpilibj.livewindow.LiveWindow;
import edu.wpi.first.wpilibj.smartdashboard.SendableChooser;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;
import java.util.Random;

import org.usfirst.frc.team159.robot.commands.Calibrate;
import org.usfirst.frc.team159.robot.subsystems.AutoSelector;
import org.usfirst.frc.team159.robot.subsystems.CubeHandler;
import org.usfirst.frc.team159.robot.subsystems.DriveTrain;
import org.usfirst.frc.team159.robot.subsystems.Elevator;

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
  public static AutoSelector autoSelector;

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
    autoSelector = new AutoSelector();

    try {
      robotPosition = Integer.parseInt(pos);
    } catch (NumberFormatException e) {
      robotPosition = POSITION_CENTER;
    }

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
   //setAutoTarget();

    autonomousCommand = new CommandGroup();
    if (calibrate) 
       autonomousCommand.addSequential(new Calibrate());
    else 
      autonomousCommand = autoSelector.getAutonomous();

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

  void getDataFromDashboard() {
    robotPosition = position_chooser.getSelected();
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
