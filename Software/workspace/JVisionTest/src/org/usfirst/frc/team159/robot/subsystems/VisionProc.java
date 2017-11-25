package org.usfirst.frc.team159.robot.subsystems;

import java.util.ArrayList;

import org.opencv.core.Core;
import org.opencv.core.Mat;
import org.opencv.core.MatOfPoint;
import org.opencv.core.Point;
import org.opencv.core.Rect;
import org.opencv.core.Scalar;
import org.opencv.videoio.VideoCapture;
import org.opencv.imgproc.Imgproc;


import edu.wpi.cscore.CvSource;
import edu.wpi.first.wpilibj.CameraServer;
import edu.wpi.first.wpilibj.command.Subsystem;
import edu.wpi.first.wpilibj.networktables.NetworkTable;
import edu.wpi.first.wpilibj.smartdashboard.SmartDashboard;

import org.usfirst.frc.team159.robot.commands.VisionUpdate;
import org.usfirst.frc.team159.robot.subsystems.GripPipeline;

/**
 *
 */
/**
 * @author dean
 *
 */
public class VisionProc extends Subsystem implements Runnable {
	static {
		System.loadLibrary(Core.NATIVE_LIBRARY_NAME);
	}
	class CameraInfo { // 
		public int screenWidth;
		public int screenHeight;
		public double fov;
		public double fovFactor;
		public double HorizontalOffset;
	}
	CameraInfo cameraInfo=new CameraInfo();
	class TargetInfo { // 
		public Point Center; // from vision targeting system
		public double screenWidth;
		public double screenHeight;
		public double Distance;
		public double HorizontalOffset;
		public double HorizontalAngle;
		public double ActualWidth;
		public double ActualHeight;
	}
	TargetInfo targetInfo=new TargetInfo();
	
	double num_targets=0;
	static boolean use_thread=false;

	NetworkTable table;
	Thread visionTread;
    // Put methods for controlling this subsystem
    // here. Call these from Commands.
    int count=0;
	public VisionProc() {
		SmartDashboard.putNumber("Targets", 0);
		SmartDashboard.putNumber("Distance", 0);
		SmartDashboard.putNumber("Angle", 0);

		setCameraInfo(320,240,60,0); // screen width, screen height, camera fov
		setTargetInfo(11.0,2.0); // gear width, gear height

		table= NetworkTable.getTable("datatable");
		if(System.getProperty("use_thread")!=null)
			use_thread=true;
		// use local thread for targeting if -Duse_thread passed in as a startup option
		// otherwise assume vision targeting is done remotely (e.g rasberrypi, jetson)
		if(use_thread) {
			visionTread = new Thread(this);
			visionTread.start();
		}		
	}
	void setCameraInfo(int width, int height, double fov, double hoff) {
		cameraInfo.screenWidth = width;
		cameraInfo.screenHeight = height;
		cameraInfo.fov = fov;
		cameraInfo.fovFactor = cameraInfo.screenWidth/(2*Math.tan(Math.toRadians(fov)/2.0));
		System.out.println("fovfactor="+cameraInfo.fovFactor);
		cameraInfo.HorizontalOffset=hoff;
	}
	void setTargetInfo(double width, double height) {
		targetInfo.ActualWidth=width;
		targetInfo.ActualHeight=height;
	}
    public void initDefaultCommand() {
        // Set the default command for a subsystem here.
        setDefaultCommand(new VisionUpdate());
    }
    /**
     * get target data from vision targeting system (sent though NetTables)
     */
    void getTargetInfo() {
    	double x1=table.getNumber("TopLeftX", 0.0);
    	double x2=table.getNumber("BotRightX", 0.0);
    	double y1=table.getNumber("TopLeftY", 0.0);
    	double y2=table.getNumber("BotRightY", 0.0);
    	num_targets=table.getNumber("Targets", 0);
     	targetInfo.screenWidth=x2-x1;
    	targetInfo.screenHeight=y2-y1;
    	targetInfo.Center=new Point(0.5*(x1+x2),0.5*(y1+y2));
    	if(num_targets>0)
    		calcTargetSpecs();
    }
	/**
	 * calculate target offsets from bounding box
	 */
	void calcTargetSpecs() {
		targetInfo.Distance=cameraInfo.fovFactor*targetInfo.ActualWidth/targetInfo.screenWidth;
		targetInfo.HorizontalOffset=targetInfo.Center.x-cameraInfo.screenWidth/2;
	    double cam_adjust=cameraInfo.fovFactor*cameraInfo.HorizontalOffset/targetInfo.Distance; // convert to pixels
	    double p=targetInfo.Center.x+targetInfo.HorizontalOffset+cam_adjust-0.5*cameraInfo.screenWidth;
	    targetInfo.HorizontalAngle=p*cameraInfo.fov/cameraInfo.screenWidth; // angle error
	}
    /**
     * send target info to SmartDashboard
     */
    void publishTargetInfo(){
    	SmartDashboard.putNumber("Targets", num_targets);
    	SmartDashboard.putNumber("Distance", targetInfo.Distance);
    	SmartDashboard.putNumber("Angle", targetInfo.HorizontalAngle);
    }
    /**
     * called from VisionUpdate command
     */
    public void process() {
    	getTargetInfo();
    	publishTargetInfo();
    }
    /* (non-Javadoc)
     * @see java.lang.Runnable#run()
     */
    public void run() {
    	GripPipeline grip=new GripPipeline();
    	VideoCapture vcap=new VideoCapture();
    	CameraServer camera=CameraServer.getInstance();

		String videoStreamAddress = "http://localhost:5004/?action=stream";
	    if(!vcap.open(videoStreamAddress))
	        System.out.println("Error opening video stream "+videoStreamAddress);
	    else
	        System.out.println("Video Stream captured "+videoStreamAddress);

	    // System.out.println(Core.NATIVE_LIBRARY_NAME );
	    // System.out.println(System.getProperty("java.library.path"));
	    // Simple test to see if OpenCV is loaded
	    //  Mat mat = Mat.eye( 3, 3, CvType.CV_8UC1 );
	    //  System.out.println( "mat = " + mat.dump() );

    	CvSource outputStream = camera.putVideo("Rectangle", 320, 240);
    	Mat mat = new Mat();
    	ArrayList<Rect>rects = new ArrayList<Rect>();
    	while(true) {
    		if(!vcap.read(mat))  // wait for frame
    			continue;
            
			grip.process(mat);
			//mat=grip.hsvThresholdOutput();
			
			ArrayList<MatOfPoint> contours=grip.filterContoursOutput();
			
			rects.clear();
			double max_area=0;
			Rect biggest=null;
			for (int i=0;i<contours.size();i++) {
				MatOfPoint contour = contours.get(i);
				Rect r=Imgproc.boundingRect(contour);
				double area=r.area();
				if(area>max_area) {
					biggest=r;
					max_area=area;
				}
				rects.add(r);
			}
			for (int i=0;i<rects.size();i++) {
				Rect r=rects.get(i);
				Point tl=r.tl();
				Point br=r.br();
				if(r==biggest)
					Imgproc.rectangle(mat, tl, br, new Scalar(255.0, 255.0, 0.0), 2);
				else
					Imgproc.rectangle(mat, tl, br, new Scalar(255.0, 255.0, 255.0), 1);			
			}

			outputStream.putFrame(mat);

			// publish target data to Robot Thread
			table.putNumber("Targets", rects.size());
			if(rects.size()>0 && biggest!=null) {  
				table.putNumber("TopLeftX", biggest.tl().x);
				table.putNumber("TopLeftY", biggest.tl().y);
				table.putNumber("BotRightX",biggest.br().x);
				table.putNumber("BotRightY",biggest.br().y);
			}
			count++;
			try {
				Thread.sleep(10);
			}catch(Exception e) {
				SmartDashboard.putNumber("Targets", 0);
			}
    	}
    }

}

