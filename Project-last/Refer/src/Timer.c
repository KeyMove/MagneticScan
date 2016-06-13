#include"Timer.H"

const TimerBase Timer={
	InitTimer,
	TimerRun,
	KillTimer,
	KillThisTimer,
	isUse,
	SetTimer,
	Delay,
	{
		AddDelay,
		SetDelay,
		ZeroDelay
	}
};

_Timer TimerList[MAXTIMER];	//��ʱ���б�
u8 this;										//��ǰִ��
TimerBit TimerUse;					//ʹ�ò�
u32 InputSP;
u32 TimerTick=0;
//��ʼ����ʱ��
void InitTimer(u8 mhz)			
{
	mhz/=8;
	SysTick->CTRL	&=	0xfffffffb;	//sysclock/8  
	SysTick->LOAD	=		(int)mhz*1000;				//1000us timer   //�δ�ʱ��
	SysTick->VAL	=		0x00;
	SysTick->CTRL	=		3;					//start timer
}
//��ʱ���ж�
void SysTick_Handler(void)
{
	u8 i;
	TimerTick++;
	for(i=0;i<MAXTIMER;i++)
		if(TimerList[i].nTime)
			TimerList[i].nTime--;
}
//��ʼ��������ʱ��
u8 SetTimer(u16 time,TimerCallBack fun)
{
	TimerBit BIT=1;
	u8 i;
	for(i=0;i<MAXTIMER;i++)
	{
		if(!(BIT&TimerUse))
		{
			TimerUse|=BIT;
			TimerList[i].fun=fun;
			TimerList[i].nTime=0;
			TimerList[i].time=time;
			return i;
		}
		BIT<<=1;
	}
	return 255;
}
//�ӽ�����ǰ��ʱ��
void KillThisTimer(void)
{
	TimerBit BIT=1;
	u8 i;
	for(i=0;i<this;i++)
		BIT<<=1;
	TimerUse&=~BIT;
}
//�ӽ���ָ����ʱ��
void KillTimer(u8 id)
{
	TimerBit BIT=1;
	u8 i;
	for(i=0;i<id;i++)
		BIT<<=1;
	TimerUse&=~BIT;
}
//�ı䶨ʱ��ʱ��
void SetDelay(u16 time)
{
	TimerList[this].time=time;
}
//�޸ĵ��ζ�ʱ��ʱ��
void AddDelay(u16 time)
{
	TimerList[this].nTime+=time;
}

void ZeroDelay(void)
{
	TimerList[this].nTime=0;
}

u8 isUse(u8 id)
{
	if(TimerUse&(1<<id))
		return 1;
	return 0;
}

__ASM u32 GetSP(void)
{
	mov r0,sp
	bx lr
}

__ASM u32 GetBreakAddr(void)
{ 
	nop
	mov r0,sp								;ȡSPָ���ַ
	ldr r1,=__cpp(&InputSP) ;��ȡȫ�ֱ���InputSP��ַ
	ldr r1,[r1,#0]					;��ȡ��ַ������
	subs r0,r1,r0           ;��ǰSP��ַ��ȥ��ʼSP��ַ
	cmp r0,#8								;����Ƿ���;�ָ�
	beq n                   ;���Ϊ��;�ָ�ȡ��R0-8
	subs r0,#8							;R0-8�������ص�ַ
n	
	mov r1,sp								;���»�ȡSPָ���ַ
	adds r1,r0,r1						;����ƫ����
	pop {r0}								;R4��ջ
	pop {r0}								;�������ص�ַ��ջ
	mov sp,r1								;����SP��ַ
	bx lr										;��������
}

void CallFun(u32 addr)
{
	(*(void(*)())addr)();
}

void Delay(u16 time)
{
	TimerList[this].breakAddr.bAddr=GetBreakAddr();
	TimerList[this].nTime+=time;
}

//�����Ѿ��ص�����
void TimerRun(void)
{
	static TimerBit BIT;
	u32 fun;
	BIT=1;
	for(this=0;this<MAXTIMER;this++)
	{
		if(BIT&TimerUse)
		{
			if(!TimerList[this].nTime)
			{
				TimerList[this].nTime=TimerList[this].time;
				InputSP=GetSP();
				if(TimerList[this].breakAddr.bAddr)
				{
					fun=TimerList[this].breakAddr.bAddr;
					TimerList[this].breakAddr.bAddr=0;
					CallFun(fun);
				}
				else
				{
					TimerList[this].fun();
				}
			}
		}
		BIT<<=1;
	}
}

