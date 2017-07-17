CAFFE_ROOT=/usr/local/ssd-caffe
#SHELL = sh -xv
FLAGS := -DCOMPILE_GPU=1 -DGPU -DOPENCV -DCUDNN -fPIC

#YOLO_LIBS=-L$(HOME)/AI/darknet-git 
YOLO_LIBS=-L/usr/local/yolo/lib

#LDIRS := -L$(CAFFE_ROOT)/lib -L/usr/local/lib -L/usr/local/cuda/lib -L$(HOME)/wpilib/simulation/lib -L/usr/local/cuda/targets/x86_64-linux/lib -L/usr/local/yolo/lib
LDIRS := -L/usr/local/lib -L/usr/local/cuda/lib -L/usr/local/cuda/targets/x86_64-linux/lib -L/usr/local/yolo/lib -L$(HOME)/wpilib/simulation/lib $(YOLO_LIBS)

WPILIBS := -lntcore -lCameraServer -lcscore
CVLIBS := -lopencv_highgui -lopencv_core -lopencv_calib3d -lopencv_contrib -lntcore -lopencv_imgproc -lopencv_video -lopencv_videoio 
CUDALIBS := -lcudart -lopencv_cudaimgproc -lcudnn  -lcublas -lcurand
CAFFELIBS := -lglog -lgflags -lprotobuf -lboost_system -lboost_filesystem -lboost_regex -lm -lhdf5_hl -lhdf5 # -lcaffe
#YOLOLIB := -ldarknet_c -lstdc++ -pthread
YOLOLIB := -ldarknet -pthread

LIBS := $(CVLIBS) $(CUDALIBS) $(WPILIBS) $(CAFFELIBS) $(YOLOLIB)

LDFLAGS = `pkg-config --libs opencv` 
CPP_SRCS := YOLOProc.cpp

OBJS := YOLOProc.o

SRCDIR=../src

#YOLO_INCLUDES=-I$(HOME)/AI/darknet-git/src -I$(HOME)/AI/darknet-git/include
YOLO_INCLUDES=-I/usr/local/yolo/include 

#INCLUDES=-I../src -I$(CAFFE_ROOT)/include -I/usr/local/include -I$(HOME)/wpilib/simulation/include -I/usr/local/cuda/targets/x86_64-linux/include -I/usr/local/yolo/include 
INCLUDES=$(YOLO_INCLUDES) -I$(CAFFE_ROOT)/include -I/usr/local/include -I$(HOME)/wpilib/simulation/include -I/usr/local/cuda/targets/x86_64-linux/include  
#RPATH=-rpath $(HOME)/wpilib/simulation/lib:../build/lib
RPATH= -Xlinker -rpath $(CAFFE_ROOT)/lib -Xlinker -rpath $(HOME)/wpilib/simulation/lib -Xlinker -rpath /usr/local/yolo/lib
#RPATH= -Xlinker -rpath $(CAFFE_ROOT)/lib -Xlinker -rpath $(HOME)/wpilib/simulation/lib

# Add inputs and outputs from these tool invocations to the build variables 

# All Target
all: YOLOProc

# Tool invocations
YOLOProc: $(OBJS) Makefile
	@echo 'Building target: $@'
	g++ $(RPATH) $(FLAGS) $(LDIRS) -o "YOLOProc"  $(OBJS) $(LIBS) 
	@echo 'Finished building target: $@'
	@echo ' '

# Other Targets
clean:
	-$(RM) $(OBJS) *.d YOLOProc
	-@echo ' '

# Each subdirectory must supply rules for building sources it contributes
%.o: ../src/%.cpp Makefile
	@echo 'Building file: $<'
	@echo 'Invoking: Cross G++ Compiler'
	g++ -std=c++11 -fpermissive $(INCLUDES) $(FLAGS) -O3 -Wall -c -fmessage-length=0 -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

YOLOProc.o: ../src/YOLOProc.cpp 


