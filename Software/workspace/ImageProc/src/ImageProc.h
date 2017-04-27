#ifndef Vision_H
#define Vision_H
#include "GripPipeline.h"


#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/core/core.hpp>
#include <opencv2/core/types.hpp>
#include "llvm/ArrayRef.h"
#include "llvm/StringRef.h"

#include <CameraServer.h>
using namespace frc;

class ImageProc{
public:
	llvm::StringRef ip;
private:
	cs::UsbCamera camera1;
	cs::UsbCamera camera2;
	cs::CvSink cvSink;
	cs::CvSource outputStream;


	llvm::ArrayRef<double>  hsvThresholdHue = {70, 110};
	llvm::ArrayRef<double>  hsvThresholdSaturation = {100, 255};
	llvm::ArrayRef<double>  hsvThresholdValue = {100, 255};
	std::shared_ptr<NetworkTable> table;

	std::vector<cv::Rect> GoodRects(std::vector<cv::Rect> r);

	GripPipeline gp;

public:
	ImageProc();
	void Process();
	void Init(char *ip);
};

#endif  // Vision_H
