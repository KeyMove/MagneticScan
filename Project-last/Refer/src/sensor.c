#include "Sensor.h"
#include "SPI.h"
static void init(){
	SPI.Init();
	HC165_CS_SET();
	GPIO_PP(GPIOA,0,00010000);
}

static SensorStatus ReadData(){
	SensorStatus v=0;
#if MaxSensor <= 8
	HC165_CS_CLR();
	v=SPI.Read();
#else
	u8 i;
	HC165_CS_CLR();
	for(i=0;i<(MaxSensor+7)/8;i++)
		{
			v<<=8;
			v|=SPI.Read();
		}
#endif
	HC165_CS_SET();
	return v;
}

const SensorBase Sensor = {
	init,	
	ReadData,
};
