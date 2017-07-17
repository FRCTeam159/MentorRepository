
#SHELL = sh -xv
#FLAGS := -DCOMPILE_GPU=1
CAFFE_ROOT=$(HOME)/ssd-caffe/distribute

LDIRS := -L$(CAFFE_ROOT)/lib -L/usr/local/lib -L/usr/local/cuda/lib -L/usr/local/cuda/targets/aarch64-linux/lib

WPILIBS := -lntcore -lCameraServer -lcscore
CVLIBS := -lopencv_highgui -lopencv_core -lopencv_calib3d -lopencv_contrib -lntcore -lopencv_imgproc -lopencv_video -lopencv_videoio 
CUDALIBS := -lcudart -lopencv_cudaimgproc
CAFFELIBS := -lglog -lgflags -lprotobuf -lboost_system -lboost_filesystem -lboost_regex -lm -lhdf5_serial_hl -lhdf5_serial -lcaffe

LIBS := $(CVLIBS) $(CUDALIBS) $(WPILIBS) $(CAFFELIBS)

CPP_SRCS := SSDProc.cpp Detector.cpp

OBJS := SSDProc.o Detector.o

SRCDIR=../src

INCLUDES=-I$(CAFFE_ROOT)/include -I../include -I/usr/local/include -I/usr/local/cuda/targets/aarch64-linux/include

RPATH= -Xlinker -rpath $(CAFFE_ROOT)/lib -Xlinker -rpath /usr/local/lib

# Add inputs and outputs from these tool invocations to the build variables 

# All Target
all: SSDProc

# Tool invocations
SSDProc: $(OBJS) 
	@echo 'Building target: $@'
	g++ $(RPATH) $(FLAGS) $(LDIRS) -o "SSDProc"  $(OBJS) $(LIBS) 
	@echo 'Finished building target: $@'
	@echo ' '

# Other Targets
clean:
	-$(RM) $(OBJS) *.d SSDProc
	-@echo ' '

# Each subdirectory must supply rules for building sources it contributes
%.o: ../src/%.cpp Makefile
	@echo 'Building file: $<'
	@echo 'Invoking: Cross G++ Compiler'
	g++ -std=c++11  $(INCLUDES) -O3 -Wall -c -fmessage-length=0 -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

SSDProc.o: ../src/SSDProc.cpp 
Detector.o: ../src/Detector.cpp ../include/Detector.h

