/*
 * Autonomous.cpp
 *
 *  Created on: Mar 3, 2016
 *      Author: alpiner
 */

#include <Commands/Autonomous.h>
#include <Commands/DriveStraight.h>
#include <Commands/ExecLoader.h>
#include <Commands/CloseGate.h>
#include <Commands/OpenGate.h>
#include <Commands/StepShooterAngle.h>
#include <Commands/ShootBall.h>
#include <Commands/Turn.h>
#include <Commands/FullStop.h>
#include "Robot.h"

Autonomous::Autonomous() : CommandGroup("Autonomous") {
	AddSequential(new CloseGate()); // hold the ball
	AddSequential(new OpenGate()); // pinch the ball
	AddSequential(new DriveStraight(7,0)); // go forward
	AddSequential(new Turn(-50)); // turn
	AddSequential(new StepShooterAngle(36)); // set angle
	AddSequential(new ShootBall()); // shoot
	AddSequential(new FullStop()); // end autonomous
}

void Autonomous::Interrupted() {
	std::cout << "Autonomous::Interruped"<<std::endl;
	_End();
}

void  Autonomous::Cancel(){
	std::cout << "Autonomous::Cancel"<<std::endl;
	_End();
	CommandGroup::Cancel();
	Robot::drivetrain->EndTravel();
}
