#ifndef COMMAND_BASE_H
#define COMMAND_BASE_H

#include <string>
#include "Commands/Command.h"
#include "OI.h"
#include "WPILib.h"
#include "Subsystems/DriveTrain.h"
#include "Subsystems/Elevator.h"
#include "Subsystems/Shooter.h"

/**
 * The base for all commands. All atomic commands should subclass CommandBase.
 * CommandBase stores creates and stores each control system. To access a
 * subsystem elsewhere in your code in your code use CommandBase.examplesubsystem
 */

//#define DEBUG_TOGGLE
class CommandBase: public Command
{
protected:
	// utility class to toggle the state of a button
	class Toggle {
	private:
		bool last_input=false;
		bool output=false;
	public:
		bool toggle(bool input){
			if (input && !last_input) { // button press off->on
				output=!output;         // switch output state
#ifdef DEBUG_TOGGLE
				std::cout<<"new button state="<<output<<std::endl;
#endif
			}
			last_input=input;
			return output;
		}
		void reset(){
			last_input=output=false;
		}
	};
public:
	CommandBase(const std::string &name);
	CommandBase();
	static void init();
	// Create a single static instance of all of your subsystems
	static std::unique_ptr<OI> oi;
	static std::shared_ptr<DriveTrain> drivetrain;
	static std::shared_ptr<Elevator> elevator;
	static std::shared_ptr<Shooter> shooter;

};

#endif
