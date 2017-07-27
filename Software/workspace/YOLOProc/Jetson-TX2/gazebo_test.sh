
killall YOLOProc gazebo_test.sh

if [ "$#" -ne 1 ] ; then
   export HOST="Ubuntu16.local"
else
   export HOST="$1";
fi
echo $HOST

APPDIR=$HOME/YOLOProc/Jetson-TX2
DATADIR=$HOME/data/yolo


WEIGHTS=${DATADIR}/yolo-300.weights
CFG=${DATADIR}/yolo.2_balltote_300x300.cfg

WEIGHTS=${DATADIR}/tiny-yolo-balltote_2000.weights
CFG=${DATADIR}/tiny-yolo-balltote.cfg

WEIGHTS=${DATADIR}/yolo-2.weights
CFG=${DATADIR}/yolo.2_balltote.cfg

WEIGHTS=${DATADIR}/yolo-300.weights
CFG=${DATADIR}/yolo.2_balltote_300x300.cfg

DATA=${DATADIR}/balltote.data

STREAM="http://"$HOST":5002/?action=stream?dummy=param.mjpg"

${APPDIR}/YOLOProc --thresh 0.3 --publish $HOST --output "Annotated" ${DATA} ${CFG} ${WEIGHTS} ${STREAM}
