#include "RC522.h"
#include "SPI.H"
#include "Timer.h"

#define MAXRLEN 18

void wait(u16 time){
  vu16 i=time;
  while(i--);
}

#define nop() wait(2000);

void WriteRawRC(u8 reg,u8 data){
  RC522_CS_CLR();
  SPI.Write((reg<<1)&0x7e);
  SPI.Write(data);
  RC522_CS_SET();
}

u8 ReadRawRC(u8 reg)
{
  u8 data;
  RC522_CS_CLR();
  SPI.Write(((reg<<1)&0x7e)|BIT7);
  data=SPI.Read();
  RC522_CS_SET();
  return data;
}

/////////////////////////////////////////////////////////////////////
//功    能：置RC522寄存器位
//参数说明：reg[IN]:寄存器地址
//          mask[IN]:置位值
/////////////////////////////////////////////////////////////////////
void SetBitMask(unsigned char reg,unsigned char mask)  
{
    char tmp = 0x0;
    tmp = ReadRawRC(reg);
    WriteRawRC(reg,tmp | mask);  // set bit mask
}

/////////////////////////////////////////////////////////////////////
//功    能：清RC522寄存器位
//参数说明：reg[IN]:寄存器地址
//          mask[IN]:清位值
/////////////////////////////////////////////////////////////////////
void ClearBitMask(unsigned char reg,unsigned char mask)  
{
    char tmp = 0x0;
    tmp = ReadRawRC(reg);
    WriteRawRC(reg, tmp & ~mask);  // clear bit mask
} 


static void init(){
	SETBIT(RCC->APB2ENR,RCC_APB2ENR_IOPAEN);
  SETBIT(GPIOA->ODR,BIT3|BIT4);
  GPIO_PP(GPIOA,0,00011000);
	SPI.Init();
  RC522_RST_SET();
    nop();
    RC522_RST_CLR();
    nop();
    RC522_RST_SET();
    nop();
    WriteRawRC(CommandReg,PCD_RESETPHASE);
    nop();
    
    WriteRawRC(ModeReg,0x3D);            //和Mifare卡通讯，CRC初始值0x6363
    WriteRawRC(TReloadRegL,30);           
    WriteRawRC(TReloadRegH,0);
    WriteRawRC(TModeReg,0x8D);
    WriteRawRC(TPrescalerReg,0x3E);
    WriteRawRC(TxAutoReg,0x40);
    
    ClearBitMask(Status2Reg,0x08);
    WriteRawRC(ModeReg,0x3D);
    WriteRawRC(RxSelReg,0x86);
    WriteRawRC(RFCfgReg,0x7F);
    WriteRawRC(TReloadRegL,30);
    WriteRawRC(TReloadRegH,0);
    WriteRawRC(TModeReg,0x8D);
    WriteRawRC(TPrescalerReg,0x3E);
    
    if (!(ReadRawRC(TxControlReg) & 0x03))
    {
        SetBitMask(TxControlReg, 0x03);
    }
}



/////////////////////////////////////////////////////////////////////
//功    能：通过RC522和ISO14443卡通讯
//参数说明：Command[IN]:RC522命令字
//          pInData[IN]:通过RC522发送到卡片的数据
//          InLenByte[IN]:发送数据的字节长度
//          pOutData[OUT]:接收到的卡片返回数据
//          *pOutLenBit[OUT]:返回数据的位长度
/////////////////////////////////////////////////////////////////////
char PcdComMF522(u8 Command, 
                 u8 *pInData, 
                 u8 InLenByte,
                 u8 *pOutData, 
                 u16  *pOutLenBit)
{
    s8 status = MI_ERR;
    unsigned char irqEn   = 0x00;
    unsigned char waitFor = 0x00;
    unsigned char lastBits;
    unsigned char n;
    unsigned int i;
    switch (Command)
    {
       case PCD_AUTHENT:
          irqEn   = 0x12;
          waitFor = 0x10;
          break;
       case PCD_TRANSCEIVE:
          irqEn   = 0x77;
          waitFor = 0x30;
          break;
       default:
         break;
    }
   
    WriteRawRC(ComIEnReg,irqEn|0x80);
    ClearBitMask(ComIrqReg,0x80);
    WriteRawRC(CommandReg,PCD_IDLE);
    SetBitMask(FIFOLevelReg,0x80);
    
    for (i=0; i<InLenByte; i++)
    {   WriteRawRC(FIFODataReg, pInData[i]);    }
    WriteRawRC(CommandReg, Command);
   
    
    if (Command == PCD_TRANSCEIVE)
    {    SetBitMask(BitFramingReg,0x80);  }
    
//    i = 600;//根据时钟频率调整，操作M1卡最大等待时间25ms
 i = 2000;
    do 
    {
         n = ReadRawRC(ComIrqReg);
         i--;
    }
    while ((i!=0) && !(n&0x01) && !(n&waitFor));
    ClearBitMask(BitFramingReg,0x80);
	if(n==0x45)i=0;
    if (i!=0)
    {    
         if(!(ReadRawRC(ErrorReg)&0x1B))
         {
             status = MI_OK;
             if (n & irqEn & 0x01)
             {   status = MI_NOTAGERR;   }
             if (Command == PCD_TRANSCEIVE)
             {
               	n = ReadRawRC(FIFOLevelReg);
              	lastBits = ReadRawRC(ControlReg) & 0x07;
                if (lastBits)
                {   *pOutLenBit = (n-1)*8 + lastBits;   }
                else
                {   *pOutLenBit = n*8;   }
                if (n == 0)
                {   n = 1;    }
                if (n > MAXRLEN)
                {   n = MAXRLEN;   }
                for (i=0; i<n; i++)
                {   pOutData[i] = ReadRawRC(FIFODataReg);    }
            }
         }
         else
         {   status = MI_ERR;   }
        
   }
   

   SetBitMask(ControlReg,0x80);           // stop timer now
   WriteRawRC(CommandReg,PCD_IDLE); 
   return status;
}




/////////////////////////////////////////////////////////////////////
//功    能：寻卡
//参数说明: req_code[IN]:寻卡方式
//                0x52 = 寻感应区内所有符合14443A标准的卡
//                0x26 = 寻未进入休眠状态的卡
//          pTagType[OUT]：卡片类型代码
//                0x4400 = Mifare_UltraLight
//                0x0400 = Mifare_One(S50)
//                0x0200 = Mifare_One(S70)
//                0x0800 = Mifare_Pro(X)
//                0x4403 = Mifare_DESFire
//返    回: 成功返回MI_OK
/////////////////////////////////////////////////////////////////////
u16 PcdRequest(u8 req_code)
{
   u16 status;  
   u16  unLen;
   u8 ucComMF522Buf[MAXRLEN]; 
   
   ClearBitMask(Status2Reg,0x08);
   WriteRawRC(BitFramingReg,0x07);
   SetBitMask(TxControlReg,0x03);
 
   ucComMF522Buf[0] = req_code;

   status = PcdComMF522(PCD_TRANSCEIVE,ucComMF522Buf,1,ucComMF522Buf,&unLen);

   if ((status == MI_OK) && (unLen == 0x10))
   {    
       _16T8H(status) = ucComMF522Buf[0];
       _16T8L(status) = ucComMF522Buf[1];
   }
   else
   {   status = 0xffff;   }
   
   return status;
}


u32 PcdAnticoll()
{
    u32 status=0xffffffff;
    u8 i,snr_check=0;
    u16  unLen;
    u8 ucComMF522Buf[MAXRLEN]; 
    
    ClearBitMask(Status2Reg,0x08);
    WriteRawRC(BitFramingReg,0x00);
    ClearBitMask(CollReg,0x80);
 
    ucComMF522Buf[0] = PICC_ANTICOLL1;
    ucComMF522Buf[1] = 0x20;

    status= PcdComMF522(PCD_TRANSCEIVE,ucComMF522Buf,2,ucComMF522Buf,&unLen);
    if (status == MI_OK)
    {
      
    	 for (i=0; i<4; i++)
         {   
             snr_check ^= ucComMF522Buf[i];
         }
         if (snr_check != ucComMF522Buf[i])
         {
           status = 0xffffffff;    
         }
         else{
           _32T8HH(status)=ucComMF522Buf[0];
          _32T8H(status)=ucComMF522Buf[1];
          _32T8L(status)=ucComMF522Buf[2];
          _32T8LL(status)=ucComMF522Buf[3];
         }
    }
    else{
      status = 0xffffffff;
    }
    SetBitMask(CollReg,0x80);
    return status;
}

ReadCallBack LoopRead=0;

void SetCallBack(ReadCallBack callback){
  LoopRead=callback;
}

void StartFindCard(){
  static u32 ID;
  static u8 Loop;
  static u8 mode;
  u8 n;
   while(1)
   {
     
     switch(mode)
     {
     case 0:
       ClearBitMask(Status2Reg,0x08);
        WriteRawRC(BitFramingReg,0x07);
        SetBitMask(TxControlReg,0x03);
       break;
     case 1:
       ClearBitMask(Status2Reg,0x08);
        WriteRawRC(BitFramingReg,0x00);
        ClearBitMask(CollReg,0x80);
       break;
     }
     
   
   
    WriteRawRC(ComIEnReg,0x77|0x80);
    ClearBitMask(ComIrqReg,0x80);
    WriteRawRC(CommandReg,PCD_IDLE);
    SetBitMask(FIFOLevelReg,0x80);
    
    
    switch(mode)
     {
     case 0:
       WriteRawRC(FIFODataReg, 0x52);
       break;
     case 1:
       WriteRawRC(FIFODataReg, 0x93);
       WriteRawRC(FIFODataReg, 0x20);
       break;
     }
    WriteRawRC(CommandReg, PCD_TRANSCEIVE);
    SetBitMask(BitFramingReg,0x80);
    
    Loop = 100;
    do 
    {
				Timer.Delay(1);
         n = ReadRawRC(ComIrqReg);
         Loop--;
    }
    while ((Loop!=0) && !(n&0x01) && !(n&0x30));
    ClearBitMask(BitFramingReg,0x80);
	if(n==0x45){Loop=0;mode=0xff;}
      while(Loop!=0){
        Loop=0;
         if((ReadRawRC(ErrorReg)&0x1B))
           continue;   
         if (n & 0x77 & 0x01)
           continue;
         //ReadRawRC(FIFOLevelReg);
         //ReadRawRC(ControlReg);
         switch(mode)
         {
         case 0:
           mode=0xff;
           if(ReadRawRC(FIFODataReg)!=0x04)continue;
           if(ReadRawRC(FIFODataReg)!=0x00)continue;
           mode=1;
           break;
         case 1:
           mode=0xff;
           n=0;
           n^=_32T8HH(ID)=ReadRawRC(FIFODataReg);
           n^=_32T8H(ID)=ReadRawRC(FIFODataReg);
           n^=_32T8L(ID)=ReadRawRC(FIFODataReg);
           n^=_32T8LL(ID)=ReadRawRC(FIFODataReg);
           if(n!=ReadRawRC(FIFODataReg))break;
           if(LoopRead)
             LoopRead(ID);
           mode=0;
           break;
         }
      }
   SetBitMask(ControlReg,0x80);           // stop timer now
   WriteRawRC(CommandReg,PCD_IDLE); 
   switch(mode)
   {
   case 0:SetBitMask(CollReg,0x80);break;
   case 1:break;
   }
	 Timer.Delay(10);
   if(mode==0xff)
     mode=0;
   //Loop=250 ;  
   //while(Loop--);
   }
}


const RC522Base RC522 = {
	init,	
    0,//PcdRequest,
    0,//PcdAnticoll,
    SetCallBack,
    StartFindCard,
};

#undef nop
