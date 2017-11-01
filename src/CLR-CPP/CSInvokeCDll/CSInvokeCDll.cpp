
#include "stdafx.h"
#include "malloc.h"
//extern "C" __declspec(dllexport)
#define CSEXPORT extern "C"  __declspec(dllexport)

#ifdef CSEXPORT
#define CSEXPORT_API extern "C" __declspec(dllexport)
#else
#define CSEXPORT_API extern "C" __declspec(dllimport)
#endif


typedef struct {
	char name[32];
	int age;
} User; 

typedef struct      
{    
    int osVersion;    
    int majorVersion;    
    int minorVersion;    
    int buildNum;    
    int platFormId;    
    char szVersion[128];    
}OSINFO;    

typedef struct  
{  
    char name[20];  
    int age;  
    double scores[30];  
}Student;  
  
// Class中包含结构体数组类型  
typedef struct  
{  
    int number;  
    Student students[50];  
}ClassGroup;  


CSEXPORT_API int  Add(int x, int y) 
{ 
	return x + y; 
}

CSEXPORT_API int  Sub(int x, int y) 
{ 
	return x - y; 
}

CSEXPORT_API int  Multiply(int x, int y) 
{ 
	return x * y; 
}

CSEXPORT_API int  Divide(int x, int y) 
{ 
	return x / y; 
}

 CSEXPORT_API int  Sum(int * parr, int length)
{
	int sum = 0;
	if(parr == NULL)
		return sum;

	for(int i = 0; i < length; i++)
	{
		sum += *parr++;
	}

	return sum;
}

CSEXPORT_API User*  Create(char* name, int age)    
{   
	User* user = (User*)malloc(sizeof(User));
	strcpy(user->name, name);
	user->age = age;

	return user; 
}      

// 1. 获取版本信息(传递结构体指针)    
CSEXPORT_API bool GetVersionPtr( OSINFO *info )
{
	info->buildNum=9;
	info->majorVersion=8;
	info->minorVersion=7;
	info->osVersion=6;
	info->platFormId=5;

	strcpy(info->szVersion,"1234");
	return true;
}
// 2.获取版本信息(传递结构体引用)    
CSEXPORT_API bool GetVersionRef(OSINFO &info)
{
	info.buildNum=0;
	info.majorVersion=1;
	info.minorVersion=2;
	info.osVersion=3;
	info.platFormId=4;

	strcpy(info.szVersion,"56789");
	return true;
}

CSEXPORT_API int GetClassGroup(ClassGroup *pClass,int len){

	
	for(int i=0;i<len;i++)
	{
		ClassGroup cg;
		cg.number=i;
		memcpy(pClass,&cg,sizeof(ClassGroup));
		pClass++;
		
	}

	return 0;
}


CSEXPORT_API int InitClassGroup(ClassGroup *pClass){

	for(int i=0;i<50;i++)
	{
		 Student m;
		 m.age=i;
		
		 strcpy(m.name,"xxxxxxx");
		 for(int j=0;j<30;j++)
			  m.scores[j]=j;

		 memcpy(&(pClass->students[i]),&m,sizeof(Student));
		
		 
	}

	return 0;
}