/*
 * Vision.h
 *
 *  Created on: Jun 20, 2016
 *      Author: dean
 */

#ifndef SRC_SUBSYSTEMS_VISION_H_
#define SRC_SUBSYSTEMS_VISION_H_

#include "WPILib.h"
#include <Commands/Subsystem.h>

#define MAX_TARGETS 4
struct TargetInfo {
    double width;
    double height;
};
struct TargetData {
    double centerX;
    double centerY;
    double width;
    double height;
    double area;
};
struct CameraInfo {
    int screen_width;
    int screen_height;
    double fov;
    double fov_factor;
};

class Vision: public Subsystem {
protected:

    CameraInfo camera;
    TargetData targets[MAX_TARGETS];
    TargetInfo target;
    TargetData closest;
    int ntargets=0;
    double camera_hoffset;
    double camera_voffset;
    void ClearTargets();
    bool CheckArrayError(int n);
    double calcTargetHOffset(double range);
    double calcTargetVOffset(double range);
public:
    std::shared_ptr<NetworkTable> table;

    Vision();
    virtual ~Vision();
    void Init();
    void InitDefaultCommand();
    void Update();
    int GetTargets();
    bool FindClosestTarget();

    void SetTargetSpecs(double w, double h);
    void SetCameraSpecs(int w, int h, double f);
    void SetCameraOffsets(double h, double v);
    double GetTargetDistance();
    double GetTargetVerticalAngle();
    double GetTargetHorizontalAngle();
    void PrintRawTargetData();
    void PrintTargetOffsets();
    CameraInfo GetCameraInfo() { return camera;}
    TargetInfo GetTargetInfo() { return target;}
};

#endif /* SRC_SUBSYSTEMS_VISION_H_ */
