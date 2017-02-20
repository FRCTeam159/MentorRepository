#ifndef Turn_H
#define Turn_H

#include "../CommandBase.h"

class TurnForTime : public CommandBase {
	double angle=0;
public:
	TurnForTime(double angle);
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
};

#endif  // Turn_H
