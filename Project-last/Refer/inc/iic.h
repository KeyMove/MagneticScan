#ifndef _IIC_H_
#define _IIC_H_
#include "stm32f10x.h"
#include "sys.h"
#include "delay.h"

//=========================
#define SDA_IN()  {GPIOB->CRH&=0XFF0FFFFF;GPIOB->CRH|=8<<20;}
#define SDA_OUT() {GPIOB->CRH&=0XFF0FFFFF;GPIOB->CRH|=3<<20;}


#define IIC_SCL    PBout(12) //SCL
#define IIC_SDA    PBout(13) //SDA	 
#define READ_SDA   PBin(13)  //ÊäÈëSDA 


typedef struct
{
  void (*Init)(void);
  void (*Start)(void);
  void (*Stop)(void);
  u8 (*Wait_Ack)(void);
  void (*Ack)(void);
  void (*NAck)(void);
  void (*Send_Byte)(u8 txd);
  u8 (*Read_Byte)(unsigned char ack);
}IICBase;
extern const IICBase IIC;

#endif
