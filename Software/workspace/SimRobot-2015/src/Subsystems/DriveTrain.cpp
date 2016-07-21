#include "DriveTrain.h"

#include "Commands/DriveWithJoystick.h"
#include <math.h>

#define GAMEPAD
// TODO
// 1. get mechanum drive to work as expected in simulation (e.g. slide or "strafe" motion)
// 2. add encoders to all 4 wheels
// 3. calculate distance traveled using formula for mechanum motion
// 4. implement "REAL" code using new WPIlib classes (not yet supported in simulation)
DriveTrain::DriveTrain(int i) : Subsystem("DriveTrain") {
	   // Channels for the wheels
#if DRIVETYPE == MECHANUM
	std::cout << "DriveTrain:Mecanum"<< std::endl;
#elif DRIVETYPE == TANK
	std::cout << "DriveTrain:Tank"<< std::endl;
#elif DRIVETYPE == SWERVE
	std::cout << "DriveTrain:Swerve"<< std::endl;
#elif DRIVETYPE == ARCADE
    std::cout << "DriveTrain:Arcade"<< std::endl;
#endif
#if DRIVETYPE == SWERVE
    int p=i+4;
    frontLeft = new PivotWheel(i++,p++);
    backLeft = new PivotWheel(i++,p++);
    frontRight = new PivotWheel(i++,p++);
    backRight = new PivotWheel(i++,p++);
    drive = new SwerveDrive(frontLeft, backLeft, frontRight, backRight);
#else
	frontLeft = new SmartMotor(i++);
	backLeft = new SmartMotor(i++);
	frontRight = new SmartMotor(i++);
	backRight = new SmartMotor(i++);
	drive = new RobotDrive(frontLeft, backLeft, frontRight, backRight);
#endif
	drive->SetExpiration(0.1);

//#if DRIVETYPE ==  MECHANUM
	// unless compensated for in hardware (e.g. reversing the current to the motors on one side),
	// wheels on opposite sides of the robot will spin in opposite directions (for the same polarity current)
	// e.g. on the right side a ccw rotation moves the wheel forward
	// whereas on the left side a cw rotation moves the wheel forward
	drive->SetInvertedMotor(RobotDrive::kFrontLeftMotor, true);	// invert the left side motors
	drive->SetInvertedMotor(RobotDrive::kRearLeftMotor, true);	// you may need to change or remove this to match your robot
//#endif
	// Encoders may measure differently in the real world and in
	// simulation. In this example the robot moves 0.042 barleycorns
	// per tick in the real world, but the simulated encoders
	// simulate 360 tick encoders. This if statement allows for the
	// real robot to handle this difference in devices.
#ifdef REAL
	//frontLeft_encoder->SetDistancePerPulse(0.042);
	//frontRight_encoder->SetDistancePerPulse(0.042);
#else
	// Circumference in ft = 6in/12(in/ft)*PI
	//frontLeft_encoder->SetDistancePerPulse((double) (6.0/12.0*M_PI) / 360.0);
	//frontRight_encoder->SetDistancePerPulse((double) (6.0/12.0*M_PI) / 360.0);
#endif
	SetFtPerTick(6.0,360.0);
	rangefinder = new AnalogInput(6);
	gyro = new AnalogGyro(1);
	SetDeadband(0.0,0.0,0.0);
	SetSquaredInputs(false);
	SetFieldCentric(false);

	// Let's show everything on the LiveWindow
	// TODO: LiveWindow::GetInstance()->AddActuator("Drive Train", "Front_Left Motor", (Talon) front_left_motor);
	// TODO: LiveWindow::GetInstance()->AddActuator("Drive Train", "Back Left Motor", (Talon) back_left_motor);
	// TODO: LiveWindow::GetInstance()->AddActuator("Drive Train", "Front Right Motor", (Talon) front_right_motor);
	// TODO: LiveWindow::GetInstance()->AddActuator("Drive Train", "Back Right Motor", (Talon) back_right_motor);
	//LiveWindow::GetInstance()->AddSensor("Drive Train", "Left Encoder", frontLeft_encoder);
	//LiveWindow::GetInstance()->AddSensor("Drive Train", "Right Encoder", frontRight_encoder);
	LiveWindow::GetInstance()->AddSensor("Drive Train", "Rangefinder", rangefinder);
	LiveWindow::GetInstance()->AddSensor("Drive Train", "Gyro", gyro);
}

/**
 * When no other command is running let the operator drive around
 * using the PS3 joystick.
 */
void DriveTrain::InitDefaultCommand() {
	SetDefaultCommand(new DriveWithJoystick());
}

/**
 * The log method puts interesting information to the SmartDashboard.
 */
void DriveTrain::Log() {
	//SmartDashboard::PutNumber("Left Distance", frontLeft_encoder->GetDistance());
	//SmartDashboard::PutNumber("Right Distance", frontRight_encoder->GetDistance());
	//SmartDashboard::PutNumber("Left Speed", frontLeft_encoder->GetRate());
	//SmartDashboard::PutNumber("Right Speed", frontRight_encoder->GetRate());
	SmartDashboard::PutNumber("Gyro", gyro->GetAngle());
	SmartDashboard::PutNumber("Distance", GetTotalDistance());

	std::cout << "Distance Traveled:"<< GetTotalDistance() << " ft"<<std::endl;
	std::cout << "Current Heading:"<< GetHeading() << std::endl;
}

void DriveTrain::Drive(double x, double y, double z, double g) {
#if DRIVETYPE == MECHANUM
	drive->MecanumDrive_Cartesian(x, y, z, g);
	//drive->MecanumDrive_Polar(x, y, 0);
#elif DRIVETYPE == TANK
	drive->TankDrive(x, y,squared_inputs);
#elif DRIVETYPE == SWERVE
	drive->PivotDrive(x, y, z, g);
#else
	drive->ArcadeDrive(x, y,squared_inputs); // right stick=speed right stick = rotation
#endif
}

void DriveTrain::Drive(Joystick* joy) {
    // Mecanum drive notes
	// 1. wheel directions
	// - forward/backward: all wheels turn in the same direction and speed
	// - (tank)turn left: right wheels turn forward, left wheels turn backward (rotates robot left)
	// - (tank)turn right: right wheels turn backward, left wheels turn forward
	// - (slide)left: front left wheel turns backwards, back left wheel turns forward (towards center)
	//               front right wheel turns forward, back right wheel turns backwards (away from center)
	// - (slide)right: front right wheel turns backwards, back back wheel turns forward (towards center)
	//               front left wheel turns forward, back left wheel turns backwards (away from center)
	// 2. gyro
	// - passing gyro heading to MecanumDrive_Cartesian changes reference frame from "local"(robot) to "global"(arena)
	// - probably only want gyro correction in autonomous mode ?
	// 3. simulation
	// - not supported in simulation using standard wheels
	// - but can emulate by adding 4 spherical wheels (collision only) with joints at 45 degrees in URDF file
	double x=0,y=0,z=0,g=0;
#if DRIVETYPE == MECHANUM
	x=-joy->GetRawAxis(0);y=-joy->GetRawAxis(1);z=-joy->GetRawAxis(4);
#elif DRIVETYPE == SWERVE
	x=-joy->GetRawAxis(1);y=-joy->GetRawAxis(0);z=joy->GetRawAxis(4);
#else
	x=-joy->GetRawAxis(1);y=-joy->GetRawAxis(4);z=0;
#endif
	if(squared_inputs){
		x=SquareValue(x);
		y=SquareValue(y);
		z=SquareValue(z);
	}
	x=Deadband(x,x_deadband);
	y=Deadband(y,y_deadband);
	z=Deadband(z,z_deadband);
	if(field_centric)
		g=GetRotation();
	//std::cout<< "DriveTrain::Drive x:"<<x<<" y:"<<y<<" z:"<<z<<std::endl;
	Drive(x, y, z, g);
}

void DriveTrain::SetFtPerTick(double wheel_diam, double encoder_ticks){
	ftpertick=wheel_diam*M_PI/encoder_ticks/12.0;
}
double DriveTrain::GetRotation() {
	return gyro->GetAngle();
}

double DriveTrain::GetHeading() {
	return gyro->GetAngle();
}

void DriveTrain::ResetWheels() {
	distance_traveled+=GetDistance();
	frontLeft->Reset();
	frontRight->Reset();
	backLeft->Reset();
	backRight->Reset();
}

double DriveTrain::GetDistance() {
	double distance=0;
	distance+=frontLeft->GetDistance();
	distance+=frontRight->GetDistance();
	distance+=backLeft->GetDistance();
	distance+=backRight->GetDistance();
	return -ftpertick*distance/4;
}

double DriveTrain::GetDistanceToObstacle() {
	// Really meters in simulation since it's a rangefinder...
	return rangefinder->GetAverageVoltage();
}

void DriveTrain::Enable(){
	frontLeft->Enable();
	backLeft->Enable();
	frontRight->Enable();
	backRight->Enable();
}
void DriveTrain::Disable(){
	frontLeft->Disable();
	backLeft->Disable();
	frontRight->Disable();
	backRight->Disable();
	//Stop();
}

void DriveTrain::Rotate(double speed){
#if DRIVETYPE == SWERVE
	drive->Rotate(speed);
#elif DRIVETYPE == MECHANUM
	Drive(0,0,-0.2*speed,0);
#endif
}

void DriveTrain::ResetGyro() {
	gyro->Reset();
}

double DriveTrain::GetTotalDistance(){
	return distance_traveled;
}
void DriveTrain::ResetTotalDistance(){
	distance_traveled=0;
}
void DriveTrain::ResetAll(){
	ResetTotalDistance();
	ResetGyro();
	ResetWheels();
	Stop();
}
void DriveTrain::Stop(){
	Drive(0, 0, 0, 0);
	std::cout<< "DriveTrain::Stop:"<<std::endl;
}
void DriveTrain::DriveStraight(double d){
#if DRIVETYPE == SWERVE
	Drive(d, 0, 0, 0);
#elif DRIVETYPE == MECHANUM
	Drive(0, d, 0, 0);
#endif

}
