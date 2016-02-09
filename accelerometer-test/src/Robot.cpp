// program to test accelerometers

#include "WPILib.h"
#include "AnalogAccelerometer.h"

#define PI 3.14159

class Robot: public IterativeRobot
{
private:
	LiveWindow *lw = LiveWindow::GetInstance();
	SendableChooser *chooser;
	const std::string autoNameDefault = "Default";
	const std::string autoNameCustom = "My Auto";
	std::string autoSelected;

	AnalogAccelerometer *analogX; // instance for analog accelerometer ADXL335
	AnalogAccelerometer *analogY; // instance for analog accelerometer
	AnalogAccelerometer *analogZ; // instance for analog accelerometer
	double analogAccelX;         // variable with smoothed acceleration value
	double analogAccelY;         // variable with smoothed acceleration value
	double analogAccelZ;         // variable with smoothed acceleration value
	double weight = 0.20;
	double initialX = 0.0;  // values measured when the device was 'flat'
	double initialY = 0.0;
	double initialZ = 0.0;
	double accelerationX;
	double accelerationY;
	double accelerationZ;


	Accelerometer *accel;  // ADXL345 digital accelerometer

	void RobotInit()
	{
		chooser = new SendableChooser();
		chooser->AddDefault(autoNameDefault, (void*)&autoNameDefault);
		chooser->AddObject(autoNameCustom, (void*)&autoNameCustom);
		SmartDashboard::PutData("Auto Modes", chooser);
		analogX = new AnalogAccelerometer(0);
		analogX->SetSensitivity(0.305);
		analogX->SetZero(1.5);
		analogY = new AnalogAccelerometer(1);
		analogY->SetSensitivity(0.305);
		analogY->SetZero(1.5);
		analogZ = new AnalogAccelerometer(2);
		analogZ->SetSensitivity(0.305);
		analogZ->SetZero(1.5);
		accel = new ADXL345_I2C(I2C::Port::kOnboard, Accelerometer::Range::kRange_4G);
	}


	/**
	 * This autonomous (along with the chooser code above) shows how to select between different autonomous modes
	 * using the dashboard. The sendable chooser code works with the Java SmartDashboard. If you prefer the LabVIEW
	 * Dashboard, remove all of the chooser code and uncomment the GetString line to get the auto name from the text box
	 * below the Gyro
	 *
	 * You can add additional auto modes by adding additional comparisons to the if-else structure below with additional strings.
	 * If using the SendableChooser make sure to add them to the chooser code above as well.
	 */
	void AutonomousInit()
	{
		autoSelected = *((std::string*)chooser->GetSelected());
		//std::string autoSelected = SmartDashboard::GetString("Auto Selector", autoNameDefault);
		std::cout << "Auto selected: " << autoSelected << std::endl;

		if(autoSelected == autoNameCustom){
			//Custom Auto goes here
		} else {
			//Default Auto goes here
		}
	}

	void AutonomousPeriodic()
	{
		if(autoSelected == autoNameCustom){
			//Custom Auto goes here
		} else {
			//Default Auto goes here
		}
	}

	void TeleopInit()
	{
	}

	void TeleopPeriodic()
	{
		double reading;
		reading = analogX->GetAcceleration() - initialX;
		analogAccelX = (reading * weight) + (analogAccelX * (1.0 - weight));
		reading = analogY->GetAcceleration() - initialY;
		analogAccelY = (reading * weight) + (analogAccelY * (1.0 - weight));
		reading = analogZ->GetAcceleration() - initialZ;
		analogAccelZ = (reading * weight) + (analogAccelZ * (1.0 - weight));
		//printf("analog values: x=%f y=%f z=%f\n", analogAccelX, analogAccelY, analogAccelZ);

		double x, y, z, result, angleX, angleY, angleZ;
		x = analogAccelX * analogAccelX;
		y = analogAccelY * analogAccelY;
		z = analogAccelZ * analogAccelZ;

		// X Axis
		result = analogAccelX / sqrt(y+z);
		angleX = atan(result) * (180.0/PI);

		// Y Axis
		result = analogAccelY / sqrt(x + z);
		angleY = atan(result) * (180.0/PI);

		// Z Axis
		result = analogAccelZ / sqrt(x + y);
		angleZ = atan(result) * (180.0/PI);

		// printf("analog angles: x=%f y=%f z=%f\n", angleX, angleY, angleZ);

		double dAngleX, dAngleY, dAngleZ;

		accelerationX = (accel->GetX() * weight) + (accelerationX * (1.0 - weight));
		accelerationY = (accel->GetY() * weight) + (accelerationY * (1.0 - weight));
		accelerationZ = (accel->GetZ() * weight) + (accelerationZ * (1.0 - weight));
		// printf("digital acceleration x=%f y=%f z=%f\n", accelerationX, accelerationY, accelerationZ);

		x = accelerationX * accelerationX;
		y = accelerationY * accelerationY;
		z = accelerationZ * accelerationZ;

		// X Axis
		result = accelerationX / sqrt(y+z);
		dAngleX = atan(result) * (180.0/PI);

		// Y Axis
		result = accelerationY / sqrt(x + z);
		dAngleY = atan(result) * (180.0/PI);

		// Z Axis
		result = accelerationZ / sqrt(x + y);
		dAngleZ = atan(result) * (180.0/PI);

//		printf("digital angles x=%f, y=%f, z=%f analog x=%f y=%f z=%f\n",
//				dAngleX, dAngleY, dAngleZ, angleX, angleY, angleZ);
		printf("%f, %f, %f, %f, %f, %f\n",
				dAngleX, dAngleY, dAngleZ, angleX, angleY, angleZ);
}

	void TestPeriodic()
	{
		lw->Run();
	}
};

START_ROBOT_CLASS(Robot)
