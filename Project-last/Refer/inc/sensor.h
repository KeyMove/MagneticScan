#ifndef _Sensor_H_
#define _Sensor_H_

#include "mcuhead.h"

#define HC165_CS_SET() SETBIT(GPIOA->BRR,BIT4)
#define HC165_CS_CLR() SETBIT(GPIOA->BSRR,BIT4)

#define MaxSensor 9

#if MaxSensor <= 8
#define SensorStatus u8
#endif
#if MaxSensor>8&&MaxSensor <= 16
#define SensorStatus u16
#endif
#if MaxSensor>16&&MaxSensor <= 32
#define SensorStatus u32
#endif
#if MaxSensor>32&&MaxSensor <= 64
#define SensorStatus u64
#endif

typedef struct {
	void(*Init)(void);
	SensorStatus(*Read)(void);
}SensorBase;

extern const SensorBase Sensor;

#endif
