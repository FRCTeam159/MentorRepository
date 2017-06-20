
RM := rm -rf

#LIBS := -lntcore -lopencv_core -lopencv_imgproc -lopencv_video -lopencv_videoio -lcscore

CPP_SRCS := src/CameraServer.cpp

OBJS := CameraServer.o 


# Add inputs and outputs from these tool invocations to the build variables 

# All Target
all: libCameraServer.so

# Tool invocations
libCameraServer.so: $(OBJS) Makefile
	@echo 'Building target: $@'
	g++ -Wl,-rpath, -Xlinker -rpath /usr/local/lib -fPIC=-shared -o "libCameraServer.so" -L/usr/local/lib $(OBJS) $(LIBS) 
	@echo 'Finished building target: $@'
	@echo ' '

# Other Targets
clean:
	-$(RM) $(OBJS) *.d libCameraServer.so
	-@echo ' '

install:
	sudo cp libCameraServer.so /usr/local/lib
	
# Each subdirectory must supply rules for building sources it contributes
%.o: src/%.cpp
	@echo 'Building file: $<'
	@echo 'Invoking: G++ Compiler'
	g++ -std=c++11 -I"include" -I/usr/local/include -I/usr/local/wpi/2017/sim/include -O0 -g3 -Wall -fPIC -c -fmessage-length=0 -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


CameraServer.o: src/CameraServer.cpp include/CameraServer.h include/CameraServer.inc

