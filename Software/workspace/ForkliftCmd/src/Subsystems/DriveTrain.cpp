#include "DriveTrain.h"
#include "Commands/DriveWithJoystick.h"
#include <math.h>


#define GAMEPAD
#define MECDRIVE

// TODO
// 1. get mechanum drive to work as expected in simulation (e.g. slide or "strafe" motion)
// 2. add encoders to all 4 wheels
// 3. calculate distance traveled using formula for mechanum motion
// 4. implement "REAL" code using new WPIlib classes (not yet supported in simulation)
DriveTrain::DriveTrain() : Subsystem("DriveTrain") {
	   // Channels for the wheels
#ifdef MECDRIVE
	std::cout << "DriveTrain:Mecanum"<< std::endl;
#else
	std::cout << "DriveTrain:Tank"<< std::endl;
#endif

#ifdef REAL
	frontLeft = new CANTalon(1);
	frontRight = new CANTalon(2);
	backLeft = new CANTalon(3);
	backRight = new CANTalon(4);
#else
	frontLeft = new Talon(1);
	backLeft = new Talon(2);
	frontRight = new Talon(3);
	backRight = new Talon(4);
#endif
	drive = new RobotDrive(frontLeft, backLeft, frontRight, backRight);

	left_encoder = new Encoder(1, 2);
	right_encoder = new Encoder(3, 4);
#ifdef MECDRIVE
	drive->SetExpiration(0.1);
	// unless compensated for in hardware (e.g. reversing the current to the motors on one side),
	// wheels on opposite sides of the robot will spin in opposite directions (for the same polarity current)
	// e.g. on the right side a ccw rotation moves the wheel forward
	// whereas on the left side a cw rotation moves the wheel forward
	drive->SetInvertedMotor(RobotDrive::kFrontLeftMotor, true);	// invert the left side motors
	drive->SetInvertedMotor(RobotDrive::kRearLeftMotor, true);	// you may need to change or remove this to match your robot
#endif
	// Encoders may measure differently in the real world and in
	// simulation. In this example the robot moves 0.042 barleycorns
	// per tick in the real world, but the simulated encoders
	// simulate 360 tick encoders. This if statement allows for the
	// real robot to handle this difference in devices.
#ifdef REAL
	left_encoder->SetDistancePerPulse(0.042);
	right_encoder->SetDistancePerPulse(0.042);
#else
	// Circumference in ft = 4in/12(in/ft)*PI
	left_encoder->SetDistancePerPulse((double) (4.0/12.0*M_PI) / 360.0);
	right_encoder->SetDistancePerPulse((double) (4.0/12.0*M_PI) / 360.0);
#endif

	rangefinder = new AnalogInput(6);
	gyro = new Gyro(1);

	// Let's show everything on the LiveWindow
	// TODO: LiveWindow::GetInstance()->AddActuator("Drive Train", "Front_Left Motor", (Talon) front_left_motor);
	// TODO: LiveWindow::GetInstance()->AddActuator("Drive Train", "Back Left Motor", (Talon) back_left_motor);
	// TODO: LiveWindow::GetInstance()->AddActuator("Drive Train", "Front Right Motor", (Talon) front_right_motor);
	// TODO: LiveWindow::GetInstance()->AddActuator("Drive Train", "Back Right Motor", (Talon) back_right_motor);
	LiveWindow::GetInstance()->AddSensor("Drive Train", "Left Encoder", left_encoder);
	LiveWindow::GetInstance()->AddSensor("Drive Train", "Right Encoder", right_encoder);
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
	SmartDashboard::PutNumber("Left Distance", left_encoder->GetDistance());
	SmartDashboard::PutNumber("Right Distance", right_encoder->GetDistance());
	SmartDashboard::PutNumber("Left Speed", left_encoder->GetRate());
	SmartDashboard::PutNumber("Right Speed", right_encoder->GetRate());
	SmartDashboard::PutNumber("Gyro", gyro->GetAngle());

	std::cout << "Distance Traveled:"<< GetDistance() << std::endl;
	std::cout << "Current Heading:"<< GetHeading() << std::endl;

}

void DriveTrain::Drive(double x, double y, double z, double g) {
#ifdef MECDRIVE
	drive->MecanumDrive_Cartesian(x, y, z, g);
#else
	drive->TankDrive(x, y);
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
	// 3. other
	// - can't seem to get "slide" motion to work in simulation
#ifdef MECDRIVE
	Drive(-joy->GetRawAxis(0), -joy->GetRawAxis(1), -joy->GetRawAxis(4), 0.0);
#else
	Drive(-joy->GetRawAxis(1), -joy->GetRawAxis(4), 0.0, 0.0);
#endif
	//drive->MecanumDrive_Cartesian(joy->GetX(), joy->GetY(), joy->GetZ());
	//drive->ArcadeDrive(joy);
}

double DriveTrain::GetRotation() {
	return gyro->GetAngle();
}

double DriveTrain::GetHeading() {
	return gyro->GetAngle();
}

void DriveTrain::Reset() {
	gyro->Reset();
	left_encoder->Reset();
	right_encoder->Reset();
}

double DriveTrain::GetDistance() {
	return (left_encoder->GetDistance() + right_encoder->GetDistance())/2;
}

double DriveTrain::GetDistanceToObstacle() {
	// Really meters in simulation since it's a rangefinder...
	return rangefinder->GetAverageVoltage();
}
