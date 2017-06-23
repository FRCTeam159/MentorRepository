#!/usr/bin/env bash

IMAGE=/home/dean/data/frc-robotics/ball_tote_devkit/data/JPEGImages/0222.jpg
IMAGE="/home/dean/data/frc-robotics/ball-tote/ball/default_ShooterCamera(1)-0246.jpg"

WEIGHTS=weights/tiny-yolo-balltote_2000.weights

CFG=cfg/tiny-yolo-2class.cfg

darknet detector test cfg/balltote.data ${CFG} ${WEIGHTS} ${IMAGE}