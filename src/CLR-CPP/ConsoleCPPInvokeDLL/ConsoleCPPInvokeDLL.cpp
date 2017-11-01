// ConsoleCPPInvokeDLL.cpp : �������̨Ӧ�ó������ڵ㡣
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
        //��Ч��ģʽ 
        effPro->ComputePot(); 
        if (i%10 == 0) printf("%5d: Potential: %10.3f\n", i, effPro->Pot); 
        */  
          
        //��Ч��ģʽ ??������Ϊʲô�������ٶȻ����ô�� ���˽�20��  
        if (i%10 == 0) printf("%5d: Potential: %10.3f\n", i, effPro->ComputePot());  
          
        effPro->UpdatePositions();  
    }  
    stop=clock();  
    printf ("Seconds = %10.9f\n",(double)(stop-start)/ CLOCKS_PER_SEC);  
      
    delete effPro;  
  
    getchar();  
      
}  

