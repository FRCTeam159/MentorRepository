/*----------------------------------------------------------------------------*/
/* Copyright (c) 2018 FIRST. All Rights Reserved.                             */
/* Open Source Software - may be modified and shared by FRC teams. The code   */
/* must be accompanied by the FIRST BSD license file in the root directory of */
/* the project.                                                               */
/*----------------------------------------------------------------------------*/

package org.usfirst.frc.team159.robot.subsystems;

//import edu.wpi.cscore.UsbCamera;
import edu.wpi.first.wpilibj.CameraServer;
import edu.wpi.first.wpilibj.SerialPort;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;
import edu.wpi.first.wpilibj.networktables.NetworkTable;
//import edu.wpi.first.networktables.NetworkTableInstance;
//import edu.wpi.first.networktables.NetworkTable;
//import edu.wpi.first.networktables.NetworkTableEntry;

import edu.wpi.cscore.CvSource;
import edu.wpi.cscore.CvSink;
import org.opencv.core.Mat;
import org.opencv.core.MatOfPoint;
import org.opencv.core.MatOfPoint2f;

import org.opencv.core.Rect;
import org.opencv.core.RotatedRect;

import org.opencv.core.Point;
import org.opencv.core.Scalar;
import org.opencv.imgproc.Imgproc;
import org.opencv.videoio.VideoCapture;
import org.opencv.core.Core;

import java.util.ArrayList;
import java.util.Timer;

import org.usfirst.frc.team159.robot.subsystems.GripPipeline;

import edu.wpi.first.wpilibj.AnalogInput;
import edu.wpi.first.wpilibj.DigitalOutput;

/**
 * Add your docs here.
 */
public class VisionProcess extends Thread {
  static {
		System.loadLibrary(Core.NATIVE_LIBRARY_NAME);
	}
  // static UsbCamera camera;
  public static double targetWidth = 13.0; // physical width of target (inches)
  public static double targetHeight = 8.0; // physical height of target (inches)
  public static double distanceToFillWidth = 16.5; // measured distance where target just fills screen (width)
  public static double distanceToFillHeight = 13.5; // measured distance where target just fills screen (height)
  public static double cameraFovW = 2 * Math.atan(0.5 * targetWidth / distanceToFillWidth); // 41.8 degrees
  public static double cameraFovH = 2 * Math.atan(0.5 * targetHeight / distanceToFillHeight); // 33.0 degrees

  public static double imageWidth = 320;
  public static double imageHeight = 240;
  // multiply these factors by target screen projection (pixels) to get distance
  double distanceFactorWidth = 0.5 * targetWidth * imageWidth / Math.tan(cameraFovW / 2.0);
  double distanceFactorHeight = 0.5 * targetHeight * imageHeight / Math.tan(cameraFovH / 2.0);
  // multiply these factors by target center offset (pixels) to get horizontal and
  // vertical angle offsets
  double angleFactorWidth = Math.toDegrees(cameraFovW) / imageWidth;
  double angleFactorHeight = Math.toDegrees(cameraFovH) / imageHeight;
  // expected width/height ratio

  AnalogInput rangefinder = new AnalogInput(1);
  double targetAspectRatio = targetWidth / targetHeight;
  // double range_inches_per_count = 1.0 / 19.744;
  // double range_inches_per_count = 1.0;
  double min_range = 35;
  double max_range = 70;
  double range = 0;
  double targetAngle = 0;
  double targetOffset = 0;
  VideoCapture vcap;

  public double getRange() {
    double meters = rangefinder.getVoltage();
    return 39.3701 * meters;
    // double range = range_inches_per_count * rangefinder.getValue();
    // return range;
  }

  public void init() {
    SmartDashboard.putNumber("Targets", 0);
    SmartDashboard.putNumber("H offset", 0);
    SmartDashboard.putNumber("Target Tilt", 0);
    SmartDashboard.putNumber("Range", 0);
    SmartDashboard.putBoolean("Show HSV", false);
    System.out.println("fov H:" + Math.toDegrees(cameraFovH) + " W:" + Math.toDegrees(cameraFovW));
  }

  double round10(double x) {
    return Math.round(x * 10 + 0.5) / 10.0;
  }

  public void run() {
    String videoStreamAddress = "http://localhost:5002/?action=stream";
    vcap = new VideoCapture();
    if (!vcap.open(videoStreamAddress))
      System.out.println("Error opening video stream " + videoStreamAddress);
    else
      System.out.println("Video Stream captured " + videoStreamAddress);

    // camera = CameraServer.getInstance().startAutomaticCapture("Targeting", 0);
    GripPipeline grip = new GripPipeline();
    // CvSink cvSink = CameraServer.getInstance().getVideo();
    CvSource outputStream = CameraServer.getInstance().putVideo("Rectangle", 320, 240);
    Mat mat = new Mat();
    ArrayList<RotatedRect> rects = new ArrayList<RotatedRect>();
    // TODO: use a network tables data structure to pass target params to Robot
    // Program
   // NetworkTableInstance inst = NetworkTableInstance.getDefault();
   // NetworkTable table = inst.getTable("TargetData");

    NetworkTable table = NetworkTable.getTable("TargetData");
    edu.wpi.first.wpilibj.Timer timer = new edu.wpi.first.wpilibj.Timer();
    timer.start();

    while (true) {
      try {
        Thread.sleep(20);
      } catch (InterruptedException ex) {
        System.out.println("exception)");
      }
      timer.reset();
      if (!vcap.read(mat)) {
        continue;
      }
      double dt = timer.get() * 1000;
      grip.process(mat);

      Boolean show_hsv = SmartDashboard.getBoolean("Show HSV", false);
      if (show_hsv) {
        Mat hsv = grip.hsvThresholdOutput(); // display HSV image
        hsv.copyTo(mat);
      }
      ArrayList<MatOfPoint> contours = grip.filterContoursOutput();
      rects.clear();
      double max_area = 0;
      RotatedRect biggest = null;
      // find the bounding boxes of all targets
      // public static RotatedRect minAreaRect(MatOfPoint2f points)
      // Imgproc.boxPoints(RotatedRect box, Mat points)
      for (int i = 0; i < contours.size(); i++) {
        MatOfPoint contour = contours.get(i);
        double area = Imgproc.contourArea(contour);
        MatOfPoint2f NewMtx = new MatOfPoint2f(contour.toArray());
        RotatedRect r = Imgproc.minAreaRect(NewMtx);
        if (area > max_area) {
          biggest = r;
          max_area = area;
        }
        rects.add(r);
      }

      // calculate distance to target
      // - using ht
      SmartDashboard.putNumber("Targets", rects.size());
      if (biggest != null) {
        double h = biggest.size.height;
        double w = biggest.size.width;
        targetAngle = biggest.angle;

        if (biggest.size.width > biggest.size.height) {
          h = biggest.size.width;
          w = biggest.size.height;
          targetAngle += 90.0;
        }
        Point ctr = biggest.center;
        targetOffset = angleFactorWidth * (ctr.x - 0.5 * imageWidth);
        SmartDashboard.putNumber("H offset", round10(targetOffset));
        SmartDashboard.putNumber("Target Tilt", round10(targetAngle));
      }
      double range = getRange();
      SmartDashboard.putNumber("Range", round10(range));

      //table.getEntry("Range").setDouble(range);
      //table.getEntry("Target Tilt").setDouble(targetAngle);
      //table.getEntry("H offset").setDouble(targetOffset);
      //table.getEntry("Targets").setDouble(rects.size());

      for (int i = 0; i < rects.size(); i++) {
        RotatedRect r = rects.get(i);
        Point[] vertices = new Point[4];
        r.points(vertices);
        for (int j = 0; j < 4; j++) {
          if (r == biggest)
            Imgproc.line(mat, vertices[j], vertices[(j + 1) % 4], new Scalar(255, 255, 0), 2);
          else
            Imgproc.line(mat, vertices[j], vertices[(j + 1) % 4], new Scalar(255, 255, 255), 1);
        }
      }
      outputStream.putFrame(mat);
    }
  }
}
