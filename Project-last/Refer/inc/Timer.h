#ifndef Timer_H
#define Timer_H

//Build for STM32 Cortex M0 2014��11��5��16:01:09 @KeyMove

#include"mcuhead.h"

#define u8 unsigned char
#define u16 unsigned short
#define u32 unsigned long

typedef void(*TimerCallBack)(void);

typedef struct{
	u16 time;
	u16 nTime;
	TimerCallBack fun;
	union{
	u32 bAddr;
	u8 cAddr[4];
	}breakAddr;
}_Timer;

#define MAXTIMER 8			//���ʱ������

#define TimerBit u8			//���ݶ�ʱ������ѡ��,С��8��ѡu8,����8��С��16��ѡu16,����16��ѡu32,���ͬʱ֧��32����ʱ��

void TimerRun(void);
void KillTimer(u8 id);
void KillThisTimer(void);
u8 SetTimer(u16 time,TimerCallBack fun);
void InitTimer(u8);
void Delay(u16 time);
void SetDelay(u16 time);
void AddDelay(u16 time);
u8 isUse(u8 id);
void ZeroDelay(void);

typedef struct{
	void(*Init)(u8);//��ʼ��
	void(*Run)();//����
	void(*Stop)(u8);
	void(*StopThis)();
	u8(*isUse)(u8);
	u8(*Start)(u16,TimerCallBack);
	void(*Delay)(u16);
	struct{
		void(*add)(u16);
		void(*set)(u16);
		void(*ready)();
	}Tick;
}TimerBase;

extern const TimerBase Timer;
extern u32 TimerTick;
#endif

