Patches for 2016 gazebo simulation

1) allwpilib-simulation-patches.tar
 - Contains fixes for PID Controller bugs in 2016 allwpilib simulation code
 - Modified source files: 
   wpilibc/simulation/src/Notifier.cpp
   wpilibc/simulation/src/PIDController.cpp
 - patch instruction (note: only needed if building)
   > cp allwpilib-simulation-patches.tar 
   > cd ~/allwpilib
   > tar -xf allwpilib-simulation-patches.tar

2) wpilib-simulation-patches.tar
 - Contains rebuilt library that has PID controller bug fixes described above
   (simulation/lib/libwpilibcSim.so)
 - plugins directory includes soft links to renamed .so files 
   o in latest 2016 wpilib bundle plugin names were changed from libgz_dc_motor.so to libdc_motor.so etc.
     but Solidworks sdf exporter creates entries in sdf file using old names 
 - Patch install instructions
   > cp wpilib-simulation-patches.tar 
   > cd ~/wpilib
   > tar -xf wpilib-simulation-patches.tar 
