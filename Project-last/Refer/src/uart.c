#include"uart.H"

const UARTBase UART={
	UART_Init,
	UART_SendByte,
	UART_SendString,
	UART_ReSetBps,
	UART_OnRecv
};

void (*OnRecv)(u8);
static u32 f;
void UART_Init(u8 f_mhz,u32 b,UARTCALLBACK fun){
	f=f_mhz*1000000;
	OnRecv=fun;
	//RCC->APB2ENR=RCC_APB2ENR_USART1EN;
	RCC->APB2ENR|=RCC_APB2ENR_IOPAEN;
	RCC->APB2ENR|=RCC_APB2ENR_USART1EN;
	GPIOA->CRH&=0xfffff00f;
	GPIOA->CRH|=0x000008b0;
	GPIOA->ODR|=GPIO_ODR_ODR10|GPIO_ODR_ODR9;
	USART1->CR1=USART1->CR2=USART1->CR3=0;
	USART1->CR1=USART_CR1_RE|USART_CR1_TE|USART_CR1_RXNEIE;
	NVIC_SetPriority(USART1_IRQn,2);
	NVIC_EnableIRQ(USART1_IRQn);
	UART_ReSetBps(b);
}

void UART_ReSetBps(u32 b)
{
	u32 baud=(25*f)/(4*b);
	u32 t=(baud/100)<<4;
	u32 t2=baud-(100*(t>>4));
	USART1->CR1&=~USART_CR1_UE;
	USART1->BRR=t|((((t2*16)+50)/100)&0x0f);
	USART1->CR1|=USART_CR1_UE;
}

void UART_SetEnable(u8 set){
	
}

void UART_SendByte(u8 dat){
	USART1->DR=dat;
	while(!(USART1->SR&USART_SR_TC));
}

void UART_SendString(u8* p){
	while(*p)
		UART_SendByte(*p++);
}

void USART1_IRQHandler(void)
{
	u8 dat=USART1->DR;
	USART1->SR;
	OnRecv(dat);
}

void UART_OnRecv(void(*p)(u8)){
	OnRecv=p;
}
