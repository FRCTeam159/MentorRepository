package org.usfirst.frc.team159.robot;

public interface Constants {
  // robot position
  public static final int POSITION_CENTER = 0;
  public static final int POSITION_LEFT = 1;
  public static final int POSITION_RIGHT = 2;

  // target position: must be the same as robot position values
  //public static final int CENTER = 0;
  //public static final int LEFT = 1;
  //public static final int RIGHT = 2;
  // target object
  public static final int OBJECT_SWITCH = 0;
  public static final int OBJECT_SCALE = 1;
  public static final int OBJECT_NONE = 2;

  public static final int SAME_SCALE = 0;
  public static final int SAME_SWITCH = 1;
  public static final int TWO_CUBE_AUTO = 2;

  public static final int OTHER_SCALE = 0;
  public static final int OTHER_SWITCH = 1;
  public static final int GO_STRAIGHT = 2;

  // grabber states
  public static final int DROP = 0;
  public static final int HOLD = 1;
  public static final int PUSH = 2;
  public static final int GRAB = 3;
}
