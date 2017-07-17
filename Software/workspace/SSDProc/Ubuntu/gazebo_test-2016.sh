export CAFFE_ROOT=/usr/local/ssd-caffe

killall SSDProc
#rm /tmp/target-camera/*
export SSD_ROOT=$HOME/AI/ssd-caffe

cd $SSD_ROOT

export project=projects/TOTEBALL/models/SSD_300x300

export NET=$project/deploy.prototxt
export MODEL=$project/VGG_TOTEBALL_SSD_300x300_iter_2000.caffemodel
export VIDEO="http://Ubuntu14.local:5002/?action=stream?dummy=param.mjpg"
export OUTPUT=Ubuntu14.local

export APPDIR=$HOME/Robotics/MentorRepository/Software/workspace/SSDProc/Ubuntu

$APPDIR/SSDProc --thresh 0.5 --publish $OUTPUT --output "Annotated" --timeit $VIDEO $NET $MODEL
