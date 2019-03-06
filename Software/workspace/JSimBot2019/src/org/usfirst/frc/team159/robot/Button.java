/*----------------------------------------------------------------------------*/
/* Copyright (c) 2018 FIRST. All Rights Reserved.                             */
/* Open Source Software - may be modified and shared by FRC teams. The code   */
/* must be accompanied by the FIRST BSD license file in the root directory of */
/* the project.                                                               */
/*----------------------------------------------------------------------------*/

package org.usfirst.frc.team159.robot;

import org.usfirst.frc.team159.robot.OI;
/**
 * Add your docs here.
 */
public class Button {
    int id=0;
    boolean released = true;
    public Button(int n){
        id=n;
    }
    public boolean isPressed(){
        if(released && OI.stick.getRawButton(id)){
            released=false;
            return true;
        }
        else if(!OI.stick.getRawButton(id)){
            released = true;
        }
        return false;
    }
    public int getID(){
        return id;
    }
}
