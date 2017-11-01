// EfficiencyNativeCPPDLL.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"

#include "EfficiencyNativeCPPDLL.h"
#define WIN32_LEAN_AND_MEAN             //  从 Windows 头文件中排除极少使用的信息  
#include <windows.h>  
#include <stdio.h>  
#include <stdlib.h>  
#include <math.h>  
#include <time.h>  
  
  
EfficiencyNativeCppDll::EfficiencyNativeCppDll(void)  
{  
    Pot = 0;  
}  
  
  
EfficiencyNativeCppDll::~EfficiencyNativeCppDll(void)  
{  
    printf("~EfficiencyNativeCppDll is called");  
}  
  
void EfficiencyNativeCppDll::InitPositions()  
{  
    for(int i = 0; i < DIMS; i++)  
    {  
        for (int j = 0; j < NPARTS; j++)  
        {  
            _r[i][j] = 0.5 + (double)rand()/RAND_MAX;  
        }  
    }  
}  
  
void EfficiencyNativeCppDll::UpdatePositions()  
{  
    for(int i = 0; i < DIMS; i++)  
    {  
        for (int j = 0; j < NPARTS; j++)  
        {  
            _r[i][j] -= 0.5 + (double)rand()/RAND_MAX;  
        }  
    }  
}  
  
double EfficiencyNativeCppDll::ComputePot()  
{  
    double distx, disty, distz, dist;  
    double pot;  
    distx = 0;  
    disty = 0;  
    distz = 0;  
    pot = 0;  
  
    for(int i=0; i<NPARTS; i++ )   
    {  
        for(int j=0; j<i-1; j++ )   
        {  
            distx = pow( (_r[0][j] - _r[0][i]), 2 );  
            disty = pow( (_r[1][j] - _r[1][i]), 2 );  
            distz = pow( (_r[2][j] - _r[2][i]), 2 );  
            dist = sqrt( distx + disty + distz );  
            pot += 1.0 / dist;  
        }         
    }  
  
    this->Pot = pot;  
  
    return pot;  
}  