#include "PivotWheel.h"

#define PI 3.141516
#define RPD(x) (x)*2*PI/360
#define DPR(x) (x)*180.0/PI

PivotWheel::PivotWheel(int w, int s) : SmartMotor(w){
	//SetAbsoluteTolerance(0.005);
	//GetPIDController()->SetContinuous(false);
	angle=0;
	//syncGroup = 0x80;
	wchnl=w;
	schnl=s;

	pivot = new SmartMotor(s);
    pivot->SetMode(SmartMotor::POSITION);
    SmartMotor::SetMode(SmartMotor::SPEED);


	#ifdef SIMULATION
		// PID is different in simulation.
	    //SetAbsoluteTolerance(5);
	    //GetPIDController()->SetPID(0.05, 0, 0.001);
    	pivot->SetPID(0.05, 0.0, 0.001);
	#endif
    //pivot->SetInputRange(-90.0,90.0);
    //pivot->SetOutputRange(-90.0,90.0);

	// Motor to move the pivot.

	// Sensors for measuring the position of the pivot.
	//upperLimitSwitch = new DigitalInput(13);
	//lowerLimitSwitch = new DigitalInput(12);

	// Put everything to the LiveWindow for testing.
	//LiveWindow::GetInstance()->AddSensor("PivotWheel", "Upper Limit Switch", upperLimitSwitch);
	//LiveWindow::GetInstance()->AddSensor("PivotWheel", "Lower Limit Switch", lowerLimitSwitch);
	// XXX: LiveWindow::GetInstance()->AddSensor("PivotWheel", "Pot", (AnalogPotentiometer) pot);
	// XXX: LiveWindow::GetInstance()->AddActuator("PivotWheel", "Motor", (Victor) motor);
	//LiveWindow::GetInstance()->AddActuator("PivotWheel", "PIDSubsystem Controller", GetPIDController());
}

void InitDefaultCommand() {}

double PivotWheel::ReturnPIDInput() {
	double angle=pivot->GetDistance();
	return angle;
}

void PivotWheel::SetAngle(double value){
	double diff =abs(value-angle);
	if(diff > 2){
		angle=value;
		pivot->SetSetpoint(value);
	}
}
void PivotWheel::SetMotorSpeed(double value){
	SmartMotor::SetVelocity(value);
}

void PivotWheel::UsePIDOutput(double output) {
	pivot->PIDWrite(output);
}

void PivotWheel::Disable(){
	SmartMotor::Disable();
	pivot->Disable();
}

void PivotWheel::Enable(){
	SmartMotor::Enable();
	pivot->Enable();
}
void PivotWheel::Reset(){
	SetAngle(0);
	SmartMotor::Reset();
}

