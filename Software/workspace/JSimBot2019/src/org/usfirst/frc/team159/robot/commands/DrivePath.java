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
import javax.swing.JFrame;

public class DrivePath extends Command {
	Trajectory trajectory;
	Trajectory leftTrajectory;
	Trajectory rightTrajectory;
	DistanceFollower leftFollower;
	DistanceFollower rightFollower;
	Trajectory.Config config;
	TankModifier modifier;

	public static double MAX_VEL = 2;
	public static double MAX_ACC = 2.5;
	public static double MAX_JRK = 0.6;
	public static double KP = 3.0;
	public static double KI = 0.0;
	public static double KD = 0.0;
	public static double GFACT = 6.0;

	double TIME_STEP = 0.02;
	double KV = 1.0 / MAX_VEL;
	double KA = 0.0;

	double wheelbase_width = 0.86;

	static public boolean print_path = false;
	static public boolean print_trajectory = false;
	static public boolean plot_trajectory = false;
	static public boolean plot_path = true;

	static public boolean use_gyro = true;
	static public boolean debug_command = false;

	ArrayList<PathData> pathdata = new ArrayList<PathData>();
	private int pathIndex = 0;
	double runtime = 0;

	final double i2m = 0.0254;
	final double m2i = (1.0 / 0.0254);
	long start_time;

	//static double last_gyro_heading = 0;
	//static double last_target_heading = 0;

	Angle calc_angle=new Angle();
	Angle gyro_angle=new Angle();

	MyWaypoint[] points = new MyWaypoint[] { 
		new MyWaypoint(0, 0, 0), 
		new MyWaypoint(3, 0, 0),
		new MyWaypoint(13.5, -3.0, 0)
	};

	public DrivePath() {
		requires(Robot.driveTrain);
		start_time = System.nanoTime();
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
		System.out.format("trajectory length:%d runtime:%f calctime:%f\n", trajectory.length(), runtime, getProcTime());

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
		if (plot_trajectory)
			plotCalculatedPath();
	}

	double getProcTime() {
		double curtime = 1e-9 * (System.nanoTime() - start_time);
		return (double) curtime;
	}

	double getSimTime() {
		return (double) TIME_STEP * pathIndex;
	}

	public static double feetToMeters(double feet) {
		return 2.54 * 12 * feet / 100;
	}

	// double unwrap(double previous_angle, double new_angle) {
	// 	double d = new_angle - previous_angle;
	// 	d = d >= 180 ? d - 360 : (d <= -180 ? d + 360 : d);
	// 	return previous_angle + d;
	// }

	// Called just before this Command runs the first time
	protected void initialize() {
		if (trajectory == null)
			return;
		System.out.println("DrivePath.initialize()");
		leftFollower.reset();
		rightFollower.reset();
		Robot.driveTrain.reset();
		pathIndex = 0;
		start_time = System.nanoTime();
	}

	double feet2meters(double x) {
		return 12 * x * 0.0254;
	}

	private static double metersToFeet(double meters) {
		return meters * 100 / (2.54 * 12);
	}

	// Called repeatedly when this Command is scheduled to run
	protected void execute() {
		if (trajectory == null)
			return;
		double ld = feet2meters(Robot.driveTrain.getLeftDistance());
		double rd = feet2meters(Robot.driveTrain.getRightDistance());
		double l = leftFollower.calculate(ld);
		double r = rightFollower.calculate(rd);

		double turn = 0;
		double scale = Robot.auto_scale;

		double gh = Robot.driveTrain.getHeading(); // Assuming the gyro is giving a value in degrees
		gh=gyro_angle.unwrap(gh);
		//gh = unwrap(last_gyro_heading, gh);
		//last_gyro_heading = gh;

		double th = -Pathfinder.r2d(leftFollower.getHeading()); // Should also be in degrees
		th=calc_angle.unwrap(th);
		//th = unwrap(last_target_heading, th);
		//last_target_heading = th;

		double herr = th - gh;
		if (use_gyro)
			turn = GFACT * (1.0 / 180.0) * herr;
		l += turn;
		r -= turn;
		l *= scale;
		r *= scale;
		if (debug_command)
			System.out.format("%f %f %f %f %f %f %f %f\n", getSimTime(), ld, rd, gh, th, herr, l, r);
		if (print_path || plot_path)
			debugPathError(ld, rd, l, r, gh, th);
		Robot.driveTrain.set(l, r);
		pathIndex++;
	}

	private void debugPathError(double ld, double rd, double l, double r, double g, double t) {
		PathData pd = new PathData();
		pd.tm = getSimTime();
		Segment ls = leftTrajectory.get(pathIndex);
		Segment rs = rightTrajectory.get(pathIndex);
		pd.d[0] = m2i * ld;// (Robot.driveTrain.getLeftDistance());
		pd.d[1] = m2i * (ls.position);
		pd.d[2] = m2i * rd;// (Robot.driveTrain.getRightDistance());
		pd.d[3] = m2i * (rs.position);

		// if (use_gyro) {
		pd.d[4] = g; // Assuming the gyro is giving a value in degrees
		pd.d[5] = t;
		// } else {
		// pd.d[4] = l * 10;
		// pd.d[5] = r * 10;
		// }

		if (print_path)
			System.out.format("%f %f %f %f %f\n", pd.tm, pd.d[0], pd.d[1], pd.d[2], pd.d[3]);
		if (plot_path)
			pathdata.add(pd);
	}

	// Make this return true when this Command no longer needs to run execute()
	protected boolean isFinished() {
		if (trajectory == null)
			return true;
		if (getSimTime() - runtime > 0.0)
			return true;
		if (leftFollower.isFinished() && rightFollower.isFinished())
			return true;
		return false;
	}

	// Called once after isFinished returns true
	protected void end() {
		System.out.println("DrivePath.end()");
		Robot.driveTrain.disable();
		if (plot_path)
			new PlotPath(pathdata, 0, 6, PlotPath.DIST_MODE);
	}

	// Called when another command which requires one or more of the same
	// subsystems is scheduled to run
	protected void interrupted() {
		System.out.println("DrivePath.interrupted()");
		end();
	}

	void plotCalculatedPath() {
		double time = 0;
		ArrayList<PathData> data = new ArrayList<>();
		for (int i = 0; i < trajectory.length(); i++) {
			Segment centerSegment = trajectory.get(i);
			Segment leftSegment = leftTrajectory.get(i);
			Segment rightSegment = rightTrajectory.get(i);
			double cx = metersToFeet(centerSegment.x);
			double cy = metersToFeet(centerSegment.y);
			double lx = metersToFeet(leftSegment.x);
			double ly = metersToFeet(leftSegment.y);
			double rx = metersToFeet(rightSegment.x);
			double ry = metersToFeet(rightSegment.y);

			time += centerSegment.dt;

			PathData pd = new PathData();
			// note: pathfinder inverts y order of left and right side
			pd.tm = time;
			pd.d[0] = lx;
			pd.d[1] = -ly;
			pd.d[2] = cx;
			pd.d[3] = -cy;
			pd.d[4] = rx;
			pd.d[5] = -ry;
			data.add(pd);
		}
		JFrame frame = new PlotPath(data, 0, 3, PlotPath.PATH_MODE);
		// frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.pack();
		frame.setLocationRelativeTo(null);
		frame.setVisible(true);
	}

	private class MyWaypoint extends Waypoint {
		MyWaypoint(double x, double y, double h) {
			super(feetToMeters(x), -feetToMeters(y), -Pathfinder.d2r(h));
		}
	};
	private class Angle {
		double previous_angle=0;
		public void reset(){
			previous_angle=0;
		}
		public double unwrap(double angle) {
			double d = angle - previous_angle;
		    d = d >= 180 ? d - 360 : (d <= -180 ? d + 360 : d);
			double new_angle=previous_angle + d;
			previous_angle=new_angle;
			return new_angle;
		}
	};
}
