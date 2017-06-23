
killall SSDProc

export SSD_ROOT=$HOME/AI/ssd-caffe

cd $SSD_ROOT

export project=projects/TAPEGEAR/models/SSD_300x300

export NET=$project/deploy.prototxt
export MODEL=$project/VGG_TAPEGEAR_SSD_300x300_iter_1000.caffemodel
export VIDEO="http://192.168.1.107:5002/?action=stream?dummy=param.mjpg"
export OUTPUT=Ubuntu14.local

export APPDIR=$HOME/Robotics/MentorRepository/Software/workspace/SSDProc/Ubuntu

$APPDIR/SSDProc --thresh 0.5 --publish $OUTPUT --output "Annotated" --timeit $VIDEO $NET $MODEL


