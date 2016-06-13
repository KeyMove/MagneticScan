#ifndef SPI_H
#define SPI_H

#include "mcuhead.h"

#define hw 0

#define SPI_MISO_SET() SETBIT(GPIOA->ODR,BIT6)
#define SPI_MISO_CLR() CLRBIT(GPIOA->ODR,BIT6)
#define SPI_MISO() (GPIOA->IDR&BIT6)

#define SPI_MOSI_SET() SETBIT(GPIOA->BSRR,BIT7)
#define SPI_MOSI_CLR() SETBIT(GPIOA->BRR,BIT7)

#define SPI_CLK_SET() SETBIT(GPIOA->BSRR,BIT5)
#define SPI_CLK_CLR() SETBIT(GPIOA->BRR,BIT5)

typedef struct {
	void(*Init)(void);
	void(*Write)(u8);
	u8(*Read)();
}SPIBase;

extern const SPIBase SPI;

#endif
