#ifndef _PathFind_H_
#define _PathFind_H_

#include"mcuhead.h"

typedef struct {
	void(*Init)(void);
}PathFindBase;

extern const PathFindBase PathFind;

#endif
