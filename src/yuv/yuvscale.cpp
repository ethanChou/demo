
#include "stdafx.h"
#include "yuvscale.h"

#define __STDC_CONSTANT_MACROS

const int MAX_PLAYER_N = 64;

IHuman*	g_encoders[MAX_PLAYER_N] = {NULL};

CRITICAL_SECTION* g_GetFreePortMutex = NULL;
CRITICAL_SECTION* g_Mutex = NULL;

BOOL APIENTRY DllMain( HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
	)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		//InitializeCriticalSection(&g_GetFreePortMutex);
		g_GetFreePortMutex = new CRITICAL_SECTION();
		InitializeCriticalSection(g_GetFreePortMutex);		
		g_Mutex = new CRITICAL_SECTION();
		InitializeCriticalSection(g_Mutex);
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	case DLL_PROCESS_DETACH:
		delete g_GetFreePortMutex;
		g_GetFreePortMutex = NULL;
		delete g_Mutex;
		g_Mutex = NULL;
		break;
	}
	return TRUE;
}

/******************************************************** 
*  @function :  
*  @brief    :  test 
*  @input    : 
*  @output   : 
*  @return   : 
*  @author   :  Name  2018/07/12 12:35
*  @History  :
*********************************************************/
JNAAPI int _stdcall yuv_sacle(uint8_t* src_buffer,int src_w,int src_h,AVPixelFormat src_pixfmt,uint8_t*  dst,int dst_w,int dst_h,AVPixelFormat dst_pixfmt)
{
	//Parameters	
	int src_bpp=av_get_bits_per_pixel(av_pix_fmt_desc_get(src_pixfmt));
	int dst_bpp=av_get_bits_per_pixel(av_pix_fmt_desc_get(dst_pixfmt));

	//Structures
	uint8_t *src_data[4];
	int src_linesize[4];

	uint8_t *dst_data[4];
	int dst_linesize[4];

	int rescale_method=SWS_BICUBIC;
	struct SwsContext *img_convert_ctx;

	int ret=0;
	ret= av_image_alloc(src_data, src_linesize,src_w, src_h, src_pixfmt, 1);
	if (ret< 0) {
		printf( "Could not allocate source image\n");
		return -1;
	}
	ret = av_image_alloc(dst_data, dst_linesize,dst_w, dst_h, dst_pixfmt, 1);
	if (ret< 0) {
		printf( "Could not allocate destination image\n");
		return -1;
	}
	///-----------------------------	
	//Init Method 1
	img_convert_ctx =sws_alloc_context();

	//Show AVOption
	//av_opt_show2(img_convert_ctx,stdout,AV_OPT_FLAG_VIDEO_PARAM,0);

	//Set Value
	av_opt_set_int(img_convert_ctx,"sws_flags",SWS_BICUBIC|SWS_PRINT_INFO,0);
	av_opt_set_int(img_convert_ctx,"srcw",src_w,0);
	av_opt_set_int(img_convert_ctx,"srch",src_h,0);
	av_opt_set_int(img_convert_ctx,"src_format",src_pixfmt,0);
	//'0' for MPEG (Y:0-235);'1' for JPEG (Y:0-255)
	av_opt_set_int(img_convert_ctx,"src_range",1,0);
	av_opt_set_int(img_convert_ctx,"dstw",dst_w,0);
	av_opt_set_int(img_convert_ctx,"dsth",dst_h,0);
	av_opt_set_int(img_convert_ctx,"dst_format",dst_pixfmt,0);
	av_opt_set_int(img_convert_ctx,"dst_range",1,0);
	sws_init_context(img_convert_ctx,NULL,NULL);

	switch(src_pixfmt){
	case AV_PIX_FMT_GRAY8:{
		memcpy(src_data[0],src_buffer,src_w*src_h);
		break;
						  }
	case AV_PIX_FMT_YUV420P:{
		memcpy(src_data[0],src_buffer,src_w*src_h);                    //Y
		memcpy(src_data[1],src_buffer+src_w*src_h,src_w*src_h/4);      //U
		memcpy(src_data[2],src_buffer+src_w*src_h*5/4,src_w*src_h/4);  //V
		break;
							}
	case AV_PIX_FMT_YUV422P:{
		memcpy(src_data[0],src_buffer,src_w*src_h);                    //Y
		memcpy(src_data[1],src_buffer+src_w*src_h,src_w*src_h/2);      //U
		memcpy(src_data[2],src_buffer+src_w*src_h*3/2,src_w*src_h/2);  //V
		break;
							}
	case AV_PIX_FMT_YUV444P:{
		memcpy(src_data[0],src_buffer,src_w*src_h);                    //Y
		memcpy(src_data[1],src_buffer+src_w*src_h,src_w*src_h);        //U
		memcpy(src_data[2],src_buffer+src_w*src_h*2,src_w*src_h);      //V
		break;
							}
	case AV_PIX_FMT_YUYV422:{
		memcpy(src_data[0],src_buffer,src_w*src_h*2);                  //Packed
		break;
							}
	case AV_PIX_FMT_RGB24:{
		memcpy(src_data[0],src_buffer,src_w*src_h*3);                  //Packed
		break;
						  }
	default:{
		printf("Not Support Input Pixel Format.\n");
		break;
			}
	}

	sws_scale(img_convert_ctx, src_data, src_linesize, 0, src_h, dst_data, dst_linesize);

	printf("Finish process frame ");

	switch(dst_pixfmt){
	case AV_PIX_FMT_GRAY8:{
		memcpy(dst,dst_data[0],dst_w*dst_h);
		break;
						  }
	case AV_PIX_FMT_YUV420P:{
		memcpy(dst,dst_data[0],dst_w*dst_h);//Y
		memcpy(dst+dst_w*dst_h,dst_data[1],dst_w*dst_h/4);//U
		memcpy(dst+dst_w*dst_h*5/4,dst_data[2],dst_w*dst_h/4); //V
		break;
							}
	case AV_PIX_FMT_YUV422P:{
		memcpy(dst,dst_data[0],dst_w*dst_h);//Y
		memcpy(dst+dst_w*dst_h,dst_data[1],dst_w*dst_h/2);//U
		memcpy(dst+dst_w*dst_h*3/2,dst_data[2],dst_w*dst_h/2); //V
		break;
							}
	case AV_PIX_FMT_YUV444P:{
		memcpy(dst,dst_data[0],dst_w*dst_h);//Y
		memcpy(dst+dst_w*dst_h,dst_data[1],dst_w*dst_h);//U
		memcpy(dst+dst_w*dst_h*2,dst_data[2],dst_w*dst_h); //V
		break;
							}
	case AV_PIX_FMT_YUYV422:{
		memcpy(dst,dst_data[0],dst_w*dst_h*2);//Packed
		break;
							}
	case AV_PIX_FMT_RGB24:{
		memcpy(dst,dst_data[0],dst_w*dst_h*3);//Packed
		break;
						  }
	default:{
		printf("Not Support Output Pixel Format.\n");
		break;
			}
	}

	sws_freeContext(img_convert_ctx);

	av_freep(&src_data[0]);
	av_freep(&dst_data[0]);

	return 0;
}


/******************************************************** 
*  @function :  
*  @brief    :  yuv合并 
*  @input    : 
*  @output   : 
*  @return   : 
*  @author   :  Name  2018/07/12 12:28
*  @History  :
*********************************************************/
JNAAPI int _stdcall yuv_merge(uint8_t *src, int src_w,int src_h,uint8_t *dst, int dst_w,int dst_h,uint32_t dx, uint32_t dy)
{
	if (NULL == src || NULL == dst) return -1;

	if (src_w > dst_w || src_h> dst_h) return -1;

	uint8_t *src_y,*src_u,*src_v;
	src_y=src;
	src_u=src+src_w*src_h;
	src_v=src+src_w*src_h*5/4;

	uint8_t *dst_y,*dst_u,*dst_v;
	dst_y=dst;
	dst_u=dst+dst_w*dst_h;
	dst_v=dst+dst_w*dst_h*5/4;

	uint32_t t_offset = 0;
	for (int i = 0; i < src_h; i++)
	{
		t_offset = dst_w*(dy + i) + dx;
		//copy Y
		memcpy(dst_y + t_offset, src_y + src_w*i, src_w);
	}

	int half_dx = dx / 2, half_dy = dy / 2;

	int half_src_w =src_w / 2, half_src_h = src_h / 2;

	int half_dst_w = dst_w / 2, half_dst_h = dst_h / 2;

	for (int j = 0; j < half_src_h; j++)
	{
		t_offset = half_dst_w*(half_dy + j) + half_dx;
		//copy u
		memcpy(dst_u + t_offset,src_u + half_src_w*j, half_src_w);
		//copy v
		memcpy(dst_v + t_offset, src_v + half_src_w*j, half_src_w);
	}

	return 0;
}


/******************************************************** 
*  @function :  
*  @brief    :  yv12 to bgr24 
*  @input    : 
*  @output   : 
*  @return   : 
*  @author   :  Name  2018/07/12 14:05
*  @History  :
*********************************************************/
JNAAPI int _stdcall yuv_yv12tobgr24(uint8_t* src,uint8_t* dst,int width,int height){
	if (width < 1 || height < 1 || src == NULL || dst == NULL)
		return -1;
	const long len = width * height;
	unsigned char* yData = src;
	unsigned char* vData = &yData[len];
	unsigned char* uData = &vData[len >> 2];

	int bgr[3];
	int yIdx,uIdx,vIdx,idx;
	for (int i = 0;i < height;i++){
		for (int j = 0;j < width;j++){
			yIdx = i * width + j;
			vIdx = (i/2) * (width/2) + (j/2);
			uIdx = vIdx;

			bgr[0] = (int)(yData[yIdx] + 1.732446 * (uData[vIdx] - 128));                                    // b分量
			bgr[1] = (int)(yData[yIdx] - 0.698001 * (uData[uIdx] - 128) - 0.703125 * (vData[vIdx] - 128));    // g分量
			bgr[2] = (int)(yData[yIdx] + 1.370705 * (vData[uIdx] - 128));                                    // r分量

			for (int k = 0;k < 3;k++){
				idx = (i * width + j) * 3 + k;
				if(bgr[k] >= 0 && bgr[k] <= 255)
					dst[idx] = bgr[k];
				else
					dst[idx] = (bgr[k] < 0)?0:255;
			}
		}
	}
	return 0;
}

/******************************************************** 
*  @function :  
*  @brief    :  yv12 to bgr24 基于查表法的实现 
*  @input    : 
*  @output   : 
*  @return   : 
*  @author   :  Name  2018/07/12 14:05
*  @History  :
*********************************************************/
JNAAPI int _stdcall yuv_yv12tobgr24_fast(uint8_t* src,uint8_t* dst,int width,int height){
	if (width < 1 || height < 1 || src == NULL || dst == NULL)
		return -1;
	const long len = width * height;
	unsigned char* yData = src;
	unsigned char* vData = &yData[len];
	unsigned char* uData = &vData[len >> 2];

	int bgr[3];
	int yIdx,uIdx,vIdx,idx;
	int rdif,invgdif,bdif;
	for (int i = 0;i < height;i++){
		for (int j = 0;j < width;j++){
			yIdx = i * width + j;
			vIdx = (i/2) * (width/2) + (j/2);
			uIdx = vIdx;

			rdif = Table_fv1[vData[vIdx]];
			invgdif = Table_fu1[uData[uIdx]] + Table_fv2[vData[vIdx]];
			bdif = Table_fu2[uData[uIdx]];

			bgr[0] = yData[yIdx] + bdif;    
			bgr[1] = yData[yIdx] - invgdif;
			bgr[2] = yData[yIdx] + rdif;

			for (int k = 0;k < 3;k++){
				idx = (i * width + j) * 3 + k;
				if(bgr[k] >= 0 && bgr[k] <= 255)
					dst[idx] = bgr[k];
				else
					dst[idx] = (bgr[k] < 0)?0:255;
			}
		}
	}
	return 0;
}

JNAAPI int _stdcall  yuv_nv12torgb24(unsigned char *rgbout, unsigned char *pdata,int DataWidth,int DataHeight)
{
	unsigned long  idx=0;
	unsigned char *ybase,*ubase;
	unsigned char y,u,v;
	ybase = pdata; //获取Y平面地址
	ubase = pdata+DataWidth * DataHeight; //获取U平面地址，由于NV12中U、V是交错存储在一个平民的，v是u+1
	for(int j=0;j<DataHeight;j++)
	{
		idx=(DataHeight-j-1)*DataWidth*3;//该值保证所生成的rgb数据逆序存放在rgbbuf中,位图是底朝上的
		for(int i=0;i<DataWidth;i++)
		{
			unsigned char r,g,b;
			y=ybase[i + j  * DataWidth];//一个像素对应一个y
			u=ubase[j/2 * DataWidth+(i/2)*2];// 每四个y对应一个uv
			v=ubase[j/2 * DataWidth+(i/2)*2+1];  //一定要注意是u+1

			b=(unsigned char)(y+1.779*(u- 128));
			g=(unsigned char)(y-0.7169*(v - 128)-0.3455*(u - 128));
			r=(unsigned char)(y+ 1.4075*(v - 128));

			rgbout[idx++]=b;
			rgbout[idx++]=g;
			rgbout[idx++]=r;
		}
	}
	return 0;
}

JNAAPI unsigned char* _stdcall yuv_nv12toi420(unsigned char *data , int dataWidth, int dataHeight){
	unsigned char *ybase,*ubase;
	ybase = data;
	ubase = data + dataWidth*dataHeight;
	unsigned char* tmpData = (unsigned char*)malloc(dataWidth*dataHeight * 1.5);
	int offsetOfU = dataWidth*dataHeight;
	int offsetOfV = dataWidth*dataHeight* 5/4;
	memcpy(tmpData, ybase, dataWidth*dataHeight);
	for (int i = 0; i < dataWidth*dataHeight/2; i++) {
		if (i % 2 == 0) {
			tmpData[offsetOfU] = ubase[i];
			offsetOfU++;
		}else{
			tmpData[offsetOfV] = ubase[i];
			offsetOfV++;
		}
	}
	free(data);
	return tmpData;
}

JNAAPI void _stdcall yuv_rotate90nv12(unsigned char *dst, const unsigned char *src, int srcWidth, int srcHeight)
{
	int wh = srcWidth * srcHeight;
	int uvHeight = srcHeight / 2;
	int uvWidth = srcWidth / 2;
	//旋转Y
	int i = 0, j = 0;
	int srcPos = 0, nPos = 0;
	for(i = 0; i < srcHeight; i++) {
		nPos = srcHeight - 1 - i;
		for(j = 0; j < srcWidth; j++) {
			dst[j * srcHeight + nPos] = src[srcPos++];
		}
	}

	srcPos = wh;
	for(i = 0; i < uvHeight; i++) {
		nPos = (uvHeight - 1 - i) * 2;
		for(j = 0; j < uvWidth; j++) {
			dst[wh + j * srcHeight + nPos] = src[srcPos++];
			dst[wh + j * srcHeight + nPos + 1] = src[srcPos++];
		}
	}
}

JNAAPI void _stdcall yuv_rotate270yuv420sp(unsigned char *dst, const unsigned char *src, int srcWidth, int srcHeight)
{
	int nWidth = 0, nHeight = 0;
	int wh = 0;
	int uvHeight = 0;
	if(srcWidth != nWidth || srcHeight != nHeight)
	{
		nWidth = srcWidth;
		nHeight = srcHeight;
		wh = srcWidth * srcHeight;
		uvHeight = srcHeight >> 1;//uvHeight = height / 2
	}

	//旋转Y
	int k = 0;
	for(int i = 0; i < srcWidth; i++){
		int nPos = srcWidth - 1;
		for(int j = 0; j < srcHeight; j++)
		{
			dst[k] = src[nPos - i];
			k++;
			nPos += srcWidth;
		}
	}

	for(int i = 0; i < srcWidth; i+=2){
		int nPos = wh + srcWidth - 1;
		for(int j = 0; j < uvHeight; j++) {
			dst[k] = src[nPos - i - 1];
			dst[k + 1] = src[nPos - i];
			k += 2;
			nPos += srcWidth;
		}
	}
}

JNAAPI BOOL _stdcall yuv_open(LONG* nPort){
	BOOL flag = FALSE;
	EnterCriticalSection(g_GetFreePortMutex); // lock	
	for (int i = 0; i < MAX_PLAYER_N; i++)
	{
		if (g_encoders[i] == NULL)
		{
			g_encoders[i] = new IHuman();
			*nPort = i;
			flag = TRUE;
			break;
		}
	}
	LeaveCriticalSection(g_GetFreePortMutex); // unlock
	return flag;
}

JNAAPI BOOL __stdcall yuv_close(LONG nPort)
{
	delete g_encoders[nPort];
	g_encoders[nPort] = NULL;
	return TRUE;
}

JNAAPI BOOL __stdcall yuv_invoke(LONG nPort,char* args)
{
	EnterCriticalSection(g_Mutex);
	g_encoders[nPort]->invoke(args);
	LeaveCriticalSection(g_Mutex);
	return TRUE;
}



//bool YV12ToBGR24_FFmpeg(unsigned char* pYUV,unsigned char* pBGR24,int width,int height)
//{
//	if (width < 1 || height < 1 || pYUV == NULL || pBGR24 == NULL)
//		return false;
//	//int srcNumBytes,dstNumBytes;
//	//uint8_t *pSrc,*pDst;
//	AVPicture pFrameYUV,pFrameBGR;
//
//	//pFrameYUV = avpicture_alloc();
//	//srcNumBytes = avpicture_get_size(PIX_FMT_YUV420P,width,height);
//	//pSrc = (uint8_t *)malloc(sizeof(uint8_t) * srcNumBytes);
//	avpicture_fill(&pFrameYUV,pYUV,PIX_FMT_YUV420P,width,height);
//
//	//U,V互换
//	uint8_t * ptmp=pFrameYUV.data[1];
//	pFrameYUV.data[1]=pFrameYUV.data[2];
//	pFrameYUV.data [2]=ptmp;
//
//	//pFrameBGR = avcodec_alloc_frame();
//	//dstNumBytes = avpicture_get_size(PIX_FMT_BGR24,width,height);
//	//pDst = (uint8_t *)malloc(sizeof(uint8_t) * dstNumBytes);
//	avpicture_fill(&pFrameBGR,pBGR24,PIX_FMT_BGR24,width,height);
//
//	struct SwsContext* imgCtx = NULL;
//	imgCtx = sws_getContext(width,height,PIX_FMT_YUV420P,width,height,PIX_FMT_BGR24,SWS_BILINEAR,0,0,0);
//
//	if (imgCtx != NULL){
//		sws_scale(imgCtx,pFrameYUV.data,pFrameYUV.linesize,0,height,pFrameBGR.data,pFrameBGR.linesize);
//		if(imgCtx){
//			sws_freeContext(imgCtx);
//			imgCtx = NULL;
//		}
//		return true;
//	}
//	else{
//		sws_freeContext(imgCtx);
//		imgCtx = NULL;
//		return false;
//	}
//}
