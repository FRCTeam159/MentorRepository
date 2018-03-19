package org.usfirst.frc.team159.robot.commands;

import java.util.ArrayList;

import org.usfirst.frc.team159.robot.Constants;
import org.usfirst.frc.team159.robot.PathData;
import org.usfirst.frc.team159.robot.PhysicalConstants;
import org.usfirst.frc.team159.robot.PlotPath;
import org.usfirst.frc.team159.robot.Robot;
import edu.wpi.first.wpilibj.Timer;
import edu.wpi.first.wpilibj.command.Command;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;
import jaci.pathfinder.Pathfinder;
import jaci.pathfinder.Trajectory;
import jaci.pathfinder.Trajectory.Segment;
import jaci.pathfinder.Waypoint;
import jaci.pathfinder.followers.DistanceFollower;
import jaci.pathfinder.modifiers.TankModifier;

/**
 *
 */
public class DrivePath extends Command implements PhysicalConstants, Constants {
  Trajectory trajectory;
  Trajectory leftTrajectory;
  Trajectory rightTrajectory;
  DistanceFollower leftFollower;
  DistanceFollower rightFollower;
  Trajectory.Config config;
  TankModifier modifier;

  ArrayList<PathData> pathdata = new ArrayList<PathData>();
  static PlotPath pp = null;

  double TIME_STEP = 0.02;

  final double i2m = 0.0254;
  final double m2i = (1.0 / 0.0254);

  double wheelbase_width = i2m * ROBOT_WIDTH; // meters
  private int pathIndex = 0;

  static public boolean print_calculated_trajectory = false;
  static public boolean print_calculated_path = false;
  static public boolean use_gyro = Robot.useGyro;
  static public boolean debug_command = false;
  static public boolean plot_path = false;
  static public boolean publish_path = false;
  static public boolean print_path = false;

  double runtime = 0;
  Timer mytimer;

  double last_heading = 0;

  int path_points = 0;
  int target = 0;
  boolean mirror=false;
  boolean reverse=false;

  public DrivePath(int target,boolean mirror,boolean reverse) {
    requires(Robot.driveTrain);
    this.target = target;
    this.mirror=mirror;
    this.reverse=reverse;

    mytimer = new Timer();
    mytimer.start();
    mytimer.reset();
    

    double MAX_VEL = Robot.MAX_VEL;
    double MAX_ACC = Robot.MAX_ACC;
    double MAX_JRK = Robot.MAX_JRK;
    double KP = Robot.KP;
    double KI = 0.0;
    double KD = Robot.KP;
    double KV = 1.0 / MAX_VEL;
    double KA = 0.0;

    plot_path = SmartDashboard.getBoolean("Plot", true);
   // Robot.showAutoTarget();

    Waypoint[] waypoints = calculatePath(target);
    for (Waypoint waypoint : waypoints) {
      System.out.println(m2i * waypoint.x + " " + m2i * waypoint.y + " " + waypoint.angle);
    }

    config = new Trajectory.Config(Trajectory.FitMethod.HERMITE_CUBIC, Trajectory.Config.SAMPLES_FAST, TIME_STEP,
        MAX_VEL, MAX_ACC, MAX_JRK);

    trajectory = Pathfinder.generate(waypoints, config);

    if (trajectory == null) {
      System.out.println("Uh-Oh! Trajectory could not be generated!\n");
      return;
    }
    path_points = trajectory.length();
    runtime = path_points * TIME_STEP;
    // Create the Modifier Object
    TankModifier modifier = new TankModifier(trajectory);

    // Generate the Left and Right trajectories using the original trajectory
    // as the midpoint
    modifier.modify(wheelbase_width);

    leftTrajectory = modifier.getLeftTrajectory(); // Get the Left Side
    rightTrajectory = modifier.getRightTrajectory(); // Get the Right Side

    leftFollower = new DistanceFollower(leftTrajectory);
    leftFollower.configurePIDVA(KP, KI, KD, KV, KA);
    rightFollower = new DistanceFollower(rightTrajectory);
    rightFollower.configurePIDVA(KP, KI, KD, KV, KA);
    System.out.format("trajectory length:%d runtime:%f calctime:%f\n", trajectory.length(), runtime, getTime());

    if (print_calculated_trajectory) {
      double t = 0;
      for (int i = 0; i < path_points; i++) {
        Segment s = trajectory.get(i);
        double h = Pathfinder.r2d(s.heading);
        h = h > 180 ? h - 360 : h;
        System.out.format("%f %f %f %f %f %f \n", t, m2i * s.x, m2i * s.y, m2i * s.velocity, m2i * s.acceleration, h);
        t += s.dt;
      }
    }
    if (print_calculated_path) {
      double t = 0;
      for (int i = 0; i < path_points; i++) {
        Segment s = trajectory.get(i);
        Segment l = leftTrajectory.get(i);
        Segment r = rightTrajectory.get(i);
        System.out.format("%f %f %f %f %f %f %f %f\n", t, m2i * l.x, m2i * l.y, m2i * r.x, m2i * r.y, m2i * l.position,
            m2i * r.position, Pathfinder.r2d(s.heading));
        t += s.dt;
      }
    }
  }

  // Called just before this Command runs the first time
  protected void initialize() {
    if (trajectory == null)
      return;
    System.out.println("DrivePath.initialize:" + target);
    leftFollower.reset();
    rightFollower.reset();
    last_heading = 0;// Robot.driveTrain.getHeading();
    // Robot.driveTrain.resetEncoders();
    Robot.driveTrain.reset();

    mytimer.start();
    mytimer.reset();
    pathIndex = 0;
  }

  double feet2meters(double x) {
    return 12 * x * 0.0254;
  }

  double getTime() {
    // double curtime=1e-9*(System.nanoTime()-start_time);
    // return (double)curtime;
    return mytimer.get();
  }

  // Called repeatedly when this Command is scheduled to run
  protected void execute() {
    if (trajectory == null)
      return;
    double scale = Robot.auto_scale;

    double ld = (Robot.driveTrain.getLeftDistance());
    double rd = (Robot.driveTrain.getRightDistance());
    if(reverse) {
      ld=-ld;
      rd=-rd;
    }
    double l = leftFollower.calculate(feet2meters(ld)); // reversal ?
    double r = rightFollower.calculate(feet2meters(rd));
    double turn = 0;
    double gh = Robot.driveTrain.getHeading(); // Assuming the gyro is giving a value in degrees
    if(reverse)
      gh=-gh;
    gh = unwrap(last_heading, gh);

    double th = Pathfinder.r2d(leftFollower.getHeading()); // Should also be in degrees
    th = th > 180 ? th - 360 : th; // convert to signed angle fixes problem:th 0->360 gh:-180->180
    double herr = th - gh;
    if (Robot.useGyro)
      turn = Robot.GFACT * (-1.0 / 180.0) * herr;
    //if(reverse)
    //  turn=-turn;
    double lval = l + turn;
    double rval = r - turn;
    lval *= scale;
    rval *= scale;
    if (Math.abs(lval) > 1.0 || Math.abs(rval) > 1.0)
      SmartDashboard.putBoolean("Error", true);
    double curtime = getTime();

    if (debug_command)
      System.out.format("%f %f %f %f %f %f %f\n", curtime, ld, rd, gh, th, lval, rval);
    if (print_path || plot_path || publish_path)
      debugPathError(ld,rd,gh);
    if(reverse)
      Robot.driveTrain.set(-lval, -rval);
    else
      Robot.driveTrain.set(lval, rval);

    pathIndex++;
    last_heading = gh;

  }

  // Make this return true when this Command no longer needs to run execute()
  protected boolean isFinished() {
    if (trajectory == null)
      return true;
    if ((pathIndex >= path_points) || (leftFollower.isFinished() && rightFollower.isFinished()))
      return true;
    else if ((getTime() - runtime) > 1) {
      System.out.println("DrivePath Timeout Expired");
      return true;
    }
    return false;
  }

  // Called once after isFinished returns true
  protected void end() {
    System.out.println("DrivePath.end()");
    if (plot_path)
      new PlotPath(pathdata, 6);
    if (publish_path)
      PlotPath.publish(pathdata, 6);
  }

  // Called when another command which requires one or more of the same
  // subsystems is scheduled to run
  protected void interrupted() {
    System.out.println("DrivePath.interrupted()");
    end();
  }

  double unwrap(double previous_angle, double new_angle) {
    double d = new_angle - previous_angle;
    d = d >= 180 ? d - 360 : (d <= -180 ? d + 360 : d);
    return previous_angle + d;
  }

  private void debugPathError(double ld, double rd,double g) {
    PathData pd = new PathData();
    pd.tm = getTime();
    pd.d[0] = 12 * ld;//(Robot.driveTrain.getLeftDistance());
    pd.d[2] = 12 * rd;//(Robot.driveTrain.getRightDistance());
    Segment ls = leftTrajectory.get(pathIndex);
    Segment rs = rightTrajectory.get(pathIndex);
    pd.d[1] = m2i * (ls.position);
    pd.d[3] = m2i * (rs.position);
    double gh = g;//unwrap(last_heading, Robot.driveTrain.getHeading());

    pd.d[4] = gh; // Assuming the gyro is giving a value in degrees
    double th = Pathfinder.r2d(rs.heading); // Should also be in degrees
    pd.d[5] = th > 180 ? th - 360 : th; // convert to signed angle fixes problem:th 0->360 gh:-180->180

    if (print_path)
      System.out.format("%f %f %f %f %f\n", pd.tm, pd.d[0], pd.d[1], pd.d[2], pd.d[3]);
    if (plot_path || publish_path)
      pathdata.add(pd);
  }

  private Waypoint[] calculateStraightPoints() {
    double x = ROBOT_TO_SWITCH + 12;
    Waypoint[] waypoints = new Waypoint[2];
    waypoints[0] = new Waypoint(0, 0, 0);
    waypoints[1] = new Waypoint(x, 0, 0);
    return waypoints;
  }

  private Waypoint[] calculateSecondCenterSwitchPoints() {
    double x = 70;//ROBOT_TO_SWITCH-36;
    double y = SWITCH_CENTER_TO_PLATE_EDGE;
    boolean delta=mirror^reverse;
    
    y -= delta ? ROBOT_Y_OFFSET_FROM_CENTER-6 : 0;

    Waypoint[] waypoints = new Waypoint[3];
    waypoints[0] = new Waypoint(0, 0, 0);
    waypoints[1] = new Waypoint(x / 2, y / 2, Pathfinder.d2r(45));
    waypoints[2] = new Waypoint(x, y, 0);
    if (mirror)
      return mirrorWaypoints(waypoints);
    else
      return waypoints;
  }
  private Waypoint[] calculateCenterSwitchPoints() {
    double x = ROBOT_TO_SWITCH;
    double y = SWITCH_CENTER_TO_PLATE_EDGE;
    y -= mirror ? ROBOT_Y_OFFSET_FROM_CENTER : 0;

    Waypoint[] waypoints = new Waypoint[3];
    waypoints[0] = new Waypoint(0, 0, 0);
    waypoints[1] = new Waypoint(x / 2, y / 2, Pathfinder.d2r(45));
    waypoints[2] = new Waypoint(x+4, y, 0);
    if (mirror)
      return mirrorWaypoints(waypoints);
    else
      return waypoints;
  }

  /**
   * Same Side Switch Path
   * - Fires cube from Switch plate corner (45 degrees)
   */
  private Waypoint[] calculateSideSwitchPoints() {
    double y = SWITCH_HOOK_Y_DISTANCE - 12;
    double x = ROBOT_TO_SWITCH;
    Waypoint[] waypoints = new Waypoint[3];
    waypoints[0] = new Waypoint(0, 0, 0);
    waypoints[1] = new Waypoint(x - 2 * y, 0, 0);
    waypoints[2] = new Waypoint(x, y, Pathfinder.d2r(45));
    if (mirror)
      return mirrorWaypoints(waypoints);
    else
      return waypoints;
  }

  /**
   * Same Side Scale Path
   * - Fires cube from Scale plate corner (45 degrees)
   */
  private Waypoint[] calculateSideScalePoints() {
    Waypoint[] waypoints = new Waypoint[4];
    double y = SCALE_HOOK_Y_DISTANCE;
    double x = ROBOT_TO_SCALE_X - 6;
    waypoints[0] = new Waypoint(0, 0, 0);
    // this path comes in from inside edge of Scale plate
    // double y = SCALE_HOOK_Y_DISTANCE+SCALE_WIDTH/2-ROBOT_WIDTH/2;
    // double x = ROBOT_TO_SCALE_X - 6;
    // waypoints[1] = new Waypoint(ROBOT_TO_SWITCH+SWITCH_WIDTH, 0, 0);
    // waypoints[2] = new Waypoint(ROBOT_TO_SCALE_X - 6, y, 0);

    waypoints[1] = new Waypoint(x - 6 * y, -6, 0);
    waypoints[2] = new Waypoint(x - 2 * y, -6, 0);
    waypoints[3] = new Waypoint(x, y - 6, Pathfinder.d2r(45));
    if (mirror)
      return mirrorWaypoints(waypoints);
    else
      return waypoints;
  }

  // calculate second switch path in 2 cube auto
  private Waypoint[] calculateSecondSwitchPoints() {
    Waypoint[] waypoints = new Waypoint[3];
    waypoints[0] = new Waypoint(0, 0, Pathfinder.d2r(0));
    waypoints[1] = new Waypoint(40, 0, Pathfinder.d2r(0));
    waypoints[2] = new Waypoint(70, 20, Pathfinder.d2r(35)); // best values by trial and error
    if (mirror)
      return mirrorWaypoints(waypoints);
    else
      return waypoints;
  }

  private Waypoint[] calculateOtherScalePoints() {
    Waypoint[] waypoints = new Waypoint[5];
    waypoints[0] = new Waypoint(0, 0, 0);
    waypoints[1] = new Waypoint(130, 0, 0);
    waypoints[2] = new Waypoint(220, 90, Pathfinder.d2r(90));
    waypoints[3] = new Waypoint(220, 135, Pathfinder.d2r(90));
    waypoints[4] = new Waypoint(270, 175, Pathfinder.d2r(-15));
    if (mirror)
      return mirrorWaypoints(waypoints);
    else
      return waypoints;
  }

  private Waypoint[] calculateOtherSwitchPoints() {
    Waypoint[] waypoints = new Waypoint[5];
    waypoints[0] = new Waypoint(0, 0, 0);
    waypoints[1] = new Waypoint(150, 0, 0);
    waypoints[2] = new Waypoint(230, 50, Pathfinder.d2r(90));
    waypoints[3] = new Waypoint(230, 150, Pathfinder.d2r(90));
    waypoints[4] = new Waypoint(200, 180, -Pathfinder.d2r(180));
    if (mirror)
      return mirrorWaypoints(waypoints);
    else
      return waypoints;
  }

  private Waypoint[] mirrorWaypoints(Waypoint[] waypoints) {
    Waypoint[] newWaypoints = new Waypoint[waypoints.length];
    for (int i = 0; i < waypoints.length; i++) {
      newWaypoints[i] = new Waypoint(waypoints[i].x, -waypoints[i].y, -waypoints[i].angle);
    }
    return newWaypoints;
  }

  private Waypoint[] waypointsInchesToMeters(Waypoint[] waypoints) {
    Waypoint[] newWaypoints = new Waypoint[waypoints.length];
    for (int i = 0; i < waypoints.length; i++) {
      newWaypoints[i] = new Waypoint(i2m * waypoints[i].x, i2m * waypoints[i].y, waypoints[i].angle);
    }
    return newWaypoints;
  }
  private Waypoint[] calculatePath(int target) {
    Waypoint[] returnWaypoints = null;
    switch(target) {
    case GO_STRAIGHT:
      returnWaypoints = calculateStraightPoints();
      break;
    case CENTER_SWITCH:
      returnWaypoints = calculateCenterSwitchPoints();
      break;
    case SAME_SWITCH:
      returnWaypoints = calculateSideSwitchPoints();
      break;
    case SAME_SCALE:
      returnWaypoints = calculateSideScalePoints();
      break;
    case OTHER_SCALE:
      returnWaypoints = calculateOtherScalePoints();
      break;
    case TWO_CUBE_SIDE:
      returnWaypoints = calculateSecondSwitchPoints();
      break;
    case TWO_CUBE_CENTER:
      returnWaypoints = calculateSecondCenterSwitchPoints();
      break;
    }
    return waypointsInchesToMeters(returnWaypoints);
  }
}
