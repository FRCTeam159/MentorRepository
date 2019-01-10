
#set HALSIM_EXTENSIONS=
#set PATH="/c/Users/Alpiner/Robotics/MentorRepository/Software/workspace/JVisionThreadTestVSC/build/tmp/jniExtractDir:/c/Program Files/Java/jdk-11.0.1/bin:"
#gradlew clean
gradlew build
gradlew jar
export BUILD_DIR=$HOME/Robotics/MentorRepository/Software/workspace/JVisionThreadTestVSC/build
export LD_LIBRARY_PATH=$BUILD_DIR/tmp/jniExtractDir
java "-Djava.library.path=$BUILD_DIR/tmp/jniExtractDir" "-jar" "$BUILD_DIR/libs/JVisionThreadTestVSC.jar"
