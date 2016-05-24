#include "Motor.h"
static void init(){
	SETBIT(RCC->APB1ENR,RCC_APB1ENR_TIM4EN);
	SETBIT(RCC->APB2ENR,RCC_APB2ENR_IOPBEN);
	GPIO_PP(GPIOB,0,11110000);
	GPIO_AF(GPIOB,00000011,0);
	SETBIT(GPIOB->ODR,BIT8|BIT9|BIT7|BIT6|BIT5|BIT4);
	TIM4->CR1|=TIM_CR1_ARPE;
	TIM4->CCER|=TIM_CCER_CC3E|TIM_CCER_CC4E;
	TIM4->PSC=72-1;
	TIM4->ARR=100-1;
	TIM4->CR1|=TIM_CR1_CEN;
	TIM4->CCMR2|=TIM_CCMR2_OC3M|TIM_CCMR2_OC3PE|TIM_CCMR2_OC3CE|TIM_CCMR2_OC4M|TIM_CCMR2_OC4PE|TIM_CCMR2_OC4CE;	
	//TIM4->EGR|=TIM_EGR_UG;
	TIM4->CCR3=0;
	TIM4->CCR4=0;
}

static void setDuty(u8 LR,u8 duty){
	if(LR)
		TIM4->CCR3=duty;
	else
		TIM4->CCR4=duty;
}

static u8 motorstats;
static u8 motorstop=0xff;

static void setSpeed(u8 LR,s8 speed)
{
	
	if(speed>32)speed=32;
	if(speed<-32)speed=-32;
	if(!LR)
	{
		if(speed<0)
		{
			if(motorstats&BIT0||motorstop&BIT0)
			{
				CLRBIT(motorstop,BIT0);
				CLRBIT(motorstats,BIT0);
				MOTOR_L_BA();
			}
			setDuty(LR,32-(-speed));
		}
		else if(speed>0){
			if((!(motorstats&BIT0))||(motorstop&BIT0))
			{
				CLRBIT(motorstop,BIT0);
				SETBIT(motorstats,BIT0);
				MOTOR_L_AB();
			}
			setDuty(LR,32-speed);
		}
		else{
			SETBIT(motorstop,BIT0);
			setDuty(LR,100);
			MOTOR_L_ST();
		}
	}
	else
	{
		if(speed<0)
		{
			if(motorstats&BIT1||motorstop&BIT1)
			{
				CLRBIT(motorstop,BIT1);
				CLRBIT(motorstats,BIT1);
				MOTOR_R_BA();
			}
			setDuty(LR,32-(-speed));
		}
		else if(speed>0){
			if((!(motorstats&BIT1))||motorstop&BIT1)
			{
				CLRBIT(motorstop,BIT1);
				SETBIT(motorstats,BIT1);
				MOTOR_R_AB();
			}
			setDuty(LR,32-speed);
		}
		else{
			SETBIT(motorstop,BIT1);
			setDuty(LR,100);
			MOTOR_R_ST();
		}
	}
		
}

const MotorBase Motor = {
	init,	
	setSpeed,
};
