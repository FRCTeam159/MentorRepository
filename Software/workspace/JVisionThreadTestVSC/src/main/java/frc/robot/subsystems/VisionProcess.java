/*----------------------------------------------------------------------------*/
/* Copyright (c) 2018 FIRST. All Rights Reserved.                             */
/* Open Source Software - may be modified and shared by FRC teams. The code   */
/* must be accompanied by the FIRST BSD license file in the root directory of */
/* the project.                                                               */
/*----------------------------------------------------------------------------*/

package frc.robot.subsystems;
import edu.wpi.cscore.UsbCamera;
import edu.wpi.first.wpilibj.CameraServer;
/**
 * Add your docs here.
 */
public class VisionProcess extends Thread {
  // Put methods for controlling this subsystem
  // here. Call these from Commands.
  static UsbCamera camera;

  public void init() {
    CameraServer server = CameraServer.getInstance();
		camera = server.startAutomaticCapture("Targeting", 0);
  }

  public void run() {
    while (true) {
      try {
        Thread.sleep(1000);
        System.out.println("Vision Thread running");

      } catch (InterruptedException ex) {
        System.out.println("exception)");
      }
    }
  }
}
