package org.usfirst.frc.team159.robot;

import edu.wpi.first.wpilibj.buttons.Button;
import edu.wpi.first.wpilibj.Joystick;

import org.usfirst.frc.team159.robot.RobotMap;

/**
 * This class is the glue that binds the controls on the physical operator
 * interface to the commands and command groups that allow control of the robot.
 */
public class OI {
	 static public Joystick stick = new Joystick(RobotMap.STICK);
	 public static void buttonTest(){
		 for (int i=0;i<16;i++){
			 if(stick.getRawButton(i))
			 	System.out.println("Button"+i+" pressed");
		 }
	 }
}

