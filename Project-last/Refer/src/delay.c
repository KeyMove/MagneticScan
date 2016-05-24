#include "delay.h"

static u8  fac_us=9;//us��ʱ������			   
static u16 fac_ms=0;//ms��ʱ������,��os��,����ÿ�����ĵ�ms��

void delay_init(u8 sysclk)
{
  fac_us=sysclk/8;
  fac_ms=1;
}

void delay_us(u32 nus)
{
  u32 reload = SysTick->LOAD;
  u32 told,tnow,tcnt=0;
  u32 ticks;
  ticks = nus*fac_us;
  told = SysTick->VAL;
  while(1)
  {
    tnow = SysTick->VAL;
    if(tnow != told)
    {
      if(tnow<told) tcnt += told-tnow;
      else tcnt += reload-tnow+told;
      told = tnow;
      if(tcnt>=ticks) break;
    }
  };
}
void delay_ms(u32 nms)
{
  while(nms--)
  {
    delay_us(1000);
  }
//  if(nms>=fac_ms)
//  {
//    delay_us();
//  }
//  nms%=fac_ms;
//  delay_us(nms);
}

void delay_sms(u32 nms)
{
  u16 i=0;
  while(nms--)
  {
    i=12000;
    while(i--);
  }
}
