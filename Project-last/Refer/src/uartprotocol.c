#include"uartprotocol.h"
#include"uart.h"

//u8 databuff[RECV_BUFF_LEN+SEND_BUFF_LEN];//定义收发缓冲区

UartCallBack UartCmdEvent[MAX_CMD];//命令回调




#ifndef UARTENABLE

typedef struct{
	void(*SendByte)(u8);
}UARTBase;

static void send(u8 dat)
{

}
	

const UARTBase UART={
	send,
};

#endif


u16 packlen=0;//包长度
u8 *packbuff;//接收区缓存
u8 *sendbuff;//发送区缓存
u8 recvmode=0;//
u8 RecvFlag=0;//接收标志位
u8 RecvCheck=0;//接收校验
u8 TimeOut=0;
u16 LinkTimeOut = 0;
u8 recvcmd=0;//接收到的串口命令
u8 savecmd;//上一次发送的命令
//u8 aliveflag=0;

static const UARTDATA UD;

u8 AutoACK;//接收数据流
u16 recvdatapos;
u16 senddatapos;

void OnRecvData(u8 dat)
{
	static u16 count;
	TimeOut=PacketTime;
	switch(recvmode)
	{
		case 0:
			if(dat==0x55)
				recvmode=1;
			break;
		case 1:
			packlen=dat;
			count=0;
			RecvCheck=0;
			recvmode=5;
			break;
		case 2:
			packbuff[count]=dat;
			RecvCheck+=dat;
			count++;
			if(packlen<=count){
				recvmode=3;
			}
			break;
		case 3:
			if(dat!=RecvCheck){
				recvmode=0;
				RecvFlag=RF_ERROR;
			}
			else{
				recvmode=4;
			}
			break;
		case 4:
			recvmode=0;
				RecvFlag=RF_OK;
			break;
		case 5:
			packlen<<=8;
			packlen|=dat;
			if(packlen>RECV_BUFF_LEN+SEND_BUFF_LEN)
			{
				recvmode=0;
				break;
			}
			recvmode=6;
			break;
		case 6:
			recvcmd=dat;
			recvmode=2;
			break;
	}
}

void TimeOutTick(void) {
	if(TimeOut)
		if (--TimeOut == 0) {
			recvmode = 0;
		}
	if (LinkTimeOut)
		LinkTimeOut--;
}
//-----------------------------------------------------------------------------

static void Setoffset(s16 offset,u8 flag)
{
	if(flag){
		recvdatapos+=offset;
	}
	else{
		recvdatapos=offset;
	}
}

static u8 ReadByte(){
	if((recvdatapos+1)>packlen){
		return 0;
	}
	return packbuff[recvdatapos++];
}

static u8 WriteByte(u8 dat){
	if((senddatapos+1)>SEND_BUFF_LEN){
		return 0;
	}
	sendbuff[senddatapos++]=dat;
	return 1;
}

static u16 ReadWord(){
	u16 v;
	if((recvdatapos+2)>packlen){
		return 0;
	}
	_16T8H(v)=packbuff[recvdatapos++];
	_16T8L(v)=packbuff[recvdatapos++];
	return v;
}

static u8 WriteWord(u16 dat){
	if((senddatapos+2)>SEND_BUFF_LEN){
		return 0;
	}
	sendbuff[senddatapos++]=_16T8H(dat);
	sendbuff[senddatapos++]=_16T8L(dat);
	return 1;
}

static u32 ReadDWord(){
	u32 v;
	if((recvdatapos+4)>packlen){
		return 0;
	}
	_32T8HH(v)=packbuff[recvdatapos++];
	_32T8H(v)=packbuff[recvdatapos++];
	_32T8L(v)=packbuff[recvdatapos++];
	_32T8LL(v)=packbuff[recvdatapos++];
	return v;
}

static u8 WriteDWord(u32 dat){
	if((senddatapos+4)>SEND_BUFF_LEN){
		return 0;
	}
	sendbuff[senddatapos++]=_32T8HH(dat);
	sendbuff[senddatapos++]=_32T8H(dat);
	sendbuff[senddatapos++]=_32T8L(dat);
	sendbuff[senddatapos++]=_32T8LL(dat);
	return 1;
}

static u16 ReadBuff(u8 *buff,u16 len)
{
	if((len+recvdatapos)>=packlen){
		len=packlen-recvdatapos;
	}
	while(len--){
		*buff++=packbuff[recvdatapos++];
	}
	return len;
}

static u16 WriteBuff(u8 *buff,u16 len)
{
	if((len+recvdatapos)>=SEND_BUFF_LEN){
		len=SEND_BUFF_LEN-recvdatapos;
	}
	while(len--){
		sendbuff[senddatapos++]=*buff++;
	}
	return len;
}

static u16 Writestr(u8 *str)
{
	u16 len=0;
	while((*str!=0)&&(recvdatapos<SEND_BUFF_LEN))
	{
		sendbuff[senddatapos++]=*str++;
		len++;
	}
	return len;
}

static u8* getbuff()
{
	return &packbuff[recvdatapos];
}

static u16 getlen(){
	return packlen;
};

static u8 getcmd(){
	return recvcmd;
}

static u8* getsendbuff() {
	return &sendbuff[senddatapos];
}
//---------------------------------------------------------------------------------

static void Init(u8* databuff)
{
	u16 i;
	for(i=0;i<MAX_CMD;i++)
	{
		UartCmdEvent[i]=0;
	}
	for(i=0;i<RECV_BUFF_LEN+SEND_BUFF_LEN;i++){
		databuff[i]=0;
	}
	packbuff=databuff;
	sendbuff=&databuff[RECV_BUFF_LEN];
}

void SendPack(u8 cmd,u8 *buff,u16 len)
{
	u8 ck=0;
	u8 val;
	UART.SendByte(0x55);
	UART.SendByte(_16T8H(len));
	UART.SendByte(_16T8L(len));
	UART.SendByte(cmd);
	while(len--)
	{
		val=*buff;
		UART.SendByte(val);
		ck+=val;
		buff++;
	}
	UART.SendByte(ck);
	UART.SendByte(0xAA);
}

void SendCmdPack(u8 cmd)
{
	UART.SendByte(0x55);
	UART.SendByte(0);
	UART.SendByte(1);
	UART.SendByte(cmd);
	UART.SendByte(0);
	UART.SendByte(0);
	UART.SendByte(0xAA);
}

void RegisterCmdEvent(u8 cmd,UartCallBack function)
{
	UartCmdEvent[cmd]=function;
}

void UnRegisterCmdEvent(UartCallBack function)
{
	u8 i;
	for(i=0;i<MAX_CMD;i++)
	{
		if(UartCmdEvent[i]==function)
		{
			UartCmdEvent[i]=0;
			return;
		}
	}
}

void aack(u8 stats)
{
	AutoACK=stats;
}

void ackpack(u8 *buff,u16 len)
{
	SendPack(recvcmd,buff,len);
	AutoACK+=2;
}

void sendackpacket(void)
{
	if(senddatapos==0)
	{
		WriteByte(0);
	}
	ackpack(sendbuff,senddatapos);
}

void UartCmd(void)
{	
	if(RecvFlag==RF_OK){
		RecvFlag=0;
		recvdatapos=0;
		senddatapos=0;
		LinkTimeOut = LinkTime;
		if(UartCmdEvent[recvcmd]!=0)
		{
			UartCmdEvent[recvcmd]((UartEvent)&UD);
			if(AutoACK==1){
				SendCmdPack(recvcmd);
			}
			else{
				AutoACK&=1;
			}
		}
	}
	savecmd=recvcmd;
}

u8 isLink(void) {
	return LinkTimeOut != 0;
}

//--------------------------------------------------------------------

static const UARTDATA UD={
	Setoffset,
	ReadByte,
	ReadWord,
	ReadDWord,
	ReadBuff,
	getbuff,
	getlen,
	getcmd,
	WriteByte,
	WriteWord,
	WriteDWord,
	WriteBuff,
	Writestr,
	getsendbuff,
	sendackpacket,
};

const UartProtocolBase UartProtocol={
	Init,
	RegisterCmdEvent,
	UnRegisterCmdEvent,
	aack,
	SendCmdPack,
	ackpack,
	SendPack,
	UartCmd,
	isLink,
};


