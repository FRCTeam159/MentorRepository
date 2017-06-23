
killall YOLOProc

export APPDIR=$HOME/Robotics/MentorRepository/Software/workspace/YOLOProc

cd $APPDIR

export WEIGHTS=weights/tiny-yolo-balltote_2000.weights
export CFG=cfg/tiny-yolo-2class.cfg
export DATA=cfg/balltote.data
export VIDEO="http://192.168.1.107:5002/?action=stream?dummy=param.mjpg"
#export VIDEO=$HOME/data/videos/balltote3.mp4

export OUTPUT=Ubuntu14.local

Ubuntu/YOLOProc --thresh 0.4 --publish $OUTPUT --output "Annotated" --print ${DATA} ${CFG} ${WEIGHTS} ${VIDEO}
