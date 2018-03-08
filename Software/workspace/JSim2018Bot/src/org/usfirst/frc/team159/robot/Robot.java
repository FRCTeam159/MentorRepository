
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
import org.usfirst.frc.team159.robot.subsystems.CubeHandler;
import org.usfirst.frc.team159.robot.subsystems.DriveTrain;
import org.usfirst.frc.team159.robot.subsystems.Elevator;
;

/**
 * The VM is configured to automatically run this class, and to call the
 * functions corresponding to each mode, as described in the IterativeRobot
 * documentation. If you change the name of this class or the package after
 * creating this project, you must also update the manifest file in the resource
 * directory.
 */
public class Robot extends IterativeRobot implements RobotMap, PhysicalConstants{

	public static DriveTrain driveTrain;
	public static Elevator elevator;
	public static CubeHandler cubeHandler;
	
	public static OI oi;
	SendableChooser<Integer> position_chooser = new SendableChooser<Integer>();
	SendableChooser<Integer> allgood_option_chooser = new SendableChooser<Integer>();
  SendableChooser<Integer> allbad_option_chooser = new SendableChooser<Integer>();

  CommandGroup autonomousCommand;
	public static double scale=0.7;
	public static boolean calibrate = false;
	public static boolean test = false;

	public static boolean usegyro = true;

	public static double MAX_VEL=1.0;
	public static double MAX_ACC=2.5;  // slower but more accurate 0.4
	public static double MAX_JRK=1.0; // slower but more accurate 0.1
	public static double KP=2.0;
	public static double KD=0.0;
	public static double GFACT=10.0;
	
	Random random = new Random();
	
	public static Integer position=RIGHT_POSITION;
	public static Integer fms_pattern=position;
	public static String fms_string="LLL";
	public static Integer allbad_option=OTHER_SCALE;
	public static Integer allgood_option=SAME_SCALE;

	public static int target_object=SCALE;
	public static int target_side=LEFT;

	/**
	 * This function is run when the robot is first started up and should be
	 * used for any initialization code.
	 */
	@Override
	public void robotInit() {
		String pos=System.getenv("POSITION");
		oi = new OI();
		driveTrain = new DriveTrain();
		elevator = new Elevator();
		cubeHandler = new CubeHandler();
		try {
			position=Integer.parseInt(pos);
		}
		catch ( NumberFormatException e) {
			position=CENTER_POSITION;
		}
		//position=p.intValue();
		//System.out.println("POSITION="+position+" POS="+pos);

		switch(position) {
		case CENTER_POSITION:
			position_chooser.addObject("Right", new Integer(RIGHT_POSITION));
			position_chooser.addObject("Left", new Integer(LEFT_POSITION));
			position_chooser.addDefault("Center", new Integer(CENTER_POSITION));
			System.out.println("CENTER");
			break;
		case RIGHT_POSITION:
			position_chooser.addObject("Center", new Integer(CENTER_POSITION));
			position_chooser.addObject("Left", new Integer(LEFT_POSITION));
			position_chooser.addDefault("Right", new Integer(RIGHT_POSITION));
			System.out.println("RIGHT");
			break;
		case LEFT_POSITION:
			position_chooser.addDefault("Left", new Integer(LEFT_POSITION));
			position_chooser.addObject("Center", new Integer(CENTER_POSITION));
			position_chooser.addObject("Right", new Integer(RIGHT_POSITION));
			System.out.println("LEFT");
			break;
		}

		allbad_option_chooser.addDefault("Opposite Scale", new Integer(OTHER_SCALE));
    allbad_option_chooser.addObject("Opposite Switch", new Integer(OTHER_SWITCH));
		allbad_option_chooser.addObject("Go Straight", new Integer(GO_STRAIGHT));
		
		allgood_option_chooser.addDefault("Prefer Scale", new Integer(SAME_SCALE));
    allgood_option_chooser.addObject("Prefer Switch", new Integer(SAME_SWITCH));

		SmartDashboard.putBoolean("Calibrate", calibrate);
		SmartDashboard.putBoolean("UseGyro", usegyro);
		SmartDashboard.putBoolean("Error", false);

		SmartDashboard.putNumber("MAX_VEL", MAX_VEL);
		SmartDashboard.putNumber("MAX_ACC", MAX_ACC);
		SmartDashboard.putNumber("MAX_JRK", MAX_JRK);
		SmartDashboard.putNumber("KP", KP);
		SmartDashboard.putNumber("KD", KD);
		SmartDashboard.putNumber("GFACT", GFACT);
		SmartDashboard.putString("FMS-STR", "LLL");

		SmartDashboard.putString("Target", "Calculating");
		SmartDashboard.putBoolean("Test", true);
		SmartDashboard.putBoolean("Plot", false);
		
		SmartDashboard.putNumber("Auto Scale", scale);
		SmartDashboard.putData("Position", position_chooser);
		SmartDashboard.putData("All Good Strategy", allgood_option_chooser);
    SmartDashboard.putData("All Bad Strategy", allbad_option_chooser);

		position = position_chooser.getSelected();

	}

	/**
	 * This function is called once each time the robot enters Disabled mode.
	 * You can use it to reset any subsystem information you want to clear when
	 * the robot is disabled.
	 */
	@Override
	public void disabledInit() {
	   System.out.println("disabledInit");

	   if (autonomousCommand != null)
	      autonomousCommand.cancel();
	}

	@Override
	public void disabledPeriodic() {
	  //getDataFromDashboard();
		Scheduler.getInstance().run();
	}

	@Override
	public void autonomousInit() {
	   System.out.println("autonomousInit");

	  reset();
	  if (autonomousCommand != null)
	      autonomousCommand.cancel();

	  SmartDashboard.putBoolean("Error", false);
	  MAX_VEL=SmartDashboard.getNumber("MAX_VEL", MAX_VEL);
	  MAX_ACC=SmartDashboard.getNumber("MAX_ACC", MAX_ACC);
	  MAX_JRK=SmartDashboard.getNumber("MAX_JRK", MAX_JRK);
	  GFACT=SmartDashboard.getNumber("GFACT", GFACT);

	  KP=SmartDashboard.getNumber("KP", KP);
	  KD=SmartDashboard.getNumber("KD", KD);

	  scale=SmartDashboard.getNumber("Auto Scale", scale);

	  calibrate=SmartDashboard.getBoolean("Calibrate", calibrate);

	  setFMS();

	  SmartDashboard.putBoolean("Error", false);
	  getDataFromDashboard();
	  setAutoTarget();
	  
    autonomousCommand=new Autonomous();

	  if(calibrate)
	    autonomousCommand.addSequential(new Calibrate());
	  else {
      autonomousCommand.addSequential(new SetGrabberState(GRAB,0.5));
      if(target_object==SCALE)
        autonomousCommand.addSequential(new SetElevator(SCALE_DROP_HEIGHT,4.0));
      else
        autonomousCommand.addSequential(new SetElevator(SWITCH_DROP_HEIGHT,2.0));
      autonomousCommand.addSequential(new SetGrabberState(HOLD,0.25));
	    autonomousCommand.addSequential(new DrivePath());
	    if(target_object != NONE)
	      autonomousCommand.addSequential(new SetGrabberState(PUSH,1.0));
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
	void setAutoTarget() {
	  String gameMessage=Robot.fms_string;
	  String which_object[]= {"Switch","Scale","Straight"};
	  String which_side[]= {"Center","Left","Right"};
	  if (position == CENTER_POSITION) {
	    target_object=SWITCH;
	    if (gameMessage.charAt(0) == 'R')
	      target_side=RIGHT;
	    else
	      target_side=LEFT;       
	  } else if (position == RIGHT_POSITION) {
	    target_side=RIGHT;
	    if (!isStraightPathForced()) {
	      if (gameMessage.charAt(1) == 'R' && gameMessage.charAt(0) == 'R') {
	        if (isScalePreferredOverSwitch()) {
	          target_object=SCALE;
	        } else {
	          target_object=SWITCH;
	        }
	      }
	      else if (gameMessage.charAt(0) == 'R') {
	        target_object=SWITCH;
	      }
	      else if (gameMessage.charAt(1) == 'R') {
	        target_object=SCALE;
	      } 
	      else if(isOtherScaleSelected()) { // LL
	        target_object=SCALE;
	        target_side=LEFT;
	      }
	      else if(isOtherSwitchSelected()) { // LL
	        target_object=SWITCH;
	        target_side=LEFT;
	      }
	      else {
	        target_object=NONE;
	      }
	    } else {
	      target_object=NONE;
	    }
	  } else if (position == LEFT_POSITION) {
	    target_side=LEFT;
	    if (!isStraightPathForced()) {
	      if (gameMessage.charAt(1) == 'L' && gameMessage.charAt(0) == 'L') { //LL
	        if (isScalePreferredOverSwitch()) {
	          target_object=SCALE;
	        } else {
	          target_object=SWITCH;
	        }
	      }
	      else if (gameMessage.charAt(0) == 'L') {  // LL LR
	        target_object=SWITCH;
	      } 
	      else if (gameMessage.charAt(1) == 'L') { // RL
	        target_object=SCALE;
	      } 
	      else if(isOtherScaleSelected()) { // RR
	        target_object=SCALE;
	        target_side=RIGHT;
	      }
	      else if(isOtherSwitchSelected()) { // LL
	        target_object=SWITCH;
	        target_side=RIGHT;
	      }
	      else {
	        target_object=NONE;
	      }
	    } else {
	      target_object=NONE;
	    }
	  }
	  String targetString=which_side[target_side]+"-"+which_object[target_object];
	  SmartDashboard.putString("Target", targetString);
	}
	private boolean isOtherScaleSelected() {
	  return allbad_option==OTHER_SCALE;
	}

	private boolean isOtherSwitchSelected() {
	  return allbad_option==OTHER_SWITCH;
	}

	private boolean isScalePreferredOverSwitch() {
	  return allgood_option==SAME_SCALE;
	}

	private boolean isStraightPathForced() {
	  return allbad_option==GO_STRAIGHT;
	}

	void getDataFromDashboard() {
	   position = position_chooser.getSelected();
	   allbad_option = allbad_option_chooser.getSelected();
	   allgood_option = allgood_option_chooser.getSelected();
	   usegyro=SmartDashboard.getBoolean("UseGyro", usegyro);
	}
	void setFMS() {
		boolean test=SmartDashboard.getBoolean("Test", false);
		if(test) {
			fms_string=SmartDashboard.getString("FMS-STR", "LLL");
		}
		else {
			String fms[]= {"LLL","LLR","LRL","LRR","RLL","RLR","RRL","RRR"};
			double rand=random.nextDouble();
			
			fms_pattern=(int)(7.99999*rand);  // 0-7	
			fms_string=fms[fms_pattern];
			System.out.println("rand="+rand+" indx="+fms_pattern);

			SmartDashboard.putString("FMS-STR", fms[fms_pattern]);
		}
	}
}
