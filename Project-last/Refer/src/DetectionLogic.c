#include "DetectionLogic.h"
#include "PID.h"
#include "Sensor.h"
#include "motor.h"
//--------线路规划-----------
//路径缓存
u8 *PathList;
//路径执行位置
u16 PathPos;
//路径长度
u16 PathLen;
//---------------------------

//-----------状态-------------

u8 waveLenght[2];
u8 waveCount[2];
u8 waveDisableLenght=10;//超声波探测距离(cm)

s16 TargetYaw;
s16 MinAngle;
s16 Yaw;
//-------------轨道运行-----------
s8 StatusCode[9] = { -70,-45,-37,-30,0,30,37,45,70 };

s16 LeftValue;
s16 RightValue;
s16 lastLeftValue;
s16 lastRightValue;
u8 LeftDt;
u8 RightDt;
PIDStruct LeftSpeed;
PIDStruct RightSpeed;
s8 MotorLeftSpeed;
s8 MotorRightSpeed;
//--------------路径探测-----------
u8 Statuscount;				//路径传感器数量
u8 lastStatus;				//最后探测到的路径状态
u8 DirectionThreshold=69;	//方向阈值
u8 WayThreshold=10;			//路口阈值
u8 turnErrorAngle = 5;		//转向Yaw差值

u8 StopTime;

u8 PathSelect;
u8 lastPathSelect;

u8 LeftSelect;
u8 MidSelect;
u8 RightSelect;
//-------------行为控制-------------
s8 turnLeftSpeed=100;
s8 turnRightSpeed=-100;

PIDStruct LeftSpeed, RightSpeed;

u8 runStatus = 0;
u8 lastrunStatus = 0;

u8 RunDir;

u16 StatusTick = 0;
u8 lastError = 0;

s16 Speed;
//---------------状态---------------


//--------------应用层----------------
ActionEvent eventlist[sizeof(ActionTypeBase)];

typedef struct {
	u8 Nop;
	u8 Stop;
	u8 turnLeft;
	u8 turnRight;
	u8 Forward;
	u8 Back;
	u8 FreeRun;
	u8 Stand;
}runStatusTypeBase;

//----------------常量定义区--------------------------

//typedef void(*ActionEnter)(u8 lastPath);
//typedef void(*ActionLeave)(u8 lastPath);
//typedef void(*ActionFail)(u8 Dir,FailType type,u8 lastPath);

#define LogicAbs(x) ((x)<0?-x:x)

#define StatusM BIT0			//处于中心的位置
#define StatusL BIT1			//偏左
#define StatusR BIT2			//偏右
#define StatusA BIT3			//全亮 路口
//#define StatusLD BIT4   //偏左全部点亮
//#define StatusRD BIT5   //偏右全部点亮

#define WaveStop BIT6		//超声波模块距离过近

#define CheckStatus(x) (lastStatus&(x))
#define CheckStatusZero() (Statuscount==0)

#define RunNop 0
#define RunNor 1
#define RunTor 2
#define RunTol 3
#define RunPts 4
#define RunTof 5
#define RunTob 6
#define RunStp 7

#define ModeTol 0
#define ModeTor 1
#define ModeTof 2
#define ModeTob 3
#define ModeRst 4

const PathTypeBase PathType = {
	StatusM,
	StatusL,
	StatusR,
};

const runStatusTypeBase runStatusType = {
	0,
	1,
	2,
	3,
	4,
	5,
	6,
};

const FailTypeBase FailType = {
	1,
	2,

};

const ActionTypeBase ActionType = {
	0,
	1,
	2,
	3,
	4,
	5,
	6,
};



void StopRun()
{
	PathPos = 0;
	runStatus = runStatusType.Stand;
	PathSelect = 0;
	lastLeftValue=lastRightValue=0;
	LeftDt=RightDt=0;
	Motor.Speed(0, MotorLeftSpeed = 0);
	Motor.Speed(1, MotorRightSpeed = 0);
}
//---------------------传感器功能-----------------------------//

//设置Yaw目标角度
void SetTargetYaw(s16 deg) {
	TargetYaw = (s16)Yaw + 180 + deg;
	TargetYaw %= 360;
	MinAngle = deg + 180;
}

//超声波检测障碍
u8 WaveDetection()
{
	u8 i;
	for (i = 0; i < 2; i++) {
		if (waveLenght[i] < waveDisableLenght)
		{
			if (waveCount[i]++)
			{
				SETBIT(lastStatus, WaveStop);
				return 0;
			}
			waveLenght[i] = waveDisableLenght;
		}
		else{
			waveCount[i]=0;
		}
	}
	CLRBIT(lastStatus, WaveStop);
	return 1;
}


//---------------------事件功能------------------------------//

static u8 getdir() {
	return RunDir;
}

static u8 ActionRun(u8 dir, u16 time) {
	switch (dir)
	{
	case Forward:
		if(!(PathSelect&StatusM))return 0;
		runStatus = runStatusType.Nop;
		break;
	case Left:
		if (!(PathSelect&StatusL))return 0;
		runStatus = runStatusType.turnLeft;
		SetTargetYaw(90);
		lastLeftValue=0xff;
		break;
	case Right:
		if (!(PathSelect&StatusR))return 0;
		runStatus = runStatusType.turnRight;
		lastRightValue=0xff;
		SetTargetYaw(-90);
		break;
	case Back:
		runStatus = runStatusType.Back;
		lastLeftValue=0xff;
		SetTargetYaw(-180);
		break;
	}
	RunDir = dir;
	StatusTick = time;
	return 1;
}

static void MotorRun(u8 dir, u16 time) {
	runStatus = runStatusType.FreeRun;
	RunDir = dir;
	StatusTick = time;
}

static void StopRunAction() {
	StatusTick = 0;
	runStatus = runStatusType.Stop;
	Motor.Speed(0, MotorLeftSpeed = 0);
	Motor.Speed(1, MotorRightSpeed = 0);
}

static u16 getTime() {
	return StatusTick;
}


static u8 getLastPath(u8 type) {
	return ((PathSelect&type)!=0);
}

static u8 getPathCount() {
	return (((PathSelect&StatusL) != 0) + ((PathSelect&StatusM) != 0) + ((PathSelect&StatusR) != 0));
}

static u8 getLastError() {
	return lastError;
}

const ActionInfoBase ActionInfo = {
	getdir,
	ActionRun,
	MotorRun,
	StopRunAction,
	getTime,
	getLastPath,
	getPathCount,
	getLastError,
	(u8*)&Speed,
};
//-----------------------------------------------------//

//检查Yaw是否接近误差值
u8 CheckTargetYaw(u8 errdeg) {
	s16 Yawdeg = (s16)Yaw + 180;
	Yawdeg = TargetYaw - Yawdeg;
	if (Yawdeg > 180)
		Yawdeg = -(Yawdeg - 360);
	else if (Yawdeg < 0)
		Yawdeg = -Yawdeg;
	if (Yawdeg<errdeg)
		return 1;
	if (MinAngle > Yawdeg)
	{
		MinAngle = Yawdeg;
	}
	else if (MinAngle < Yawdeg) {
		if (eventlist[ActionType.turnFail]) {
			lastError = FailType.AngleOut;
			eventlist[ActionType.turnFail]((ActionData)&ActionInfo);
			StopRun();
		}
	}
	return 0;
}

s16 CheckSensor()
{
	u8 i;
	s16 v = 0;
	u8 lc = 0, rc = 0;
	SensorStatus dat = Sensor.Read();
	CLRBIT(lastStatus, StatusM | StatusL | StatusR);
	lastStatus = LeftValue = RightValue = 0;
	Statuscount = 0;
	for (i = 0; i<MaxSensor; i++) {
		if (!(dat&BIT15))
		{
			Statuscount++;
			v += StatusCode[i];
			if (i>4)
			{
				lc++;
				LeftValue += StatusCode[i];
				if (LeftValue>DirectionThreshold)
					SETBIT(lastStatus, StatusL);
			}
			else if (i<4)
			{
				rc++;
				RightValue += StatusCode[i];
				if (RightValue<-DirectionThreshold)
					SETBIT(lastStatus, StatusR);
			}
			else {
				SETBIT(lastStatus, StatusM);
			}
		}
		dat <<= 1;
	}
	if (Statuscount == MaxSensor)
		SETBIT(lastStatus, StatusA);
	LeftValue /= lc;
	RightValue = -RightValue;
	RightValue /= rc;
	return v / Statuscount;
}

s16 SensorDetection()
{
	static u8 mode;
	static u16 tick;
	s16 v = CheckSensor();

	if (LeftDt)
		if (--LeftDt == 0)
			lastLeftValue = 0;
	if (RightDt)
		if (--RightDt == 0)
			lastRightValue = 0;

	if (StopTime)
	{
		if (!CheckStatusZero())
		{
			mode = 0;
			StopTime = 0;
		}
		else if (--StopTime == 0)
		{
			lastPathSelect = PathSelect;
			PathSelect = 0;
			SETBIT(PathSelect, StatusA);
			runStatus = runStatusType.Nop;
			RightDt=LeftDt=0;
			lastRightValue = lastLeftValue = 0;
			lastError = 0;
			StatusTick = 0;
			Motor.Speed(0, MotorLeftSpeed = 0);
			Motor.Speed(1, MotorRightSpeed = 0);
			if (eventlist[ActionType.EnterPoint] != 0)
				eventlist[ActionType.EnterPoint]((ActionData)&ActionInfo);
			return 0;
		}
		else
		{
			runStatus = runStatusType.Stand;
			return 0;
		}
	}



	if (!tick&&!StatusTick&&runStatus == runStatusType.Nop)
	{

		if (LeftValue){
			if (LeftValue>lastLeftValue) {
				lastLeftValue = LeftValue;
				LeftDt += 10;
			}
			else if (LeftValue == lastLeftValue) {
				LeftDt++;
			}
		}
		if (RightValue){
			if (RightValue>lastRightValue) {
				lastRightValue = RightValue;
				RightDt += 10;
			}
			else if (RightValue == lastRightValue)
				RightDt++;
		}



		if (((CheckStatus(StatusL) && LeftDt>20) || (CheckStatus(StatusR) && RightDt>20)))
		{
			lastPathSelect = PathSelect;
			LeftSelect = RightSelect = MidSelect = 0;
			PathSelect = 0;
			//StatusTick = 500;
			tick = 500;
			mode = 1;//runStatus = RunPts;
			lastRightValue = lastLeftValue = 0;
		}
		else if (CheckStatusZero())
		{
			StopTime = 100;
			mode = 1; //runStatus = RunStp;
			return 0;
		}
	}
	else if(runStatus==runStatusType.turnLeft||runStatus==runStatusType.turnRight||runStatus==runStatusType.Back)
	{
		if (LeftValue){
			if (LeftValue<lastLeftValue) {
				if((lastLeftValue-LeftValue)<200)
				{	
					lastLeftValue = LeftValue;
					LeftDt += 10;
				}
			}
			else if (LeftValue == lastLeftValue) {
				LeftDt++;
			}
		}
		if (RightValue){
			if (RightValue<lastRightValue) {
				if((lastRightValue-RightValue)<200)
				{	
					lastRightValue = RightValue;
					RightDt += 10;
				}
			}
			else if (RightValue == lastRightValue)
				RightDt++;
		}
		
		if(runStatus==runStatusType.turnLeft||runStatus==runStatusType.Back)
		{
			v = turnLeftSpeed;
			if(LeftDt>20&&(CheckStatus(StatusM)||CheckTargetYaw(turnErrorAngle)))
			{
				runStatus=runStatusType.Nop;
				StatusTick=0;
				if (eventlist[ActionType.turnDone] != 0)
					eventlist[ActionType.turnDone]((ActionData)&ActionInfo);
			}
		}
		if(runStatus==runStatusType.turnRight)
		{
			v = turnRightSpeed;
			if(RightDt>20&&(CheckStatus(StatusM)||CheckTargetYaw(turnErrorAngle)))
			{
				runStatus=runStatusType.Nop;
				StatusTick=0;
				if (eventlist[ActionType.turnDone] != 0)
					eventlist[ActionType.turnDone]((ActionData)&ActionInfo);
			}
		}
		return v;
	}
	else
	{
		if (mode)
		{
			if (CheckStatus(StatusL))
				LeftSelect++;
			if (CheckStatus(StatusR))
				RightSelect++;
			if (tick>50)
			{
				v /= 16;
				if (!CheckStatus(StatusR | StatusL))
				{
					tick = 50;
				}
			}
			else {
				if (CheckStatus(StatusM))
				MidSelect++;
				v *= 2;
			}
			if (tick == 1)
			{
				lastPathSelect = PathSelect;
				PathSelect=0;
				if (LeftSelect>WayThreshold)
				{
					SETBIT(PathSelect, StatusL);
				}
				if (RightSelect>WayThreshold)
				{
					SETBIT(PathSelect, StatusR);
				}
				if (MidSelect>WayThreshold)
					SETBIT(PathSelect, StatusM);
				if (!(PathSelect&(StatusL | StatusR | StatusM)))
					SETBIT(PathSelect, StatusA);
				runStatus = runStatusType.Nop;
				RightDt=LeftDt=0;
				lastRightValue = lastLeftValue = 0;
				lastError = 0;
				StatusTick = 0;
				mode = 0;
				if (eventlist[ActionType.EnterWay] != 0)
					eventlist[ActionType.EnterWay]((ActionData)&ActionInfo);
			}
		}
	}
	if(tick)
		tick--;
	return v;
}


void LogicRun() {
	//超声波测距
	if (!WaveDetection()) {
		Motor.Speed(0, MotorLeftSpeed = 0);
		Motor.Speed(1, MotorRightSpeed = 0);
		return;
	}

	//检测路口
	Speed = SensorDetection();

	if (runStatus == runStatusType.Stand) {
		if (CheckStatusZero()) {
			return;
		}
		else{
			runStatus=runStatusType.Nop;
		}
	}
	//暂停模式
	if (runStatus == runStatusType.Stop)
		return;

	//MinAngle

	if (StatusTick)
	{
		if(runStatus==runStatusType.Nop)
		{
			if (StatusTick != 0)
			{
				Speed *= 2;
			}
		}
//		else if(runStatus==runStatusType.turnRight)
//		{
//			Speed = turnRightSpeed;
//			if (StatusTick > 500)
//			{
//				if (CheckStatus(StatusR))
//					StatusTick = 500;
//				if (CheckTargetYaw(turnErrorAngle))
//					StatusTick = 100;
//			}
//			else if (StatusTick>100)
//			{
//				if (CheckStatus(StatusM))
//				{
//					StatusTick = 50;
//					runStatus = runStatusType.Nop;
//				}
//			}
//		}
//		else if(runStatus==runStatusType.turnLeft)
//		{
//			Speed = turnLeftSpeed;
//			if (StatusTick > 500)
//			{
//				if (CheckStatus(StatusL))
//					StatusTick = 500;

//				if (CheckTargetYaw(turnErrorAngle))
//					StatusTick = 100;
//			}
//			else if (StatusTick>100)
//			{
//				if (CheckStatus(StatusM))
//				{
//					StatusTick = 50;
//					runStatus = runStatusType.Nop;
//				}
//			}
//		}
//		else if(runStatus==runStatusType.Back)
//		{
//			Speed = turnLeftSpeed;
//			if (StatusTick > 1500) {
//				if (CheckStatus(StatusL))
//					StatusTick = 1500;
//				if (CheckTargetYaw(turnErrorAngle))
//					StatusTick = 100;
//			}
//			else if (StatusTick > 1000) {
//				if (CheckStatus(StatusM))
//					StatusTick = 1000;
//				if (CheckTargetYaw(turnErrorAngle))
//					StatusTick = 100;
//			}
//			else if (StatusTick > 500) {
//				if (CheckStatus(StatusL))
//					StatusTick = 500;
//				if (CheckTargetYaw(turnErrorAngle))
//					StatusTick = 100;
//			}
//			else if (StatusTick>50) {
//				if (CheckStatus(StatusM))
//				{
//					StatusTick = 50;
//					runStatus = runStatusType.Nop;
//				}
//			}
//		}
		if (--StatusTick == 0)
		{
			if (runStatus != runStatusType.Nop) {
				lastError = FailType.NullDetection;
				if (eventlist[ActionType.turnFail]) {
					eventlist[ActionType.turnFail]((ActionData)&ActionInfo);
				}
			}
			else {
				lastrunStatus = runStatus;
				if (eventlist[ActionType.LeaveWay]) {
					eventlist[ActionType.LeaveWay]((ActionData)&ActionInfo);
				}
			}
			runStatus = runStatusType.Nop;
		}

	}
	Motor.Speed(0, MotorLeftSpeed = PID.Update(&LeftSpeed, +Speed));
	Motor.Speed(1, MotorRightSpeed = PID.Update(&RightSpeed, -Speed));
}

void MagneticRun() {
	s16 vy = (s16)Yaw + 180;
	s16 Speed;
	
	//超声波测距
	if (!WaveDetection()) {
		Motor.Speed(0, MotorLeftSpeed = 0);
		Motor.Speed(1, MotorRightSpeed = 0);
		return;
	}

	//检测路口
	Speed = SensorDetection();
	
	//暂停模式
	if (runStatus == RunStp)
		return;
	
	//普通运行
	if (runStatus == RunNor)
	{
		if (PathLen != 0 && PathPos < PathLen)
		{
			if (!StatusTick) {
				if (PathSelect == 0)
				{
					StopRun();
					return;
				}
				switch (PathList[PathPos])
				{
				case ModeTol:
					if (PathSelect&StatusL) {
						StatusTick = 1000;
						runStatus = RunTol;
						TargetYaw = ((s16)Yaw + 180 + 90) % 360;
						PathPos++;
					}
					if (PathSelect&StatusR) {
						Speed = 0;
					}
					if (PathSelect&(StatusA))
					{
						StopRun();
						return;
					}
					break;
				case ModeTor:
					if (PathSelect&StatusR) {
						StatusTick = 1000;
						runStatus = RunTor;
						TargetYaw = ((s16)Yaw + 180 - 90) % 360;
						PathPos++;
					}
					if (PathSelect&StatusL) {
						Speed = 0;
					}
					if (PathSelect&(StatusA))
					{
						StopRun();
						return;
					}
					break;
				case ModeTof:
					if (PathSelect&(StatusR | StatusL)) {
						StatusTick = 10;
						Speed = 0;
						PathPos++;
					}
					if (PathSelect&(StatusA))
					{
						StopRun();
						return;
					}
					break;
				case ModeTob:
					if (PathSelect&(StatusA)) {
						StatusTick = 1000;
						runStatus = RunTob;
						TargetYaw = ((s16)Yaw) % 360;
						PathPos++;
					}
					else if (PathSelect)
					{
						Speed = 0;
						runStatus = RunNop;
					}

					break;
				}
				PathSelect = 0;
				if (PathPos == PathLen)
					PathPos = 0;
			}
		}
		else {
			StopRun();
			return;
		}
	}

	if (StatusTick)
	{
		switch (runStatus)
		{
		case RunNop:
			if (StatusTick != 0)
			{
				Speed *= 2;
			}
			break;
		case RunTor:
			Speed = turnRightSpeed;
			if (StatusTick > 500)
			{
				if (CheckStatus(StatusR))
					StatusTick = 500;

				vy = TargetYaw - vy;
				if (vy > 180)
					vy = -(vy - 360);
				else if (vy < 0)
					vy = -vy;

				if (vy < 5)
					StatusTick = 100;
			}
			else if (StatusTick>100)
			{
				if (CheckStatus(StatusM))
				{
					StatusTick = 50;
					runStatus = RunNop;
				}
			}
			break;
		case RunTol:
			Speed = turnLeftSpeed;
			if (StatusTick > 500)
			{
				if (CheckStatus(StatusL))
					StatusTick = 500;

				vy = TargetYaw - vy;
				if (vy > 180)
					vy = -(vy - 360);
				else if (vy < 0)
					vy = -vy;

				if (vy < 5)
					StatusTick = 100;
			}
			else if (StatusTick>100)
			{
				if (CheckStatus(StatusM))
				{
					StatusTick = 50;
					runStatus = RunNop;
				}
			}
			break;
		case RunTob:
			Speed = turnLeftSpeed;
			if (StatusTick > 1500) {
				if (CheckStatus(StatusL))
					StatusTick = 500;
			}
			else if (StatusTick > 1000) {
				if (!CheckStatus(StatusR | StatusL) && (CheckStatus(StatusM) || vy < 10))
					StatusTick = 1000;
			}
			else if (StatusTick > 500) {
				if (CheckStatus(StatusL))
					StatusTick = 500;
				vy = TargetYaw - vy;
				if (vy > 180)
					vy = -(vy - 360);
				else if (vy < 0)
					vy = -vy;
				if (vy < 10)
					StatusTick = 100;
			}
			else if (StatusTick>50) {
				if (CheckStatus(StatusM))
				{
					StatusTick = 50;
					runStatus = RunNop;
				}
			}
			break;
		}
		if (--StatusTick == 0)
			runStatus = RunNop;
	}
	Motor.Speed(0, MotorLeftSpeed = PID.Update(&LeftSpeed, +Speed));
	Motor.Speed(1, MotorRightSpeed = PID.Update(&RightSpeed, -Speed));
}

void RegisterEvent(u8 type, ActionEvent e) {
	eventlist[type] = e;
}

static void inturndone(ActionData e)
{
	PathPos++;
	if(PathPos>=PathLen)
	{
		PathPos=0;
	}
}

static void inway(ActionData e) {
	switch (PathList[PathPos]) {
	case ModeTof:
		PathPos++;
		if(PathPos>=PathLen)
		{
			PathPos=0;
		}
		if(!e->ActionRun(Forward, 1)){
			StopRun();
		}
		break;
	case ModeTol:
		if (!e->ActionRun(Left, 1000))
			if (!e->ActionRun(Forward, 1))
				StopRun();
		break;
	case ModeTor:
		if (!e->ActionRun(Right, 1000))
			if (!e->ActionRun(Forward, 1))
				StopRun();
		break;
	}
}

static void inPoint(ActionData e) {
	if (PathList[PathPos] == ModeTob) {
		e->ActionRun(Back, 1000);
	}
	else {
		StopRun();
	}
}

void DefConfig(u8* buff) {
	PathList = buff;
	DetectionLogic.RegisterEvent(ActionType.EnterPoint, inPoint);
	DetectionLogic.RegisterEvent(ActionType.EnterWay, inway);
	DetectionLogic.RegisterEvent(ActionType.turnDone,inturndone);
}

static void init(){
	PID.Init(&LeftSpeed,100,90,30);
	PID.Init(&RightSpeed,100,90,30);
	LeftSpeed.value=32;
	RightSpeed.value=32;
	LeftSpeed.max=RightSpeed.max=32;
	LeftSpeed.min=RightSpeed.min=-32;
}

const DetectionLogicBase DetectionLogic = {
	init,
	LogicRun,
	RegisterEvent,
	DefConfig,
	(ActionData)&ActionInfo,
};
