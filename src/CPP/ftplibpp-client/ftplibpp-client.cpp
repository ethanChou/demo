// ftplibpp-client.cpp : �������̨Ӧ�ó������ڵ㡣
//

#include "stdafx.h"
#include <io.h>

#include "ftplib.h"

#if NDEBUG
#pragma comment(lib,"..\\Release\\ftplibpp.lib")
#else
#pragma comment(lib,"..\\Debug\\ftplibpp.lib")
#endif

DWORD WINAPI ThreadPorc(LPVOID pM)
{
	printf("���̵߳�ID��Ϊ��%d\n���߳����hello world!\n",GetCurrentThreadId());

	ftplib *ftp = new ftplib();

	int res=ftp->Connect("192.168.0.151:21");
	res=ftp->Login("test","test");

	//GetTempPath()
	if (access("d:\\ftp\\setup.exe",0)==0)
	{
		printf("�ļ�����");

		return 0;
	}

	res=ftp->Get("d:\\ftp\\setup.exe.tmp","/ftp/setup.exe",ftplib::transfermode::image);

	if(res==1){
		rename("d:\\ftp\\setup.exe.tmp","d:\\ftp\\setup.exe");
		MessageBoxW(NULL,_T("�������"),_T("INFO"),0);
		WinExec("d:\\ftp\\setup.exe", SW_SHOW);

	}
	//res=ftp->Dir("d:\\ftp\\t.txt", "/ftp/hik");
	res=ftp->Quit();


	return 0;
}

int _tmain(int argc, _TCHAR* argv[])
{
	printf("��򵥵Ĵ����̵߳�ʵ��\n");

	HANDLE handle = CreateThread(NULL,0,ThreadPorc,NULL,0,NULL);

	WaitForSingleObject(handle,INFINITE);
	Sleep(1000);
	DWORD dwExitCode = 0;  
	GetExitCodeThread(handle, &dwExitCode);  
	if (dwExitCode == STILL_ACTIVE)  
	{  
		TerminateThread(handle,-1);  
		CloseHandle(handle);  
	}  


	DWORD nBufferLength = MAX_PATH;
	LPTSTR lpBuffer = (new TCHAR[nBufferLength]);
	DWORD tempPath = GetTempPath(nBufferLength, lpBuffer);
	TCHAR* temp=TEXT("elec");

	memcpy(lpBuffer+tempPath,temp,lstrlen(temp)*sizeof(TCHAR));

	bool f= CreateDirectory(lpBuffer,NULL);

	//system("pause");

	return 0;
}

