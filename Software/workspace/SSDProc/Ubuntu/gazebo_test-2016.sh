export CAFFE_ROOT=/usr/local/ssd-caffe

killall SSDProc

export SSD_ROOT=$HOME/AI/ssd-caffe

cd $SSD_ROOT

export project=projects/TOTEBALL/models/SSD_300x300

export NET=$project/deploy.prototxt
export MODEL=$project/VGG_TOTEBALL_SSD_300x300_iter_2000.caffemodel
export VIDEO="http://192.168.1.107:5002/?action=stream?dummy=param.mjpg"
export OUTPUT=Ubuntu14.local

export APPDIR=$HOME/Robotics/MentorRepository/Software/workspace/SSDProc/Ubuntu

$APPDIR/SSDProc --thresh 0.3 --publish $OUTPUT --output "Annotated" --timeit $VIDEO $NET $MODEL
