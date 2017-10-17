#include "CommandBase.h"

#include <Commands/Scheduler.h>

shared_ptr<DriveTrain> CommandBase::driveTrain;
std::unique_ptr<OI> CommandBase::oi = std::make_unique<OI>();

CommandBase::CommandBase(const std::string &name) :
		frc::Command(name) {

}

void CommandBase::RobotInit(){
	driveTrain.reset(new DriveTrain());
	oi.reset(new OI());
}

void CommandBase::TeleopInit(){
	driveTrain->Reset();
}

void CommandBase::AutonomousInit(){
	driveTrain->Reset();
}
