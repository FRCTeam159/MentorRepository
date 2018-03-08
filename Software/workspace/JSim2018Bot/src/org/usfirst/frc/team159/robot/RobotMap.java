package org.usfirst.frc.team159.robot;

/**
 * The RobotMap is a mapping from the ports sensors and actuators are wired into
 * to a variable name. This provides flexibility changing wiring, makes checking
 * the wiring easier and significantly reduces the number of magic numbers
 * floating around.
 */
public interface RobotMap {
  public static final int FRONTLEFT = 1;
  public static final int FRONTRIGHT = 4;
  public static final int BACKLEFT = 2;
  public static final int BACKRIGHT = 3;

  public static final int CUBEWHEELS = 5;
  public static final int ELEVATORMOTOR = 7;

  public static final int STICK = 0;
  public static final int GEAR_TOGGLE_BUTTON = 4;

  public static final int LEFTTRIGGER = 2;
  public static final int RIGHTTRIGGER = 5;

  public static final int GEARSHIFTID = 0;

  public static final boolean SQUARE_INPUTS = true;
  // robot position
  public static final int CENTER_POSITION = 0;
  public static final int LEFT_POSITION = 1;
  public static final int RIGHT_POSITION = 2;

  // target position: must be the same as robot position values
  public static final int CENTER = 0;
  public static final int LEFT = 1;
  public static final int RIGHT = 2;
  // target object
  public static final int SWITCH = 0;
  public static final int SCALE = 1;
  public static final int NONE = 2;

  public static final int SAME_SCALE = 0;
  public static final int SAME_SWITCH = 1;

  public static final int OTHER_SCALE = 0;
  public static final int OTHER_SWITCH = 1;
  public static final int GO_STRAIGHT = 2;

  // grabber states
  public static final int DROP = 0;
  public static final int HOLD = 1;
  public static final int PUSH = 2;
  public static final int GRAB = 3;

}
