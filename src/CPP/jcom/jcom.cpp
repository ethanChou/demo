// jcom.cpp : ���� DLL Ӧ�ó���ĵ���������
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