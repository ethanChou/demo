#include "StdAfx.h"
#include <string.h>
#include <math.h>
#include "drawshape.h"
#define FRAME_WIDTH             (352)
#define FRAME_HEIGHT            (288)
#define FRAME_SIZE              (FRAME_WIDTH*FRAME_HEIGHT*3/2)

void swap(int &a, int &b)
{
	int iTemp;
	iTemp = a;
	a = b;
	b = iTemp;
}

void draw_rect_yuv(unsigned char *data, int imgw, int imgh, int x, int y, int w, int h){
	if (x<0 || y<0 || x+w>=imgw || y+h>=imgh || w<=4 || h<=4) {
		return ;  
	}
	int line_color[3] = {76, 84, 255};
	int line_width = 2;
	unsigned char *tlstart = data + y*imgw+x;

	//two horizontal lines, y channels
	memset(tlstart, line_color[0], w);
	memset(tlstart+imgw, line_color[0], w);
	memset(tlstart+(h-1)*imgw, line_color[0], w);
	memset(tlstart+h*imgw, line_color[0], w);

	//two horizontal lines, uv
	int halfw = imgw/2;
	int halfh = imgh/2;

	int halfrw = w/2;
	int halfrh = h/2;
	//for (int i=0; i<halfh; i++) {
	tlstart = data+imgw*imgh + y/2 * imgw+x;
	for (int j=0; j<w/2; j++) {
		tlstart[j*2] = line_color[1];
		tlstart[j*2+1] = line_color[2];
	}  

	memcpy(tlstart+imgw, tlstart, w);

	tlstart = data+imgw*imgh + (y+h)/2 * imgw+x;
	for (int j=0; j<w/2; j++) {
		tlstart[j*2] = line_color[1];
		tlstart[j*2+1] = line_color[2];
	}
	memcpy(tlstart-imgw, tlstart, w);

	unsigned char *tlstarty = data + y*imgw+x;
	unsigned char *tlstartuv = data+ imgw*imgh + y/2*imgw+x;

	//two vertical lines, y
	for (int i=0; i<halfrh; i++) {
		tlstart[x] = line_color[0];
		tlstart[x+1] = line_color[0];
		tlstart[x+w-1] = line_color[0];
		tlstart[x+w-2] = line_color[0];
	}

	//two vertical lines, uv
	for (int i=0; i<halfrh; i++) {
		tlstartuv[i*imgw] = line_color[1];
		tlstartuv[i*imgw+1] = line_color[2];
		tlstartuv[i*imgw+2] = line_color[1];
		tlstartuv[i*imgw+1+2] = line_color[2];
		tlstartuv[i*imgw+w] = line_color[1];
		tlstartuv[i*imgw+w+1] = line_color[2];
		tlstartuv[i*imgw+w-2] = line_color[1];
		tlstartuv[i*imgw+w+1-2] = line_color[2];

	}
return ;  
}

void draw_rect2_yuv(unsigned char *dst_buf, int w, int h, int lpitch, int x0, int y0, int x1, int y1, unsigned long rgba){

	unsigned char r, g, b, y, cb, cr;
	b = (rgba >> 8) & 0xFF;
	g = (rgba >> 16) & 0xFF;
	r = (rgba >> 24) & 0xFF;
	y = 16 + 0.257*r + 0.504*g + 0.098*b;
	cb = 128 - 0.148*r - 0.291*g + 0.439*b;
	cr = 128 + 0.439*r - 0.368*g - 0.071*b;

	int rect_width, rect_height;
	x0 = x0 & 0xFFFE;
	y0 = y0 & 0xFFFE;
	x1 = x1 & 0xFFFE;
	y1 = y1 & 0xFFFE;
	rect_width = x1 - x0;
	rect_height = y1 - y0;

	unsigned char *xoff;
	unsigned char *yoff;
	xoff = dst_buf + y0*lpitch + x0;
	memset(xoff, y, rect_width);//rowline
	xoff = dst_buf + (y0 + 1)*lpitch + x0;
	memset(xoff, y, rect_width);

	xoff = dst_buf + (y1 - 1)*lpitch + x0;
	memset(xoff, y, rect_width);//colline
	xoff = dst_buf + y1*lpitch + x0;
	memset(xoff, y, rect_width);

	//y first
	for (int i = 0; i < rect_height; i++)
	{
		yoff = dst_buf + (i + y0)*lpitch + x0;
		*yoff = y;
		yoff = dst_buf + (i + y0)*lpitch + x0 + 1;
		*yoff = y;

		yoff = dst_buf + (i + y0)*lpitch + x1 - 1;
		*yoff = y;
		yoff = dst_buf + (i + y0)*lpitch + x1;
		*yoff = y;
	}

	//cb next
	xoff = dst_buf + lpitch * h + y0*lpitch / 4 + x0 / 2;
	memset(xoff, cb, rect_width / 2);

	xoff = dst_buf + lpitch * h + y1*lpitch / 4 + x0 / 2;
	memset(xoff, cb, rect_width / 2);

	for (int i = 0; i < rect_height; i += 2)
	{
		yoff = dst_buf + lpitch * h + (i + y0)*lpitch / 4 + x0 / 2;
		*yoff = cb;

		yoff = dst_buf + lpitch * h + (i + y0)*lpitch / 4 + x1 / 2;
		*yoff = cb;
	}

	//cr last
	xoff = dst_buf + lpitch * h + lpitch * h / 4 + y0*lpitch / 4 + x0 / 2;
	memset(xoff, cr, rect_width / 2);

	xoff = dst_buf + lpitch * h + lpitch * h / 4 + y1*lpitch / 4 + x0 / 2;
	memset(xoff, cr, rect_width / 2);

	for (int i = 0; i < rect_height; i += 2)
	{
		yoff = dst_buf + lpitch * h + lpitch * h / 4 + (i + y0)*lpitch / 4 + x0 / 2;
		*yoff = cr;

		yoff = dst_buf + lpitch * h + lpitch * h / 4 + (i + y0)*lpitch / 4 + x1 / 2;
		*yoff = cr;
	}
	
}

/*
  *http://www.cnblogs.com/eustoma/p/6661839.html
  * Function:     draw_Font_Func
  * Description:  实现在yuv420图片上面画字 
  * Input:        char *ptr_frame             一帧视频的首地址
  *               const unsigned char font[]  画的字模
  *               int startx                  写字的起点坐标x
  *               int starty                  写字的起点坐标y
  *               int color                   字颜色的选择，具体颜色在程序代码
  * Return:       这里会把传进来的一帧视频的地址返回，可以不调用  
  */
 void draw_font_yuv(char *ptr_frame,const unsigned char font[],int startx,int starty,int color)
  {
      int tagY=0,tagU=0,tagV=0;
      char *offsetY=NULL,*offsetU=NULL,*offsetV=NULL;
      unsigned short p16, mask16; // for reading hzk16 dots
        
      /*yuv 地址的设置 */
      offsetY = ptr_frame;
      offsetU = offsetY + FRAME_WIDTH * FRAME_HEIGHT;
      offsetV = offsetU + FRAME_WIDTH * FRAME_HEIGHT/4;
        
      switch (color)
      {
          case 0:         // Yellow
              tagY = 226;tagU = 0;tagV = 149;
              break;
          case 1:         // Red
              tagY = 76;tagU = 85;tagV = 255;
              break;
          case 2:         // Green
              tagY = 150;tagU = 44;tagV = 21;
              break;
          case 3:         // Blue
              tagY = 29;tagU = 255;tagV = 107;
              break;
          default:        // White
              tagY = 128;tagU = 128;tagV = 128;
      }  
        
      int x=0,y=0,i=1,j=0,k=0;
      for(i = 0; i < 3; i++)
      {
      #if 0
          for (j = 0, y = starty; j < 16 && y < FRAME_HEIGHT - 1; j++, y+=2)    // line dots per char
          {
              p16 = *(unsigned short *)(font + j*2 + i*32);/*取字模数据*/
              mask16 = 0x0080;  /* 二进制 1000 0000 */
              for (k = 0, x = startx +i*36; k < 16 && x < FRAME_WIDTH - 1; k++, x+=2)   // dots in a line
              {
                  if (p16 & mask16)
                  {
                      *(offsetY + y*FRAME_WIDTH + x) = *(offsetY + y*FRAME_WIDTH + x+1) = tagY;
                      *(offsetY + (y+1)*FRAME_WIDTH + x) = *(offsetY + (y+1)*FRAME_WIDTH + x+1) = tagY;   
                     *(offsetU + y * FRAME_WIDTH/4 + x/2) =tagU;
                     *(offsetV + y * FRAME_WIDTH/4 + x/2) = tagV;
                 }
                 mask16 = mask16 >> 1;  /* 循环移位取数据 */
                 if (mask16 == 0)
                     mask16 = 0x8000;
             }
         }
     #else
         for (j = 0, y = starty; j < 16 && y < FRAME_HEIGHT - 1; j++, y++) // line dots per char
         {
             p16 = *(unsigned short *)(font + j*2 + i*32);/*取字模数据*/
             mask16 = 0x0080;  /* 二进制 1000 0000 */
             for (k = 0, x = startx +i*18; k < 16 && x < FRAME_WIDTH - 1; k++, x++) // dots in a line
             {
                 if (p16 & mask16)
                 {
                     *(offsetY + y*FRAME_WIDTH + x) = 255;
                 //  *(offsetU + y * FRAME_WIDTH/4 + x/2) = 85;
                 //  *(offsetV + y * FRAME_WIDTH/4 + x/2) = 255;
                 }
                 mask16 = mask16 >> 1;  /* 循环移位取数据 */
                 if (mask16 == 0)
                     mask16 = 0x8000;
             }
         }
     #endif
     }
 }
   
void draw_line(unsigned char* imgdata, int width, int height, nPoint startPoint, nPoint endPoint, nColor color)  
{  
	if (!imgdata)  
	{  
		return ;  
	}  
	if (width < 0 || height < 0 )  
	{  
		return ;  
	}  
	if (startPoint.x<0 || startPoint.x > width || startPoint.y < 0 || startPoint.y > height || endPoint.x < 0 || endPoint.x > width || endPoint.y < 0 || endPoint.y > height)  
	{  
		return ;  
	}  

	int imgSize = width*height;  

	int x0 = startPoint.x, x1 = endPoint.x;  
	int y0 = startPoint.y, y1 = endPoint.y;  
	int dy = abs(y1 - y0);  
	int dx = abs(x1 - x0);  
	bool steep = dy>dx ?true:false;  

	if (steep)  
	{  
		swap(x0, y0);  
		swap(x1, y1);  
	}  
	if (x0 > x1)  
	{  
		swap(x0, x1);  
		swap(y0, y1);  
	}  

	int deltax = x1 - x0;  
	int deltay = abs(y1 - y0);  

	int error = deltax/2;  
	int ystep;  
	int y = y0;  

	if (y0 < y1)  
		ystep = 1;  
	else  
		ystep = -1;  

	for (int x = x0; x < x1; x++)  
	{  
		if (steep)  
		{  
			imgdata[x*width + y] = color.r;  
			imgdata[imgSize + x/2*width/2 + y/2] = color.g;  
			imgdata[imgSize + imgSize/4 + x/2*width/2 + y/2] = color.b;  
		}  
		else  
		{  
			imgdata[y*width + x] = color.r;  
			imgdata[imgSize + y/2*width/2 + x/2] = color.g;  
			imgdata[imgSize + imgSize/4 + y/2*width/2 + x/2] = color.b;  
		}  

		error -= deltay;  
		if (error < 0)   
		{  
			y += ystep;  
			error += deltax;  
		}  
	}  
	return ;
}  

void draw_line2(unsigned char* imgData, int width, int height, nPoint startPoint, nPoint endPoint, nColor color)  
{  
	if (!imgData)  
	{  
		return ;  
	}  
	if (width<0 || height<0)  
	{  
		return;  
	}  
	if (startPoint.x < 0 || startPoint.x > width || startPoint.y < 0 || startPoint.y > height ||  
		endPoint.x < 0 || endPoint.x >width || endPoint.y < 0 || endPoint.y > height)  
	{  
		return;  
	}  

	int imgSize = width*height;  

	int yStep, xStep;  
	int error, errorPrev;  
	int y = startPoint.y, x = startPoint.x;  
	int ddy, ddx;  
	int dx = endPoint.x - startPoint.x;  
	int dy = endPoint.y - startPoint.y;  

	if (dy < 0)  
	{  
		yStep = -1;  
		dy = -dy;  
	}  
	else  
		yStep = 1;  

	if (dx < 0)  
	{  
		xStep = -1;  
		dx = -dx;  
	}  
	else  
		xStep = 1;  

	ddx = 2*dx;  
	ddy = 2*dy;  

	if (ddx >= ddy)  
	{  
		errorPrev = error = dx;  
		for (int i=0; i<dx; i++)  
		{  
			x += xStep;  
			error += ddy;  
			if (error > ddx)  
			{  
				y += yStep;  
				error -= ddx;  
				if (error + errorPrev < ddx)  
				{//POINT(y-ystep, x)  
					int tmpY = y - yStep;  
					imgData[tmpY*width + x] = color.r;  
					imgData[imgSize + tmpY/2*width/2 + x/2] = color.g;  
					imgData[imgSize + imgSize/4 + tmpY/2*width/2 + x/2] = color.b;  
				}  
				else if (error + errorPrev > ddx)  
				{  
					//POINT(y, x-xstep)  
					int tmpX = x - xStep;  
					imgData[y*width + tmpX] = color.r;
						imgData[imgSize + y/2*width/2 + tmpX/2] = color.g;  
					imgData[imgSize + imgSize/4 + y/2*width/2 + tmpX/2] = color.b;  
				}  
				else  
				{  
					//POINT(y-ystep, x)  
					int tmpY = y - yStep;  
					imgData[tmpY*width + x] = color.r;  
					imgData[imgSize + tmpY/2*width/2 + x/2] = color.g;  
					imgData[imgSize + imgSize/4 + tmpY/2*width/2 + x/2] = color.b;  

					//POINT(y, x-xstep)  
					int tmpX = x - xStep;  
					imgData[y*width + tmpX] = color.r;  
					imgData[imgSize + y/2*width/2 + tmpX/2] = color.g;  
					imgData[imgSize + imgSize/4 + y/2*width/2 + tmpX/2] = color.b;  
				}  
			}  

			//POINT(y,x)  
			imgData[y*width + x] = color.r;  
			imgData[imgSize + y/2*width/2 + x/2] = color.g;  
			imgData[imgSize + imgSize/4 + y/2*width/2 + x/2] = color.b;  
			errorPrev = error;  
		}  
	}  

	else  
	{  
		errorPrev = error = dy;  

		for (int i=0; i<dy; i++)  
		{  
			y += yStep;  
			error += ddx;  
			if (error > ddy)  
			{  
				x += xStep;  
				error -= ddy;  
				if (error + errorPrev < ddy)  
				{  
					//POINT(y, x-xstep)  
					int tmpX = x - xStep;  
					imgData[y*width + tmpX] = color.r;  
					imgData[imgSize + y/2*width/2 + tmpX/2] = color.g;  
					imgData[imgSize + imgSize/4 + y/2*width/2 + tmpX/2] = color.b;  
				}  
				else if (error + errorPrev > ddy)  
				{  
					//POINT(y-ystep, x)  
					int tmpY = y - yStep;  
					imgData[tmpY*width + x] = color.r;  
					imgData[imgSize + tmpY/2*width/2 + x/2] = color.g;  
					imgData[imgSize + imgSize/4 + tmpY/2*width/2 + x/2] = color.b;  
				}  
				else  
				{  
					//POINT(y, x-xstep)  
					int tmpX = x - xStep;  
					imgData[y*width + tmpX] = color.r;  
					imgData[imgSize + y/2*width/2 + tmpX/2] = color.g;  
					imgData[imgSize + imgSize/4 + y/2*width/2 + tmpX/2] = color.b;  

					//POINT(y-ystep, x)  
					int tmpY = y - yStep;  
					imgData[tmpY*width + x] = color.r;  
					imgData[imgSize + tmpY/2*width/2 + x/2] = color.g;  
					imgData[imgSize + imgSize/4 + tmpY/2*width/2 + x/2] = color.b;  
				}  
			}  

			//POINT(y, x)  
			imgData[y*width + x] = color.r;  
			imgData[imgSize + y/2*width/2 + x/2] = color.g;  
			imgData[imgSize + imgSize/4 + y/2*width/2 + x/2] = color.b;  
			errorPrev = error;  
		}  
	}  
}  