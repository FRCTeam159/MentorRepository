#include "DrivePath.h"


#define POINT_LENGTH 4

#define I2M(x) x*0.0254
static Waypoint points[POINT_LENGTH];

//static Waypoint p1 = { -4, -1, d2r(45) };      // Waypoint @ x=-4, y=-1, exit angle=45 degrees
//static Waypoint p2 = { -1, 2, 0 };             // Waypoint @ x=-1, y= 2, exit angle= 0 radians
//static Waypoint p3 = {  2, 4, 0 };             // Waypoint @ x= 2, y= 4, exit angle= 0 radians

static Waypoint p0 = { 0, 0, 0 };
static Waypoint p1 = { 2, 0, 0 };
static Waypoint p2 = { 4, 4, d2r(45)};
static Waypoint p3 = { 8, 8, 0 };

static TrajectoryCandidate candidate;

#define PRINT_PATH
#define PRINT_TRAJECTORY

#define TIME_STEP 0.02
#define MAX_VEL 1.0
#define MAX_ACC 12.0
#define MAX_JRK 20.0
#define KP 0.1
#define KI 0.0
#define KD 0.0
#define KV 0.2/MAX_VEL
#define KA 0.0
#define WHEELBASE_WIDTH  0.64

static Timer mytimer;

DrivePath::DrivePath() : config{KP,KI,KD,KV,KA} {
	Requires(driveTrain.get());
	points[0] = p0;
	points[1] = p1;
	points[2] = p2;
	points[3] = p3;

    std::cout << "new DrivePath()"<< std::endl;


	// Prepare the Trajectory for Generation.
	//
	// Arguments:
	// Fit Function:        FIT_HERMITE_CUBIC or FIT_HERMITE_QUINTIC
	// Sample Count:        PATHFINDER_SAMPLES_HIGH (100 000)
	//                      PATHFINDER_SAMPLES_LOW  (10 000)
	//                      PATHFINDER_SAMPLES_FAST (1 000)
	// Time Step:           0.001 Seconds
	// Max Velocity:        15 m/s
	// Max Acceleration:    10 m/s/s
	// Max Jerk:            60 m/s/s/s

	pathfinder_prepare(points, POINT_LENGTH, FIT_HERMITE_CUBIC, PATHFINDER_SAMPLES_FAST, TIME_STEP, MAX_VEL, MAX_ACC, MAX_JRK, &candidate);

	length = candidate.length;

	// Array of Segments (the trajectory points) to store the trajectory in
	trajectory = (Segment*)malloc(length * sizeof(Segment));
	leftTrajectory = (Segment*)malloc(length * sizeof(Segment));
	rightTrajectory = (Segment*)malloc(length * sizeof(Segment));

	// Generate the trajectory
	int result = pathfinder_generate(&candidate, trajectory);
	if (result < 0) {
	    // An error occured
	    printf("Uh-Oh! Trajectory could not be generated!\n");
		cleanup();
	    return;
	}
	printf("Trajectory length:%d path_length:%d totalLength:%f travel_time:%f\n",
			 candidate.length,candidate.path_length,candidate.totalLength,length*TIME_STEP);
#ifdef PRINT_PATH
	for (int i = 0; i < length; i++) {
	    Segment s = trajectory[i];
	    printf("Time Step: %f\n", s.dt);
	    printf("Coords: (%f, %f)\n", s.x, s.y);
	    printf("Position (Distance): %f\n", s.position);
	    printf("Velocity: %f\n", s.velocity);
	    printf("Acceleration: %f\n", s.acceleration);
	    printf("Jerk (Acceleration per Second): %f\n", s.jerk);
	    printf("Heading (radians): %f\n", s.heading);
	}
#endif

    // Generate the Left and Right trajectories of the wheelbase using the
    // originally generated trajectory
    pathfinder_modify_tank(trajectory, length, leftTrajectory, rightTrajectory, WHEELBASE_WIDTH);
    std::cout << "Path generated length="<<length<< std::endl;
#ifdef PRINT_TRAJECTORY
    double tm=0;
	for (int i = 0; i < length; i++) {
		Segment l = leftTrajectory[i];
		Segment r = rightTrajectory[i];
	    printf("%-3d time:%f left:%f right:%f\n", i, tm, l.position,r.position);
	    tm+=TIME_STEP;
	}
#endif
}

DrivePath::~DrivePath() {
	cleanup();
}

// Called just before this Command runs the first time
void DrivePath::Initialize() {
	leftFollower.finished=0;leftFollower.last_error=0;leftFollower.segment=0;
	rightFollower.finished=0;rightFollower.last_error=0;rightFollower.segment=0;
	std::cout << "DrivePath Started .."<< std::endl;
	driveTrain->Reset();
	mytimer.Start();
	mytimer.Reset();
}

// Called repeatedly when this Command is scheduled to run
//double pathfinder_follow_distance(FollowerConfig c, DistanceFollower *follower, Segment *trajectory, int trajectory_length, double distance) {
#define DEBUG_COMMAND
void DrivePath::Execute() {
	double ld=I2M(driveTrain->GetLeftDistance());
	double rd=I2M(driveTrain->GetRightDistance());
	double l=pathfinder_follow_distance(config,&leftFollower,leftTrajectory,length,ld);
	double r=pathfinder_follow_distance(config,&rightFollower,rightTrajectory,length,rd);
	double gyro_heading = driveTrain->GetHeading() ;     // Assuming gyro angle is given in degrees
	double desired_heading = r2d(leftFollower.heading);
	double angle_difference = desired_heading - gyro_heading;    // Make sure to bound this from -180 to 180, otherwise you will get super large values
	double turn = 0.8 * (-1.0/80.0) * angle_difference;
#ifdef DEBUG_COMMAND
    printf("%f %f %f %f %f %f %f\n", mytimer.Get(), ld, rd, d2r(gyro_heading),d2r(desired_heading),turn+l,r-turn);
#endif
	driveTrain->TankDrive(l+turn,r-turn);
}

// Make this return true when this Command no longer needs to run execute()
bool DrivePath::IsFinished() {
	if(trajectory==NULL)
		return true;
	if(leftFollower.finished && rightFollower.finished)
		return true;
	return false;
}

// Called once after isFinished returns true
void DrivePath::End() {
    std::cout << "DrivePath End" << std::endl;
	mytimer.Stop();
}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void DrivePath::Interrupted() {
    std::cout << "DrivePath Interrupted" << std::endl;
}

void DrivePath::cleanup() {
	if(trajectory)
		delete trajectory;
	if(leftTrajectory)
		delete leftTrajectory;
	if(rightTrajectory)
		delete rightTrajectory;
	trajectory=NULL;
	leftTrajectory=NULL;
	rightTrajectory=NULL;
}
