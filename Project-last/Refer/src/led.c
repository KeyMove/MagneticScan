#include "led.h"


void LED_Init(void)
{
  RCC->APB2ENR|=1<<4;				 
	GPIOC->CRH&=0XFF0FFFFF;
	GPIOC->CRH|=0X00300000;	 
  LED=1;
}
