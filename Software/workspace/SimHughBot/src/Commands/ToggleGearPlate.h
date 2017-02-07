#ifndef ToggleGearPlate_H
#define ToggleGearPlate_H

#include "../CommandBase.h"

class ToggleGearPlate : public CommandBase {
	bool isOpen=false;
public:
	ToggleGearPlate();
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
};

#endif  // ToggleGearPlate_H
