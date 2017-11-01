// ConsoleCPPInvokeDLL.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"

#include "stdafx.h"  
#include "EfficiencyNativeCppDll.h"  
#include <time.h>  
#define NITER 201  
  
int _tmain(int argc, _TCHAR* argv[])  
{  
    int i;  
    clock_t start, stop;  
  
    EfficiencyNativeCppDll* effPro = new EfficiencyNativeCppDll();  
    effPro->InitPositions();  
    effPro->UpdatePositions();  
  
    start=clock();  
    for( i=0; i<NITER; i++ ) {  
        effPro->Pot = 0.0;     
          
        /* 
        //低效率模式 
        effPro->ComputePot(); 
        if (i%10 == 0) printf("%5d: Potential: %10.3f\n", i, effPro->Pot); 
        */  
          
        //高效率模式 ??不晓得为什么这样的速度会快那么多 快了近20倍  
        if (i%10 == 0) printf("%5d: Potential: %10.3f\n", i, effPro->ComputePot());  
          
        effPro->UpdatePositions();  
    }  
    stop=clock();  
    printf ("Seconds = %10.9f\n",(double)(stop-start)/ CLOCKS_PER_SEC);  
      
    delete effPro;  
  
    getchar();  
      
}  

