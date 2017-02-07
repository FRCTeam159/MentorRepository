#include "CommandBase.h"
#include "Subsystems/DriveTrain.h"
#include <Commands/Scheduler.h>


// Initialize a single static instance of all of your subsystems. The following
// line should be repeated for each subsystem in the project.
shared_ptr<DriveTrain> CommandBase::driveTrain;
shared_ptr<GearSubsystem> CommandBase::gearSubsystem;
shared_ptr<Vision> CommandBase::visionSubsystem;
shared_ptr<UltrasonicSubsystem>CommandBase::ultrasonicSubsystem;

unique_ptr<OI> CommandBase::oi = std::make_unique<OI>();

CommandBase::CommandBase(const std::string &name) :
		frc::Command(name) {
}

void CommandBase::RobotInit(){
	// Create a single static instance of all of your subsystems. The following
	// line should be repeated for each subsystem in the project.
	visionSubsystem.reset(new Vision());
	visionSubsystem->Init();
	driveTrain.reset(new DriveTrain());
	gearSubsystem.reset(new GearSubsystem());
	ultrasonicSubsystem.reset(new UltrasonicSubsystem());
	ultrasonicSubsystem->Init();


	oi.reset(new OI());
}

void CommandBase::AutonomousInit() {
	driveTrain->Enable();
	ultrasonicSubsystem->Enable();
}

void CommandBase::TeleopInit() {
	driveTrain->Enable();
	ultrasonicSubsystem->Enable();
}

void CommandBase::DisabledInit() {
	driveTrain->Disable();
	ultrasonicSubsystem->Disable();
	gearSubsystem->Close();

}
