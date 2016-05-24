#ifndef _Motor_H_
#define _Motor_H_

#include "mcuhead.h"

#define MOTOR_L_AB() SETBIT(GPIOB->BRR,BIT6|BIT7);SETBIT(GPIOB->BSRR,BIT6)
#define MOTOR_L_BA() SETBIT(GPIOB->BRR,BIT6|BIT7);SETBIT(GPIOB->BSRR,BIT7)
#define MOTOR_L_ST() SETBIT(GPIOB->BRR,BIT6|BIT7)

#define MOTOR_R_AB() SETBIT(GPIOB->BRR,BIT4|BIT5);SETBIT(GPIOB->BSRR,BIT4)
#define MOTOR_R_BA() SETBIT(GPIOB->BRR,BIT4|BIT5);SETBIT(GPIOB->BSRR,BIT5)
#define MOTOR_R_ST() SETBIT(GPIOB->BRR,BIT4|BIT5)

typedef struct {
	void(*Init)(void);
	void(*Speed)(u8 lr,s8 duty);
}MotorBase;

extern const MotorBase Motor;

#endif
