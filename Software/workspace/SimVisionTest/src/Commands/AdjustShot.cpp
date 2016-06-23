/*
 * AdjustShot.cpp
 *
 *  Created on: Jun 20, 2016
 *      Author: dean
 */

#include <Commands/AdjustShot.h>
#include <Robot.h>


#define VP 0.05
#define VI 0.0
#define VD 0.0

#define HP 0.05
#define HI 0.0001
#define HD 0.2

#define ADJUST_TIMEOUT 2
#define VPIDUPDATERATE 0.03
#define HPIDUPDATERATE 0.01
#define MAX_ANGLE_ERROR 1

#define FWSPEED 50

//#define DEBUG_COMMAND

AdjustShot::AdjustShot() : Command("AdjustShot"),
    Hcntrl(HP,HI,HD),Vcntrl(VP,VI,VD)
{
    Requires(Robot::vision.get());
    std::cout << "new AdjustShot"<< std::endl;
}

void AdjustShot::Initialize() {
    ntargets=Robot::vision->GetTargets();
    if(ntargets>0){
        SetTimeout(ADJUST_TIMEOUT);
        std::cout << "AdjustShot Started .."<<std::endl;
        Robot::shooter->SetTargetSpeed(FWSPEED);


        Hcntrl.Initialize();
        Vcntrl.Initialize();
        //Robot::vision.get()->PrintRawTargetData();
        //double d=Robot::vision.get()->GetTargetDistance();
    }
}
void AdjustShot::Execute() {
#ifdef DEBUG_COMMAND
    std::cout << "Updating targets"<< std::endl;
#endif
    ntargets=Robot::vision->GetTargets();
}

bool AdjustShot::IsFinished() {
    if(ntargets==0){
        std::cout << "AdjustShot:no targets found"<< std::endl;
        return true;
    }
    if(IsTimedOut()){
        std::cout << "AdjustShot Error:  Timeout expired"<<std::endl;
        return true;
    }
    bool Vok=Vcntrl.AtTarget();
    bool Hok=Hcntrl.AtTarget();
    return(Vok && Hok);
}

void AdjustShot::End() {
    Hcntrl.End();
    Vcntrl.End();
    std::cout << TimeSinceInitialized()<< "  End AdjustShot"<<std::endl;
}

// =============== Inner Class AdjustVAngle =============================

AdjustShot::AdjustVAngle::AdjustVAngle(double P, double I, double D):
   pid(P,I,D,this,this,VPIDUPDATERATE)
{
}

double AdjustShot::AdjustVAngle::PIDGet() {
    return Robot::vision->GetTargetVerticalAngle();
}

// - Since shooter->SetTargetAngle is itself controlled by a PID loop it needs
//   to run faster than this outer adjustment loop in order to avoid oscillation
// - It might be better to disable the shooter angle pid while this command is running
void AdjustShot::AdjustVAngle::PIDWrite(float d) {
    double current=Robot::shooter->GetTargetAngle();
    double target=current-d;
    Robot::shooter->SetTargetAngle(target);
#ifdef DEBUG_COMMAND
    double err=Robot::vision->GetTargetVerticalAngle();
   std::cout << "Changing Shooter Angle - old:"<< current <<" new:"<<target<<" err:"<<err<<std::endl;
#endif
}

bool AdjustShot::AdjustVAngle::AtTarget() {
    return pid.OnTarget();
    //return true;
}

void AdjustShot::AdjustVAngle::Initialize() {
    pid.Reset();
    pid.SetAbsoluteTolerance(MAX_ANGLE_ERROR);
    pid.SetSetpoint(0);
    pid.Enable();
}

void AdjustShot::AdjustVAngle::End() {
    pid.Disable();
}

// =============== Inner Class AdjustHAngle =============================

AdjustShot::AdjustHAngle::AdjustHAngle(double P, double I, double D) :
   pid(P,I,D,this,this,HPIDUPDATERATE)
{
}

double AdjustShot::AdjustHAngle::PIDGet() {
    return Robot::vision->GetTargetHorizontalAngle();
}

void AdjustShot::AdjustHAngle::PIDWrite(float d) {
#ifdef DEBUG_COMMAND
    double err=Robot::vision->GetTargetHorizontalAngle();
    std::cout << "Changing heading error:"<<err<<" correction:"<<d<<std::endl;
#endif
    Robot::drivetrain->Turn(-d);
}

bool AdjustShot::AdjustHAngle::AtTarget() {
     return pid.OnTarget();
    //return true;
}

void AdjustShot::AdjustHAngle::Initialize() {
    pid.Reset();
    pid.SetAbsoluteTolerance(MAX_ANGLE_ERROR);
    pid.SetSetpoint(0.0);
    pid.Enable();
}

void AdjustShot::AdjustHAngle::End() {
    pid.Disable();
    Robot::drivetrain->EndTravel();
}
