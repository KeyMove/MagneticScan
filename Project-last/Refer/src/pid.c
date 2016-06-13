#include"PID.h"

static void Init(PIDStruct *pid,u8 P,u8 I,u8 D) {
	pid->value = pid->pre =pid->sum= pid->last = 0;
	pid->P = P;
	pid->I = I;
	pid->D = D;
}


static s16 Update(PIDStruct *pid, s16 v1) {
	s32 v2;
	v1 = pid->value - v1;
	v2 = (((s32)pid->P*v1) - ((s32)pid->I*pid->last) + ((s32)pid->D * pid->pre));
	//y(n)=y(n-1)+Kp*[(e(n)-(n-1))+I*e(n)+D*(e(n)-2*e(n-1)+e(e-2))]
	//v2 = pid->last+ pid->P*(((v1 - pid->last))+(pid->I*v1)+(pid->D*(v1 - 2 * pid->last + pid->pre)));
	//v2 = ((s32)pid->P*(v1 - pid->last));
	//v2 += ((s32)pid->I*v1);
	//v2 += ((s32)pid->D*(v1 - 2 * pid->last + pid->pre));
	//v2 *= pid->P;
	//v2 += pid->last;
	pid->pre = pid->last;
	pid->last = v1;
    v2/=100;
	pid->sum = v2;
	if(pid->sum>pid->max)pid->sum=pid->max;
	else if(pid->sum<pid->min)pid->sum=pid->min;
	return pid->sum;
}

const PIDBase PID = {
	Init,
	Update,
};


