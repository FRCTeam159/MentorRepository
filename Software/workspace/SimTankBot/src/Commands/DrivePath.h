#ifndef DrivePath_H
#define DrivePath_H

#include "../CommandBase.h"
#include "pathfinder.h"

class DrivePath : public CommandBase {
	int length;
	Segment *trajectory=NULL;
	Segment *leftTrajectory=NULL;
	Segment *rightTrajectory=NULL;
	DistanceFollower leftFollower;
	DistanceFollower rightFollower;
	FollowerConfig config;

	void ModifyTank();
	double FollowDistance(DistanceFollower *follower, Segment *trajectory, double distance);

public:
	DrivePath();
	~DrivePath();
	void Initialize();
	void Execute();
	bool IsFinished();
	void End();
	void Interrupted();
	void cleanup();
};

#endif  // DrivePath_H
