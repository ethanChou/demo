#include "StdAfx.h"
#include "stringutils.h"
#include <atlstr.h>  
#include <comutil.h>
#include <string>
#include <sstream>
#include <iostream>
#include <stdint.h>

using namespace std;
//comsuppw.lib
#pragma comment(lib, "comsupp.lib")

std::string chartostr(const char* c){
	std::string s1(c);  
	return s1;
}

const char* strtocchar(const std::string & str){
	return str.data();
}

char* strtochar(const std::string & str){

	char* c;
	const int len = str.length();
	c =new char[len+1];
	strcpy(c,str.c_str());

	return c;
}

char* cchartochar(const char* c){
	const int len=strlen(c);
	char* pc =new char[len];//足够长
	strcpy(pc,c);
	return pc;
}

CString LPCTSTRToCString(LPCTSTR str){
	CString s1(str); 
	return s1;
}

LPCTSTR CStringToLPCTSTR(CString str){
	//LPCTSTR p2 = (LPCTSTR)str;  

	LPTSTR p3 = str.GetBuffer(); //
	str.ReleaseBuffer();
	return p3;
}

CString StrToCString(std::string& str)
{
	//C++ -> C (-> Win32) -> MFC
	return str.c_str();
}

std::string CStringToStr(CString cstr){

	/*string s(cstr.GetBuffer());
	cstr.ReleaseBuffer();*/

	//std::string tmp(str.GetBuffer(0));
	//MFC -> Win32 (-> C) -> C++

	TCHAR *m=cstr.GetBuffer(0);;
	std::wstring s1(m);
	return WStringToString(s1);

}

BOOL StringToWString(const std::string &str,std::wstring &wstr)
{    
	int nLen = (int)str.length();    
	wstr.resize(nLen,L' ');

	int nResult = MultiByteToWideChar(CP_ACP,0,(LPCSTR)str.c_str(),nLen,(LPWSTR)wstr.c_str(),nLen);

	if (nResult == 0)
	{
		return FALSE;
	}

	return TRUE;
}
//wstring高字节不为0，返回FALSE
BOOL WStringToString(const std::wstring &wstr,std::string &str)
{    
	int nLen = (int)wstr.length();    
	str.resize(nLen,' ');

	int nResult = WideCharToMultiByte(CP_ACP,0,(LPCWSTR)wstr.c_str(),nLen,(LPSTR)str.c_str(),nLen,NULL,NULL);

	if (nResult == 0)
	{
		return FALSE;
	}

	return TRUE;
}

std::wstring StringToWString(const std::string &str)
{
	std::wstring wstr(str.length(),L' ');
	std::copy(str.begin(), str.end(), wstr.begin());
	return wstr; 
}

//只拷贝低字节至string中
std::string WStringToString(const std::wstring &wstr)
{
	std::string str(wstr.length(), ' ');
	std::copy(wstr.begin(), wstr.end(), str.begin());
	return str; 

}

wstring s2ws_2(const string& s) 
{ 
	_bstr_t t = s.c_str(); 
	wchar_t* pwchar = (wchar_t*)t; 
	wstring result = pwchar; 
	return result; 
} 

string ws2s_2(const wstring& ws) 
{ 
	_bstr_t t = ws.c_str(); 
	char* pchar = (char*)t; 
	string result = pchar; 
	return result; 
} 

CString StringToCString(string str) 
{ 
	//a:CString.format("%s", string.c_str());  

	CString result; 
	for (int i=0;i<(int)str.length();i++) 
	{ 
		result+=str[i]; 
	} 
	return result; 
} 

void ConvertCString2string(CString& strSrc,std::string& strDes) 
{ 
#ifndef UNICODE 
	strDes = strSrc; 
#else 
	USES_CONVERSION; 
	strDes = W2A(strSrc.LockBuffer()); 
	strSrc.UnlockBuffer(); 
#endif 
} 

string CStringToString(CString cstr)
{
	string result(cstr.GetLength(),'e');
	for (int i=0;i<cstr.GetLength();i++)
	{
		result[i]=(char)cstr[i];
	}
	return result;
} 

char * StringToChar(string &str) 
{ 
	int len=str.length(); 
	char *p= new char[len+1]; 
	for (int i=0;i<len;i++) 
	{ 
		p[i]=str[i]; 
	} 
	p[len]='/0'; 
	return p;
} 

char* BSTR2Char(BSTR bstr){
	// 第一
	//　_bstr_t b = bstrText;
	//　　char* lpszText2 = b;

	//第二
	char* lpszText2 = _com_util::ConvertBSTRToString(bstr);
    return lpszText2;

}


BSTR Char2BSTR(const char* str){
	//char*转换成BSTR
	//	　　方法一，使用SysAllocString等API函数。例如：
	//	  　　BSTR bstrText = ::SysAllocString(L"Test");
	//　　BSTR bstrText = ::SysAllocStringLen(L"Test",4);
	//  　　BSTR bstrText = ::SysAllocStringByteLen("Test",4);

	//	　　方法二，使用COleVariant或_variant_t。例如：
	//		  　　//COleVariant strVar("This is a test");
	//			　　_variant_t strVar("This is a test");
	//	  　　BSTR bstrText = strVar.bstrVal;

	//		　　方法三，使用_bstr_t，这是一种最简单的方法。例如：
	//			  　　BSTR bstrText = _bstr_t("This is a test");

	//		  　　方法四，使用CComBSTR。例如：
	//				　　BSTR bstrText = CComBSTR("This is a test");
	//			　　或
	//				  　　CComBSTR bstr("This is a test");
	//			  　　BSTR bstrText = bstr.m_str;

	//				　　方法五，使用ConvertStringToBSTR。例如：
	//					  　　char* lpszText = "Test";
	//				  　　BSTR bstrText = _com_util::ConvertStringToBSTR(lpszText);	　　
	BSTR bstrText;
	bstrText=_com_util::ConvertStringToBSTR(str);
	return bstrText;
}

BSTR CString2BSTR(CString str){
	BSTR bstrText = str.AllocSysString();
	return bstrText;
}

CString BSTR2CString(BSTR bstr){
	CStringA str(bstr);
	return str;
}

string CharToString(char*arr,int count)
{
string result(arr,count);
return result;
} 

wchar_t *chr2wch(const char *buffer)
{
        size_t len = strlen(buffer);
        size_t wlen = MultiByteToWideChar(CP_ACP, 0, (const char*)buffer, int(len), NULL, 0);
        wchar_t *wBuf = new wchar_t[wlen + 1];
        MultiByteToWideChar(CP_ACP, 0, (const char*)buffer, int(len), wBuf, int(wlen));
        return wBuf;
} 

char * wch2chr(LPCTSTR lpString)
{
// Calculate unicode string length.
UINT len = wcslen(lpString)*2;
char *buf = (char *)malloc(len);
UINT i = wcstombs(buf,lpString,len);
return buf;
} 

///http://www.cnblogs.com/qiyebao/archive/2011/12/01/2270843.html
//
//5 CString to char *
//strcpy(char,CString,sizeof(char));
//6 char * to CString
//CString.format("%s",char*);


/*string 转 int
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
 

 　　方法一，使用MultiByteToWideChar将ANSI字符转换成Unicode字符，使用WideCharToMultiByte将Unicode字符转换成ANSI字符。

   　　方法二，使用“_T”将ANSI转换成“一般”类型字符串，使用“L”将ANSI转换成Unicode，而在托管C++环境中还可使用S将ANSI字符串转换成String*对象。例如：
	 　　TCHAR tstr[] = _T("this is a test");
	   　　wchar_t wszStr[] = L"This is a test";
		 　　String* str = S”This is a test”;

 */ 

void test(){
CString   str= "i   am   good ";  
 
char*   lp=(char*)str.GetBuffer(str.GetLength());  
 
str.ReleaseBuffer();  
}

void test2(){

CString   str;  
 
char   pStr[100];  
 
str.Format(_T("%s"),pStr); 

}

char* THCAR2char(TCHAR* tchStr)  
 
{  
 
int iLen = 2*wcslen(tchStr);//CString,TCHAR汉字算一个字符，因此不用普通计算长度  
 
char* chRtn = new char[iLen+1]  ;
 
wcstombs(chRtn,tchStr,iLen+1);//转换成功返回为非负值  
 
return chRtn;  
 
} 

std::string int2string(int v){
	std::stringstream ss;
	std::string str;
	ss<<v;
	ss>>str;
	return str;
}

int string2int(string str){
	int n = atoi(str.c_str());

	
	int number; 
	std::stringstream ss;


	ss << str;//可以是其他数据类型
	ss >> number; //string -> int
	if (! ss.good()) 
	{ 
		//错误发生 
	}


	return n;
}

string GetString ( const int n ){
	std::stringstream newstr;
	newstr<<n;
	return newstr.str();

}

CString Int2CString(int v){
	
	CString ttt;
	ttt.Format(_T("%d"),v);
	return ttt;
}

int CString2Int(CString cstr){
	int tmp=atoi((char*)cstr.GetBuffer(cstr.GetLength()));
	cstr.ReleaseBuffer();
	return tmp;
}

void int2string(){



	string str;

	CString   cstr;  

	cstr.Format(_T("%s"), str.c_str());

	/*string 转 CString
	CString.format(”%s”, string.c_str());

	char 转 CString
	CString.format(”%s”, char*);

	char 转 string
	string s(char *);

	string 转 char *
	char *p = string.c_str();

	CString 转 string
	string s(CString.GetBuffer());*/

}

bool utf8CharToUcs2Char(const char* utf8Tok, wchar_t* ucs2Char, uint32_t* utf8TokLen)
{
	//We do math, that relies on unsigned data types
	const unsigned char* utf8TokUs = reinterpret_cast<const unsigned char*>(utf8Tok);

	//Initialize return values for 'return false' cases.
	*ucs2Char = L'?';
	*utf8TokLen = 1;

	//Decode
	if (0x80 > utf8TokUs[0])
	{
		//Tokensize: 1 byte
		*ucs2Char = static_cast<const wchar_t>(utf8TokUs[0]);
	}
	else if (0xC0 == (utf8TokUs[0] & 0xE0))
	{
		//Tokensize: 2 bytes
		if ( 0x80 != (utf8TokUs[1] & 0xC0) )
		{
			return false;
		}
		*utf8TokLen = 2;
		*ucs2Char = static_cast<const wchar_t>(
			(utf8TokUs[0] & 0x1F) << 6
			| (utf8TokUs[1] & 0x3F)
			);
	}
	else if (0xE0 == (utf8TokUs[0] & 0xF0))
	{
		//Tokensize: 3 bytes
		if (   ( 0x80 != (utf8TokUs[1] & 0xC0) )
			|| ( 0x80 != (utf8TokUs[2] & 0xC0) )
			)
		{
			return false;
		}
		*utf8TokLen = 3;
		*ucs2Char = static_cast<const wchar_t>(
			(utf8TokUs[0] & 0x0F) << 12
			| (utf8TokUs[1] & 0x3F) << 6
			| (utf8TokUs[2] & 0x3F)
			);
	}
	else if (0xF0 == (utf8TokUs[0] & 0xF8))
	{
		//Tokensize: 4 bytes
		*utf8TokLen = 4;
		return false;                        //Character exceeds the UCS-2 range (UCS-4 would be necessary)
	}
	else if ((0xF8 == utf8TokUs[0] & 0xFC))
	{
		//Tokensize: 5 bytes
		*utf8TokLen = 5;
		return false;                        //Character exceeds the UCS-2 range (UCS-4 would be necessary)
	}
	else if (0xFC == (utf8TokUs[0] & 0xFE))
	{
		//Tokensize: 6 bytes
		*utf8TokLen = 6;
		return false;                        //Character exceeds the UCS-2 range (UCS-4 would be necessary)
	}
	else
	{
		return false;
	}

	return true;
}

void ucs2CharToUtf8Char(const wchar_t ucs2Char, char* utf8Tok)
{
	//We do math, that relies on unsigned data types
	uint32_t ucs2CharValue = static_cast<uint32_t>(ucs2Char);   //The standard doesn't specify the signed/unsignedness of wchar_t
	unsigned char* utf8TokUs = reinterpret_cast<unsigned char*>(utf8Tok);

	//Decode
	if (0x80 > ucs2CharValue)
	{
		//Tokensize: 1 byte
		utf8TokUs[0] = static_cast<unsigned char>(ucs2CharValue);
		utf8TokUs[1] = '\0';
	}
	else if (0x800 > ucs2CharValue)
	{
		//Tokensize: 2 bytes
		utf8TokUs[2] = '\0';
		utf8TokUs[1] = static_cast<unsigned char>(0x80 | (ucs2CharValue & 0x3F));
		ucs2CharValue = (ucs2CharValue >> 6);
		utf8TokUs[0] = static_cast<unsigned char>(0xC0 | ucs2CharValue);
	}
	else
	{
		//Tokensize: 3 bytes
		utf8TokUs[3] = '\0';
		utf8TokUs[2] = static_cast<unsigned char>(0x80 | (ucs2CharValue & 0x3F));
		ucs2CharValue = (ucs2CharValue >> 6);
		utf8TokUs[1] = static_cast<unsigned char>(0x80 | (ucs2CharValue & 0x3F));
		ucs2CharValue = (ucs2CharValue >> 6);
		utf8TokUs[0] = static_cast<unsigned char>(0xE0 | ucs2CharValue);
	}
}

std::wstring utf8ToUcs2(const std::string& utf8Str)
{
	std::wstring ucs2Result;
	wchar_t ucs2CharToStrBuf[] = { 0, 0 };
	const char* cursor = utf8Str.c_str();
	const char* const end = utf8Str.c_str() + utf8Str.length();

	while (end > cursor)
	{
		uint32_t utf8TokLen = 1;
		utf8CharToUcs2Char(cursor, &ucs2CharToStrBuf[0], &utf8TokLen);
		ucs2Result.append(ucs2CharToStrBuf);
		cursor += utf8TokLen;
	}

	return ucs2Result;
}

std::string ucs2ToUtf8(const std::wstring& ucs2Str)
{
	std::string utf8Result;
	char utf8Sequence[] = { 0, 0, 0, 0, 0 };
	const wchar_t* cursor = ucs2Str.c_str();
	const wchar_t* const end = ucs2Str.c_str() + ucs2Str.length();

	while (end > cursor)
	{
		const wchar_t ucs2Char = *cursor;
		ucs2CharToUtf8Char(ucs2Char, utf8Sequence);
		utf8Result.append(utf8Sequence);
		cursor++;
	}

	return utf8Result;
}

std::string ws2s(const std::wstring& ws)
{
	std::string curLocale = setlocale(LC_ALL, NULL);        // curLocale = "C";
	setlocale(LC_ALL, "chs");
	const wchar_t* _Source = ws.c_str();
	size_t _Dsize = 2 * ws.size() + 1;
	char *_Dest = new char[_Dsize];
	memset(_Dest,0,_Dsize);
	wcstombs(_Dest,_Source,_Dsize);
	std::string result = _Dest;
	delete []_Dest;
	setlocale(LC_ALL, curLocale.c_str());
	return result;
}

std::wstring s2ws(const std::string& s)
{
	setlocale(LC_ALL, "chs"); 
	const char* _Source = s.c_str();
	size_t _Dsize = s.size() + 1;
	wchar_t *_Dest = new wchar_t[_Dsize];
	wmemset(_Dest, 0, _Dsize);
	mbstowcs(_Dest,_Source,_Dsize);
	std::wstring result = _Dest;
	delete []_Dest;
	setlocale(LC_ALL, "C");
	return result;
}
