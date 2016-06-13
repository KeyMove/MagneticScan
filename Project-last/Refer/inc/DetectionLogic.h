#ifndef _DetectionLogic_H_
#define _DetectionLogic_H_

#include"mcuhead.h"
#include "PID.h"

//--------��·�滮-----------
//·������
extern u8 *PathList;
//·��ִ��λ��
extern u16 PathPos;
//·������
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

	//����ָ������
	u8(*ActionRun)(u8 DIR, u16 time);
	
	//�õ��ֱ������ ���������
	void(*MotorRun)(u8 dir,u16 time);
	
	//ֹͣ����״̬������
	void(*StopRun)(void);

	//��ȡ�������е�ʣ��ʱ��
	u16(*GetRunTime)(void);

	//ʹ��PathType��ѯ��·�Ƿ����
	u8(*CheckPath)(u8 Type);

	//��ȡ����·����
	u8(*GetPathCount)(void);

	//��ȡ������Ĵ���
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
