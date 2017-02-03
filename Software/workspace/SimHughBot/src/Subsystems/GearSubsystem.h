#ifndef GearSubsystem_H
#define GearSubsystem_H

#include <Commands/Subsystem.h>

class GearSubsystem : public Subsystem {
private:
	// It's desirable that everything possible under private except
	// for methods that implement subsystem capabilities

public:
	GearSubsystem();
	void InitDefaultCommand();
};

#endif  // GearSubsystem_H
