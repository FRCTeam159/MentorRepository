killall SSDProc
#rm /tmp/target-camera/*
export SSD_ROOT=$HOME/Robotics/ssd-caffe

if [ "$#" -ne 1 ] ; then
   export HOST="Ubuntu16.local"
else
   export HOST="$1";
fi
echo $HOST

cd $SSD_ROOT
PROJECT=$SSD_ROOT/projects/TAPEGEAR/models/SSD_300x300

NET=$PROJECT/deploy.prototxt
MODEL=$PROJECT/VGG_TAPEGEAR_SSD_300x300_iter_2000.caffemodel
STREAM="http://"$HOST":5002/?action=stream?dummy=param.mjpg"

APPDIR=$HOME/Robotics/MentorRepository/Software/workspace/SSDProc/Ubuntu

$APPDIR/SSDProc --thresh 0.5 --publish $HOST --output "Annotated" --timeit $STREAM $NET $MODEL


