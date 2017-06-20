
export CAFFE_ROOT=/usr/local/ssd-caffe

killall SSDProc gazebo_test.sh

export DATAPATH=$HOME/data/frc-robotics/BallToteData/caffe

export NET=$DATAPATH/gazebo/deploy.prototxt
export MODEL=$DATAPATH/toteball.caffemodel
export STREAM="http://192.168.1.107:5002/?action=stream?dummy=param.mjpg"
export OUTPUT=Ubuntu14.local

APPDIR=$HOME/SSDProc/Jetson-tk1

$APPDIR/SSDProc --thresh 0.3 --publish $OUTPUT --timeit $STREAM $NET $MODEL
