################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
CPP_SRCS += \
../src/Subsystems/DriveTrain.cpp \
../src/Subsystems/Elevator.cpp \
../src/Subsystems/Shooter.cpp 

OBJS += \
./src/Subsystems/DriveTrain.o \
./src/Subsystems/Elevator.o \
./src/Subsystems/Shooter.o 

CPP_DEPS += \
./src/Subsystems/DriveTrain.d \
./src/Subsystems/Elevator.d \
./src/Subsystems/Shooter.d 


# Each subdirectory must supply rules for building sources it contributes
src/Subsystems/%.o: ../src/Subsystems/%.cpp
	@echo 'Building file: $<'
	@echo 'Invoking: GCC C++ Compiler'
	g++ -std=c++0x -I"/home/dean/Robotics/DemoBots/CmdDiscusBot.demo/src" -I/usr/include -I/usr/local/wpi/include -I/usr/include/gazebo -I/usr/include/ignition/math2 -I/usr/include/sdformat -I"/home/dean/wpilib/user/cpp/include" -O0 -g3 -Wall -c -fmessage-length=0 -MMD -MP -MF"$(@:%.o=%.d)" -MT"$(@)" -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


