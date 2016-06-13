#include "delay.h"
#include "led.h"
#include "wave.h"
#include "motor.h"
#include "pid.h"
#include "sensor.h"
#include "timer.h"
#include "Uart.h"
#include "UartProtocol.h"
#include "mpu6050.h"
#include "inv_mpu_dmp_motion_driver.h"
#include "inv_mpu.h"
#include "DetectionLogic.h"
#include "RC522.h"
#include "adc.h"

float pitch,roll,yaw;

u8 buffdata[ RECV_BUFF_LEN+SEND_BUFF_LEN ];
u16 flen=0xffff,blen=0xffff;
//u16 fmaxlen=10,bmaxlen=10;//������̽�����(cm)
//u8 fth=69;//�жϷ���Ȩֵ
//u8 pth=10;//�ж�·��Ȩֵ
//s8 turnLeftSpeed=100,turnRightSpeed=-100;//ת���ٶ�
//s8 StatsCode[9]={-70,-45,-37,-30,0,30,37,45,70};
//PIDStruct leftspeed,rightspeed;
//u8 stop=0;

//void wait(u8 time)
//{
//	u32 i;
//	while(time--)
//		for(i=0;i<255;i++);
//}

void BatteryCheckInit(){
	GPIO_AI(GPIOA,0,00000010);
	SETBIT(GPIOA->ODR,BIT2);
}


#define ADCBit 12
#define ADCVOP 3300
#define ADCLsb (31-ADCBit)

u16 toVoltage(u16 value) {
	u32 v = value;
    v <<=ADCLsb;
	v /= (((u32)1<<(ADCBit+ADCLsb))/ADCVOP);
	return v;
}


#define Lbattery (u32)10800 //�����͵�ѹ
#define Hbattery (u32)12600 //�����ߵ�ѹ
#define R1 (u32)995       //��ѹ������ֵ
#define R2 (u32)196         
#define RV(v) ((v * (R1+R2)) / R2)

//��ȡ����0-100%
u8 getBatteryValue() {
	u32 v = toVoltage(ADC.getADCValue(1));
	v = RV(v);
	if (v < Lbattery)return 0;
	if (v > Hbattery)v = Hbattery;
	return (v - Lbattery) * 100 / (Hbattery - Lbattery);
}

u8 lastBatteryValue;
void UpdateBatteryValue(){
	lastBatteryValue=getBatteryValue();
}

void tick1()
{
	static u8 ch;
	u32 temp;
	//flen=Wave.Time2Length(Wave.GetTime(0));
	//blen=Wave.Time2Length(Wave.GetTime(1));
	//return;
	while(1)
	{		
		switch(ch)
		{
			case 0:
				Trig1=1;
				delay_us(11);
				//wait(40);
				Trig1=0;
				Timer.Delay(65);
				if(TIM3CH3_CAPTURE_STA&0x80)
				{
					temp=TIM3CH3_CAPTURE_STA&0x3f;
					temp*=65536;
					temp+=TIM3CH3_CAPTURE_VAL;
					
					flen=(float)temp*.017;
					
				}
				else
					flen=10000;
				TIM3CH3_CAPTURE_STA=0;
				ch=1;
				break;
			case 1:
				Trig2=1;
				delay_us(11);
				Trig2=0;
				Timer.Delay(65);
				if(TIM3CH4_CAPTURE_STA&0x80)
				{
					temp=TIM3CH4_CAPTURE_STA&0x3f;
					temp*=65536;
					temp+=TIM3CH4_CAPTURE_VAL;
					
					blen=(float)temp*.017;
					
				}
				else
					blen=10000;
				TIM3CH4_CAPTURE_STA=0;
				ch=0;
				break;
		}
	}
  //Timer.Tick.add(60);
}

u8 pathbuff[1024];
//u16 pathlen;
//u16 pathpos;

//u8 Statuscount=0;
//u8 lastStatus=0;
//u8 PathSelect=0,lastPathSelect=0;
//u8 leftselect,rightselect,midselect;
//u8 leftlevel=0,rightlevel=0;
//u8 runStatus=0;
//u16 StatusTick=0;
//s16 Lvalue=0,Rvalue=0;
//s16 lastLv=0,lastRv=0;
//u8 lvl,rvl;
//#define StatusM BIT0			//�������ĵ�λ��
//#define StatusL BIT1			//ƫ��
//#define StatusR BIT2			//ƫ��
//#define StatusA BIT3			//ȫ�� ·��
//#define StatusLD BIT4   //ƫ��ȫ������
//#define StatusRD BIT5   //ƫ��ȫ������

//#define WaveStop BIT6		//������ģ��������

//#define CheckStatus(x) (lastStatus&(x))
//#define CheckStatusZero() (Statuscount==0)

//#define RunNop 0
//#define RunNor 1
//#define RunTor 2
//#define RunTol 3
//#define RunPts 4
//#define RunTof 5
//#define RunTob 6
//#define RunStp 7

//#define ModeTol 0
//#define ModeTor 1
//#define ModeTof 2
//#define ModeTob 3
//#define ModeRst 4

//	u8 lc=0,rc=0;

//s16 CheckSensor()
//{
//	u8 i;
//	s16 v=0;
//	SensorStatus dat=Sensor.Read();	
//	CLRBIT(lastStatus,StatusM|StatusL|StatusR);
//	Lvalue=Rvalue=0;
//	Statuscount=0;
//	lc=rc=0;
//	for(i=0;i<MaxSensor;i++){
//		if(!(dat&BIT15))
//		{
//			Statuscount++; 
//			v+=StatsCode[i];
//			if(i>4)
//			{
//				lc++;
//				Lvalue+=StatsCode[i];
//				if(Lvalue>fth)
//					SETBIT(lastStatus,StatusL);
//			}
//			else if(i<4)
//			{
//				rc++;
//				Rvalue+=StatsCode[i];
//				if(Rvalue<-fth)
//					SETBIT(lastStatus,StatusR);
//			}
//			else{
//				SETBIT(lastStatus,StatusM);
//			}
//		}
//		dat<<=1;
//	}
//	if(Statuscount==MaxSensor)
//		SETBIT(lastStatus,StatusA);
//	Lvalue/=lc;
//	Rvalue=-Rvalue;
//	Rvalue/=rc;
//	return v/Statuscount;
//}

//s8 lspeed,rspeed;
//s8 lastflen;

//s16 lastyaw;

//void StopRun()
//{
//	pathpos=0;
//	runStatus=RunNop;
//	PathSelect=0;
//	lspeed=rspeed=0;
//	Motor.Speed(0,0);
//	Motor.Speed(1,0);
//}

//u8 WaveDetection()
//{
//	if(flen<fmaxlen||blen<fmaxlen)
//	{
//		if(CheckStatus(WaveStop))
//		{
//			return 0;
//		}
//		else{
//			SETBIT(lastStatus,WaveStop);
//			if(flen<fmaxlen)
//				flen=fmaxlen;
//			else
//				blen=fmaxlen;
//		}
//	}
//	else{
//		if(flen>fmaxlen&&blen>fmaxlen)
//		{
//			CLRBIT(lastStatus,WaveStop);
//		}
//	}
//	return 1;
//}

//s16 SensorDetection()
//{
//	s16 v=CheckSensor();
//	
//	if(lvl)
//			if(--lvl==0)
//				lastLv=0;
//			
//		if(rvl)
//			if(--rvl==0)
//				lastRv=0;
//			
//	if(stop)
//	{
//		if(!CheckStatusZero())
//		{
//			runStatus=RunNop;
//			stop=0;
//		}
//		else if(--stop==0)
//		{
//			lastPathSelect=PathSelect;
//			PathSelect=0;
//			SETBIT(PathSelect,StatusA);
//			runStatus=RunNor;
//			StatusTick=0;
//			return 0;
//		}
//		else
//		{
//			runStatus=RunStp;
//			return 0;
//		}
//	}
//	
//	
//	
//	if(!StatusTick&&runStatus==RunNop)
//	{
//		
//		if(Lvalue)
//		{
//			if(Lvalue>lastLv){
//				lastLv=Lvalue;
//				lvl+=10;
//			}
//			else if(Lvalue==lastLv){
//				lvl++;
//			}
//		}
//		
//		if(Rvalue)
//		{
//			if(Rvalue>lastRv){
//				lastRv=Rvalue;
//				rvl+=10;
//			}
//			else if(Rvalue==lastRv)
//				rvl++;
//		}
//		
//		
//		
//		if(((CheckStatus(StatusL)&&lvl>20)||(CheckStatus(StatusR)&&rvl>20)))
//		{
//			lastPathSelect=PathSelect;
//			leftselect=rightselect=midselect=0;
//			PathSelect=0;
//			StatusTick=500;
//			runStatus=RunPts;
//			lastRv=lastLv=0;
//		}
//		else if(CheckStatusZero())
//		{
//			stop=100;
//			runStatus=RunStp;
//			return 0;
//		}
//	}
//	else
//	{
//		if(runStatus==RunPts)
//		{
//				if(CheckStatus(StatusL))
//					leftselect++;
//				if(CheckStatus(StatusR))
//					rightselect++;
//				if(CheckStatus(StatusM))
//					midselect++;
//				if(StatusTick>50)
//				{
//					v/=16;
//					if(!CheckStatus(StatusR|StatusL))
//					{
//						StatusTick=50;
//					}
//				}
//				else{
//					v*=2;
//				}
//				if(StatusTick==1)
//				{
//					if(leftselect>pth)
//					{
//						SETBIT(PathSelect,StatusL);
//					}
//					if(rightselect>pth)
//					{
//						SETBIT(PathSelect,StatusR);
//					}
//					if(midselect>pth)
//						SETBIT(PathSelect,StatusM);
//					if(!(PathSelect&(StatusL|StatusR|StatusM)))
//						SETBIT(PathSelect,StatusA);
//					runStatus=RunNor;
//					StatusTick=0;
//				}
//		}
//	}
//	return v;
//}

//void MagneticRun(){
//	s16 speed=0;
////	s8 s1,s2;
//	s16 vy=(s16)yaw+180;
////u8 oldStatus=runStatus;
//	if(!WaveDetection()){
//		lspeed=rspeed=0;
//		Motor.Speed(0,0);
//		Motor.Speed(1,0);
//		return;
//	}
//	
//	speed=SensorDetection();
//	
//	if(runStatus==RunStp)
//		return;
//	
////	if(v==0&&(lastPathSelect&StatusA))
////	{
////		if(!CheckStatus(StatusL|StatusR|StatusM))
////		{
////			if(stop)
////			{
////				if(--stop==0)
////				{
////					pathpos=0;
////					runStatus=RunNop;
////					lspeed=rspeed=0;
////					Motor.Speed(0,0);
////					Motor.Speed(1,0);
////				}
////				return;
////			}
////		}
////		else{
////			stop=0;
////		}
////		if(!CheckStatus(StatusL|StatusR|StatusM)){
////			stop=80;
////			return;
////		}
////	}
////	else{
////		stop=0;
////	}
//	if(runStatus==RunNor)
//	{
//		if(pathlen!=0&&pathpos<pathlen)
//		{
//			if(!StatusTick){
////				switch(PathSelect)
////				{
////					case StatusL|StatusM:
////					case StatusL:
////					if(pathbuff[pathpos]!=ModeTol)
////					{
////						speed=0;
////					}
////					pathpos++;
////					StatusTick=1000;
////					runStatus=RunTol;
////					lastyaw=((s16)yaw+180+90)%360;
////						break;
////					case StatusR|StatusM:
////					case StatusR:
////					if(pathbuff[pathpos]!=ModeTor)
////					{
////						speed=0;
////					}
////					pathpos++;
////					StatusTick=1000;
////					runStatus=RunTor;
////					lastyaw=((s16)yaw+180-90)%360;
////						break;
////					case StatusA:
////					if(pathbuff[pathpos]!=ModeTob)
////					{
////						StopRun();
////						break;
////					}
////					pathpos++;
////					StatusTick=1000;
////					runStatus=RunTob;
////				  lastyaw=((s16)yaw)%360;
////						break;
////				}
//				
//				if(PathSelect==0)
//				{
//					StopRun();
//					return;
//				}
//				switch(pathbuff[pathpos])
//				{
//					case ModeTol:
//						if(PathSelect&StatusL){
//							StatusTick=1000;
//							runStatus=RunTol;
//							lastyaw=((s16)yaw+180+90)%360;
//							pathpos++;
//						}
//						if(PathSelect&StatusR){
//							speed=0;
//						}
//						if(PathSelect&(StatusA))
//						{
//							StopRun();
//							return;
//						}
//						break;
//					case ModeTor:
//						if(PathSelect&StatusR){
//							StatusTick=1000;
//							runStatus=RunTor;
//							lastyaw=((s16)yaw+180-90)%360;
//							pathpos++;
//						}
//						if(PathSelect&StatusL){
//							speed=0;
//						}
//						if(PathSelect&(StatusA))
//						{
//							StopRun();
//							return;
//						}
//						break;
//					case ModeTof:
//						if(PathSelect&(StatusR|StatusL)){
//							StatusTick=10;
//							speed=0;
//							pathpos++;
//						}
//						if(PathSelect&(StatusA))
//						{
//							StopRun();
//							return;
//						}
//						break;
//					case ModeTob:
//						if(PathSelect&(StatusA)){
//							StatusTick=1000;
//							runStatus=RunTob;
//							lastyaw=((s16)yaw)%360;
//							pathpos++;
//						}
//						else if(PathSelect)
//						{
//							speed=0;
//							runStatus=RunNop;
//						}
//						
//						break;
//				}
//				PathSelect=0;
//				if(pathpos==pathlen)
//					pathpos=0;
//			}
//		}
//		else{
//				StopRun();
//				return;
//		}
//	}
//	
////	s1=PID.Update(&leftspeed, +speed);
////	s2=PID.Update(&rightspeed, -speed);
////	if(s1>32)
////		s1=32;
////	if(s1<-32)
////		s1=-32;
////	if(s2>32)
////		s2=32;
////	if(s2<-32)
////		s2=-32;
//	if(StatusTick)
//	{
//			switch(runStatus)
//			{
//				case RunNop:
//					if(StatusTick!=0)
//					{
//						speed*=2;
//					}
//					break;
//				case RunTor:speed=turnRightSpeed;
//					if(StatusTick>500)
//					{
//						if(CheckStatus(StatusR))
//							StatusTick=500;
//						
//						vy=lastyaw-vy;
//						if(vy>180)
//							vy=-(vy-360);					
//						else if(vy<0)
//							vy=-vy;
//						
//						if(vy<5)
//							StatusTick=100;
//					}
//					else if(StatusTick>100)
//					{
//						if(CheckStatus(StatusM))
//						{
//							StatusTick=50;
//							runStatus=RunNop;
//						}
//					}
//					break;
//				case RunTol:speed=turnLeftSpeed;
//					if(StatusTick>500)
//					{
//						if(CheckStatus(StatusL))
//							StatusTick=500;
//							
//						vy=lastyaw-vy;
//						if(vy>180)
//							vy=-(vy-360);
//						else if(vy<0)
//							vy=-vy;
//							
//						if(vy<5)
//							StatusTick=100;
//					}
//					else if(StatusTick>100)
//					{
//						if(CheckStatus(StatusM))
//						{
//							StatusTick=50;
//							runStatus=RunNop;
//						}
//					}
//					break;
//				case RunTob:speed=turnLeftSpeed;
//					if(StatusTick>1500){
//						if(CheckStatus(StatusL))
//							StatusTick=500;
//					}
//					else if(StatusTick>1000){
//						if(!CheckStatus(StatusR|StatusL)&&(CheckStatus(StatusM)||vy<10))
//							StatusTick=1000;
//					}
//					else if(StatusTick>500){
//						if(CheckStatus(StatusL))
//							StatusTick=500;
//						vy=lastyaw-vy;
//						if(vy>180)
//							vy=-(vy-360);
//						else if(vy<0)
//							vy=-vy;
//						if(vy<10)
//							StatusTick=100;
//					}
//					else if(StatusTick>50){
//						if(CheckStatus(StatusM))
//						{
//							StatusTick=50;
//							runStatus=RunNop;
//						}
//					}
//					break;
//			}
//		
//		if(--StatusTick==0)
//			runStatus=RunNop;
//	}
////	lspeed=s1;
////	rspeed=s2;
//	Motor.Speed(0,lspeed=PID.Update(&leftspeed, +speed));
//	Motor.Speed(1,rspeed=PID.Update(&rightspeed, -speed));
//}

u16 lastAction;
u16 lastCodeID;
u8 EnableTime=0;
u32 lastTick;
u32 lastTime;
u32 lastIDCard;
u8 RecvCode;
void AliveEvent(UartEvent e)
{
	
}

void GetDataEvent(UartEvent e)
{
	u8 i;
	s16 yawvalue;
	SensorStatus val=Sensor.Read();
  //mpu_dmp_get_data(&pitch,&roll,&yaw);
	yawvalue=yaw;
	e->WriteDWord(0);
	e->WriteDWord(val&0xffffffff);
	e->WriteWord(flen);
	e->WriteWord(blen);
	e->WriteByte(MotorLeftSpeed);//lspeed
	e->WriteByte(MotorRightSpeed);//rspeed
  e->WriteWord(yawvalue);
	e->WriteWord(PathPos);
	e->WriteByte(lastAction);
	e->WriteByte(PathSelect);
	e->WriteByte(lastCodeID);
	e->WriteByte(RecvCode);
	if(EnableTime)lastTime=TimerTick-lastTick;
	e->WriteWord(lastTime);
	e->WriteDWord(lastIDCard);
	for(i=0;i<MaxSensor;i++)
	{
		e->WriteWord(Sensor.GetCHValue(i));		
	}
	e->SendAckPacket();
}


u8 AveSpeedL=3;
u8 AveSpeedR=3;
u8 statusRL=0;
void SetDataEvent(UartEvent e)
{
	u8 id;
	id=e->ReadByte();
	if(lastCodeID==id)return;
	lastCodeID=id;
	switch(e->ReadByte())
	{
		case 0:
			RightSpeed.P=LeftSpeed.P=e->ReadByte();
			RightSpeed.I=LeftSpeed.I=e->ReadByte();
			RightSpeed.D=LeftSpeed.D=e->ReadByte();			
			break;
		case 1:
			//e->ReadBuff((u8*)StatsCode,9);
			break;
		case 2:
			//fmaxlen= e->ReadByte();
			break;
		case 3:
			PathPos=e->ReadWord();
			PathLen=e->ReadByte();
			e->ReadBuff(&pathbuff[PathPos],PathLen);
			PathLen+=PathPos;
			PathPos=0;
			break;
		case 4:
			switch(e->ReadByte())
			{
				case 0:PathSelect|=PathType.Forward; DetectionLogic.Control->ActionRun(Forward,1);lastAction=Forward; 
				EnableTime=1;
				lastTick=TimerTick;
				lastTime=0;
				if(PathPos<PathLen&&PathLen!=0xffff){
					PathPos++;
				}
				RecvCode++;
				break;
				case 1:PathSelect|=PathType.Left; DetectionLogic.Control->ActionRun(Left,1000);lastAction=Left;break;
				case 2:PathSelect|=PathType.Right; DetectionLogic.Control->ActionRun(Right,1000);lastAction=Right;break;
				case 3: DetectionLogic.Control->ActionRun(Back,1000);lastAction=Back;break;
			}
			break;
		case 5:
			DetectionLogic.Control->StopRun();
			PathSelect=PathType.Forward;
			DetectionLogic.Control->ActionRun(Forward,1);
			PathLen=0xffff;
			PathPos=0;
			lastAction=Forward;
			DetectionLogic.Control->StopRun();
			lastTick=TimerTick;
			lastTime=0;
			EnableTime=0;
//			lastCodeID=255;
			break;
		case 6:
			AveSpeedL=e->ReadByte();
			AveSpeedR=e->ReadByte();
			break;
		case 7:
			statusRL = e->ReadByte();
			switch(statusRL)
		{
				case 0:
					Motor.Speed(0,MotorLeftSpeed=0);
					Motor.Speed(1,MotorRightSpeed=0);
					break;
				case 1:
					Motor.Speed(0,MotorLeftSpeed=AveSpeedL);
					Motor.Speed(1,MotorRightSpeed=AveSpeedR);
					break;
				case 2:
					Motor.Speed(0,MotorLeftSpeed=-AveSpeedL);
					Motor.Speed(1,MotorRightSpeed=-AveSpeedR);
					break;
				case 3:
					Motor.Speed(0,MotorLeftSpeed=-AveSpeedL);
					Motor.Speed(1,MotorRightSpeed=AveSpeedR);
					break;
				case 4:
					Motor.Speed(0,MotorLeftSpeed=AveSpeedL);
					Motor.Speed(1,MotorRightSpeed=-AveSpeedR);
					break;
		}
			break;
	}
}
void led(void)
{
  LED=~LED;
	UART.SendString((u8*)"test!");
	//mpu_dmp_get_data(&pitch,&roll,&yaw);
	Yaw=yaw;
}

void turndone(ActionData e)
{
	EnableTime=1;
	lastTick=TimerTick;
	lastTime=0;
	if(PathPos<PathLen&&PathLen!=0xffff)
	{
		PathPos++;
	}
	if(PathPos>=PathLen)
	{
		PathPos=0xffff;
		PathLen=0xffff;
	}
	RecvCode++;
}

#define ModeTol 0
#define ModeTor 1
#define ModeTof 2
#define ModeTob 3

void inway(ActionData e){
	EnableTime=0;
	RecvCode++;
	if(PathPos<PathLen&&PathLen!=0xffff)
	{
		switch (PathList[PathPos]) {
		case ModeTof:
			EnableTime=1;
			lastTick=TimerTick;
			lastTime=0;
			if(PathPos<PathLen&&PathLen!=0xffff)
			{
				PathPos++;
			}
			if(PathPos>=PathLen)
			{
				PathPos=0xffff;
				PathLen=0xffff;
			}
			if(!e->ActionRun(Forward, 1)){
				e->StopRun();
			}
			break;
		case ModeTol:
			if (!e->ActionRun(Left, 1000))
				if (!e->ActionRun(Forward, 1))
					e->StopRun();
			break;
		case ModeTor:
			if (!e->ActionRun(Right, 1000))
				if (!e->ActionRun(Forward, 1))
					e->StopRun();				
			break;
		}
	}
	else{
		e->StopRun();
		lastAction=0xff;
	}
}

void outway(ActionData e){
	
}

void inPoint(ActionData e)
{
	EnableTime=0;
	RecvCode++;
	if(PathPos<PathLen&&PathLen!=0xffff)
	{
		if (PathList[PathPos] == ModeTob) {
			e->ActionRun(Back, 1000);
		}
		else {
			PathPos=0xffff;
			PathLen=0xffff;
			e->StopRun();
		}
	}
	else{
		PathPos++;
		if(PathPos>=PathLen)
		{
			PathLen=0xffff;
		}
		e->StopRun();
		lastAction=0xff;
	}
}

void ReadCard(u32 id)
{
	if(id!=0)
		lastIDCard=id;
}


int main(void)
{
	Stm32_Clock_Init(9);
	JTAG_Set(1);
	Wave.init();
  Motor.Init();
  Sensor.Init();
	BatteryCheckInit();
  Timer.Init(72);
  LED_Init();
//  Timer.Run();
//  delay_init(72);
  
//  while(1)
//  {
//     delay_ms(500);
//    LED=~LED;
//    
//  }
  // LED=0;
  if(!MPU.Init())
	{
		while(mpu_dmp_init())
		{
			delay_ms(500);
			//  LED=~LED;
		}
	}
	//mpu_dmp_get_data();
  
  LED=0;
	
	UART.Init(72,115200,OnRecvData);
	UART.SendByte(0);
	UART.SendByte(0xff);
	UART.SendByte(0x55);
	
	RC522.Init();
	RC522.SetCallBack(ReadCard);
	
	Timer.Start(0,UartProtocol.Check);
  Timer.Start(0,tick1);
  Timer.Start(10,DetectionLogic.LogicRun);
	Timer.Start(10,Sensor.LoopScan);
	PathList=pathbuff;
	DetectionLogic.Init();
	DetectionLogic.RegisterEvent(ActionType.EnterWay,inway);
	DetectionLogic.RegisterEvent(ActionType.EnterPoint,inPoint);
	DetectionLogic.RegisterEvent(ActionType.turnDone,turndone);
	DetectionLogic.Control->StopRun();
	lastAction=0xff;
	//DetectionLogic.LoadDefConfig(pathbuff );
  //Timer.Start(500,led);
	Timer.Start(0,RC522.ReadLoop);
	Timer.Start(500,UpdateBatteryValue);
	
  UartProtocol.Init(buffdata);
	UartProtocol.AutoAck(ENABLE);
	UartProtocol.RegisterCmd(Alive,AliveEvent);
  UartProtocol.RegisterCmd(GetData,GetDataEvent);
	UartProtocol.RegisterCmd(SetData,SetDataEvent);
	
	while(1)
		Timer.Run();
	
	
}
