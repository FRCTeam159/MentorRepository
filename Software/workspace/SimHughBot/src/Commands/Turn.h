#ifndef Turn_H
#define Turn_H

#include "../CommandBase.h"

class Turn : public CommandBase {
	double angle=0;
public:
	Turn(double angle);
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
};

#endif  // Turn_H
