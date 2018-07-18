
#include <stdio.h>
#include <WinSock2.h>
#include <string>
 
#pragma comment(lib, "WS2_32")
 
enum emTLVNodeType
{
	emTlvNNone = 0,
	emTlvNRoot,			//���ڵ�
	emTlvName,			//����
	emTlvAge,			//����
	emTlvColor			//��ɫ 1 ��ɫ 2 ��ɫ
};
 
 
typedef struct _CAT_INFO
{
	char szName[12];
	int	iAge;
	int iColor;
}CAT_INFO,*LPCAT_INFO;
 
class CTlvPacket
{
public:
	CTlvPacket(char *pBuf,unsigned int len):m_pData(pBuf),m_uiLength(len),m_pEndData(m_pData+len),m_pWritePtr(m_pData),m_pReadPtr(m_pData) { }
	~CTlvPacket() { }
 
	bool WriteInt(int data,bool bMovePtr = true)
	{
		int tmp = htonl(data);
		return Write(&tmp,sizeof(int));
	}
 
	bool Write(const void *pDst,unsigned int uiCount)
	{
		::memcpy(m_pWritePtr,pDst,uiCount);
		m_pWritePtr += uiCount;
		return m_pWritePtr < m_pEndData ? true : false;
	}
 
	bool ReadInt(int *data,bool bMovePtr = true)
	{
		Read(data,sizeof(int));
		*data = ntohl(*data);
		return true;
	}
 
	bool Read(void *pDst,unsigned int uiCount)
	{
		::memcpy(pDst,m_pReadPtr,uiCount);
		m_pReadPtr += uiCount;
		return m_pReadPtr < m_pEndData ? true : false;
	}
 
private:
	char *m_pData;
	unsigned int m_uiLength;
	char *m_pEndData;
	char *m_pWritePtr;
	char *m_pReadPtr;
};
 
/*
��ʽ��
	root L1 V
		T L V T L V T L V
	L1 �ĳ��ȼ�Ϊ��T L V T L V T L V���ĳ���
*/
 
int TLV_EncodeCat(LPCAT_INFO pCatInfo, char *pBuf, int &iLen)
{
	if (!pCatInfo || !pBuf)
	{
		return -1;
	}
 
	CTlvPacket enc(pBuf,iLen);
	enc.WriteInt(emTlvNRoot);
	enc.WriteInt(20+12+12); //���ڵ�emTlvNRoot�е�L��20=4+4+12��12=4+4+4��12=4+4+4
 
	enc.WriteInt(emTlvName);
	enc.WriteInt(12);
	enc.Write(pCatInfo->szName,12);
 
	enc.WriteInt(emTlvAge);
	enc.WriteInt(4);
	enc.WriteInt(pCatInfo->iAge);
 
	enc.WriteInt(emTlvColor);
	enc.WriteInt(4);
	enc.WriteInt(pCatInfo->iColor);
 
	iLen = 8+20+12+12; //�ܳ����ټ���emTLVNRoot��T��L��8=4+4
 
	return 0;
}
 
int TLV_DecodeCat(char *pBuf, int iLen, LPCAT_INFO pCatInfo)
{
	if (!pCatInfo || !pBuf)
	{
		return -1;
	}
 
	CTlvPacket encDec(pBuf,iLen);
	int iType;
	int iSum,iLength;
 
	encDec.ReadInt(&iType);
	if (emTlvNRoot != iType)
	{
		return -2;
	}
	encDec.ReadInt(&iSum);
 
	while (iSum > 0)
	{
		encDec.ReadInt(&iType);
		encDec.ReadInt(&iLength);
		switch(iType)
		{
		case emTlvName:
			encDec.Read(pCatInfo->szName,12);
			iSum -= 20;
			break;
		case emTlvAge:
			encDec.ReadInt(&pCatInfo->iAge);
			iSum -= 12;
		    break;
		case emTlvColor:
			encDec.ReadInt(&pCatInfo->iColor);
			iSum -= 12;
			break;
		default:
			printf("TLV_DecodeCat unkonwn error. \n");
		    break;
		}
	}
 
	return 0;
}
 
int main(int argc, char* argv[])
{
 
	int iRet, iLen;
	char buf[256] = {0};
 
	CAT_INFO cat;
	memset(&cat,0,sizeof(cat));
	strcpy(cat.szName,"Tom");
	cat.iAge = 5;
	cat.iColor = 2;
 
	iRet = TLV_EncodeCat(&cat,buf,iLen);
	if ( 0 == iRet )
	{
		printf("TLV_EncodeCat ok, iLen = %d. \n",iLen);
	}
	else
	{
		printf("TLV_EncodeCat error \n");
	}
 
	memset(&cat,0,sizeof(cat));
	iRet = TLV_DecodeCat(buf,iLen,&cat);
	if ( 0 == iRet )
	{
		printf("TLV_DecodeCat ok, cat name = %s, age = %d, color = %d. \n",cat.szName,cat.iAge,cat.iColor);
	}
	else
	{
		printf("TLV_DecodeCat error, code = %d. \n", iRet);
	}
 
	int iWait = getchar();
	return 0;
}