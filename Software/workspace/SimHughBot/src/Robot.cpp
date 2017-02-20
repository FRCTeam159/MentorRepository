#include <memory>

#include <Commands/Command.h>
#include <Commands/Scheduler.h>
#include <IterativeRobot.h>
#include <LiveWindow/LiveWindow.h>
#include <SmartDashboard/SendableChooser.h>
#include <SmartDashboard/SmartDashboard.h>
#include "CommandBase.h"
#include "Commands/Autonomous.h"

#include <thread>
#include <CameraServer.h>
#include <Commands/TurnForTime.h>
#include <IterativeRobot.h>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/core/core.hpp>
#include <opencv2/core/types.hpp>
#include "Commands/DriveToTarget.h"
#include "Commands/DriveForTime.h"
#include "Commands/DriveStraight.h"
#include "Commands/TurnToAngle.h"



class Robot: public frc::IterativeRobot {

public:
	void RobotInit() override {
		CommandBase::RobotInit();
		frc::SmartDashboard::PutString("AutoMode", "Right");
	}
	/**
	 * This function is called once each time the robot enters Disabled mode.
	 * You can use it to reset any subsystem information you want to clear when
	 * the robot is disabled.
	 */
	void DisabledInit() override {
		CommandBase::DisabledInit();
	}

	void DisabledPeriodic() override {
		frc::Scheduler::GetInstance()->Run();
	}

	/**
	 * This autonomous (along with the chooser code above) shows how to select
	 * between different autonomous modes using the dashboard. The sendable
	 * chooser code works with the Java SmartDashboard. If you prefer the
	 * LabVIEW Dashboard, remove all of the chooser code and uncomment the
	 * GetString code to get the auto name from the text box below the Gyro.
	 *
	 * You can add additional auto modes by adding additional commands to the
	 * chooser code above (like the commented example) or additional comparisons
	 * to the if-else structure below with additional strings & commands.
	 */
#define TURNANGLE 60
#define DRIVEDISTANCE 5.5*12

	void AutonomousInit() override {
		std::string autoSelected = frc::SmartDashboard::GetString("AutoMode", "Right");
		CommandGroup *autonomous=new Autonomous();
		if (autoSelected == "Right") {
			//autonomous->AddSequential(new DriveForTime(4.0,0.45));
			autonomous->AddSequential(new DriveStraight(DRIVEDISTANCE));
			//autonomous->AddSequential(new Turn(-0.27));
			autonomous->AddSequential(new TurnToAngle(-TURNANGLE));

			cout<<"Chose::Right Auto"<<endl;
		}
		else if(autoSelected == "Left"){
			//autonomous->AddSequential(new DriveForTime(4.0,0.45));
			autonomous->AddSequential(new DriveStraight(DRIVEDISTANCE));
			//autonomous->AddSequential(new Turn(0.27));
			autonomous->AddSequential(new TurnToAngle(TURNANGLE));
			cout<<"Chose::Left Auto"<<endl;
		}
		else if(autoSelected == "Center"){
			cout<<"Chose::Center Auto"<<endl;
		}
		else{
			cout<<"Chose::Default Auto"<<endl;
		}
		autonomous->AddSequential(new DriveToTarget());

		autonomousCommand.reset(autonomous);

		CommandBase::AutonomousInit();

		if (autonomousCommand.get() != nullptr) {
			autonomousCommand->Start();
		}
	}

	void AutonomousPeriodic() override {
		frc::Scheduler::GetInstance()->Run();
	}

	void TeleopInit() override {
		// This makes sure that the autonomous stops running when
		// teleop starts running. If you want the autonomous to
		// continue until interrupted by another command, remove
		// this line or comment it out.
		if (autonomousCommand != nullptr) {
			autonomousCommand->Cancel();
		}
		CommandBase::TeleopInit();

	}

	void TeleopPeriodic() override {
		frc::Scheduler::GetInstance()->Run();
	}

	void TestInit(){
	}

	void TestPeriodic() override {
		frc::LiveWindow::GetInstance()->Run();
	}

private:
	std::unique_ptr<frc::Command> autonomousCommand;
	frc::SendableChooser<Command*> chooser;
	std::unique_ptr<frc::Command> disabledCommand;
};

START_ROBOT_CLASS(Robot)
