#include "Subsystems/Claw.h"
#include <iostream>
#include "Robot.h"

Claw::Claw() : Subsystem("Claw") {
#ifdef REAL
	claw = new DoubleSolenoid(0, 0, 1);
#else
    motor = new Victor(7);
#endif
    contact = new DigitalInput(5);
   //clawInput=clawOutput=false;
	// Let's show everything on the LiveWindow
    // TODO: LiveWindow::GetInstance()->AddActuator("Claw", "Motor", (Victor) motor);
    // TODO: contact
}

void Claw::Open()
{
#ifdef REAL
	claw->Set(DoubleSolenoid::kReverse);
#else
	motor->Set(-1);
#endif
	std::cout << "Open Claw" << std::endl;
}

void Claw::Close()
{
#ifdef REAL
	claw->Set(DoubleSolenoid::kForward);
#else
	motor->Set(1);
#endif
	std::cout << "Close Claw" << std::endl;
}


void Claw::Stop() {
#ifndef REAL
	motor->Set(0); // stop driving motor
#else
	Open();
#endif
}

bool Claw::IsGripping() {
	return contact->Get();
}

