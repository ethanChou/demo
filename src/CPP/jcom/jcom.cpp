// jcom.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include <iostream>  

extern "C" _declspec(dllexport) void hello();  

extern "C"_declspec(dllexport) int add(int a, int b);


int add(int a, int b) {
    return a + b;
}

void hello()  
{  
    printf("Hello World!\n");  
}  

void main(){}