#!/bin/bash

echo $#
roxterm --tab-name=run -e build_and_run.sh
sleep 1;
roxterm --tab --tab-name=dashboard -e start_smartdashboard;
