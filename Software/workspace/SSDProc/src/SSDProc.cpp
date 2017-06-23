
#include <unistd.h>
#include <caffe/caffe.hpp>

#include "Detector.h"

#include <opencv2/core/types.hpp>
#include "llvm/ArrayRef.h"
#include "llvm/StringRef.h"

#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <ctime>

#include <CameraServer.h>
#include "cscore_oo.h"

#define IMAGE_WIDTH 320
#define IMAGE_HEIGHT 240

#define SIMULATION
//#define VCAP_BUFFER_HACK
//#define PRINT
#define VIDEO_BUFFER_HACK

using namespace frc;

using caffe::Timer;


DEFINE_double(thresh, 0.3,
    "Only pass detections with confidence scores higher than this [default=0.3]");
DEFINE_string(publish, "",
    "NetTables publish URL [no NetTables output if not specified]");
DEFINE_string(output, "Annotated",
    "Output video Name [no image output if not specified]");
DEFINE_bool(timeit, false,
    "display cycle-time and FPS if true [default=false]");
DEFINE_bool(print, false,
    "display bounding box detection info if true [default=false]");
DEFINE_bool(nobuffer, false,
    "disable image buffer if true [default=false]");
DEFINE_bool(nodisplay, false,
    "Don't sent annotated images if set");

int main(int argc, char *argv[]) {
    ::google::InitGoogleLogging(argv[0]);

    FLAGS_alsologtostderr = 1;
#ifndef GFLAGS_GFLAGS_H_
  namespace gflags = google;
#endif
  gflags::SetUsageMessage("Do detection using SSD-caffe\n"
        "Usage:\n"
        "    SSDProc [FLAGS] video_stream model_file weights_file \n");
  gflags::ParseCommandLineFlags(&argc, &argv, true);

  if (argc < 4) {
    gflags::ShowUsageWithFlagsRestrict(argv[0], "SSDProc");
    return 1;
  }

#ifndef SIMULATION
    cs::UsbCamera camera1;
    cs::UsbCamera camera2;
#endif
    cs::CvSink cvSink;

    cs::CvSource outputStream;
    std::shared_ptr<NetworkTable> table;
    const string& videoStreamAddress = argv[1];
    const string& model_file = argv[2];
    const string& weights_file = argv[3];
    const string& publish_addrs = FLAGS_publish;
    const string& output_name = FLAGS_output;
    const float threshold = FLAGS_thresh;
    bool publish=publish_addrs.empty()?false:true;
    const bool timeit = FLAGS_timeit;
    const bool print = FLAGS_print;
    const bool display = !FLAGS_nodisplay;

    string camname;

    Detector detector(model_file, weights_file, "", "104,117,123");
    if(publish){
        std::cout << "Starting SSDProc nettables="<<publish_addrs<<std::endl;
        NetworkTable::SetClientMode();
        NetworkTable::SetIPAddress(llvm::StringRef(publish_addrs));
        table=NetworkTable::GetTable("datatable");
    }

#ifdef SIMULATION
    camname="simcam";
    cs::HttpCamera simcam(camname, videoStreamAddress);
    cvSink = CameraServer::GetInstance()->GetVideo(simcam);

#else
    camera1 = CameraServer::GetInstance()->StartAutomaticCapture("Logitech",0);
    camera2 = CameraServer::GetInstance()->StartAutomaticCapture("DriverCam",1);
    // Set the resolution
    camera1.SetResolution(320, 240);
    camera2.SetResolution(320, 240);
    //camera.SetFPS(1);
    camera1.SetFPS(10);
    camname="Logitech";
    cvSink = CameraServer::GetInstance()->GetVideo("Logitech");
#endif
    // Setup a CvSource. This will send images back to the Dashboard
    if(display)
        outputStream = CameraServer::GetInstance()->PutVideo(output_name, IMAGE_WIDTH, IMAGE_HEIGHT);
    cv::Mat img;
    cv::Mat mat;

    Timer frame_timer;
    int frame_count = 0;
    double ave_ftm=0;
    double ave_ptm=0;
    cv::Scalar color0(0, 255, 255);
    cv::Scalar color1(255, 255, 0);
    cv::Scalar target_color(0, 0, 255);

    //double test_time=0;
    while(true){
        frame_timer.Start();
        if (cvSink.GrabFrame(img) == 0) {
            //std::cout << " waiting for image" << std::endl;
            continue;
        }
        if(display)
            img.copyTo(mat);

        Timer iter_timer;
        iter_timer.Start();
        std::vector<vector<float> > detections = detector.Detect(img);

        double proctime=iter_timer.Seconds();
        double cycletime=frame_timer.Seconds();

        ave_ptm+=proctime;
        ave_ftm+=cycletime;

        if((frame_count%10)==0){
            ave_ptm/=10;
            ave_ftm/=10;
            if(timeit)
                LOG(INFO) <<"frame:"<<frame_count <<" proc:"<<ave_ptm<<" FPS: "<<1.0/ave_ptm<<" cycle:"<<ave_ftm<<" FPS:"<<1.0/ave_ftm<<std::endl;
            ave_ptm=0;
            ave_ftm=0;
        }

        std::vector<vector<float> > good_detections;
        int n=0;
        for (unsigned int i = 0; i < detections.size(); ++i) {
           const vector<float>& d = detections[i];
           // Detection format: [image_id, label, score, xmin, ymin, xmax, ymax].
           CHECK_EQ(d.size(), 7);
           const float score = d[2];
           if (score >= threshold) {
               n++;
               good_detections.push_back(d);
           }
        }
        cv::Point target_point;
        int target_type=0;
        float max_score=0;

        for (unsigned int i = 0; i < good_detections.size(); ++i) {
            const vector<float>& d = good_detections[i];
            const float score = d[2];
            cv::Point tl((d[3] * img.cols),(d[4] * img.rows));
            cv::Point br((int)(d[5] * img.cols),(int)(d[6] * img.rows));
            cv::Point ctr((tl.x+br.x)/2,(tl.y+br.y)/2);
            int type= static_cast<int>(d[1]);
            if(score>max_score){
                max_score=score;
                target_type=type;
                target_point=ctr;
            }
            if(display){
                int lw=score>0.8 ? 2:1;
                cv::Scalar color=(int)d[1]==0?color0:color1;
                rectangle(mat, tl, br, color, lw);
            }
            if(print){
                printf("%d: %.0f%% box: cx:%d cy:%d w:%d h:%d\n", type, score*100,ctr.x,ctr.y,br.x-tl.x,br.y-tl.y);
            }
        }
        if(display){
            if(max_score>0){
                circle(mat, target_point, 15, target_color, 2);
                drawMarker(mat, target_point, target_color, 2);
            }
            outputStream.PutFrame(mat);
        }
        if(publish){
            table->PutNumber("FPS",(int)(1.0/proctime));
            table->PutNumber("TargetType", target_type+1);
            table->PutNumber("TargetScore", int(max_score*100));
            table->PutNumber("TargetX", target_point.x);
            table->PutNumber("TargetY", target_point.y);
        }

        frame_count++;
    }
    CameraServer::GetInstance()->RemoveCamera(camname);
    std::cout << "Exiting SSDProc"<<std::endl;
}
