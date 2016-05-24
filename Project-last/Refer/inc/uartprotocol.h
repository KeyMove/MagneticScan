#ifndef _uartprotocol_h
#define _uartprotocol_h

#include"mcuhead.h"

//���ջ�������С
#define RECV_BUFF_LEN 64
//���ͻ�������С
#define SEND_BUFF_LEN 64
//�����������
#define MAX_CMD 16
//����ID��
#define pubilc typedef

typedef enum
        {
			Alive = 0,
			GetData=1,
			SetData=2,

        }PacketCmd;

//������ɱ�־λ
#define RF_OK 		1
//���մ����־λ
#define RF_ERROR 	2

//��ʼλ��ƫ��
#define SEEK_BEGIN  0
//��ǰλ��ƫ��
#define SEEK_OFFSET 1

#define LinkTime 2000
#define PacketTime 30




//------------------------------------------------------------------

typedef struct{
	void(*Seek)(s16,u8);
	//��ȡһ���ֽ�
	u8(*ReadByte)();
	//��ȡ�����ֽ�
	u16(*ReadWord)();
	//��ȡ�ĸ��ֽ�
	u32(*ReadDWord)();
	//��ȡָ���ֽ�
	u16(*ReadBuff)(u8* buff,u16 len);

	u8*(*GetBuff)();
	u16(*GetLen)();
	u8(*GetCmd)();
	u8(*WriteByte)(u8);
	u8(*WriteWord)(u16);
	u8(*WriteDWord)(u32);
	u16(*WriteBuff)(u8* buff,u16 len);
	u16(*WriteString)(u8*);
	u8*(*GetSendBuff)(void);
	void(*SendAckPacket)(void);
}UARTDATA;

typedef UARTDATA* UartEvent;
typedef void(*UartCallBack)(UartEvent);

typedef struct{
	//����Э���ʼ��
	//��ʼ�����н�����ص����ݣ��������Զ���ʼ������
	//buff:���շ��ͻ�����
	void(*Init)(u8* buff);
	
	//ע��һ�����һ������
	//���յ���������ʱ����Զ����øú���
	//��������Ϊ void function(UartEvent e);
	//ͨ������e���ɷ����յ���ȫ������
	//cmd��Ҫע�������ID function������
	void(*RegisterCmd)(u8 cmd,UartCallBack function);
	

	//ע��һ������
	//ע���ú�����������
	//function������
	void(*unRegisterCmd)(UartCallBack function);
	

	//�Զ���Ӧ
	//����ص��������Ƿ��ͻ�Ӧ��
	//1���Զ����ͻ�Ӧ�� 0�����Զ����ͻ�Ӧ��
	void(*AutoAck)(u8 stats);
	
	
	//���ͻ�Ӧ��
	//����ָ��ID�ŵĻ�Ӧ��
	//��������ˡ��Զ���Ӧ����ִ�лص�����ִ������Զ�����һ����Ӧ��
	//cmd����Ӧ������ID
	void(*SendACKPacket)(u8 cmd);
	
	//���ʹ����ݻ�Ӧ��
	//���͵�ǰID�ŵĻ�Ӧ��
	//��������ˡ��Զ���Ӧ������Ḳ�ǵ��Զ���Ӧ����������
	//buff���������� len�����ݳ���
	void(*SendACKDataPacket)(u8 *buff,u16 len);
	

	//�������ݰ�
	//��ָ��������������
	//cmd������id buff���������� len�����ݳ���
	void(*SendPacket)(u8 cmd,u8* buff,u16 len);
	
	
	//����Ƿ�������ݰ�
	void(*Check)(void);
	
	//�Ƿ�������״̬
	u8(*isLink)(void);
}UartProtocolBase;

extern const UartProtocolBase UartProtocol;

void OnRecvData(u8 dat);
void TimeOutTick(void);

#endif
