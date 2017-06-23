#!/usr/bin/env bash

LOG=logs/balltote_tiny_train.txt
exec &> >(tee "$LOG")
echo Logging output to "$LOG"

WEIGHTS=weights/yolo-tiny-2class.1000.weights
CFG=cfg/tiny-yolo-2class.cfg

#darknet detector train cfg/voc.data cfg/yolo-voc.2.0.train.cfg darknet19_448.conv.23
#darknet detector train cfg/balltote.data cfg/tiny-yolo-balltote-train.cfg output/tiny-yolo-balltote-train_100-last.weights
time darknet detector train cfg/balltote.data ${CFG} ${WEIGHTS}

