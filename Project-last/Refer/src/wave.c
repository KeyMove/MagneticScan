#include "wave.h"
#include "timer.h"

u16 *len1=0,*len2=0;

static void TIM3CapInit(u16 arr,u16 psc)
{
	
	RCC->APB1ENR|=1<<1;
	RCC->APB2ENR|=1<<3;
	 
	GPIOB->CRL&=0XFFFFFF00;	
	GPIOB->CRL|=0X00000088;	
	GPIOB->ODR|=0<<1;		
	  
 	TIM3->ARR=arr;  		 
	TIM3->PSC=psc;  		

	TIM3->CCMR2|=1<<8;
 	TIM3->CCMR2|=0<<12;
 	TIM3->CCMR2|=0<<10;

	TIM3->CCER|=0<<13;
	TIM3->CCER|=1<<12;

	TIM3->DIER|=1<<4;
	TIM3->DIER|=1<<0;
	TIM3->CR1|=0x01;
	
	//=============================
	//ch3
	TIM3->CCMR2|=1<<0;
 	TIM3->CCMR2|=0<<4;
 	TIM3->CCMR2|=0<<2;

	TIM3->CCER|=0<<9;
	TIM3->CCER|=1<<8;

	TIM3->DIER|=1<<3;
	TIM3->DIER|=1<<0;
	TIM3->CR1|=0x01;

  MY_NVIC_Init(2,0,TIM3_IRQn,2);
}

static void Init(void)
{
  
  RCC->APB2ENR|=1<<3;
	   	 
	GPIOB->CRH&=0XFFFF00FF; 
	GPIOB->CRH|=0X00003300;
  GPIOB->ODR|=3<<10;
  
	Trig1=0;
	Trig2=0;
	
	
	
	TIM3CapInit(0xffff,72-1);

}



static u32 GetTime(u8 ch)
{
	u32 temp=0;
	
	switch(ch)
	{
		case 0:
			Trig1=1;
			delay_us(20);
			Trig1=0;
			delay_ms(60);
		
			if(TIM3CH3_CAPTURE_STA&0x80)
			{
				temp=TIM3CH3_CAPTURE_STA&0x3f;
				temp*=65536;
				temp+=TIM3CH3_CAPTURE_VAL;
				
				TIM3CH3_CAPTURE_STA=0;
			}
		
		
			break;
		case 1:
			Trig2=1;
			delay_us(20);
			Trig2=0;
			delay_ms(60);
		
			if(TIM3CH4_CAPTURE_STA&0x80)
			{
				temp=TIM3CH4_CAPTURE_STA&0x3f;
				temp*=65536;
				temp+=TIM3CH4_CAPTURE_VAL;
				
				TIM3CH4_CAPTURE_STA=0;
			}
			break;
	}
	

	
	return temp;
}
static u32 Time2Length(u32 time)
{
	u32 cm;
	cm = ((float)time*1.7)/100;
	return cm;
}

u8  TIM3CH3_CAPTURE_STA=0;
u16	TIM3CH3_CAPTURE_VAL;
u8  TIM3CH4_CAPTURE_STA=0;  				
u16	TIM3CH4_CAPTURE_VAL;


void WaveCheck()
{
	static u8 ch;
	u32 temp;
	while(1)
	{		
		switch(ch)
		{
			case 0:
				Trig1=1;
				delay_us(11);
				Trig1=0;
				Timer.Delay(65);
				if(TIM3CH3_CAPTURE_STA&0x80)
				{
					temp=TIM3CH3_CAPTURE_STA&0x3f;
					temp*=65536;
					temp+=TIM3CH3_CAPTURE_VAL;
					if(len1)
					*len1=(float)temp*.017;
					
				}
				else
					if(len1)
					*len1=10000;
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
					if(len2)
					*len2=(float)temp*.017;
					
				}
				else
					if(len2)
					*len2=10000;
				TIM3CH4_CAPTURE_STA=0;
				ch=0;
				break;
		}
	}
  //Timer.Tick.add(60);
}

void setLenghtData(u16* l,u16* r)
{
	len1=l;
	len2=r;
}

void TIM3_IRQHandler(void)
{ 
	u16 tsr;
	tsr=TIM3->SR;
	
	
 	if((TIM3CH3_CAPTURE_STA&0X80)==0)
	{
		if(tsr&0X01)
		{	    
			if(TIM3CH3_CAPTURE_STA&0X40)
			{
				if((TIM3CH3_CAPTURE_STA&0X3F)==0X3F)
				{
					TIM3CH3_CAPTURE_STA|=0X80;
					TIM3CH3_CAPTURE_VAL=0XFFFF;
				}else TIM3CH3_CAPTURE_STA++;
			}	 
		}
		if(tsr&0x08)
		{	
			if(TIM3CH3_CAPTURE_STA&0X40)
			{	  			
				TIM3CH3_CAPTURE_STA|=0X80;
			    TIM3CH3_CAPTURE_VAL=TIM3->CCR3;
	 			TIM3->CCER&=~(1<<9);
			}else
			{
				TIM3CH3_CAPTURE_STA=0;
				TIM3CH3_CAPTURE_VAL=0;
				TIM3CH3_CAPTURE_STA|=0X40;
	 			TIM3->CNT=0;
	 			TIM3->CCER|=1<<9;
			}		    
		}			     	    					   
 	}	
	
 	if((TIM3CH4_CAPTURE_STA&0X80)==0)
	{
		if(tsr&0X01)
		{	    
			if(TIM3CH4_CAPTURE_STA&0X40)
			{
				if((TIM3CH4_CAPTURE_STA&0X3F)==0X3F)
				{
					TIM3CH4_CAPTURE_STA|=0X80;
					TIM3CH4_CAPTURE_VAL=0XFFFF;
				}else TIM3CH4_CAPTURE_STA++;
			}
		}
		if(tsr&0x10)
		{	
			if(TIM3CH4_CAPTURE_STA&0X40)
			{	  			
				TIM3CH4_CAPTURE_STA|=0X80;
			    TIM3CH4_CAPTURE_VAL=TIM3->CCR4;
	 			TIM3->CCER&=~(1<<13);
			}else
			{
				TIM3CH4_CAPTURE_STA=0;
				TIM3CH4_CAPTURE_VAL=0;
				TIM3CH4_CAPTURE_STA|=0X40;
	 			TIM3->CNT=0;
	 			TIM3->CCER|=1<<13;
			}
		}			     	    					   
 	}
	TIM3->SR=0;
 
}


const WaveBase Wave={
  Init,
  setLenghtData,
  //Time2Length,
};


