package org.usfirst.frc.team159.robot.commands;

import org.usfirst.frc.team159.robot.Robot;

import edu.wpi.first.wpilibj.Timer;
import edu.wpi.first.wpilibj.command.Command;
import jaci.pathfinder.Pathfinder;
import jaci.pathfinder.Trajectory;
import jaci.pathfinder.Trajectory.Segment;
import jaci.pathfinder.Waypoint;
import jaci.pathfinder.followers.DistanceFollower;
import jaci.pathfinder.modifiers.TankModifier;
import org.usfirst.frc.team159.robot.subsystems.PlotPath;
import org.usfirst.frc.team159.robot.PathData;
import java.util.ArrayList;

public class DrivePath extends Command {
	Trajectory trajectory;
	Trajectory leftTrajectory;
	Trajectory rightTrajectory;
	DistanceFollower leftFollower;
	DistanceFollower rightFollower;
	Trajectory.Config config;
	TankModifier modifier;

	public static double MAX_VEL = 1.9;
  public static double MAX_ACC = 2.2;
  public static double MAX_JRK = 1.0;
  public static double KP = 3.0;
	public static double KI = 0.0;
	public static double KD = 0.0;
	public static double GFACT = 10.0;
	
	double TIME_STEP = 0.02;
	double KV = 1.0 / MAX_VEL;
	double KA = 0.0;

	double wheelbase_width = 0.66;

	static public boolean print_path = false;
	static public boolean print_trajectory = false;
	static public boolean plot_path = true;

	static public boolean use_gyro = false;
	static public boolean debug_command = false;

	ArrayList<PathData> pathdata = new ArrayList<PathData>();
	private int pathIndex = 0;
	double runtime = 0;
	Timer mytimer;

	final double i2m = 0.0254;
  final double m2i = (1.0 / 0.0254);

	Waypoint[] points = new Waypoint[] { new Waypoint(0, 0, 0), new Waypoint(3, 0, 0) // ,
			// new Waypoint(4, 4, Pathfinder.d2r(45)),
			// new Waypoint(8, 8, 0)
	};

	public DrivePath() {
		requires(Robot.driveTrain);
		mytimer = new Timer();
		mytimer.start();
		mytimer.reset();

		config = new Trajectory.Config(Trajectory.FitMethod.HERMITE_CUBIC, Trajectory.Config.SAMPLES_FAST, TIME_STEP,
				MAX_VEL, MAX_ACC, MAX_JRK);
		trajectory = Pathfinder.generate(points, config);
		if (trajectory == null) {
			System.out.println("Uh-Oh! Trajectory could not be generated!\n");
			return;
		}

		runtime = trajectory.length() * TIME_STEP;
		// Create the Modifier Object
		TankModifier modifier = new TankModifier(trajectory);

		// Generate the Left and Right trajectories using the original trajectory
		// as the center
		modifier.modify(wheelbase_width);

		leftTrajectory = modifier.getLeftTrajectory(); // Get the Left Side
		rightTrajectory = modifier.getRightTrajectory(); // Get the Right Side

		leftFollower = new DistanceFollower(leftTrajectory);
		leftFollower.configurePIDVA(KP, KI, KD, KV, KA);
		rightFollower = new DistanceFollower(rightTrajectory);
		rightFollower.configurePIDVA(KP, KI, KD, KV, KA);
		System.out.format("trajectory length:%d runtime:%f calctime:%f\n", trajectory.length(), runtime, mytimer.get());

		if (print_path) {
			double t = 0;
			for (int i = 0; i < trajectory.length(); i++) {
				Segment s = trajectory.get(i);
				System.out.format("%f %f %f %f %f %f \n", t, s.x, s.y, s.velocity, s.acceleration, s.heading);
				t += s.dt;
			}
		}
		if (print_trajectory) {
			double t = 0;
			for (int i = 0; i < trajectory.length(); i++) {
				Segment s = trajectory.get(i);
				Segment l = leftTrajectory.get(i);
				Segment r = rightTrajectory.get(i);
				System.out.format("%f %f %f %f %f\n", t, l.x, l.y, r.x, r.y);
				t += s.dt;
			}
		}
	}
	double getTime() {
    // double curtime=1e-9*(System.nanoTime()-start_time);
    // return (double)curtime;
    return mytimer.get();
  }
	// Called just before this Command runs the first time
	protected void initialize() {
		if (trajectory == null)
			return;
		System.out.println("DrivePath.initialize()");
		leftFollower.reset();
		rightFollower.reset();
		Robot.driveTrain.reset();
		mytimer.start();
		mytimer.reset();
		pathIndex = 0;
	}

	double feet2meters(double x) {
		return 12 * x * 0.0254;
	}

	// Called repeatedly when this Command is scheduled to run
	protected void execute() {
		if (trajectory == null)
			return;
		double ld = feet2meters(Robot.driveTrain.getRightDistance());
		double rd = feet2meters(Robot.driveTrain.getLeftDistance());
		double l = leftFollower.calculate(ld);
		double r = rightFollower.calculate(rd);

		double turn = 0;

		double gh = Robot.driveTrain.getHeading(); // Assuming the gyro is giving a value in degrees
		double th = Pathfinder.r2d(leftFollower.getHeading()); // Should also be in degrees
		double herr = th - gh;
		if (use_gyro)
			turn = GFACT * (-1.0 / 180.0) * herr;
		l+=turn;
		r-=turn;
		if (debug_command)
			System.out.format("%f %f %f %f %f %f %f %f\n", mytimer.get(), ld, rd, gh, th, herr, l, r);
		if (print_path || plot_path)
      debugPathError(ld,rd,l,r,gh);
		Robot.driveTrain.tankDrive(l, r);
		pathIndex++;
	}
	private void debugPathError(double ld, double rd,double l, double r, double g) {
    PathData pd = new PathData();
    pd.tm = getTime();
    Segment ls = leftTrajectory.get(pathIndex);
		Segment rs = rightTrajectory.get(pathIndex);  
		pd.d[0] = m2i * ld;//(Robot.driveTrain.getLeftDistance());
		pd.d[1] = m2i * (ls.position);
    pd.d[2] = m2i * rd;//(Robot.driveTrain.getRightDistance());
		pd.d[3] = m2i * (rs.position);
		
		if (use_gyro){
			double gh = g;//unwrap(last_heading, Robot.driveTrain.getHeading());
			pd.d[4] = gh; // Assuming the gyro is giving a value in degrees
			double th = Pathfinder.r2d(rs.heading); // Should also be in degrees
			pd.d[5] = th > 180 ? th - 360 : th; // convert to signed angle fixes problem:th 0->360 gh:-180->180
		}
		else{
			pd.d[4]=l*100;
			pd.d[5]=r*100;
		}

    if (print_path)
      System.out.format("%f %f %f %f %f\n", pd.tm, pd.d[0], pd.d[1], pd.d[2], pd.d[3]);
    if (plot_path)
      pathdata.add(pd);
  }
	// Make this return true when this Command no longer needs to run execute()
	protected boolean isFinished() {
		if (trajectory == null)
			return true;
		if (mytimer.get() - runtime > 0.5)
			return true;
		if (leftFollower.isFinished() && rightFollower.isFinished())
			return true;

		return false;
	}

	// Called once after isFinished returns true
	protected void end() {
		System.out.println("DrivePath.end()");
		if (plot_path)
      new PlotPath(pathdata,0, 6, PlotPath.DIST_MODE);
	}

	// Called when another command which requires one or more of the same
	// subsystems is scheduled to run
	protected void interrupted() {
		System.out.println("DrivePath.interrupted()");
		end();
	}
	
}
