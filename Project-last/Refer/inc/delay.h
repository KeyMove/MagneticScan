#ifndef __DELAY_H
#define __DELAY_H 			   
#include "sys.h"  

void delay_init(u8 sysclk);
void delay_ms(u32 nms);
void delay_us(u32 nus);
void delay_sms(u32 nms);


#endif





























