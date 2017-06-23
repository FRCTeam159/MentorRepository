#!/usr/bin/env bash

WEIGHTS=weights/tiny-yolo-balltote_2000.weights
CFG=cfg/tiny-yolo-2class.cfg
FILE=$HOME/data/videos/balltote3.mp4
DATA=cfg/balltote.data

#darknet detector demo cfg/balltote.data ${CFG} ${WEIGHTS} ${FILE} -thresh 0.2 -w 320 -h 240 -fps 20 -clear
darknet detector demo ${DATA} ${CFG} ${WEIGHTS} ${FILE} -thresh 0.6
