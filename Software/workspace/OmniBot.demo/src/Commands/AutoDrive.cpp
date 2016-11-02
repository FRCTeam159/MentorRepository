/*
 * AutoDrive.cpp
 *
 *  Created on: Oct 30, 2016
 *      Author: dean
 */

#include <Commands/AutoDrive.h>


AutoDrive::AutoDrive(double d, double s, double t) {
	Requires(drivetrain.get());
	direction=d;
	speed=s;
	duration=t;
	stop_time=t;
	start_time=0;
}

void AutoDrive::Initialize() {
	start_time=Timer::GetFPGATimestamp();
	stop_time=start_time+duration;
	std::cout << "AutoDrive::Started"<<std::endl;
}

void AutoDrive::Execute() {
	double n=direction;
	double s=-direction;
	double e=1-direction;
	double w=direction-1;
	drivetrain->Drive(n*speed,s*speed,e*speed,w*speed);
}

bool AutoDrive::IsFinished() {
	if(Timer::GetFPGATimestamp()>=stop_time)
		return true;
	return false;
}

void AutoDrive::End() {
	std::cout << "AutoDrive::Finished:"<<Timer::GetFPGATimestamp()-start_time<<std::endl;
}

void AutoDrive::Interrupted() {
	End();
}
