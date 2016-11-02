#include "CommandBase.h"
#include "Subsystems/DriveTrain.h"
#include "Subsystems/Shooter.h"
#include "Commands/Scheduler.h"

// Initialize a single static instance of all of your subsystems to NULL
std::unique_ptr<OI> CommandBase::oi;
std::shared_ptr<DriveTrain> CommandBase::drivetrain;
std::shared_ptr<Elevator> CommandBase::elevator;
std::shared_ptr<Shooter> CommandBase::shooter;

CommandBase::CommandBase(const std::string &name) :
		Command(name)
{
}

CommandBase::CommandBase() :
		Command()
{

}

void CommandBase::init()
{
	// Create a single static instance of all of your subsystems. The following
	// line should be repeated for each subsystem in the project.
	oi.reset(new OI());
	drivetrain.reset(new DriveTrain());
	elevator.reset(new Elevator());
	shooter.reset(new Shooter());

}

