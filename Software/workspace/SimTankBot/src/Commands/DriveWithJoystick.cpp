#include "DriveWithJoystick.h"
#include "Subsystems/DriveTrain.h"
#include "RobotMap.h"

//#define DEBUG_COMMAND
#define I2M(x) x*0.0254

static Timer mytimer;

//#define APPLY_DEADBAND
#define SQUARE_INPUTS  false

DriveWithJoystick::DriveWithJoystick()
{
	// Use Requires() here to declare subsystem dependencies
	Requires(driveTrain.get());
}

// Called just before this Command runs the first time
void DriveWithJoystick::Initialize()
{
	std::cout << "DriveWithJoystick::Initialize()" << std::endl;
	driveTrain->SetHighGear();
	mytimer.Start();
	mytimer.Reset();

}

// Called repeatedly when this Command is scheduled to run
void DriveWithJoystick::Execute()
#define MINTHRESHOLD 0.3
#define MINOUTPUT 0.0
{
	// Get axis values
	Joystick *stick = oi->GetJoystick();

	if (stick->GetRawButton(LOWGEAR_BUTTON)){
		driveTrain->SetLowGear();
	}
	else if(stick->GetRawButton(HIGHGEAR_BUTTON)){
		driveTrain->SetHighGear();
	}
	float xAxis =0;
	float yAxis =0;
	float zAxis = 0;

#if DRIVETYPE == ARCADE2
	yAxis=-stick->GetRawAxis(1); // left stick - drive
	xAxis=-stick->GetRawAxis(3); // right stick - rotate
#elif DRIVETYPE == TANK
	yAxis=-stick->GetRawAxis(4); // left stick - drive
	xAxis=-stick->GetRawAxis(1); // right stick - drive
#else // 3-axis Joystick
	float yAxis = stick-> GetY();
	float xAxis = stick-> GetX();
	float zAxis = stick-> GetZ();
#endif

#ifdef APPLY_DEADBAND
	// Run axis values through deadband
	yAxis = quadDeadband(MINTHRESHOLD, MINOUTPUT, yAxis);
	xAxis = quadDeadband(MINTHRESHOLD, MINOUTPUT, xAxis);
	zAxis = quadDeadband(MINTHRESHOLD, MINOUTPUT, zAxis);
#endif
#ifdef SIMULATION // simulate high/low gearbox switch
	if(driveTrain->InLowGear()){
		xAxis*=0.5;
		yAxis*=0.5;
	}
#endif
#if DRIVETYPE == ARCADE3
	driveTrain->CustomArcade(xAxis, yAxis, zAxis,SQUARE_INPUTS);
#elif DRIVETYPE == ARCADE2
	driveTrain->ArcadeDrive(xAxis, yAxis, SQUARE_INPUTS);
#else // TANK
	driveTrain->TankDrive(xAxis, yAxis);
#endif
#ifdef DEBUG_COMMAND
    printf("%f %f %f %f %f\n",mytimer.Get(),I2M(driveTrain->GetDistance()),  I2M(driveTrain->GetVelocity()),xAxis,yAxis);
#endif

}

// Make this return true when this Command no longer needs to run execute()
bool DriveWithJoystick::IsFinished()
{
	return false;
}

// Called once after isFinished returns true
void DriveWithJoystick::End()
{
	std::cout << "DriveWithJoystick Finished" << std::endl;
	mytimer.Stop();

}

// Called when another command which requires one or more of the same
// subsystems is scheduled to run
void DriveWithJoystick::Interrupted()
{
	End();
}
float DriveWithJoystick::quadDeadband(float minThreshold, float minOutput, float input)
{
	if (input > minThreshold) {
		return ((((1 - minOutput)
				/ ((1 - minThreshold)* (1 - minThreshold)))
				* ((input - minThreshold)* (input - minThreshold)))
				+ minOutput);
	} else {
		if (input < (-1 * minThreshold)) {
			return (((minOutput - 1)
					/ ((minThreshold - 1)* (minThreshold - 1)))
					* ((minThreshold + input)* (minThreshold + input)))
					- minOutput;
		}

		else {
			return 0;
		}
	}
}
