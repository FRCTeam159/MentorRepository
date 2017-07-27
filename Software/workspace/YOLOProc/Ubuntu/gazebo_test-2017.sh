
killall YOLOProc

if [ "$#" -ne 1 ] ; then
   export HOST="Ubuntu16.local"
else
   export HOST="$1";
fi
echo $HOST

APPDIR=$HOME/Robotics/MentorRepository/Software/workspace/YOLOProc

DARKNET=$HOME/AI/darknet

cd $DARKNET

WEIGHTS=weights/tiny-yolo-balltote_2000.weights
CFG=cfg/tiny-yolo-2class.cfg

WEIGHTS=weights/yolo-300.weights
CFG=cfg/yolo.2_300x300_test.cfg

DATA=cfg/tapegear.data

STREAM="http://"$HOST":5002/?action=stream?dummy=param.mjpg"

$APPDIR/Ubuntu/YOLOProc --thresh 0.4 --publish $HOST --output "Annotated" ${DATA} ${CFG} ${WEIGHTS} ${STREAM}
