
#export PROJECT=$HOME/ssd-caffe/projects/TOTEBALL/models

killall SSDProc gazebo_test.sh

if [ "$#" -ne 1 ] ; then
   export HOST="Ubuntu16.local"
else
   export HOST="$1";
fi
echo $HOST

NET=$HOME/data/caffe/models/SSD_300x300_deploy.prototxt
WEIGHTS=$HOME/data/caffe/weights/VGG_TOTEBALL_SSD_300x300_iter_2000.caffemodel

STREAM="http://"$HOST":5002/?action=stream?dummy=param.mjpg"

APPDIR=$HOME/SSDProc/Jetson-TX2

$APPDIR/SSDProc --thresh 0.3 --publish $HOST --output "Annotated" --timeit $STREAM $NET $WEIGHTS

