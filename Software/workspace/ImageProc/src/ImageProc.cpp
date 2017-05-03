
#include <unistd.h>

#include "ImageProc.h"
#include "GripPipeline.h"


#define RPD(x) (x)*2*M_PI/360
#define IMAGE_WIDTH 320
#define IMAGE_HEIGHT 240
#define DFOV 60   // diagonal spec
#define HFOV 49.6 // measured 44.28
#define VFOV 30.2 //

#define ASPECT_RATIO (4.0/3.0)

#define SIMULATION

ImageProc::ImageProc(){
}

void ImageProc::Init(std::string addrs) {
	NetworkTable::SetClientMode();
	if(addrs.empty()){
	    addrs="Ubuntu16.local";
	}
	ip=addrs;
    NetworkTable::SetIPAddress(llvm::StringRef(addrs));
	table=NetworkTable::GetTable("datatable");

#ifndef SIMULATION
	camera1 = CameraServer::GetInstance()->StartAutomaticCapture("Logitech",0);
	camera2 = CameraServer::GetInstance()->StartAutomaticCapture("DriverCam",1);

	// Set the resolution
	camera1.SetResolution(320, 240);
	camera2.SetResolution(320, 240);
	//camera.SetFPS(1);
	camera1.SetFPS(10);
#endif
}

#define SHOW_COLOR_THRESHOLD
//#define VCAP_BUFFER_HACK // on pi-3 set via Makefile compiler -D arg

void ImageProc::Process() {
#ifdef SIMULATION
	cv::VideoCapture vcap;
    //vcap.set(CV_CAP_PROP_BUFFERSIZE, 1); // set frame buffer depth to 1 (doesn't work on pi-3)

	const std::string videoStreamAddress = "http://"+ip+":5002/?action=stream?dummy=param.mjpg";
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
	outputStream = CameraServer::GetInstance()->PutVideo("Rectangle", IMAGE_WIDTH, IMAGE_HEIGHT);

	cv::Mat mat;
	cv::Point tl=cv::Point(10, 10);
	cv::Point br=cv::Point(20, 20);
    auto start = std::chrono::high_resolution_clock::now();

	while(true){

#ifdef SIMULATION
#ifdef VCAP_BUFFER_HACK
	    // need to flush 5-deep frame buffer and then wait for a new image otherwise can see a 1-2 second pipeline delay
	    // in processing loop (pi-3)
	    for(int i=0;i<6;i++) // discard any images that were pre-loaded or arrive during previous processing
	        vcap.read(mat);
#endif
		if(!vcap.read(mat)) { // now wait for a fresh image to arrive
#else
		if (cvSink.GrabFrame(mat) == 0) {
#endif
			continue;
		}
//#define NOPROC // skip all processing
#ifdef NOPROC
		outputStream.PutFrame(mat);
        continue;
#endif
		gp.setHSVThresholdHue(hsvThresholdHue);
		gp.setHSVThresholdValue(hsvThresholdValue);
		gp.setHSVThresholdSaturation(hsvThresholdSaturation);

		gp.process(mat);

		bool showColorThreshold=table->GetBoolean("showColorThreshold", false);

		if(showColorThreshold){
			cv::Mat* mat2=gp.getColorThresholdOutput();
			mat2->copyTo(mat);
		}

		int minx = 1000, maxx = 0, miny = 1000, maxy = 0;
		std::vector<cv::Rect> rects= *gp.getRectangles();

		bool showGoodRects = table->GetBoolean("showGoodRects", true);

		std::vector<cv::Rect> rectsPointer=rects;
		if (showGoodRects){
			rectsPointer=GoodRects(rects);
		}
		int n=rectsPointer.size();
		table->PutNumber("NumRects", n);
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
		table->PutNumber("TopLeftX", tl.x);
		table->PutNumber("TopLeftY", tl.y);
		table->PutNumber("BotRightX", br.x);
		table->PutNumber("BotRightY", br.y);
#define DEBUG
#ifdef DEBUG
		auto end = std::chrono::high_resolution_clock::now();
		auto elapsed = std::chrono::duration_cast<std::chrono::milliseconds>(end - start);
        time_t currentTime;
        struct tm *localTime;
        time( &currentTime );                   // Get the current time
        localTime = localtime( &currentTime );  // Convert the current time to the local time
        std::cout<<"calc time :"<<localTime->tm_min<<":"<<localTime->tm_sec<<" dt:"<<elapsed.count()<< " ms num targets:"<<n<<std::endl;
        start=end;
#endif
	}
}

std::vector<cv::Rect> ImageProc::GoodRects(std::vector<cv::Rect> rects) {
	if(rects.size()<2){
		return rects;
	}
	std::vector<cv::Rect> goodrects;
	int gfw=5;
	double maxarea=0;
	cv::Rect Rect2=rects[0];

	for (unsigned int i = 0; i < rects.size(); i++) {
		int score = 0;
		cv::Rect Rect1=rects[i];
		int cx1=0.5*(Rect1.tl().x+Rect1.br().x);
		int cy1=0.5*(Rect1.tl().y+Rect1.br().y);
		double w1=rects[i].width;
		double area=Rect1.area();

		if(i>0 && area>maxarea){
			Rect2=Rect1;
			maxarea=area;
		}
		for (unsigned int j = 0; j < rects.size(); j++) {
			if (i==j)
				continue;
			cv::Rect Rect2=rects[j];
			int cx2=0.5*(Rect2.tl().x+Rect2.br().x);
			int cy2=0.5*(Rect2.tl().y+Rect2.br().y);
			int dx=cx1-cx2;
			int dy=cy1-cy2;
			int d=sqrt(dx*dx+dy*dy);
			double r1=d/w1;
			if (r1<gfw )
				score++;
		}
		if (score>0)
			goodrects.push_back(Rect1);
	}
	if(goodrects.size()==0)
		goodrects.push_back(Rect2);
	return goodrects;
}

int main(int argc, char *argv[]) {
  //setDevice(0);
  static ImageProc image_proc;
  std::string addrs("Ubuntu16.local");
  if(argc>1){
      addrs=std::string(argv[1]);
  }
  std::cout << "Starting ImageProcess ip="<<addrs<<std::endl;
  image_proc.Init(addrs);

  image_proc.Process();
  std::cout << "Exiting ImageProcess"<<std::endl;
}
