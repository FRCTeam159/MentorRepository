
killall YOLOProc gazebo_test.sh

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

STREAM="http://Ubuntu14.local:5002/?action=stream?dummy=param.mjpg"
OUTPUT=Ubuntu14.local

${APPDIR}/YOLOProc --thresh 0.3 --publish $OUTPUT --output "Annotated" ${DATA} ${CFG} ${WEIGHTS} ${STREAM}
