#ifndef _DetectionLogic_H_
#define _DetectionLogic_H_

#include"mcuhead.h"
#include "PID.h"

//--------线路规划-----------
//路径缓存
extern u8 *PathList;
//路径执行位置
extern u16 PathPos;
//路径长度
extern u16 PathLen;

extern u8 PathSelect;

extern PIDStruct LeftSpeed;
extern PIDStruct RightSpeed;

extern s8 MotorLeftSpeed;
extern s8 MotorRightSpeed;

extern s16 Yaw;

extern u8 waveLenght[2];

typedef struct {
	u8(*GetDIR)(void);

	//运行指定动作
	u8(*ActionRun)(u8 DIR, u16 time);
	
	//让电机直接运行 不修正轨道
	void(*MotorRun)(u8 dir,u16 time);
	
	//停止所有状态的运行
	void(*StopRun)(void);

	//获取正在运行的剩余时间
	u16(*GetRunTime)(void);

	//使用PathType查询道路是否可用
	u8(*CheckPath)(u8 Type);

	//获取可用路线数
	u8(*GetPathCount)(void);

	//获取最后发生的错误
	u8(*GetLastError)(void);

	u8* Speed;
}ActionInfoBase;

typedef ActionInfoBase* ActionData;

typedef struct {
	u8 Join;
	u8 Quit;
	u8 EnterWay;
	u8 LeaveWay;
	u8 turnFail;
	u8 turnDone;
	u8 EnterPoint;
}ActionTypeBase;
extern const ActionTypeBase ActionType;
typedef struct
{
	u8 NullDetection;
	u8 AngleOut;
}FailTypeBase;
extern const FailTypeBase FailType;

typedef void(*ActionEvent)(ActionData);

typedef struct {
	u8 Forward;
	u8 Left;
	u8 Right;
}PathTypeBase;
extern const PathTypeBase PathType;

typedef enum {
	Forward = 0,
	Left = 1,
	Right = 2,
	Back = 3,
}Direction;

typedef struct {
	void(*Init)(void);
	void(*LogicRun)(void);
	void(*RegisterEvent)(u8 type, ActionEvent event);
	void(*LoadDefConfig)(u8* buff);
	ActionData Control;
}DetectionLogicBase;

extern const DetectionLogicBase DetectionLogic;

#endif
