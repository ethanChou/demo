
struct nPoint{
	int x;
	int y;
};

struct nColor{
	int r;
	int g;
	int b;
};

void draw_rect_yuv(unsigned char *data, int imgw, int imgh, int x, int y, int w, int h);

void draw_rect2_yuv(unsigned char *dst_buf, int w, int h, int lpitch, int x0, int y0, int x1, int y1, unsigned long rgba);

void draw_line(unsigned char* imgdata, int width, int height, nPoint startPoint, nPoint endPoint, nColor color)  ;

void draw_line2(unsigned char* imgdata, int width, int height, nPoint startPoint, nPoint endPoint, nColor color)  ;