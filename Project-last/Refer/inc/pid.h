#ifndef _PID_H_
#define _PID_H_

#include"mcuhead.h"

typedef struct {
	s32 value;
	s32 pre;
	s32 last;
	s32 sum;
	s16 P;
	s16 I;
	s16 D;
	s16 max;
	s16 min;
}PIDStruct;

typedef struct {
	void(*Init)(PIDStruct *pid, u8 P, u8 I, u8 D);
	s16(*Update)(PIDStruct* pid,s16 value);
}PIDBase;

extern const PIDBase PID;

#endif
