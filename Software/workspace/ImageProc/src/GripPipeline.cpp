#ifndef APP_TEST

#include "GripPipeline.h"
/**
 * Initializes a GripPipeline.
 */

GripPipeline::GripPipeline() {
    int gpus=cv::cuda::getCudaEnabledDeviceCount();
    cout<<"gpus="<<gpus<<endl;
    setDevice(0);
}
/**
 * Runs an iteration of the Pipeline and updates outputs.
 *
 * Sources need to be set before calling this method.
 *
 */
//#define BLUR
#define FILTERCONTOURS

void GripPipeline::process(cv::Mat source0) {
    cv::Mat matInput;
    cv::Mat matOutput;

    source0.copyTo(matInput);

#ifdef BLUR
    BlurType blurType = BlurType::MEDIAN;
    double blurRadius = 4.5;  // default Double
    blur(matInput, blurType, blurRadius, matOutput);
    matOutput.copyTo(matInput);
#endif
#ifdef USE_GPU
    GpuMat gpuInput(matInput);
    GpuMat gpuOutput;
    cv::cuda::cvtColor(gpuInput, gpuOutput, cv::COLOR_BGR2HSV);
    gpuOutput.download(matOutput);
    gpuOutput.release();
    gpuInput.release();
#else
    cv::cvtColor(matInput, matOutput, cv::COLOR_BGR2HSV);
#endif
    matOutput.copyTo(colorThresholdOutput);
    cv::inRange(colorThresholdOutput,
            cv::Scalar(hsvThresholdHue[0], hsvThresholdSaturation[0], hsvThresholdValue[0]),
            cv::Scalar(hsvThresholdHue[1], hsvThresholdSaturation[1], hsvThresholdValue[1]),
            matOutput);
    findContours(matOutput, true, vecInput);
    convexHulls(vecInput, vecOutput);

	//Filter_Contours:
    vecInput=vecOutput;
    //vecOutput=vecInput;

	double filterContoursMinArea = 5.0;  // default Double
	double filterContoursMinPerimeter = 0;  // default Double
	double filterContoursMinWidth = 5.0;  // default Double
	double filterContoursMaxWidth = 100;  // default Double
	double filterContoursMinHeight = 5.0;  // default Double
	double filterContoursMaxHeight = 1000;  // default Double
	double filterContoursSolidity[] = { 0, 100 };
	double filterContoursMaxVertices = 100;  // default Double
	double filterContoursMinVertices = 3;  // default Double
	double filterContoursMinRatio = 0;  // default Double
	double filterContoursMaxRatio = 1.2;  // default Double
	filterContours(vecInput, filterContoursMinArea,
			filterContoursMinPerimeter, filterContoursMinWidth,
			filterContoursMaxWidth, filterContoursMinHeight,
			filterContoursMaxHeight, filterContoursSolidity,
			filterContoursMaxVertices, filterContoursMinVertices,
			filterContoursMinRatio, filterContoursMaxRatio,
			vecOutput);
	vecInput=vecOutput;
	findRectangles(vecInput, returnRectangles);
}

/**
 * This method is a generated getter for the output of a color_Threshold.
 * @return Mat output from color_Threshold.
 */
cv::Mat* GripPipeline::getColorThresholdOutput() {
	return   &colorThresholdOutput;
}
/**
 * Softens an image using one of several filters.
 *
 * @param input The image on which to perform the blur.
 * @param type The blurType to perform.
 * @param doubleRadius The radius for the blur.
 * @param output The image in which to store the output.
 */
void GripPipeline::blur(cv::Mat &input, BlurType &type, double doubleRadius,
		cv::Mat &output) {
	int radius = (int) (doubleRadius + 0.5);
	int kernelSize;
	switch (type) {
	case BOX:
		kernelSize = 2 * radius + 1;
		cv::blur(input, output, cv::Size(kernelSize, kernelSize));
		break;
	case GAUSSIAN:
		kernelSize = 6 * radius + 1;
		cv::GaussianBlur(input, output, cv::Size(kernelSize, kernelSize),
				radius);
		break;
	case MEDIAN:
		kernelSize = 2 * radius + 1;
		cv::medianBlur(input, output, kernelSize);
		break;
	case BILATERAL:
		cv::bilateralFilter(input, output, -1, radius, radius);
		break;
	}
}

/**
 * Finds contours in an image.
 *
 * @param input The image to find contours in.
 * @param externalOnly if only external contours are to be found.
 * @param contours vector of contours to put contours in.
 */
void GripPipeline::findContours(cv::Mat &input, bool externalOnly,
		std::vector<std::vector<cv::Point> > &contours) {
	std::vector<cv::Vec4i> hierarchy;
	contours.clear();
	int mode = externalOnly ? cv::RETR_EXTERNAL : cv::RETR_LIST;
	int method = cv::CHAIN_APPROX_SIMPLE;
	cv::findContours(input, contours, hierarchy, mode, method);
}

/**
 * Compute the convex hulls of contours.
 *
 * @param inputContours The contours on which to perform the operation.
 * @param outputContours The contours where the output will be stored.
 */
void GripPipeline::convexHulls(
	std::vector<std::vector<cv::Point> > &inputContours,
	std::vector<std::vector<cv::Point> > &outputContours) {
	std::vector<std::vector<cv::Point> > hull(inputContours.size());
	outputContours.clear();
	for (size_t i = 0; i < inputContours.size(); i++) {
		cv::convexHull(cv::Mat((inputContours)[i]), hull[i], false);
	}
	outputContours = hull;
}

/**
 * Filters through contours.
 * @param inputContours is the input vector of contours.
 * @param minArea is the minimum area of a contour that will be kept.
 * @param minPerimeter is the minimum perimeter of a contour that will be kept.
 * @param minWidth minimum width of a contour.
 * @param maxWidth maximum width.
 * @param minHeight minimum height.
 * @param maxHeight  maximimum height.
 * @param solidity the minimum and maximum solidity of a contour.
 * @param minVertexCount minimum vertex Count of the contours.
 * @param maxVertexCount maximum vertex Count.
 * @param minRatio minimum ratio of width to height.
 * @param maxRatio maximum ratio of width to height.
 * @param output vector of filtered contours.
 */
void GripPipeline::filterContours(
		std::vector<std::vector<cv::Point> > &inputContours, double minArea,
		double minPerimeter, double minWidth, double maxWidth, double minHeight,
		double maxHeight, double solidity[], double maxVertexCount,
		double minVertexCount, double minRatio, double maxRatio,
		std::vector<std::vector<cv::Point> > &output) {
	std::vector<cv::Point> hull;
	output.clear();
	for (std::vector<cv::Point> contour : inputContours) {
		cv::Rect bb = boundingRect(contour);
		if (bb.width < minWidth || bb.width > maxWidth)
			continue;
		if (bb.height < minHeight || bb.height > maxHeight)
			continue;
		double area = cv::contourArea(contour);
		if (area < minArea)
			continue;
		if (arcLength(contour, true) < minPerimeter)
			continue;
		//cv::convexHull(cv::Mat(contour, true), hull);
		//double solid = 100 * area / cv::contourArea(hull);
		//if (solid < solidity[0] || solid > solidity[1]) continue;
		//if (contour.size() < minVertexCount || contour.size() > maxVertexCount)	continue;
		double ratio = bb.width / bb.height;
		if (ratio < minRatio || ratio > maxRatio)
			continue;
		output.push_back(contour);
	}
}
void GripPipeline::findRectangles(
	std::vector<std::vector<cv::Point> > &inputContours,
	std::vector<cv::Rect> &output) {
	output.clear();
	for (std::vector<cv::Point> contour : inputContours) {
		cv::Rect bb = boundingRect(contour);
		output.push_back(bb);
	}
}

#endif
