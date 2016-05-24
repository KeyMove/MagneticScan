#include "mpu6050.h"

static u8 Init(void)
{ 
	u8 res;
	IIC.Init();
	MPU.Write_Byte(MPU_PWR_MGMT1_REG,0X80);
  delay_ms(100);
	MPU.Write_Byte(MPU_PWR_MGMT1_REG,0X00);
	MPU.Set_Gyro_Fsr(3);
	MPU.Set_Accel_Fsr(0);
	MPU.Set_Rate(50);
	MPU.Write_Byte(MPU_INT_EN_REG,0X00);
	MPU.Write_Byte(MPU_USER_CTRL_REG,0X00);
	MPU.Write_Byte(MPU_FIFO_EN_REG,0X00);
	MPU.Write_Byte(MPU_INTBP_CFG_REG,0X80);
	res=MPU.Read_Byte(MPU_DEVICE_ID_REG);
	if(res==MPU_ADDR)//器件ID正确
	{
		MPU.Write_Byte(MPU_PWR_MGMT1_REG,0X01);
		MPU.Write_Byte(MPU_PWR_MGMT2_REG,0X00);
		MPU.Set_Rate(50);
 	}else return 1;
	return 0;
}
//设置MPU6050陀螺仪传感器满量程范围
//fsr:0,±250dps;1,±500dps;2,±1000dps;3,±2000dps
//返回值:0,设置成功
//    其他,设置失败 
static u8 Set_Gyro_Fsr(u8 fsr)
{
	return MPU.Write_Byte(MPU_GYRO_CFG_REG,fsr<<3);//设置陀螺仪满量程范围  
}
//设置MPU6050加速度传感器满量程范围
//fsr:0,±2g;1,±4g;2,±8g;3,±16g
//返回值:0,设置成功
//    其他,设置失败 
static u8 Set_Accel_Fsr(u8 fsr)
{
	return MPU.Write_Byte(MPU_ACCEL_CFG_REG,fsr<<3);//设置加速度传感器满量程范围  
}
//设置MPU6050的数字低通滤波器
//lpf:数字低通滤波频率(Hz)
//返回值:0,设置成功
//    其他,设置失败 
static u8 Set_LPF(u16 lpf)
{
	u8 data=0;
	if(lpf>=188)data=1;
	else if(lpf>=98)data=2;
	else if(lpf>=42)data=3;
	else if(lpf>=20)data=4;
	else if(lpf>=10)data=5;
	else data=6; 
	return MPU.Write_Byte(MPU_CFG_REG,data);//设置数字低通滤波器  
}
//设置MPU6050的采样率(假定Fs=1KHz)
//rate:4~1000(Hz)
//返回值:0,设置成功
//    其他,设置失败 
static u8 Set_Rate(u16 rate)
{
	u8 data;
	if(rate>1000)rate=1000;
	if(rate<4)rate=4;
	data=1000/rate-1;
	data=MPU.Write_Byte(MPU_SAMPLE_RATE_REG,data);	//设置数字低通滤波器
 	return MPU.Set_LPF(rate/2);	//自动设置LPF为采样率的一半
}

//得到温度值
//返回值:温度值(扩大了100倍)
static short Get_Temperature(void)
{
    u8 buf[2]; 
    short raw;
	float temp;
	MPU.Read_Len(MPU_ADDR,MPU_TEMP_OUTH_REG,2,buf); 
    raw=((u16)buf[0]<<8)|buf[1];  
    temp=36.53+((double)raw)/340;  
    return temp*100;;
}
//得到陀螺仪值(原始值)
//gx,gy,gz:陀螺仪x,y,z轴的原始读数(带符号)
static u8 Get_Gyroscope(short *gx,short *gy,short *gz)
{
    u8 buf[6],res;  
	res=MPU.Read_Len(MPU_ADDR,MPU_GYRO_XOUTH_REG,6,buf);
	if(res==0)
	{
		*gx=((u16)buf[0]<<8)|buf[1];  
		*gy=((u16)buf[2]<<8)|buf[3];  
		*gz=((u16)buf[4]<<8)|buf[5];
	} 	
    return res;;
}
//得到加速度值(原始值)
//gx,gy,gz:陀螺仪x,y,z轴的原始读数(带符号)
static u8 Get_Accelerometer(short *ax,short *ay,short *az)
{
    u8 buf[6],res;  
	res=MPU.Read_Len(MPU_ADDR,MPU_ACCEL_XOUTH_REG,6,buf);
	if(res==0)
	{
		*ax=((u16)buf[0]<<8)|buf[1];  
		*ay=((u16)buf[2]<<8)|buf[3];  
		*az=((u16)buf[4]<<8)|buf[5];
	} 	
    return res;;
}

static u8 Write_Len(u8 addr,u8 reg,u8 len,u8 *buf)
{
	u8 i; 
    IIC.Start(); 
	IIC.Send_Byte((addr<<1)|0);
	if(IIC.Wait_Ack())
	{
		IIC.Stop();		 
		return 1;		
	}
    IIC.Send_Byte(reg);
    IIC.Wait_Ack();
	for(i=0;i<len;i++)
	{
		IIC.Send_Byte(buf[i]);
		if(IIC.Wait_Ack())
		{
			IIC.Stop();	 
			return 1;		 
		}		
	}    
    IIC.Stop();	 
	return 0;	
} 

static u8 Read_Len(u8 addr,u8 reg,u8 len,u8 *buf)
{ 
 	IIC.Start(); 
	IIC.Send_Byte((addr<<1)|0);
	if(IIC.Wait_Ack())
	{
		IIC.Stop();		 
		return 1;		
	}
    IIC.Send_Byte(reg);
    IIC.Wait_Ack();
    IIC.Start();
	IIC.Send_Byte((addr<<1)|1);
    IIC.Wait_Ack();
	while(len)
	{
		if(len==1)*buf=IIC.Read_Byte(0);
		else *buf=IIC.Read_Byte(1);
		len--;
		buf++; 
	}    
    IIC.Stop();
	return 0;	
}

static u8 Write_Byte(u8 reg,u8 data) 				 
{ 
    IIC.Start(); 
	IIC.Send_Byte((MPU_ADDR<<1)|0);
	if(IIC.Wait_Ack())
	{
		IIC.Stop();		 
		return 1;		
	}
    IIC.Send_Byte(reg);
    IIC.Wait_Ack();
	IIC.Send_Byte(data);
	if(IIC.Wait_Ack())
	{
		IIC.Stop();	 
		return 1;		 
	}		 
    IIC.Stop();	 
	return 0;
}

static u8 Read_Byte(u8 reg)
{
	u8 res;
    IIC.Start(); 
	IIC.Send_Byte((MPU_ADDR<<1)|0);
	IIC.Wait_Ack();
    IIC.Send_Byte(reg);
    IIC.Wait_Ack();
    IIC.Start();
	IIC.Send_Byte((MPU_ADDR<<1)|1);
    IIC.Wait_Ack();
	res=IIC.Read_Byte(0);
    IIC.Stop();
	return res;		
}


const MPUBase MPU={
  Init,
  Set_Gyro_Fsr,
  Set_Accel_Fsr,
  Set_LPF,
  Set_Rate,
  Get_Temperature,
  Get_Gyroscope,
  Get_Accelerometer,
  Write_Len,
  Read_Len,
  Write_Byte,
  Read_Byte,
};
