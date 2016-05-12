Patches for 2016 gazebo simulation
1) wpilib.simulation.tar
 - Contains fixes for PID Controller bugs in 2016 allwpilib simulation code
 - Contains limited CANTalon class support in simulation build
 - For backwards compatability plugins directory includes soft links to renamed .so files 
   * in latest 2016 wpilib bundle plugin names were changed from libgz_dc_motor.so to libdc_motor.so etc.
     but Solidworks sdf exporter creates entries in sdf file using old names 
   * soft links not necessary if sdf file was created using modified gazebo_exporter
 - Patch install instructions
   > tar -xf  wpilib.simulation.tar -C ~/wpilib
2) gazebo_exporter dll
 - On Windows, copy the dll file to C:\ProgramFiles\GazeboExporter
 - TODO: add the dll file to the repo

