========================================================================
    CONSOLE APPLICATION : UTF8Conversion Project Overview
========================================================================

Console test application for Unicode UTF-8 <-> UTF-16 conversions.

Reusable code implementing UTF-8 conversion routines is contained in
"utf8conv.h" (public header) and "utf8conv_inl.h" (inline 
implementations) header files.

This code just uses Win32 Platform SDK and C++ standard library; 
so it can be built also with the Express editions of Visual Studio.


2011, October 15th

Giovanni Dicanio <gdicanio@mvps.org>

http://lib.csdn.net/article/cplusplus/21551?knId=1153
/////////////////////////////////////////////////////////////////////////////
http://www.cnblogs.com/time-is-life/p/5660046.html

string��wstring��cstring�� char�� tchar��int��dwordת������(ת)
string��wstring��cstring�� char�� tchar��int��dwordת������(ת) 
 
������һֱͷʹ�⼯�����͵�ת������֪������תȴ���Ǽǲ�ס�����ϵ�����������ȥ��������С��һ�¡��Ա��Ժ󷽱�ʹ�ã���Ȼ��Щ�������ܲ������µģ�������򵥵ģ����Ƕ����Լ��Ѿ��˽��ʹ������Ӧ�÷���Ķࣺ 
1��stringתwstring 
wstring s2ws(const string& s) 
{ 
    _bstr_t t = s.c_str(); 
    wchar_t* pwchar = (wchar_t*)t; 
    wstring result = pwchar; 
    return result; 
} 
2��wstringתstring 
string ws2s(const wstring& ws) 
{ 
    _bstr_t t = ws.c_str(); 
    char* pchar = (char*)t; 
    string result = pchar; 
    return result; 
} 
3��stringתcstring  
a��CString.format("%s", string.c_str());   
   
b��CString StringToCString(string str) 
{ 
CString result; 
for (int i=0;i<(int)str.length();i++) 
{ 
 result+=str[i]; 
} 
return result; 
} 
   
4��cstringתstring 
a��void ConvertCString2string(CString& strSrc,std::string& strDes) 
{ 
#ifndef UNICODE 
    strDes = strSrc; 
#else 
USES_CONVERSION; 
    strDes = W2A(strSrc.LockBuffer()); 
    strSrc.UnlockBuffer(); 
#endif 
} 
b�� 
string s(CString.GetBuffer());   
ReleaseBuffer()�� 
 
GetBuffer()��һ��ҪReleaseBuffer(),�����û���ͷŻ�������ռ�Ŀռ�. 
c�� 
string CStringToString(CString cstr)
{
string result(cstr.GetLength(),'e');
for (int i=0;i<cstr.GetLength();i++)
{
 result[i]=(char)cstr[i];
}
return result;
} 
5��stringתchar * 
a��char *p = string.c_str(); 
������ 
string aa("aaa"); 
char *c=aa.c_str(); 
string mngName�� 
char t[200]; 
memset(t,0,200); 
strcpy(t,mngName.c_str()); 
b��һ��һ���ַ��ĸ�ֵ 
   
char *p = new char[sring�ĳ���+1]; 
 
p[string�ĳ���]='/0'; 
 
����Ҫע�����ֵ'/0'!!! 
 
   
 
char * StringToChar(string &str) 
 
{ 
 
int len=str.length(); 
 
char * p= new char[len+1]; 
 
for (int i=0;i<len;i++) 
 
{ 
 
p[i]=str[i]; 
 
} 
 
p[len]='/0'; 
 
} 
 
6��char* תstring 
string s(char *); 
���ֻ�ܳ�ʼ�����ڲ��ǳ�ʼ���ĵط���û�����assign()�� 
 
string CharToString(char*arr,int count)
{
string result(arr,4);
return result;
} 
string��ansi�����ַ�char 
 
TCHAR��unicode�����ַ�wchar_t 
 
7��stringתTCHAR * 
 
/*
  wBuf ����Ϊָ�뼴�ɡ�
*/
wchar_t *chr2wch(const char *buffer)
{
        size_t len = strlen(buffer);
        size_t wlen = MultiByteToWideChar(CP_ACP, 0, (const char*)buffer, int(len), NULL, 0);
        wchar_t *wBuf = new wchar_t[wlen + 1];
        MultiByteToWideChar(CP_ACP, 0, (const char*)buffer, int(len), wBuf, int(wlen));
        return wBuf;
} 

8��TCHAR *תstring 
 
char * wch2chr(LPCTSTR lpString)
{
// Calculate unicode string length.
UINT len = wcslen(lpString)*2;
char *buf = (char *)malloc(len);
UINT i = wcstombs(buf,lpString,len);
return buf;
} 
 
9��string ��char*תint 
 
string ת int
..............................
char* ת int 
 #include <stdlib.h> 
  
 int atoi(const char *nptr); 
 long atol(const char *nptr); 
 long long atoll(const char *nptr); 
 long long atoq(const char *nptr);  
 
10��intתchar*��string 
 
��stdlib.h���и�����itoa() 
 itoa���÷��� 
 itoa(i,num,10); 
 i ��Ҫת�����ַ������� 
 num ת���󱣴��ַ��ı���  
 
11��wstringתCsting 
 
std::wstringתCString 
 
CString str( filename.c_str() );  
 
 
12��Cstringתwstring 
CStringתstd::wstring 
 
std::wstring str = filename.GetString(); 
 
13��Cstringתchar * 
 
CString cstr(asdd); 
 
const char* ch = (LPCTSTR)cstr; 
 
������ 
 
CString   str= "i   am   good ";  
 
char*   lp=str.GetBuffer(str.GetLength());  
 
str.ReleaseBuffer();  
 
14��char *תCstring 
 
������ 
 
CString   str;  
 
char   pStr[100];  
 
str.Format( "%s ",pStr); 
 
 
15��TCHarתchar 
 
***********************************************************************  
 
* ������ THCAR2Char  
 
* ��������TCHAR* ת��Ϊ char*  
 
***********************************************************************  
 
char* CPublic::THCAR2char(TCHAR* tchStr)  
 
{  
 
int iLen = 2*wcslen(tchStr);//CString,TCHAR������һ���ַ�����˲�����ͨ���㳤��  
 
char* chRtn = new char[iLen+1]  
 
wcstombs(chRtn,tchStr,iLen+1);//ת���ɹ�����Ϊ�Ǹ�ֵ  
 
return chRtn;  
 
}  
 
16��charתtchar 
 
 
������UNICODE��֮��TCHAR���ǿ��ַ�wchar_t������TCHAR��char��һ����^_ 
   
�������������������ư���һ�н��Ǹ���..... 
����ժ¼�����磺 
.............................................................. 
��C++��׼�����⡷��˵��   
�������������Խ��ַ���������ת��Ϊ�ַ������C��string   
1.data(),����û��"/0"���ַ�������   
2,c_str()��������"/0"���ַ�������   
3��copy()  
................................................................. 
int ת CString�� 
CString.Format("%d",int); 
............................... 
string ת CString   
CString.format("%s", string.c_str());   
��c_str()ȷʵ��data()Ҫ��.   
....................................... 
char* ת CString   
CString.format("%s", char*);  
 CString strtest;   
 char * charpoint;   
 charpoint="give string a value";   
 strtest=charpoint; //ֱ�Ӹ�ֵ 
................................................................... 
CString ת int 
 CString  ss="1212.12";   
 int temp=atoi(ss); //atoi _atoi64��atol 
    
���ַ�ת��Ϊ����������ʹ��atoi��_atoi64��atol��   
int int_chage = atoi((lpcstr)ss) ; 
�� 
   CString str = "23"; 
   UINT uint; 
   sscanf(str, "%d", uint); 
.............................. 
string ת int 
.............................. 
char* ת int  
 #include <stdlib.h>  
    
 int atoi(const char *nptr);  
 long atol(const char *nptr);  
 long long atoll(const char *nptr);  
 long long atoq(const char *nptr);  
................................................................... 
CString ת string 
  string s(CString.GetBuffer());   
  GetBuffer()��һ��ҪReleaseBuffer(),�����û���ͷŻ�������ռ�Ŀռ�.   
.......................................... 
int ת string  
.......................................... 
char* ת string   
 string s(char *);   
 ���ֻ�ܳ�ʼ�����ڲ��ǳ�ʼ���ĵط���û�����assign(). 
................................................................... 
CString ת char *  
 CString strtest="wwwwttttttt"; 
 charpoint=strtest.GetBuffer(strtest.GetLength()); 
CStringת�� char[100]   
 char a[100];   
 CString str("aaaaaa");   
 strncpy(a,(LPCTSTR)str,sizeof(a)); 
  CString  str="aaa";    
  char*  ch;    
  ch=(char*)(LPCTSTR)str; 
.......................................... 
int ת char * 
 ��stdlib.h���и�����itoa()  
 itoa���÷���  
 itoa(i,num,10);  
 i ��Ҫת�����ַ�������  
 num ת���󱣴��ַ��ı���  
 10 ת�����ֵĻ��������ƣ�10����˵����10����ת�����֡���������2��8��16�ȵ���ϲ���Ľ�������  
 ԭ�Σ�char *itoa(int value, char* string, int radix);  
 ʵ����  
 #include "stdlib.h"  
 #include "stdio.h"  
 main()  
 {  
 int i=1234;  
 char s[5];  
 itoa(i,s,10);  
 printf("%s",s);  
 getchar();  
} 
.......................................... 
string ת char *   
char *p = string.c_str();   
   
 string aa("aaa");  
 char *c=aa.c_str(); 
 string mngName��  
 char t[200];  
 memset(t,0,200);  
 strcpy(t,mngName.c_str()); 
................................................................... 
��׼C��û��string,char *==char []==string 
������CString.Format("%s",char *)�����������char *ת��CString��Ҫ��CStringת��char *���ò� 
������LPCSTR��CString�Ϳ����ˡ� 
cannot convert from 'const char *' to 'char *'  
const char *c=aa.c_str();   
string.c_str()ֻ��ת����const char * 
 
 
 
 
 
 
 
 
 
 
 
#include <string> 
// ʹ��CString����ʹ��MFC�����Ҳ��ɰ���<windows.h> 
#define _AFXDLL 
#include <afx.h> 
using namespace std; 
//������������������������������������������������������- 
//�� ���ֽ�char* ת��Ϊ ���ֽ� wchar* 
inline wchar_t* AnsiToUnicode( const char* szStr ) 
{ 
int nLen = MultiByteToWideChar( CP_ACP, MB_PRECOMPOSED, szStr, -1, NULL, 0 ); 
if (nLen == 0) 
{ 
   return NULL; 
} 
wchar_t* pResult = new wchar_t[nLen]; 
MultiByteToWideChar( CP_ACP, MB_PRECOMPOSED, szStr, -1, pResult, nLen ); 
return pResult; 
} 
//������������������������������������������������������- 
// �� ���ֽ�wchar_t* ת�� ���ֽ�char* 
inline char* UnicodeToAnsi( const wchar_t* szStr ) 
{ 
int nLen = WideCharToMultiByte( CP_ACP, 0, szStr, -1, NULL, 0, NULL, NULL ); 
if (nLen == 0) 
{ 
   return NULL; 
} 
char* pResult = new char[nLen]; 
WideCharToMultiByte( CP_ACP, 0, szStr, -1, pResult, nLen, NULL, NULL ); 
return pResult; 
} 
//������������������������������������������������������- 
// �����ַ� string ת��Ϊ���ַ� wstring 
inline void Ascii2WideString( const std::string& szStr, std::wstring& wszStr ) 
{ 
int nLength = MultiByteToWideChar( CP_ACP, 0, szStr.c_str(), -1, NULL, NULL ); 
wszStr.resize(nLength); 
LPWSTR lpwszStr = new wchar_t[nLength]; 
MultiByteToWideChar( CP_ACP, 0, szStr.c_str(), -1, lpwszStr, nLength ); 
wszStr = lpwszStr; 
delete [] lpwszStr; 
} 
//������������������������������������������������������- 
int _tmain(int argc, _TCHAR* argv[]) 
{ 
char*   pChar = "��ϲ��char"; 
wchar_t* pWideChar = L"������wchar_t"; 
wchar_t   tagWideCharList[100] ; 
char   ch = 'A'; 
char   tagChar[100] = {NULL}; 
CString   cStr; 
std::string str; 
 
 
// ע���������Ի����Ա����WideChar 
setlocale(LC_ALL,"chs"); 
 
 
// ע�� char* ת�� wchar_t* 
// ע�� wchar_t δ���� << �����Բ���ʹ�� cout << ��� 
pWideChar = AnsiToUnicode( pChar ); 
// ע��printf("%ls") �� wprintf(L"%s") һ�� 
printf( "%ls/n", pWideChar ); 
 
 
// ע��wchar_t* ת�� wchar_t[] 
wcscpy ( tagWideCharList, pWideChar ); 
wprintf( L"%s/n", tagWideCharList ); 
 
 
// ע��wchar_t[] ת�� wchar_t* 
pWideChar = tagWideCharList; 
wprintf( L"%s/n", pWideChar ); 
 
 
// ע��char ת�� string 
str.insert( str.begin(), ch ); 
cout << str << endl; 
 
 
// ע��wchar_t* ת�� string 
pWideChar = new wchar_t[str.length()]; 
swprintf( pWideChar, L"%s", str.c_str()); 
wprintf( L"%s/n", pWideChar ); 
 
 
// ע��string ת�� char* 
pChar = const_cast<char*>(str.c_str()); 
cout << pChar << endl; 
 
 
// ע��char* ת�� string 
str = std::string(pChar); 
// ע�� cout �� << ������string, ��printf �Ļ����� printf("%s", str.c_str()); 
//   ������ print( "%s", str ); ��Ϊ str �Ǹ� string �� 
cout << str << endl; 
 
 
// ע��string ת�� char[] 
str = "���İ�����"; 
strcpy( tagChar, str.c_str() ); 
printf( "%s/n", tagChar ); 
 
 
// ע��string ת�� CString; 
cStr = str.c_str(); 
 
 
// ע��CString ת�� string 
str = string(cStr.GetBuffer(cStr.GetLength())); 
 
 
// ע��char* ת�� CString 
cStr = pChar; 
 
 
// ע��CString ת�� char* 
pChar = cStr.GetBuffer( cStr.GetLength() ); 
 
 
// ע��CString ת�� char[] 
strncpy( tagChar, (LPCTSTR)CString, sizeof(tagChar)); 
 
 
// ע��CString ת�� wchar_t* 
pWideChar = cStr.AllocSysString(); 
printf( "%ls/n", pWideChar ); 
} 
 
