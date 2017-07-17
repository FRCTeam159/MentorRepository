
export PROJECT=$HOME/ssd-caffe/projects/TOTEBALL/models

killall SSDProc gazebo_test.sh

NET=$PROJECT/SSD_300x300/deploy.prototxt
WEIGHTS=$HOME/data/caffe/weights/VGG_TOTEBALL_SSD_300x300_iter_2000.caffemodel
STREAM="http://Ubuntu14.local:5002/?action=stream?dummy=param.mjpg"
OUTPUT=Ubuntu14.local

APPDIR=$HOME/SSDProc/Jetson-TX2

$APPDIR/SSDProc --thresh 0.3 --publish $OUTPUT --output "Annotated" --timeit $STREAM $NET $WEIGHTS

