
#SHELL = sh -xv
FLAGS := -DCOMPILE_GPU=1 -DGPU -DOPENCV -DCUDNN -fPIC

YOLO_LIBS=-L$(HOME)/darknet-cpp/lib

LDIRS := $(YOLO_LIBS) -L/usr/local/lib -L/usr/local/cuda/lib -L/usr/local/cuda/targets/aarch64-linux/lib

WPILIBS := -lntcore -lCameraServer -lcscore
CVLIBS := -lopencv_highgui -lopencv_core -lopencv_calib3d -lopencv_contrib -lntcore -lopencv_imgproc -lopencv_video -lopencv_videoio 
CUDALIBS := -lcudart -lopencv_cudaimgproc -lcudnn  -lcublas -lcurand
CAFFELIBS := -lglog -lgflags -lprotobuf -lboost_system -lboost_filesystem -lboost_regex -lm -lhdf5_serial_hl -lhdf5_serial
#YOLOLIB := -ldarknet_c -lstdc++ -pthread
YOLOLIB := -ldarknet-cpp-shared -pthread

LIBS := $(CVLIBS) $(CUDALIBS) $(WPILIBS) $(CAFFELIBS) $(YOLOLIB)

LDFLAGS = `pkg-config --libs opencv` 
CPP_SRCS := YOLOProc.cpp

OBJS := YOLOProc.o

SRCDIR=../src

YOLO_INCLUDES=-I$(HOME)/darknet-cpp/src 

INCLUDES=$(YOLO_INCLUDES) -I/usr/local/include -I/usr/local/cuda/targets/aarch64-linux/include  

RPATH= -Xlinker -rpath $(YOLO_LIBS) -Xlinker -rpath -Xlinker -rpath /usr/local/lib

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


