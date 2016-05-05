
pushd allwpilib/wpilibc/simulation
cp include/*.h $ALLWPILIB/wpilibc/simulation/include
cp src/*.cpp $ALLWPILIB/wpilibc/simulation/src
popd
pushd simulation/frc_gazebo_plugins
cp -R * $ALLWPILIB/simulation/frc_gazebo_plugins
popd
pushd $ALLWPILIB/build
make can_motor
make -j4 wpilibcSim
