#ifndef _ADC_H_
#define _ADC_H_

#include"mcuhead.h"

typedef struct {
	void(*Init)(void);
	u16(*getADCValue)(u8 ch);
}ADCBase;

extern const ADCBase ADC;

#endif
