#include <wpilib.h>

//Command Register
//Write 0x04 to Register 0x00: Take acquisition & correlation processing with DC correction

//0x01 - Mode/Status (control_reg[1]:)
//
//Bit	Function	Notes
//Bit 7	Eye Safe	This bit will go high if eye-safety protection has been activated
//Bit 6	Error Detection	Process error detected / measurement invalid
//Bit 5	Health	“1” if good, “0” if bad
//Bit 4	Secondary return	Secondary return detected above correlation noise floor threshold
//Bit 3	Signal not valid	Indicates that the signal correlation peak is equal to or below correlation record noise threshold
//Bit 2	Sig overflow flag	Overflow detected in correlation process associated with a signal acquisition
//Bit 1	Ref overflow flag	Overflow detected in correlation process associated with a reference acquisition
//Bit 0	Ready Status	“0” is ready for new command, “1” is busy with acquisition
//Health status indicates that the preamp is operating properly, transmit power is active and a reference pulse has been processed and has been stored.

class Lidar
{
public:
	Lidar();
	void AquireDistance(Timer*);
private:
	enum Address {ADDRESS_DEFAULT=0x62}; // default I2C bus address for the LIDAR Lite v2
	enum Register {COMMAND=0x00, STATUS=0x01, DISTANCE_1_2=0x8f};
	enum Command {ACQUIRE_DC_CORRECT=0x04};
	enum NumberOfRegistersToRead {READ_1_REGISTER=0x01, READ_2_REGISTERS=0x02};
	enum NumberOfRegistersToWrite {WRITE_1_REGISTER=0x01};
	I2C* I2CBus;

	bool Busy()
	{
		unsigned char Status[Lidar::READ_1_REGISTER];
		unsigned char statusRegister[Lidar::WRITE_1_REGISTER];
		statusRegister[Lidar::WRITE_1_REGISTER-1] = Lidar::STATUS;

		/**********read status**********/
		if ( I2CBus->WriteBulk(statusRegister, Lidar::WRITE_1_REGISTER)) {printf ( "WriteBulk status failed! line %d\n", __LINE__ ); return true;}
		if ( I2CBus->ReadOnly(Lidar::READ_1_REGISTER, Status) ) {printf ( "ReadOnly status failed! line %d\n", __LINE__ ); return true;}
		//printf("Status at line %d %0x, bit0=%0x\n", __LINE__, Status[0], Status[0] & (unsigned char)0x01);
		return (Status[0] & (unsigned char)0x01); // bit 0 is LIDAR Lite v2 busy bit
	};
};

Lidar::Lidar()
{
	I2CBus = new I2C(I2C::kMXP, Lidar::ADDRESS_DEFAULT);
	Wait(1.);
}

void Lidar::AquireDistance(Timer* m_timer)
{
	unsigned char distance[Lidar::READ_2_REGISTERS];
	unsigned char distanceRegister_1st[Lidar::WRITE_1_REGISTER];
	distanceRegister_1st[Lidar::WRITE_1_REGISTER-1] = Lidar::DISTANCE_1_2;

	printf("Time =  %f starting Lidar::AquireDistance\n", m_timer->Get());

	// do{Wait(.0001);} while (Busy());
	Wait(0.5);

	printf("Time =  %f acquiring distance\n", m_timer->Get());

	/***********acquire distance**********/		//	WriteBulk() also works
	if ( I2CBus->Write(Lidar::COMMAND, Lidar::ACQUIRE_DC_CORRECT) )printf ( "Write operation failed! line %d\n", __LINE__ ); // initiate distance acquisition with DC stabilization

	// do{Wait(.0001);} while (Busy());

	printf("Time =  %f reading distance\n", m_timer->Get());

	/**********read distance**********/     // Read() does not work
	if ( I2CBus->WriteBulk(distanceRegister_1st, Lidar::WRITE_1_REGISTER)) printf ( "WriteBulk distance failed! line %d\n", __LINE__ );
	else
	if ( I2CBus->ReadOnly(Lidar::READ_2_REGISTERS, distance)) printf ( "ReadOnly distance failed! line %d\n", __LINE__ );

	I2CBus->Read(DISTANCE_1_2, Lidar::READ_2_REGISTERS, distance);
	//int retval1, retval2;
	//retval1 = I2CBus->Read(0x0f, 1, &distance[0]);
	//retval2 = I2CBus->Read(0x10, 1, &distance[1]);
	//printf("retval1 = %d, retval2 = %d, distance=%d\n",
	//		retval1, retval2, ((distance[1] * 256) + distance[0]));

	unsigned int dist = (unsigned int)(distance[0]<<8) + (unsigned int)(distance[1]);

	printf("Time =  %f, Distance= %d (0x%0x)\n", m_timer->Get(), dist, dist);
}
/*
Time =  122.058885 starting Lidar::AquireDistance
Time =  122.060009 acquiring distance
Time =  122.063925 reading distance
Time =  122.064248, Distance= 748 (0x2ec)
Time =  122.064740 starting Lidar::AquireDistance
Time =  122.065287 acquiring distance
Time =  122.069185 reading distance
Time =  122.069577, Distance= 747 (0x2eb)
Time =  122.069986 starting Lidar::AquireDistance
Time =  122.070534 acquiring distance
Time =  122.074098 reading distance
Time =  122.074666, Distance= 745 (0x2e9)
Time =  122.076632 starting Lidar::AquireDistance
Time =  122.078220 acquiring distance
Time =  122.082863 reading distance
Time =  122.083228, Distance= 742 (0x2e6)
Time =  122.083632 starting Lidar::AquireDistance
Time =  122.084104 acquiring distance
Time =  122.088063 reading distance
Time =  122.088394, Distance= 738 (0x2e2)
Time =  122.088820 starting Lidar::AquireDistance
Time =  122.089350 acquiring distance
Time =  122.092824 reading distance
Time =  122.093151, Distance= 734 (0x2de)
Time =  122.093567 starting Lidar::AquireDistance
Time =  122.094196 acquiring distance
Time =  122.098187 reading distance
Time =  122.098577, Distance= 731 (0x2db)
Time =  122.099367 starting Lidar::AquireDistance
Time =  122.099903 acquiring distance
Time =  122.104642 reading distance
Time =  122.104975, Distance= 727 (0x2d7)
Time =  122.105488 starting Lidar::AquireDistance
Time =  122.106017 acquiring distance
Time =  122.109924 reading distance
Time =  122.110301, Distance= 726 (0x2d6)
Time =  122.110704 starting Lidar::AquireDistance
Time =  122.111246 acquiring distance
Time =  122.114823 reading distance
Time =  122.115169, Distance= 724 (0x2d4)
Time =  122.116194 starting Lidar::AquireDistance
Time =  122.116760 acquiring distance
Time =  122.120257 reading distance
Time =  122.120658, Distance= 725 (0x2d5)
Time =  122.121642 starting Lidar::AquireDistance
Time =  122.122193 acquiring distance
Time =  122.126196 reading distance
Time =  122.126603, Distance= 726 (0x2d6)
Time =  122.127277 starting Lidar::AquireDistance
Time =  122.127826 acquiring distance
Time =  122.131726 reading distance
Time =  122.132122, Distance= 729 (0x2d9)
Time =  122.132534 starting Lidar::AquireDistance
Time =  122.133079 acquiring distance
Time =  122.136700 reading distance
Time =  122.137073, Distance= 726 (0x2d6)
Time =  122.138788 starting Lidar::AquireDistance
Time =  122.139304 acquiring distance
Time =  122.143286 reading distance
Time =  122.143613, Distance= 727 (0x2d7)
Time =  122.144488 starting Lidar::AquireDistance
Time =  122.145007 acquiring distance
Time =  122.148530 reading distance
Time =  122.148852, Distance= 723 (0x2d3)
Time =  122.149324 starting Lidar::AquireDistance
Time =  122.149844 acquiring distance
Time =  122.153407 reading distance
Time =  122.153720, Distance= 718 (0x2ce)
Time =  122.154977 starting Lidar::AquireDistance
Time =  122.155539 acquiring distance
Time =  122.159988 reading distance
Time =  122.160356, Distance= 712 (0x2c8)
Time =  122.161317 starting Lidar::AquireDistance
Time =  122.161829 acquiring distance
Time =  122.165851 reading distance
Time =  122.166177, Distance= 704 (0x2c0)
Time =  122.166809 starting Lidar::AquireDistance
Time =  122.167312 acquiring distance
Time =  122.172206 reading distance
Time =  122.172529, Distance= 698 (0x2ba)
Time =  122.173021 starting Lidar::AquireDistance
Time =  122.173564 acquiring distance
Time =  122.177589 reading distance
Time =  122.178006, Distance= 688 (0x2b0)
Time =  122.179527 starting Lidar::AquireDistance
Time =  122.180103 acquiring distance
Time =  122.184005 reading distance
Time =  122.184403, Distance= 678 (0x2a6)
Time =  122.185326 starting Lidar::AquireDistance
Time =  122.185839 acquiring distance
Time =  122.190327 reading distance
Time =  122.190657, Distance= 664 (0x298)
Time =  122.191137 starting Lidar::AquireDistance
Time =  122.191677 acquiring distance
Time =  122.195729 reading distance
Time =  122.196101, Distance= 648 (0x288)
Time =  122.197042 starting Lidar::AquireDistance
Time =  122.197594 acquiring distance
Time =  122.202130 reading distance
Time =  122.202499, Distance= 634 (0x27a)
Time =  122.203457 starting Lidar::AquireDistance
Time =  122.203961 acquiring distance
Time =  122.208862 reading distance
Time =  122.209197, Distance= 619 (0x26b)
Time =  122.209681 starting Lidar::AquireDistance
Time =  122.210223 acquiring distance
Time =  122.215166 reading distance
Time =  122.215540, Distance= 602 (0x25a)
Time =  122.216480 starting Lidar::AquireDistance
Time =  122.217054 acquiring distance
Time =  122.221485 reading distance
Time =  122.221893, Distance= 583 (0x247)
Time =  122.222801 starting Lidar::AquireDistance
Time =  122.223353 acquiring distance
Time =  122.228808 reading distance
Time =  122.229222, Distance= 566 (0x236)
Time =  122.229642 starting Lidar::AquireDistance
Time =  122.230189 acquiring distance
Time =  122.234769 reading distance
Time =  122.235190, Distance= 554 (0x22a)
Time =  122.235991 starting Lidar::AquireDistance
Time =  122.236531 acquiring distance
Time =  122.241241 reading distance
Time =  122.241577, Distance= 549 (0x225)
Time =  122.242143 starting Lidar::AquireDistance
Time =  122.242704 acquiring distance
Time =  122.247255 reading distance
Time =  122.247579, Distance= 541 (0x21d)
Time =  122.248046 starting Lidar::AquireDistance
Time =  122.252606 reading distance
Time =  122.252922, Distance= 533 (0x215)
Time =  122.253426 starting Lidar::AquireDistance
Time =  122.258287 reading distance
Time =  122.258704, Distance= 524 (0x20c)
Time =  122.265380 reading distance
Time =  122.265712, Distance= 515 (0x203)
Time =  122.266444 starting Lidar::AquireDistance
Time =  122.271380 reading distance
Time =  122.271700, Distance= 510 (0x1fe)
Time =  122.272194 starting Lidar::AquireDistance
Time =  122.277550 reading distance
Time =  122.277920, Distance= 507 (0x1fb)
Time =  122.279535 acquiring distance
Time =  122.283440 reading distance
Time =  122.283857, Distance= 502 (0x1f6)
Time =  122.289197 reading distance
Time =  122.289595, Distance= 496 (0x1f0)
Time =  122.290004 starting Lidar::AquireDistance
Time =  122.295007 reading distance
Time =  122.295339, Distance= 498 (0x1f2)
Time =  122.300843 reading distance
Time =  122.301167, Distance= 492 (0x1ec)
*/
