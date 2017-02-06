#include <Commands/VisionUpdate.h>
#include "Vision.h"
#include "../RobotMap.h"
#include "Subsystems/GripPipeline.h"

#define RPD(x) (x)*2*M_PI/360
#define IMAGE_WIDTH 320
#define IMAGE_HEIGHT 240
#define FOV 38 //60.0
#define ASPECT_RATIO (4.0/3.0)

#define HOFFSET 5 // camera offset from robot center


using namespace frc;

llvm::ArrayRef<double>  Vision::hsvThresholdHue = {70, 110};
llvm::ArrayRef<double>  Vision::hsvThresholdSaturation = {100, 255};
llvm::ArrayRef<double>  Vision::hsvThresholdValue = {100, 255};
cs::UsbCamera Vision::camera1;
cs::UsbCamera Vision::camera2;
cs::CvSink Vision::cvSink;
cs::CvSource Vision::outputStream;

static 	GripPipeline gp;

#define CAMERASENABLED


Vision::Vision() : Subsystem("VisionSubsystem") {
	SetCameraInfo(IMAGE_WIDTH,IMAGE_HEIGHT,FOV,HOFFSET);
}

void Vision::InitDefaultCommand() {
#ifdef CAMERASENABLED
	// Set the default command for a subsystem here.
	SetDefaultCommand(new VisionUpdate());
#endif
}

void Vision::Init() {
	table=NetworkTable::GetTable("datatable");
	frc::SmartDashboard::PutBoolean("showColorThreshold", false);
	frc::SmartDashboard::PutNumber("HueMax", hsvThresholdHue[1]);
	frc::SmartDashboard::PutNumber("HueMin", hsvThresholdHue[0]);
	//frc::SmartDashboard::PutNumberArray("hue", hsvThresholdHue);
	frc::SmartDashboard::PutNumber("SaturationMax", hsvThresholdSaturation[1]);
	frc::SmartDashboard::PutNumber("SaturationMin", hsvThresholdSaturation[0]);
	frc::SmartDashboard::PutNumber("ValueMax", hsvThresholdValue[1]);
	frc::SmartDashboard::PutNumber("ValueMin", hsvThresholdValue[0]);
	frc::SmartDashboard::PutNumber("Rectangles", 0);
	frc::SmartDashboard::PutBoolean("showGoodRects", true);
	frc::SmartDashboard::PutNumber("Distance", 0);
	frc::SmartDashboard::PutNumber("HorizontalOffset", 0);
	frc::SmartDashboard::PutNumber("HorizontalAngle", 0);
#ifdef CAMERASENABLED
#ifndef SIMULATION
	camera1 = CameraServer::GetInstance()->StartAutomaticCapture("Logitech",0);
	camera2 = CameraServer::GetInstance()->StartAutomaticCapture("DriverCam",1);

	// Set the resolution
	camera1.SetResolution(320, 240);
	camera2.SetResolution(320, 240);
	//camera.SetFPS(1);
	camera1.SetFPS(10);
#endif
	std::thread visionThread(VisionThread);
	visionThread.detach();
#endif
}

#define SHOW_COLOR_THRESHOLD

void Vision::Process() {
	GetTargetInfo (targetInfo);
	PublishTargetInfo(targetInfo);
}


void Vision::VisionThread(){
	std::shared_ptr<NetworkTable> table2=NetworkTable::GetTable("datatable");
#ifdef SIMULATION
	cv::VideoCapture vcap;

    const std::string videoStreamAddress = "http://localhost:5002/?action=stream";
    //open the video stream and make sure it's opened
    if(!vcap.open(videoStreamAddress))
        std::cout << "Error opening video stream "<<videoStreamAddress << std::endl;
    else
        std::cout << "Video Stream captured "<<videoStreamAddress << std::endl;
#else
	// Get a CvSink. This will capture Mats from the Camera
	cvSink = CameraServer::GetInstance()->GetVideo("Logitech");
#endif
	// Setup a CvSource. This will send images back to the Dashboard
	outputStream = CameraServer::GetInstance()->PutVideo("Rectangle", 320, 240);

	cv::Mat mat;

	cv::Point tl=cv::Point(10, 10);
	cv::Point br=cv::Point(20, 20);


	while(true){
#ifdef SIMULATION
		if(!vcap.read(mat)) {
#else
		if (cvSink.GrabFrame(mat) == 0) {
#endif
			//outputStream.NotifyError(cvSink.GetError());
			continue;
		}
		bool showColorThreshold = frc::SmartDashboard::GetBoolean("showColorThreshold", false);

		hsvThresholdHue={SmartDashboard::GetNumber("HueMin",70),SmartDashboard::GetNumber("HueMax", 100)};
		hsvThresholdValue={SmartDashboard::GetNumber("ValueMin", 100),SmartDashboard::GetNumber("ValueMax",255)};
		hsvThresholdSaturation={SmartDashboard::GetNumber("SaturationMin", 100),SmartDashboard::GetNumber("SaturationMax",255)};

		gp.setHSVThresholdHue(hsvThresholdHue);
		gp.setHSVThresholdValue(hsvThresholdValue);
		gp.setHSVThresholdSaturation(hsvThresholdSaturation);

		gp.process(mat);

		if(showColorThreshold){
			cv::Mat* mat2=gp.getColorThresholdOutput();
			mat2->copyTo(mat);
		}

		int minx = 1000, maxx = 0, miny = 1000, maxy = 0;
		std::vector<cv::Rect> rects= *gp.getRectangles();

		bool showGoodRects = frc::SmartDashboard::GetBoolean("showGoodRects", true);

		std::vector<cv::Rect> rectsPointer=rects;
		if (showGoodRects){
			rectsPointer=GoodRects(rects);
		}

		frc::SmartDashboard::PutNumber("Rectangles",rectsPointer.size());
		table2->PutNumber("NumRects", rectsPointer.size());
		for (unsigned int i = 0; i < rectsPointer.size(); i++) {
			cv::Rect r= rectsPointer[i];
			cv::Point p= r.tl();
			minx = p.x < minx ? p.x : minx;
			miny = p.y < miny ? p.y : miny;
			p= r.br();
			maxy = p.y > maxy ? p.y : maxy;
			maxx = p.x > maxx ? p.x : maxx;
			rectangle(mat, r.tl(), r.br(), cv::Scalar(0, 255, 255), 1);
		}

		tl=cv::Point(minx, miny);
		br=cv::Point(maxx, maxy);

		rectangle(mat, cv::Point(minx, miny), cv::Point(maxx, maxy), cv::Scalar(255, 255, 255), 1);

		outputStream.PutFrame(mat);
		table2->PutNumber("TopLeftX", tl.x);
		table2->PutNumber("TopLeftY", tl.y);
		table2->PutNumber("BotRightX", br.x);
		table2->PutNumber("BotRightY", br.y);
	}
}

std::vector<cv::Rect> Vision::GoodRects(std::vector<cv::Rect> rects) {
	if(rects.size()<3)
		return rects;
	std::vector<cv::Rect> goodrects;
	int goodFactor=5;
	for (unsigned int i = 0; i < rects.size(); i++) {
		int score = 0;
		cv::Rect Rect1=rects [i];
		int cx1=0.5*(Rect1.tl().x+Rect1.br().x);
		int cy1=0.5*(Rect1.tl().y+Rect1.br().y);
		double w1=rects[i].width;
		for (unsigned int j = 0; j < rects.size(); j++) {
			if (i==j)
				continue;
			cv::Rect Rect2=rects [j];
			int cx2=0.5*(Rect2.tl().x+Rect2.br().x);
			int cy2=0.5*(Rect2.tl().y+Rect2.br().y);
			int dx=cx1-cx2;
			int dy=cy1-cy2;
			int d=sqrt(dx*dx+dy*dy);
			double r=d/w1;
			if (r<goodFactor)
				score++;
		}
		if (score>0)
			goodrects.push_back(Rect1);
	}
	return goodrects;
}

void Vision::SetCameraInfo(int width, int height, double fov, double hoff) {
	cameraInfo.screenWidth = width;
	cameraInfo.screenHeight = height;
	cameraInfo.fov = fov;
	cameraInfo.fovFactor = 1/(2*tan(RPD(fov/2.0)));
	cameraInfo.HorizontalOffset=hoff;
	cout<<"fovFactor: "<<cameraInfo.fovFactor<<endl;
}

void Vision::CalcTargetInfo(int n,cv::Point top, cv::Point bottom, TargetInfo &info) {
	info.numrects=n;
	if(n>0){
		info.Height=bottom.y-top.y; // screen y is inverted
		info.Width=bottom.x-top.x;
		info.Center.x=0.5*(bottom.x+top.x);
		info.Center.y=0.5*(bottom.y+top.y);
		info.HorizontalOffset=targetInfo.Center.x-cameraInfo.screenWidth/2;
		info.ActualHeight=5.0;
		info.ActualWidth=(n==1?2.0:10.25);	//inches
		info.Distance=cameraInfo.fovFactor*cameraInfo.screenHeight*targetInfo.ActualHeight/targetInfo.Height;
		// convert camera offset to pixels
	    double adjust=cameraInfo.fovFactor*cameraInfo.screenWidth*cameraInfo.HorizontalOffset/info.Distance;
	    double p=info.Center.x+adjust-0.5*cameraInfo.screenWidth;
	    info.HorizontalAngle=p*cameraInfo.fov/cameraInfo.screenWidth;
	}
}

double Vision::GetTargetAngle() {
	return targetInfo.HorizontalAngle;
}
double Vision::GetTargetDistance() {
	return targetInfo.Distance;
}
int Vision::GetNumTargets() {
	return targetInfo.numrects;
}

void Vision::GetTargetInfo(TargetInfo & info) {
	cv::Point top;
	cv::Point bot;
	int n=table->GetNumber("NumRects", 0);
	top.x=table->GetNumber("TopLeftX", 10);
	top.y=table->GetNumber("TopLeftY", 10);
	bot.x=table->GetNumber("BotRightX",20);
	bot.y=table->GetNumber("BotRightY",20);

	CalcTargetInfo(n,top,bot,info);
}

void Vision::PublishTargetInfo(TargetInfo &info) {
	frc::SmartDashboard::PutNumber("Distance", info.Distance);
	frc::SmartDashboard::PutNumber("HorizontalOffset", info.HorizontalOffset);
	frc::SmartDashboard::PutNumber("HorizontalAngle", info.HorizontalAngle);
	frc::SmartDashboard::PutNumber("TargetHeight", info.Height);
	frc::SmartDashboard::PutNumber("TargetWidth", info.Width);
	frc::SmartDashboard::PutNumber("TargetCenter.x", info.Center.x);
	frc::SmartDashboard::PutNumber("TargetCenter.y", info.Center.y);

}

	// Put methods for controlling this subsystem
// here. Call these from Commands.
