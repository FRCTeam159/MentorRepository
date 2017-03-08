#ifndef Vision_H
#define Vision_H
#include "Subsystems/GripPipeline.h"

#include <Commands/Subsystem.h>
#include <CameraServer.h>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/core/core.hpp>
#include <opencv2/core/types.hpp>
#include "llvm/ArrayRef.h"
#include "llvm/StringRef.h"

using namespace grip;

class Vision : public Subsystem {
public:
	struct CameraInfo {
		int screenWidth;
		int screenHeight;
		double fov;
		double fovFactor;
		double HorizontalOffset;
	};

	CameraInfo cameraInfo;

	struct TargetInfo {
		cv::Point Center;
		double Distance;
		double HorizontalOffset;
		double HorizontalAngle;
		double Height;
		double Width;
		double ActualWidth;
		double ActualHeight;
		int numrects;
	};
	TargetInfo targetInfo;

private:
	static cs::UsbCamera camera1;
	static cs::UsbCamera camera2;
	static cs::CvSink cvSink;
	static cs::CvSource outputStream;


	static llvm::ArrayRef<double>  hsvThresholdHue;
	static llvm::ArrayRef<double>  hsvThresholdSaturation;
	static llvm::ArrayRef<double>  hsvThresholdValue;
	static void VisionThread();
	std::shared_ptr<NetworkTable> table;

	static std::vector<cv::Rect> GoodRects(std::vector<cv::Rect> r);


public:
	Vision();
	void InitDefaultCommand();
	void Process();
	void Init();
	double GetTargetDistance();
	double GetTargetAngle();
	int GetNumTargets();
	void SetCameraInfo(int width, int height, double fov, double hoff);
	void CalcTargetInfo(int n,cv::Point top, cv::Point bottom);
	void GetTargetInfo();
	void PublishTargetInfo();


};

#endif  // Vision_H
