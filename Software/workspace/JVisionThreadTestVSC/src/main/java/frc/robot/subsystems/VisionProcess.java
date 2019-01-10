/*----------------------------------------------------------------------------*/
/* Copyright (c) 2018 FIRST. All Rights Reserved.                             */
/* Open Source Software - may be modified and shared by FRC teams. The code   */
/* must be accompanied by the FIRST BSD license file in the root directory of */
/* the project.                                                               */
/*----------------------------------------------------------------------------*/

package frc.robot.subsystems;

import edu.wpi.cscore.UsbCamera;
import edu.wpi.first.wpilibj.CameraServer;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;
import edu.wpi.first.networktables.NetworkTableInstance;
import edu.wpi.first.networktables.NetworkTable;
import edu.wpi.first.networktables.NetworkTableEntry;

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

import java.util.ArrayList;
import frc.robot.subsystems.GripPipeline;

/**
 * Add your docs here.
 */
public class VisionProcess extends Thread {
  static UsbCamera camera;
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
  double targetAspectRatio=targetWidth/targetHeight; 

  public void init() {
    camera = CameraServer.getInstance().startAutomaticCapture("Targeting", 0);
    camera.setFPS(15);
    camera.setResolution(320, 240);
    SmartDashboard.putNumber("Targets", 0);
    SmartDashboard.putNumber("H distance", 0);
    SmartDashboard.putNumber("W distance", 1);
    SmartDashboard.putNumber("V offset", 0);
    SmartDashboard.putNumber("H offset", 0);
    SmartDashboard.putNumber("Target Width", 0);
    SmartDashboard.putNumber("Target Height", 0);

    SmartDashboard.putNumber("Target Aspect", 0);
    SmartDashboard.putNumber("Target Angle", 0);


    // SmartDashboard.putNumber("Angle", 0);
    SmartDashboard.putBoolean("Show HSV", false);
    System.out.println("fov H:" + Math.toDegrees(cameraFovH) + " W:" + Math.toDegrees(cameraFovW));
    System.out.println("Expected Target Aspect ratio:" + round10(targetAspectRatio));
  }

  Point center(Rect r) {
    double cx = r.tl().x + 0.5 * r.width;
    double cy = r.tl().y + 0.5 * r.height;
    return new Point(cx, cy);
  }
  double round10(double x){
    return Math.round(x * 10+0.5) / 10.0;
  }
  public void run() {
    GripPipeline grip = new GripPipeline();
    CvSink cvSink = CameraServer.getInstance().getVideo();
    CvSource outputStream = CameraServer.getInstance().putVideo("Rectangle", 320, 240);
    Mat mat = new Mat();
    ArrayList<RotatedRect> rects = new ArrayList<RotatedRect>();
    ArrayList<MatOfPoint> contours = new ArrayList<MatOfPoint>();

    // TODO: use a network tables data struct to pass target params to Robot Program
    NetworkTableInstance inst = NetworkTableInstance.getDefault();
    NetworkTable table = inst.getTable("targetdata");
    while (true) {
      try {
        Thread.sleep(20);
      } catch (InterruptedException ex) {
        System.out.println("exception)");
      }
      if (cvSink.grabFrame(mat) == 0) {
        // Send the output the error.
        outputStream.notifyError(cvSink.getError());
        // skip the rest of the current iteration
        continue;
      }

      grip.process(mat);
      Boolean show_hsv = SmartDashboard.getBoolean("Show HSV", false);
      if (show_hsv) {
        Mat hsv = grip.hsvThresholdOutput(); // display HSV image
        hsv.copyTo(mat);
      }
      ArrayList<MatOfPoint> grip_contours = grip.filterContoursOutput();

      rects.clear();
      contours.clear();
      double max_area = 0;
      RotatedRect biggest = null;
      
      // find the bounding boxes of all targets
      for (int i = 0; i < grip_contours.size(); i++) {
        MatOfPoint contour = grip_contours.get(i);
        //Rect r = Imgproc.boundingRect(contour);
        double area = Imgproc.contourArea(contour);
        MatOfPoint2f  NewMtx = new MatOfPoint2f( contour.toArray() );
        RotatedRect r = Imgproc.minAreaRect(NewMtx);
        if (area > max_area) {
          biggest = r;
          max_area = area;
        }
        rects.add(r);
        contours.add(contour);
      }
      // calculate distance to target
      // - using ht
      SmartDashboard.putNumber("Targets", rects.size());
      if (biggest != null) {
        double h=biggest.size.height;
        double w=biggest.size.width;
        double a=biggest.angle;

        if(biggest.size.width > biggest.size.height){
          h=biggest.size.width;
          w=biggest.size.height;
          a+=90.0;
        }
        double dh = distanceFactorHeight / h;
        double dw = distanceFactorWidth / w;
        //Point ctr = center(biggest);
        Point ctr = biggest.center;

        double hoff=angleFactorWidth*(ctr.x-0.5*imageWidth);
        double voff=-angleFactorHeight*(ctr.y-0.5*imageHeight); // invert y !

        SmartDashboard.putNumber("H distance", round10(dh));
        SmartDashboard.putNumber("W distance", round10(dw));
        SmartDashboard.putNumber("H offset", round10(hoff));
        SmartDashboard.putNumber("V offset", round10(voff));
        SmartDashboard.putNumber("Target Width", round10(w));
        SmartDashboard.putNumber("Target Height", round10(h));
        SmartDashboard.putNumber("Target Aspect", round10((double)(w)/h));
        SmartDashboard.putNumber("Target Angle", round10((double)a));

      }
      for (int i = 0; i < rects.size(); i++) {
        RotatedRect r = rects.get(i);
        //Rect b=r.boundingRect();
        //Point tl = b.tl();
        //Point br = b.br();
        // if (r == biggest)
        //    Imgproc.rectangle(mat, tl, br, new Scalar(255.0, 255.0, 0.0), 2);
        // else
        //    Imgproc.rectangle(mat, tl, br, new Scalar(255.0, 255.0, 255.0), 1);
        Point[] vertices = new Point[4];  
        r.points(vertices);  
        for (int j = 0; j < 4; j++){ 
          if (r == biggest)
            Imgproc.line(mat, vertices[j], vertices[(j+1)%4], new Scalar(255,255,0),2);
          else
            Imgproc.line(mat, vertices[j], vertices[(j+1)%4], new Scalar(255,255,255),1);
        }
      }
      outputStream.putFrame(mat);
    }
  }
}
