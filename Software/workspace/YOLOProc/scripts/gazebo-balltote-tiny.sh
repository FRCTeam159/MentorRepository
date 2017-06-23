#!/usr/bin/env bash

rm -f /tmp/target-camera/*
WEIGHTS=weights/tiny-yolo-balltote_2000.weights

CFG=cfg/tiny-yolo-2class.cfg

FILE="http://192.168.1.107:5002/?action=stream?dummy=param.mjpg"
FILE=$HOME/data/videos/balltote3.mp4

#darknet detector demo cfg/balltote.data ${CFG} ${WEIGHTS} ${FILE} -thresh 0.2 -w 320 -h 240 -fps 20 -clear
darknet detector demo cfg/balltote.data ${CFG} ${WEIGHTS} ${FILE} -thresh 0.6 

