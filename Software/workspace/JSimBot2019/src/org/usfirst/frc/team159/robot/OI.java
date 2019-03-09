package org.usfirst.frc.team159.robot;

import edu.wpi.first.wpilibj.buttons.Button;
import edu.wpi.first.wpilibj.Joystick;

import org.usfirst.frc.team159.robot.RobotMap;

/**
 * This class is the glue that binds the controls on the physical operator
 * interface to the commands and command groups that allow control of the robot.
 */
public class OI {
	 static double last_dpad=-1;
	 static double last_axis[] = new double[8];
	 static public Joystick stick = new Joystick(RobotMap.STICK);

	 public static void buttonTest(){
		 int dpad = stick.getPOV(0); // alays returns 0 in simulationlittle 
		 
		 if(dpad!=last_dpad){
			System.out.println("Dpad = "+dpad);
			last_dpad=dpad;
		 }
		 for (int i=0;i<12;i++){
			 if(stick.getRawButton(i))
			 	System.out.println("Button"+i+" pressed");
		 }
		 for (int i=0;i<6;i++){
			double axis=stick.getRawAxis(i);
			if(axis!=last_axis[i])
				System.out.println("Axis["+i+"]="+axis);
			last_axis[i]=axis;
		}
	 }
}

