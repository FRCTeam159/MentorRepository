/*
 * OI.h
 *
 *  Created on: Jun 3, 2014
 *      Author: alex
 */

#ifndef OI_H_
#define OI_H_

#include "WPILib.h"

class OI {
private:
	Joystick* stick;
public:
	OI();
	Joystick* GetJoystick();
	float GetX();
	float GetY();
	float GetZ();
};

#endif /* OI_H_ */
