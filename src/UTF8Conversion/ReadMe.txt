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

string、wstring、cstring、 char、 tchar、int、dword转换方法(转)
string、wstring、cstring、 char、 tchar、int、dword转换方法(转) 
 
最近编程一直头痛这集中类型的转化，明知都可以转却总是记不住，不断的上网查来查去，在这里小结一下。以备以后方便使用，当然有些方法可能不是最新的，或者最简单的，但是对于自己已经了解的使用起来应该方便的多： 
1》string转wstring 
wstring s2ws(const string& s) 
{ 
    _bstr_t t = s.c_str(); 
    wchar_t* pwchar = (wchar_t*)t; 
    wstring result = pwchar; 
    return result; 
} 
2》wstring转string 
string ws2s(const wstring& ws) 
{ 
    _bstr_t t = ws.c_str(); 
    char* pchar = (char*)t; 
    string result = pchar; 
    return result; 
} 
3》string转cstring  
a）CString.format("%s", string.c_str());   
   
b）CString StringToCString(string str) 
{ 
CString result; 
for (int i=0;i<(int)str.length();i++) 
{ 
 result+=str[i]; 
} 
return result; 
} 
   
4》cstring转string 
a）void ConvertCString2string(CString& strSrc,std::string& strDes) 
{ 
#ifndef UNICODE 
    strDes = strSrc; 
#else 
USES_CONVERSION; 
    strDes = W2A(strSrc.LockBuffer()); 
    strSrc.UnlockBuffer(); 
#endif 
} 
b） 
string s(CString.GetBuffer());   
ReleaseBuffer()； 
 
GetBuffer()后一定要ReleaseBuffer(),否则就没有释放缓冲区所占的空间. 
c） 
string CStringToString(CString cstr)
{
string result(cstr.GetLength(),'e');
for (int i=0;i<cstr.GetLength();i++)
{
 result[i]=(char)cstr[i];
}
return result;
} 
5》string转char * 
a）char *p = string.c_str(); 
举例： 
string aa("aaa"); 
char *c=aa.c_str(); 
string mngName； 
char t[200]; 
memset(t,0,200); 
strcpy(t,mngName.c_str()); 
b）一个一个字符的赋值 
   
char *p = new char[sring的长度+1]; 
 
p[string的长度]='/0'; 
 
但是要注意最后赋值'/0'!!! 
 
   
 
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
 
6》char* 转string 
string s(char *); 
你的只能初始化，在不是初始化的地方最好还是用assign()； 
 
string CharToString(char*arr,int count)
{
string result(arr,4);
return result;
} 
string是ansi编码字符char 
 
TCHAR是unicode编码字符wchar_t 
 
7》string转TCHAR * 
 
/*
  wBuf 申明为指针即可。
*/
wchar_t *chr2wch(const char *buffer)
{
        size_t len = strlen(buffer);
        size_t wlen = MultiByteToWideChar(CP_ACP, 0, (const char*)buffer, int(len), NULL, 0);
        wchar_t *wBuf = new wchar_t[wlen + 1];
        MultiByteToWideChar(CP_ACP, 0, (const char*)buffer, int(len), wBuf, int(wlen));
        return wBuf;
} 

8》TCHAR *转string 
 
char * wch2chr(LPCTSTR lpString)
{
// Calculate unicode string length.
UINT len = wcslen(lpString)*2;
char *buf = (char *)malloc(len);
UINT i = wcstombs(buf,lpString,len);
return buf;
} 
 
9》string 和char*转int 
 
string 转 int
..............................
char* 转 int 
 #include <stdlib.h> 
  
 int atoi(const char *nptr); 
 long atol(const char *nptr); 
 long long atoll(const char *nptr); 
 long long atoq(const char *nptr);  
 
10》int转char*和string 
 
在stdlib.h中有个函数itoa() 
 itoa的用法： 
 itoa(i,num,10); 
 i 需要转换成字符的数字 
 num 转换后保存字符的变量  
 
11》wstring转Csting 
 
std::wstring转CString 
 
CString str( filename.c_str() );  
 
 
12》Cstring转wstring 
CString转std::wstring 
 
std::wstring str = filename.GetString(); 
 
13》Cstring转char * 
 
CString cstr(asdd); 
 
const char* ch = (LPCTSTR)cstr; 
 
举例： 
 
CString   str= "i   am   good ";  
 
char*   lp=str.GetBuffer(str.GetLength());  
 
str.ReleaseBuffer();  
 
14》char *转Cstring 
 
举例： 
 
CString   str;  
 
char   pStr[100];  
 
str.Format( "%s ",pStr); 
 
 
15》TCHar转char 
 
***********************************************************************  
 
* 函数： THCAR2Char  
 
* 描述：将TCHAR* 转换为 char*  
 
***********************************************************************  
 
char* CPublic::THCAR2char(TCHAR* tchStr)  
 
{  
 
int iLen = 2*wcslen(tchStr);//CString,TCHAR汉字算一个字符，因此不用普通计算长度  
 
char* chRtn = new char[iLen+1]  
 
wcstombs(chRtn,tchStr,iLen+1);//转换成功返回为非负值  
 
return chRtn;  
 
}  
 
16》char转tchar 
 
 
定义了UNICODE宏之后，TCHAR就是宽字符wchar_t，否则TCHAR跟char是一样的^_ 
   
具体问题具体分析，浮云啊，一切皆是浮云..... 
以下摘录自网络： 
.............................................................. 
《C++标准函数库》中说的   
有三个函数可以将字符串的内容转换为字符数组和C—string   
1.data(),返回没有"/0"的字符串数组   
2,c_str()，返回有"/0"的字符串数组   
3，copy()  
................................................................. 
int 转 CString： 
CString.Format("%d",int); 
............................... 
string 转 CString   
CString.format("%s", string.c_str());   
用c_str()确实比data()要好.   
....................................... 
char* 转 CString   
CString.format("%s", char*);  
 CString strtest;   
 char * charpoint;   
 charpoint="give string a value";   
 strtest=charpoint; //直接付值 
................................................................... 
CString 转 int 
 CString  ss="1212.12";   
 int temp=atoi(ss); //atoi _atoi64或atol 
    
将字符转换为整数，可以使用atoi、_atoi64或atol。   
int int_chage = atoi((lpcstr)ss) ; 
或： 
   CString str = "23"; 
   UINT uint; 
   sscanf(str, "%d", uint); 
.............................. 
string 转 int 
.............................. 
char* 转 int  
 #include <stdlib.h>  
    
 int atoi(const char *nptr);  
 long atol(const char *nptr);  
 long long atoll(const char *nptr);  
 long long atoq(const char *nptr);  
................................................................... 
CString 转 string 
  string s(CString.GetBuffer());   
  GetBuffer()后一定要ReleaseBuffer(),否则就没有释放缓冲区所占的空间.   
.......................................... 
int 转 string  
.......................................... 
char* 转 string   
 string s(char *);   
 你的只能初始化，在不是初始化的地方最好还是用assign(). 
................................................................... 
CString 转 char *  
 CString strtest="wwwwttttttt"; 
 charpoint=strtest.GetBuffer(strtest.GetLength()); 
CString转换 char[100]   
 char a[100];   
 CString str("aaaaaa");   
 strncpy(a,(LPCTSTR)str,sizeof(a)); 
  CString  str="aaa";    
  char*  ch;    
  ch=(char*)(LPCTSTR)str; 
.......................................... 
int 转 char * 
 在stdlib.h中有个函数itoa()  
 itoa的用法：  
 itoa(i,num,10);  
 i 需要转换成字符的数字  
 num 转换后保存字符的变量  
 10 转换数字的基数（进制）10就是说按照10进制转换数字。还可以是2，8，16等等你喜欢的进制类型  
 原形：char *itoa(int value, char* string, int radix);  
 实例：  
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
string 转 char *   
char *p = string.c_str();   
   
 string aa("aaa");  
 char *c=aa.c_str(); 
 string mngName；  
 char t[200];  
 memset(t,0,200);  
 strcpy(t,mngName.c_str()); 
................................................................... 
标准C里没有string,char *==char []==string 
可以用CString.Format("%s",char *)这个方法来将char *转成CString。要把CString转成char *，用操 
作符（LPCSTR）CString就可以了。 
cannot convert from 'const char *' to 'char *'  
const char *c=aa.c_str();   
string.c_str()只能转换成const char * 
 
 
 
 
 
 
 
 
 
 
 
#include <string> 
// 使用CString必须使用MFC，并且不可包含<windows.h> 
#define _AFXDLL 
#include <afx.h> 
using namespace std; 
//———————————————————————————- 
//将 单字节char* 转换为 宽字节 wchar* 
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
//———————————————————————————- 
// 将 宽字节wchar_t* 转换 单字节char* 
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
//———————————————————————————- 
// 将单字符 string 转换为宽字符 wstring 
inline void Ascii2WideString( const std::string& szStr, std::wstring& wszStr ) 
{ 
int nLength = MultiByteToWideChar( CP_ACP, 0, szStr.c_str(), -1, NULL, NULL ); 
wszStr.resize(nLength); 
LPWSTR lpwszStr = new wchar_t[nLength]; 
MultiByteToWideChar( CP_ACP, 0, szStr.c_str(), -1, lpwszStr, nLength ); 
wszStr = lpwszStr; 
delete [] lpwszStr; 
} 
//———————————————————————————- 
int _tmain(int argc, _TCHAR* argv[]) 
{ 
char*   pChar = "我喜欢char"; 
wchar_t* pWideChar = L"我讨厌wchar_t"; 
wchar_t   tagWideCharList[100] ; 
char   ch = 'A'; 
char   tagChar[100] = {NULL}; 
CString   cStr; 
std::string str; 
 
 
// 注：设置语言环境以便输出WideChar 
setlocale(LC_ALL,"chs"); 
 
 
// 注： char* 转换 wchar_t* 
// 注： wchar_t 未重载 << ，所以不可使用 cout << 输出 
pWideChar = AnsiToUnicode( pChar ); 
// 注：printf("%ls") 和 wprintf(L"%s") 一致 
printf( "%ls/n", pWideChar ); 
 
 
// 注：wchar_t* 转换 wchar_t[] 
wcscpy ( tagWideCharList, pWideChar ); 
wprintf( L"%s/n", tagWideCharList ); 
 
 
// 注：wchar_t[] 转换 wchar_t* 
pWideChar = tagWideCharList; 
wprintf( L"%s/n", pWideChar ); 
 
 
// 注：char 转换 string 
str.insert( str.begin(), ch ); 
cout << str << endl; 
 
 
// 注：wchar_t* 转换 string 
pWideChar = new wchar_t[str.length()]; 
swprintf( pWideChar, L"%s", str.c_str()); 
wprintf( L"%s/n", pWideChar ); 
 
 
// 注：string 转换 char* 
pChar = const_cast<char*>(str.c_str()); 
cout << pChar << endl; 
 
 
// 注：char* 转换 string 
str = std::string(pChar); 
// 注： cout 的 << 重载了string, 若printf 的话必须 printf("%s", str.c_str()); 
//   而不可 print( "%s", str ); 因为 str 是个 string 类 
cout << str << endl; 
 
 
// 注：string 转换 char[] 
str = "无聊啊无聊"; 
strcpy( tagChar, str.c_str() ); 
printf( "%s/n", tagChar ); 
 
 
// 注：string 转换 CString; 
cStr = str.c_str(); 
 
 
// 注：CString 转换 string 
str = string(cStr.GetBuffer(cStr.GetLength())); 
 
 
// 注：char* 转换 CString 
cStr = pChar; 
 
 
// 注：CString 转换 char* 
pChar = cStr.GetBuffer( cStr.GetLength() ); 
 
 
// 注：CString 转换 char[] 
strncpy( tagChar, (LPCTSTR)CString, sizeof(tagChar)); 
 
 
// 注：CString 转换 wchar_t* 
pWideChar = cStr.AllocSysString(); 
printf( "%ls/n", pWideChar ); 
} 
 
