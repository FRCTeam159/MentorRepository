
package org.usfirst.frc.team159.robot;

import edu.wpi.first.wpilibj.IterativeRobot;
import edu.wpi.first.wpilibj.command.Command;
import edu.wpi.first.wpilibj.command.Scheduler;
import edu.wpi.first.wpilibj.livewindow.LiveWindow;

import org.usfirst.frc.team159.robot.commands.Autonomous;
import org.usfirst.frc.team159.robot.subsystems.DriveTrain;
import org.usfirst.frc.team159.robot.subsystems.Climber;
import org.usfirst.frc.team159.robot.subsystems.Elevator;
import org.usfirst.frc.team159.robot.subsystems.Grabber;
import org.usfirst.frc.team159.robot.subsystems.VisionProcess;

/**
 * The VM is configured to automatically run this class, and to call the
 * functions corresponding to each mode, as described in the IterativeRobot
 * documentation. If you change the name of this class or the package after
 * creating this project, you must also update the manifest file in the resource
 * directory.
 */
public class Robot extends IterativeRobot {

	public static final DriveTrain driveTrain = new DriveTrain();
	public static final Climber climber = new Climber();
	public static final Elevator elevator = new Elevator();
	public static final Grabber grabber = new Grabber();
	public static final VisionProcess vision = new VisionProcess();
	public static OI oi;

	public static boolean isAuto = false;
	public static boolean isTele = false;
	public static boolean doAuto = true;
	public static boolean haveAuto = true;
	public static boolean hatchMode = true;

	public static double auto_scale = 0.75;
	// SendableChooser<Command> chooser = new SendableChooser<>();
	Command autonomousCommand;

	/**
	 * This function is run when the robot is first started up and should be used
	 * for any initialization code.
	 */
	@Override
	public void robotInit() {
		oi = new OI();
		autonomousCommand = new Autonomous();
		System.out.println("robotInit");
		grabber.init();
		elevator.init();
		climber.init();
		vision.init();
        vision.start();
	}

	/**
	 * This function is called once each time the robot enters Disabled mode. You
	 * can use it to reset any subsystem information you want to clear when the
	 * robot is disabled.
	 */
	@Override
	public void disabledInit() {
		System.out.println("disabledInit");
	}

	@Override
	public void disabledPeriodic() {
		oi.buttonTest();
		Scheduler.getInstance().run();
	}

	@Override
	public void autonomousInit() {
		// autonomousCommand = chooser.getSelected();
		System.out.println("autonomousInit");
		if (doAuto) {
			isAuto = true;
			isTele = false;
		} else {
			isAuto = false;
			isTele = true;
		}
		/*
		 * String autoSelected = SmartDashboard.getString("Auto Selector", "Default");
		 * switch(autoSelected) { case "My Auto": autonomousCommand = new
		 * MyAutoCommand(); break; case "Default Auto": default: autonomousCommand = new
		 * ExampleCommand(); break; }
		 */

		// schedule the autonomous command (example)
		if (doAuto && autonomousCommand != null)
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
		isAuto = false;
		isTele = true;
		if (doAuto && autonomousCommand != null)
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
}
