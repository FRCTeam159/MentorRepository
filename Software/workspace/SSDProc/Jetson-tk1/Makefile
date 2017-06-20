# Makefile for Jetson TK1 build (with gpu support)
RM := rm -rf

FLAGS := -DCOMPILE_GPU=1
LDIRS := -L/usr/local/lib -L/usr/local/cuda-6.5/targets/armv7-linux-gnueabihf/lib -L../build/lib

WPILIBS := -lntcore -lCameraServer -lcscore
CVLIBS := -lopencv_highgui -lopencv_core -lopencv_calib3d -lopencv_contrib -lntcore -lopencv_imgproc -lopencv_video -lopencv_videoio 
CUDALIBS := -lcudart -lopencv_cudaimgproc
CAFFELIBS := -lglog -lgflags -lprotobuf -lboost_system -lboost_filesystem -lboost_regex -lm -lhdf5_hl -lhdf5 -lcaffe

LIBS := $(CVLIBS) $(CUDALIBS) $(WPILIBS) $(CAFFELIBS)

CPP_SRCS := SSDProc.cpp Detector.cpp 

OBJS := SSDProc.o Detector.o  

RPATH := -Xlinker -rpath $(HOME)/SSDCaffe/build/lib -Xlinker -rpath /usr/local/lib -Xlinker -rpath /usr/local/cuda/lib

# Add inputs and outputs from these tool invocations to the build variables 

# All Target
all: SSDProc

# Tool invocations
SSDProc: $(OBJS) Makefile
	@echo 'Building target: $@'
	g++  $(RPATH) $(FLAGS) $(LDIRS) -o "SSDProc"  $(OBJS) $(LIBS) 
	#nvcc $(FLAGS) -o "ImageProc" -L/usr/local/lib $(OBJS) $(LIBS) 
	@echo 'Finished building target: $@'
	@echo ' '

# Other Targets
clean:
	-$(RM) $(OBJS) *.d ImageProc
	-@echo ' '

# Each subdirectory must supply rules for building sources it contributes
%.o: ../src/%.cpp Makefile
	@echo 'Building file: $<'
	@echo 'Invoking: Cross G++ Compiler'
	g++ -std=c++11 -I../include -I/usr/local/cuda/targets/armv7-linux-gnueabihf/include -I/usr/local/include -I/usr/local/include/cscore -O3 -Wall -c -fmessage-length=0 -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


SSDProc.o: ../src/SSDProc.cpp 
Detector.o: ../src/Detector.cpp ../include/Detector.h

