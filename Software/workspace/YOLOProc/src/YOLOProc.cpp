#include <unistd.h>

#include <boost/shared_ptr.hpp>
#include <gflags/gflags.h>
#include <glog/logging.h>

#include <opencv2/core/types.hpp>
#include "llvm/ArrayRef.h"
#include "llvm/StringRef.h"

#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <ctime>

#include <chrono>
#include <thread>

#include <network.h>
#include <parser.h>
#include <option_list.h>
#include <image.h>
#include <detection_layer.h>
#include <region_layer.h>
#include <utils.h>
#include <sys/time.h>

#include <CameraServer.h>
#include "cscore_oo.h"

#include "opencv2/highgui/highgui_c.h"
#include "opencv2/imgproc/imgproc_c.h"
#include <opencv2/imgproc.hpp>
#include <opencv2/videoio.hpp>


extern image get_image_from_stream(CvCapture *cap);
extern image ipl_to_image(IplImage* src);
extern void draw_box(image a, int x1, int y1, int x2, int y2, float r, float g, float b);
void draw_bbox(image a, box bbox, int w, float r, float g, float b);

//#define SIMULATION
#define IMAGE_WIDTH 320
#define IMAGE_HEIGHT 240

DEFINE_double(thresh, 0.4,
        "Only pass detections with confidence scores higher than this [default=0.3]");
DEFINE_string(publish, "",
        "NetTables publish URL [no NetTables output if not specified]");
DEFINE_string(output, "",
        "Output video Name [no image output if not specified]");
DEFINE_bool(timeit, false,
        "display cycle-time and FPS if true [default=false]");
DEFINE_bool(print, false,
        "display bounding box detection info if true [default=false]");
DEFINE_int32(flush, 0,
        "flush image buffer by dummy reads if set");
DEFINE_bool(usage, false,
        "print usage if set");

static double get_wall_time()
{
    struct timeval time;
    if (gettimeofday(&time,NULL)){
        return 0;
    }
    return (double)time.tv_sec + (double)time.tv_usec * .000001;
}

image mat_to_image(cv::Mat mat)
{
    unsigned char *data = mat.data;
    int h = mat.rows;
    int w = mat.cols;
    int c = mat.channels();
    int step = mat.step;

    image out = make_image(w, h, c);
    int i, j, k, count=0;
    for(k= 0; k < c; ++k){
        for(i = 0; i < h; ++i){
            for(j = 0; j < w; ++j){
                out.data[count++] = data[i*step + j*c + k]/255.0;
            }
        }
    }
    rgbgr_image(out); // swaps red and blue
    return out;
}

// YOLO image format: colors are in separate 2-d planes (float 0->1.0)
// cv image format: colors are sequential in rows (byte 0->255)
cv::Mat image_to_mat(image p){
    image copy = copy_image(p);
    rgbgr_image(copy); // swaps red and blue
    int x,y,k;

    IplImage *disp = cvCreateImage(cvSize(p.w,p.h), IPL_DEPTH_8U, p.c);
    int step = disp->widthStep; // 3*image_width
    for(y = 0; y < p.h; ++y){
        for(x = 0; x < p.w; ++x){
            for(k= 0; k < p.c; ++k){
                disp->imageData[y*step + x*p.c + k] = (unsigned char)(get_pixel(copy,x,y,k)*255);
            }
        }
    }
    free_image(copy);
    cv::Mat img = cv::cvarrToMat(disp);
    return img;
}

int main(int argc, char *argv[]) {
    ::google::InitGoogleLogging(argv[0]);

    FLAGS_alsologtostderr = 1;
#ifndef GFLAGS_GFLAGS_H_
    namespace gflags = google;
#endif
    gflags::SetUsageMessage("Do detection using YOLO\n"
            "Usage:\n"
            "    YOLOProc [FLAGS] data_cfg model_cfg weights_file video_stream\n");
    gflags::ParseCommandLineFlags(&argc, &argv, true);

    const bool usage = FLAGS_usage;

    if (argc < 5 || usage) {
        gflags::ShowUsageWithFlagsRestrict(argv[0], "YOLOProc");
        return 1;
    }
    float **probs;
    box *boxes;
    network net;

    double before_predict;
    double after_predict;

    char *datacfg = argv[1];
    char *cfgfile = argv[2];
    char *weightfile = argv[3];
    char *videoAddress = argv[4];

    float thresh = FLAGS_thresh;
    bool timeit = FLAGS_timeit;
    bool print = FLAGS_print;
    const std::string& publish_addrs = FLAGS_publish;
    bool publish=publish_addrs.empty()?false:true;
    std::shared_ptr<NetworkTable> table;
    const std::string& output_name = FLAGS_output;
    const bool display = output_name.empty()?false:true;
    int skip_frames= FLAGS_flush;
    cs::CvSource outputStream;

    int classes;
    char *name_list;
    char **names;
    std::string camname;
    cs::CvSink cvSink;
 
    cv::Scalar color0(0, 255, 255);
    cv::Scalar color1(255, 255, 0);
    cv::Scalar target_color(0, 0, 255);

    if(publish){
         std::cout << "Starting YOLOProc nettables="<<publish_addrs<<std::endl;
         NetworkTable::SetClientMode();
         NetworkTable::SetIPAddress(llvm::StringRef(publish_addrs));
         table=NetworkTable::GetTable("datatable");
    }
#ifdef SIMULATION
    camname="simcam";
    cs::HttpCamera simcam(camname, videoAddress);
    cvSink = frc::CameraServer::GetInstance()->GetVideo(simcam);
#else
    camname="Logitech";

    cs::UsbCamera camera1 = frc::CameraServer::GetInstance()->StartAutomaticCapture(camname,1);
    std::this_thread::sleep_for(std::chrono::milliseconds(500));

    camera1.SetResolution(640, 480);

    //camera1.SetFPS(10);
    cvSink = frc::CameraServer::GetInstance()->GetVideo(camera1);
#endif

    if(display)
        outputStream = frc::CameraServer::GetInstance()->PutVideo(output_name, IMAGE_WIDTH, IMAGE_HEIGHT);

    struct list *options = read_data_cfg(datacfg);
    classes = option_find_int(options, (char*)"classes", 20);
    name_list = option_find_str(options, (char*)"names", (char*)"data/names.list");
    names = get_labels(name_list);

    printf("classes=%d name-list=%s\n",classes,name_list);

    net = parse_network_cfg(cfgfile);
    if(weightfile)
        load_weights(&net, weightfile);
    set_batch_network(&net, 1);
    layer l = net.layers[net.n-1];

    printf("video stream: %s\n", videoAddress);

//    cv::VideoCapture vcap;
//    vcap.set(CV_CAP_PROP_BUFFERSIZE, 1); // set frame buffer depth to 1 (doesn't work on pi-3)
//    if(!vcap.open(videoAddress))
//        error("Couldn't connect to video\n");
//    else
//        std::cout << "Video Stream captured "<<videoAddress << std::endl;


    int num=l.w*l.h*l.n;
    std::cout << "net:"<<net.n<<"x"<<net.w<<"x"<<net.h<<" l:"<<l.w<<"x"<<l.h<<"x"<<l.n<<std::endl;
    boxes = (box *)calloc(num, sizeof(box));
    probs = (float **)calloc(num, sizeof(float *));
    for(int j = 0; j < num; ++j)
        probs[j] = (float *)calloc(l.classes, sizeof(float));
    int count = 0;
    double before_frame = get_wall_time();
    while(true){
        ++count;
        cv::Mat img;
        cv::Mat mat;
        image im;

        if (cvSink.GrabFrame(img) == 0) {
            continue;
        }

//       for(int i=0;i<skip_frames;i++) // discard any images that were pre-loaded or arrived during previous processing
//            vcap.read(img);
//        if(!vcap.read(img)){
//            std::cout << "Waiting for image"<<videoAddress << std::endl;
//            continue;
//        }
        im = mat_to_image(img);
        img.copyTo(mat);

        if(!im.data){
            std::cout << "Stream closed"<<std::endl;
            break;
        }
        image im_s = resize_image(im, net.w, net.h);
        float *X = im_s.data;
        before_predict = get_wall_time();
        float *prediction = network_predict(net, X);
        after_predict = get_wall_time();
        l.output = prediction;

        get_region_boxes(l, 1, 1, thresh, probs, boxes, 0, 0, thresh);
        do_nms(boxes, probs, num, l.classes, 0.4);

        cv::Point target_point;
        int target_type=0;
        float max_prob=0;
        for(int i = 0; i < num; ++i){
            int class1 = max_index(probs[i], classes);
            float prob = probs[i][class1];
            if(prob > thresh){
                box b = boxes[i];
                int left  = (b.x-b.w/2.)*im.w;
                int right = (b.x+b.w/2.)*im.w;
                int top   = (b.y-b.h/2.)*im.h;
                int bot   = (b.y+b.h/2.)*im.h;
                if(left < 0) left = 0;
                if(right > im.w-1) right = im.w-1;
                if(top < 0) top = 0;
                if(bot > im.h-1) bot = im.h-1;
                cv::Point ctr((int)(b.x*im.w),(int)(b.y*im.h));

                if(display){
                    int lw=prob>0.8 ? 3:2;
                    cv::Scalar color=class1==0?color0:color1;
                    cv::Point tl(left,top);
                    cv::Point br(right,bot);
                    rectangle(mat, tl, br, color, lw);
                }
                if(print){
                    int w=b.w*im.w;
                    int h=b.h*im.h;
                    printf("%s: %.0f%% box: cx:%d cy:%d w:%d h:%d\n", names[class1], prob*100,ctr.x,ctr.y,w,h);
                }
                if(prob>max_prob){
                    max_prob=prob;
                    target_type=class1;
                    target_point=ctr;
                }
            }
        }
        double after_frame=get_wall_time();
        double delp=after_predict-before_predict;
        double delf=after_frame-before_frame;
        if(timeit){
            printf("predict: %2.1f ms FPS:%2.1f cycle:%2.1f FPS\n",1000*delp, 1.0/delp,1.0/delf);
        }
        if(display){
            if(max_prob>0){
                circle(mat, target_point, 15, target_color, 2);
                drawMarker(mat, target_point, target_color, 2);
            }
            outputStream.PutFrame(mat);
        }
        if(publish){
            table->PutNumber("FPS",int(1.0/delp));
            table->PutNumber("TargetType", target_type+1);
            table->PutNumber("TargetScore", int(max_prob*100));
            table->PutNumber("TargetX", target_point.x);
            table->PutNumber("TargetY", target_point.y);
        }
        before_frame=after_frame;
        free_image(im_s);
    }
}



