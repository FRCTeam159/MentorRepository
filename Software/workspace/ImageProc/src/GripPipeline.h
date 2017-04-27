#pragma once
#ifndef APP_TEST

#include <opencv2/objdetect/objdetect.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
//#include <opencv2/contrib/contrib.hpp>
#include <opencv2/core/core.hpp>
#include <opencv2/features2d.hpp>
#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <map>
#include <vector>
#include <string>
#include <math.h>
#include "llvm/StringRef.h"
#include "llvm/ArrayRef.h"

//namespace grip {

/**
* A representation of the different types of blurs that can be used.
*
*/
enum BlurType {
	BOX, GAUSSIAN, MEDIAN, BILATERAL
};
/**
* GripPipeline class.
* 
* An OpenCV pipeline generated by GRIP.
*/
class GripPipeline {
	private:
		cv::Mat resizeImageOutput;
		cv::Mat blurOutput;
		cv::Mat colorThresholdOutput;
		cv::Mat source0;
		cv::Mat returnImage;
		std::vector<std::vector<cv::Point> > findContoursOutput;
		std::vector<std::vector<cv::Point> > convexHullsOutput;
		std::vector<std::vector<cv::Point> > filterContoursOutput;
		std::vector<std::vector<cv::Point> > returnVector;
		std::vector<cv::Rect> returnRectangles;

		void resizeImage(cv::Mat &, double , double , int , cv::Mat &);
		void blur(cv::Mat &, BlurType &, double , cv::Mat &);
		void rgbThreshold(cv::Mat &, double [], double [], double [], cv::Mat &);
		void hsvThreshold(cv::Mat &input, llvm::ArrayRef<double>, llvm::ArrayRef<double>, llvm::ArrayRef<double>, cv::Mat &out);
		void findContours(cv::Mat &, bool , std::vector<std::vector<cv::Point> > &);
		void convexHulls(std::vector<std::vector<cv::Point> > &, std::vector<std::vector<cv::Point> > &);
		void filterContours(std::vector<std::vector<cv::Point> > &, double , double , double , double , double , double , double [], double , double , double , double , std::vector<std::vector<cv::Point> > &);
		void findRectangles(std::vector<std::vector<cv::Point> > &inputContours, std::vector<cv::Rect> &output);

		llvm::ArrayRef<double>  hsvThresholdHue = {71, 115};
		llvm::ArrayRef<double>  hsvThresholdSaturation = {134, 255};
		llvm::ArrayRef<double>  hsvThresholdValue = {121, 211};

	public:
		GripPipeline();
		void process(cv::Mat source0);
		cv::Mat* getresizeImageOutput();
		cv::Mat* getblurOutput();
		cv::Mat* getColorThresholdOutput();
		std::vector<std::vector<cv::Point> >* getfindContoursOutput();
		std::vector<std::vector<cv::Point> >* getconvexHullsOutput();
		std::vector<std::vector<cv::Point> >* getfilterContoursOutput();
		void setsource0(cv::Mat &source0);
		std::vector<std::vector<cv::Point> >*getResultVector();
		std::vector<cv::Rect>*getRectangles(){
			return &returnRectangles;
		}

		void setHSVThresholdHue(llvm::ArrayRef<double> value) { hsvThresholdHue=value;}
		void setHSVThresholdSaturation(llvm::ArrayRef<double> value) {hsvThresholdSaturation =value;}
		void setHSVThresholdValue(llvm::ArrayRef<double> value) {hsvThresholdValue = value;}
		llvm::ArrayRef<double> getHSVHue() { return hsvThresholdHue;}
		llvm::ArrayRef<double> getHSVSaturation() {return hsvThresholdSaturation;}
		llvm::ArrayRef<double> getHSVValue() {return hsvThresholdValue;}
};

#endif
//} // end namespace grip


