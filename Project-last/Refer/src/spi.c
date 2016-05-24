#include "spi.h"

#if !hw
static void Init() {
	SETBIT(RCC->APB2ENR,RCC_APB2ENR_IOPAEN);
	SETBIT(GPIOA->ODR, BIT7 | BIT6 | BIT5);
	GPIO_PP(GPIOA,0,10100000);
	GPIO_PU(GPIOA,0,01000000);
}
static void WriteByte(u8 temp)
{
  u8 i;
  for(i=0;i<8;i++)
  {
	  SPI_CLK_CLR();
	  if (temp&BIT7)
		  SPI_MOSI_SET();
	  else
		  SPI_MOSI_CLR();
	  SPI_CLK_SET();
	  temp <<= 1;
  }
}

static u8 ReadByte(void)
{
  u8 i,dat1;
  dat1=0;
  for(i=0;i<8;i++)
  {	
		SPI_CLK_CLR();
		dat1<<=1;
	  if (SPI_MISO())
		  dat1++;
		SPI_CLK_SET();
  }
  return dat1;
}


#else
static void Init(void)
{
	SPI_CR1 = 3;

}
static void WriteByte(u8 x)//Ó²¼þSPIÐ´
{
	while (!(SPI_SR&MASK_SPI_SR_TXE));
	SPI_DR = x;
}
static u8 ReadByte(void)//Ó²¼þSPIÐ´
{
	SPI_DR = 0xff;
	while (!(SPI_SR&MASK_SPI_SR_RXNE));
	return SPI_DR;
}
#endif


const SPIBase SPI = {
	Init,
	WriteByte,
	ReadByte,
};
