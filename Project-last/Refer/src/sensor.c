#include "Sensor.h"
#include "ADC.h"

SensorStatus Status=0;
u16 CHTH[MaxSensor];
u16 CHVA[MaxSensor];
u16 PutOut=0;

static void switchCH(u8 ch) {
	if (ch&BIT0)
		SETBIT(GPIOB->ODR, BIT14);
	else
		CLRBIT(GPIOB->ODR, BIT14);
	
	if (ch&BIT1)
		SETBIT(GPIOB->ODR, BIT15);
	else
		CLRBIT(GPIOB->ODR, BIT15);
	
	if (ch&BIT2)
		SETBIT(GPIOA->ODR, BIT8);
	else
		CLRBIT(GPIOA->ODR, BIT8);
	
	if (ch&BIT3)
		SETBIT(GPIOA->ODR, BIT11);
	else
		CLRBIT(GPIOA->ODR, BIT11);
}

static void init(){
	u8 i;
	ADC.Init();
	SETBIT(RCC->APB2ENR, RCC_APB2ENR_IOPAEN | RCC_APB2ENR_IOPBEN);
	GPIO_AI(GPIOA,0,00000100);
	GPIO_PP(GPIOA, 00001001, 00000000);
	GPIO_PP(GPIOB, 11000000, 0);
	switchCH(0);
	for(i=0;i<MaxSensor;i++){
		CHVA[i]=0;
		CHTH[i]=1900;
	}
}


u16 RTADC(){
	u32 v=0;
	u16 dat;
	u16 max=0;
	u16 min=0xffff;
	u8 i;
	for(i=0;i<10;i++)
	{
		dat=ADC.getADCValue(2);
		v+=dat;
		if(max<dat)
			max=dat;
		if(min>dat)
			min=dat;
	}
	v-=max+min;
	v/=8;
	return v;
}

static void Loop() {
	static u8 count;
	CHVA[count] = RTADC();
	if (CHVA[count] > CHTH[count])
		SETBIT(Status, (u16)BIT15 >> count);
	else
		CLRBIT(Status, (u16)BIT15 >> count);
	if (++count >= MaxSensor) {
		count = 0;
		PutOut=Status;
	}
	switchCH(count);
}

static u16 getCHVA(u8 ch){
	return CHVA[ch];
}

static void setCHTH(u8 ch,u16 value){
	CHTH[ch]=value;
}

static SensorStatus ReadData(){
	return ~PutOut;
}

const SensorBase Sensor = {
	init,	
	Loop,
	ReadData,
	setCHTH,
	getCHVA,
	
};
