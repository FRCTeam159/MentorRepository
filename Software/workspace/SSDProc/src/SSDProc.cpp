
#include <unistd.h>
//#include <caffe/caffe.hpp>

#undef NDEBUG // uncomment to get DLOG output

#include "Detector.h"

#include <opencv2/core/types.hpp>
#include "llvm/ArrayRef.h"
#include "llvm/StringRef.h"

#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <ctime>
#include <chrono>
#include <thread>

#include <CameraServer.h>
#include "cscore_oo.h"

#define SIMULATION

#define IMAGE_WIDTH 640
#define IMAGE_HEIGHT 480

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
DEFINE_int32(flush, 0,
     "flush image buffer by dummy reads if set");

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
    const bool display = output_name.empty()?false:true;
    //int skip_frames= FLAGS_flush;

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
    camname="Logitech";
    CameraServer *server=CameraServer::GetInstance();
    cs::UsbCamera camera1 = server->StartAutomaticCapture(camname,1);
    std::this_thread::sleep_for(std::chrono::milliseconds(500));
        // Set the resolution
    camera1.SetResolution(320, 240);

    //camera1.SetFPS(10);
    cvSink = CameraServer::GetInstance()->GetVideo(camera1);
#endif
    // Setup a CvSource. This will send images back to the Dashboard
    if(display)
        outputStream = CameraServer::GetInstance()->PutVideo(output_name, IMAGE_WIDTH, IMAGE_HEIGHT);
        
        /*
    std::vector<cs::VideoProperty> props;
    props=cvSink.GetSource().EnumerateProperties();
    for (int i=0;i< props.size();i++){
        cs::VideoProperty prop= props[i];
        DLOG(INFO) << "cvSink: " << prop.GetName()<< " "<<prop.Get();
    }
   
    //DLOG(INFO) << "cvSink: " << prop.GetName()<< " "<<prop.Get();
    //DLOG(INFO) << "cvSink: " << cvSink.GetSource().GetDescription();
       */  
    cv::Mat img;
    cv::Mat mat;

    Timer frame_timer;
    Timer image_timer;
    int frame_count = 0;
    double ave_ftm=0;
    double ave_ptm=0;
    double ave_rtm=0;

    cv::Scalar color0(0, 255, 255);
    cv::Scalar color1(255, 255, 0);
    cv::Scalar target_color(0, 0, 255);

    while(true){
        frame_timer.Start();
        image_timer.Start();
        //DLOG(INFO) << " frame start: 1";
        if (cvSink.GrabFrame(img) == 0) {
            DLOG(INFO) << " Waiting for image";
            continue;
        }
        //DLOG(INFO) << " frame start: 2";
        double rdtime=image_timer.MicroSeconds()/1000.0;
        ave_rtm+=rdtime;


        if(display)
            img.copyTo(mat);

        Timer iter_timer;
        iter_timer.Start();
        std::vector<vector<float> > detections = detector.Detect(img);

        double proctime=iter_timer.Seconds();
        double cycletime=frame_timer.Seconds();

        ave_ptm+=proctime;
        ave_ftm+=cycletime;

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
        cv::Point target_tl;
        cv::Point target_br;

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
                target_tl=tl;
                target_br=br;
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
            table->PutNumber("TopLeftX", target_tl.x);
            table->PutNumber("TopLeftY", target_tl.y);
            table->PutNumber("BotRightX", target_br.x);
            table->PutNumber("BotRightY", target_br.y);
            int n=good_detections.size()>0?2:0;
            table->PutNumber("NumRects", n);

        }
        if((frame_count%10)==0){
            ave_ptm/=10;
            ave_ftm/=10;
            ave_rtm/=10;
            if(timeit)
                LOG(INFO) <<"frame:"<<frame_count<<" read:"<<ave_rtm <<" proc:"<<1000*ave_ptm<<" FPS: "<<1/ave_ptm<<" cycle:"<<ave_ftm<<" FPS:"<<1.0/ave_ftm<<std::endl;
            ave_ptm=0;
            ave_ftm=0;
            ave_rtm=0;
        }
        frame_count++;
    }
    CameraServer::GetInstance()->RemoveCamera(camname);
    std::cout << "Exiting SSDProc"<<std::endl;
}
