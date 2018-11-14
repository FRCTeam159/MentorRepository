
import java.util.ArrayList;
import javax.swing.JFrame;
import javax.swing.SwingUtilities;

import jaci.pathfinder.Pathfinder;
import jaci.pathfinder.Trajectory;
import jaci.pathfinder.Trajectory.Segment;
import jaci.pathfinder.Waypoint;
import jaci.pathfinder.modifiers.TankModifier;

import edu.wpi.first.wpilibj.networktables.NetworkTable;

public class PathPlotTest {
    public static final int LEFT = 0;
    public static final int RIGHT = 1;
    public static final int CENTER = 2;

    public static final int STRAIGHT = 0;
    public static final int H_TURN = 1;
    public static final int S_TURN = 2;

    int robotSide = LEFT;
    int pathTest = H_TURN;

    private boolean printCalculatedTrajectory = false;
    private boolean plotCalculatedTrajectory = false;
    private boolean publishCalculatedTrajectory = true;

    private boolean printCalculatedPath = false;
    private boolean plotCalculatedPath = false;
    private boolean publishCalculatedPath = true;

    double distance = feetToMeters(10); // forward distance
    double offset = feetToMeters(3); // turn distance
    double ROBOT_WIDTH = 34.25;

    public static double MAX_VEL = 1.5;
    public static double MAX_ACC = 22.25;
    public static double MAX_JRK = 12;

    private static final double TIME_STEP = 0.02;
    double wheelbase = inchesToMeters(ROBOT_WIDTH);
    private Trajectory trajectory;
    private Trajectory leftTrajectory;
    private Trajectory rightTrajectory;
    static double last_heading = 0;
    private static NetworkTable table = null;
    private static int pathIndex = 0;

    // IDE PROBLEM
    // Sometimes after a coding error a "build error" popup is always displayed on
    // subsequent runs
    // even when the original problem has been corrected
    // Only thing that seems to fix this is to clear the workspace cache:
    // 1) exit vscode
    // 2) open a bash shell (e.g. gitbash)
    // 3) clear the workspace cache
    // rm -fr ~/AppData/Roaming/Code/User/workspaceStorage/*
    // 4) restart vscode and wait until the Run/Debug control reappears
    //
    // ref:https://github.com/Microsoft/vscode-java-debug/blob/master/Troubleshooting.md#try
    public static void main(String[] args) {
        table = NetworkTable.getTable("datatable"); // starst up a new table server

        SwingUtilities.invokeLater(new Runnable() {
            public void run() {
                try { // Allow time for client to connect to server before sending table data
                    Thread.sleep(1000); // 1 second
                    PathPlotTest test = new PathPlotTest();
                    test.showPathDynamics(); // if publish enabled writes trajectory plot data to server
                    Thread.sleep(1000); // 1 second
                    test.showPathMotion();  // publish path plot data 
                    return;  
                } catch (InterruptedException ex) {
                    System.out.println("Thread.sleep exception in main)");
                }
            }
        });
    }

    public PathPlotTest() {
        trajectory = calculateTrajectory(distance, offset, MAX_VEL, MAX_ACC, MAX_JRK);
        TankModifier modifier = new TankModifier(trajectory);
        modifier.modify(wheelbase);
        leftTrajectory = modifier.getLeftTrajectory(); // Get the Left Side
        rightTrajectory = modifier.getRightTrajectory(); // Get the Right Side
    }

    void showPathMotion() {
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

            if (printCalculatedPath)
                System.out.format("%f %f %f %f %f %f %f\n", time, lx, ly, cx, cy, rx, ry);
            time += centerSegment.dt;
            if (plotCalculatedPath || publishCalculatedPath) {
                PathData pd = new PathData();
                pd.tm = time;
                pd.d[0] = lx;
                pd.d[1] = ly;
                pd.d[2] = cx;
                pd.d[3] = cy;
                pd.d[4] = rx;
                pd.d[5] = ry;

                data.add(pd);
            }
        }
        if (plotCalculatedPath) {
            JFrame frame = new PlotPath(data, 3, PlotPath.PATH_MODE);
            //frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
            frame.pack();
            frame.setLocationRelativeTo(null);
            frame.setVisible(true);
        }
        if (publishCalculatedPath) {
            publish(data, 3, PlotPath.PATH_MODE);
        }
    }

    void showPathDynamics() {
        double time = 0;
        ArrayList<PathData> data = new ArrayList<>();

        for (int i = 0; i < trajectory.length(); i++) {
            Segment segment = trajectory.get(i);
            double seg_heading = Pathfinder.r2d(segment.heading);
            double heading = unwrap(last_heading, seg_heading);
            if (i == 0)
                last_heading = segment.heading;
            double x = metersToFeet(segment.x);
            double y = metersToFeet(segment.y);
            double v = metersToFeet(segment.velocity);
            double a = metersToFeet(segment.acceleration);
            double h = heading;

            if (printCalculatedTrajectory)
                System.out.format("%f %f %f %f %f %f \n", time, x, y, v, a, h);
            if (plotCalculatedTrajectory || publishCalculatedTrajectory) {
                PathData pd = new PathData();
                pd.tm = time;
                pd.d[0] = x;
                pd.d[1] = y;
                pd.d[2] = v;
                pd.d[3] = a;
                pd.d[4] = Pathfinder.d2r(heading);
                data.add(pd);
            }
            time += segment.dt;
            last_heading = heading;
        }
        if (plotCalculatedTrajectory) {
            JFrame frame = new PlotPath(data, 5, PlotPath.TRAJ_MODE);
            frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
            frame.pack();
            frame.setLocationRelativeTo(null);
            frame.setVisible(true);
        }
        if (publishCalculatedTrajectory) {
            publish(data, 5, PlotPath.TRAJ_MODE);
        }
    }

    private void publish(ArrayList<PathData> dataList, int traces, int mode) {
        double info[] = new double[4];
        int points = dataList.size();
        info[0] = pathIndex;
        info[1] = traces;
        info[2] = points;
        info[3] = mode;
       
        System.out.println(
                "Publishing Plot Data id:" + info[0] + " traces:" + info[1] + " pts:" + info[2] + " mode:" + info[3]);
        // Problem:
        //  1) NetworkTable valueChanged listener in client does not respond to changes in the data part of Double arrays
        //  - e.g table.putValue("NewPlot", NetworkTableValue.makeDoubleArray(info));
        //  - A solution is to assign a new String label to each plot (e.g. "NewPlot"+id)
        //  - This requires restarting the client when the server is restarted to reset the plot id on both sides to 0
        //  2) Another solution is to use a simple number value for the plot id instead of a number array
        //  - The valueChanged listener in the client does respond to changes in the data part in this case
        //  - Once the client knows the plot id it can look for the plot parameters in a separate double array like "PlotParams"+id"
        table.putNumber("NewPlot",pathIndex); 
        table.putNumberArray("PlotParams" + pathIndex, info);
        if(mode==PlotPath.PATH_MODE)
            traces*=2;

        for (int i = 0; i < points; i++) {
            PathData pathData = dataList.get(i);
            double data[] = new double[traces + 2];
            data[0] = (double) i;
            data[1] = pathData.tm;
            for (int j = 0; j < traces; j++) {
                data[j + 2] = pathData.d[j];
            }
            table.putNumberArray("PlotData" + i, data);
        }
        dataList.clear();
        pathIndex++;
    }

    double unwrap(double previous_angle, double new_angle) {
        double d = new_angle - previous_angle;
        d = d >= 180 ? d - 360 : (d <= -180 ? d + 360 : d);
        return previous_angle + d;
    }

    private static double inchesToMeters(double inches) {
        return inches * 0.0254;
    }

    private static double metersToInches(double meters) {
        return meters / 0.0254;
    }

    private static double feetToMeters(double feet) {
        return 2.54 * 12 * feet / 100;
    }

    private static double metersToFeet(double meters) {
        return meters * 100 / (2.54 * 12);
    }

    private Waypoint[] calculatePathWaypoints(double d, double y) {
        Waypoint[] returnWaypoints = null;
        switch (pathTest) {
        case STRAIGHT:
            returnWaypoints = calculateStraightPoints(d);
            break;
        case H_TURN:
            returnWaypoints = calculateHookpoints(d, y);
            break;
        case S_TURN:
            returnWaypoints = calculateSpoints(d, y);
            break;
        }
        return returnWaypoints;
    }

    private Waypoint[] calculateStraightPoints(double x) {
        return new Waypoint[] { new Waypoint(0, 0, 0), new Waypoint(x, 0, 0) };
    }

    private Waypoint[] calculateHookpoints(double x, double y) {
        Waypoint[] waypoints = new Waypoint[3];
        waypoints[0] = new Waypoint(0, 0, 0);
        waypoints[1] = new Waypoint(x - y, 0, 0);
        waypoints[2] = new Waypoint(x, y, Pathfinder.d2r(90));
        if (robotSide == LEFT)
            return waypoints;
        else
            return mirrorWaypoints(waypoints);
    }

    private Waypoint[] calculateSpoints(double x, double y) {
        Waypoint[] waypoints = new Waypoint[3];
        waypoints[0] = new Waypoint(0, 0, 0);
        waypoints[1] = new Waypoint(x / 2, y / 2, Pathfinder.d2r(45));
        waypoints[2] = new Waypoint(x, y, 0);
        if (robotSide == LEFT)
            return mirrorWaypoints(waypoints);
        else
            return waypoints;
    }

    private Trajectory calculateTrajectory(double d, double y, double maxVelocity, double maxAcceleration,
            double maxJerk) {
        Trajectory.Config config = new Trajectory.Config(Trajectory.FitMethod.HERMITE_CUBIC,
                Trajectory.Config.SAMPLES_FAST, TIME_STEP, maxVelocity, maxAcceleration, maxJerk);

        Waypoint[] waypoints = calculatePathWaypoints(d, y);
        if (waypoints == null)
            return null;

        Trajectory calculatedTrajectory = Pathfinder.generate(waypoints, config);

        if (waypoints != null) {
            for (Waypoint waypoint : waypoints) {
                System.out.println(waypoint.x + " " + waypoint.y + " " + waypoint.angle);
            }
        }
        return calculatedTrajectory;
    }

    private Waypoint[] mirrorWaypoints(Waypoint[] waypoints) {
        Waypoint[] newWaypoints = new Waypoint[waypoints.length];
        for (int i = 0; i < waypoints.length; i++) {
            newWaypoints[i] = new Waypoint(waypoints[i].x, -waypoints[i].y, -waypoints[i].angle);
        }
        return newWaypoints;
    }
}
