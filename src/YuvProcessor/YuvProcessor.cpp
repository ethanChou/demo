// YuvProcessor.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include <stdlib.h>
#include <sys/stat.h>
#include "drawshape.h"

int main(int argc,char * argv[])
{
	int size=720*576*3/2;
	int ret = 0;
	FILE *in_file,*out_file;

	struct _stat buf;
	int  iresult = _stat("720x576.yuv",&buf);
	int len= buf.st_size;;

	unsigned  char *frame_buffer = NULL;
	frame_buffer = (unsigned char*)malloc(size);

	
	//read frame file 读原来的一帧数据
	in_file = fopen("720x576.yuv","r");
	if (in_file == NULL)
	{
		printf("open in file error!\n");
	}

	ret = fread(frame_buffer,size,sizeof(unsigned char),in_file);
	if (ret != 1)
	{
		printf("ret = %d\n");
		printf("fread file error!\n");
	}
	fclose(in_file);
	nPoint s,e,s1,e1;
	s.x=50;
	s.y=50;

	e.x=350;
	e.y=400;


	s1.x=50;
	s1.y=50;

	e1.x=350;
	e1.y=0;

	nColor color;
	color.r=255;
	color.g=0;
	color.b=0;

	draw_line2(frame_buffer,720,576,s,e,color);

	draw_line2(frame_buffer,720,576,s1,e1,color);
	//draw_rect_yuv(frame_buffer,720,576,50,120,200,80);

	//数据转换
	//draw_Font_Func(frame_buffer,table,20,10,1);

	//write frame file 把数据写回
	out_file = fopen("720x576_2.yuv","w");
	if (out_file == NULL)
	{
		printf("open in file error!\n");
	}

	ret = fwrite(frame_buffer,size,sizeof(unsigned char),out_file);
	if (ret != 1)
	{
		printf("ret = %d\n");
		printf("fwrite file error!\n");
	}
	fclose(out_file);
	free(frame_buffer);

	printf("Done!\n");
	return 0;
}