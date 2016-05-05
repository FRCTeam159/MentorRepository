/*
 * SimContinuousOutput.cpp
 *
 *  Created on: May 28, 2014
 *      Author: alex
 */

#include "simulation/SimContinuousOutput.h"
#include "simulation/MainNode.h"

SimContinuousOutput::SimContinuousOutput(std::string topic) {
    pub = MainNode::Advertise<msgs::Float64>("~/simulator/"+topic);
	std::cout << "Initialized ~/simulator/"+topic << std::endl;
}

void SimContinuousOutput::Set(float _speed) {
	speed=_speed;
	msgs::Float64 msg;
	msg.set_data(_speed);
	pub->Publish(msg);
}

float SimContinuousOutput::Get() {
	return speed;
}
