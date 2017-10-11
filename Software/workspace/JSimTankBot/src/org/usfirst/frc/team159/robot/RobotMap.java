package org.usfirst.frc.team159.robot;

/**
 * The RobotMap is a mapping from the ports sensors and actuators are wired into
 * to a variable name. This provides flexibility changing wiring, makes checking
 * the wiring easier and significantly reduces the number of magic numbers
 * floating around.
 */
public interface RobotMap {
	public static int FRONTLEFT = 1;
	public static int FRONTRIGHT = 4;
	public static int BACKLEFT = 2;
	public static int BACKRIGHT = 3;

	public static int STICK = 0;

	public static int LOWGEAR_BUTTON = 4;
	public static int HIGHGEAR_BUTTON = 3;

	public static int GEARSHIFTID=0;

	public static int TANK = 1;     // 2-axis gamepad left side, right side
	public static int ARCADE2 = 2;  // 2-axis gamepad throttle,turn
	public static int ARCADE3 = 3;  // 3-axis joystick throttle,turn,twist
	
	public static int DRIVETYPE=ARCADE2;
	
	public static boolean APPLY_DEADBAND = false;
	public static boolean SQUARE_INPUTS = true;


}
