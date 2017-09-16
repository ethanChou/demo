using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace VisitorManager
{
    /// <summary>
    /// Utils
    /// </summary>
    class _PicUtil
    {
        ///<summary>
        /// 会产生graphics异常的PixelFormat
        /// </summary>
        private static PixelFormat[] indexedPixelFormats = { PixelFormat.Undefined, PixelFormat.DontCare,
PixelFormat.Format16bppArgb1555, PixelFormat.Format1bppIndexed, PixelFormat.Format4bppIndexed,
PixelFormat.Format8bppIndexed
    };

        /// <summary>
        /// 读指定图片文件成图片，非独占方式读取，不占用原图片文件
        /// </summary>
        /// <param name="fileName">图片文件路径</param>
        /// <returns>成功返回图片，失败返回空</returns>
        public static Image GetImage(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }
            //win7系统上新建一个空的位图，长度为0，什么也没有，处理时会报错
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            if (fs.Length == 0)
            {
                fs.Close();
                fs.Dispose();
                return null;
            }
            BinaryReader br = new BinaryReader(fs);
            byte[] bytes = br.ReadBytes((int)fs.Length);
            MemoryStream ms = new MemoryStream(bytes);
            Image im = null;
            try
            {
                im = Image.FromStream(ms);//生成图片
            }
            catch
            {
                im = null;
            }
            finally
            {
                br.Close();
                br.Dispose();
                //ms不能在这里销毁
                fs.Close();
                fs.Dispose();
            }

            return im;
        }

        /// <summary>
        /// 判断是否为图片文件的方法
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsPicture(string filePath)
        {
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(fs);
                string fileClass;
                byte buffer;
                byte[] b = new byte[2];
                buffer = reader.ReadByte();
                b[0] = buffer;
                fileClass = buffer.ToString();
                buffer = reader.ReadByte();
                b[1] = buffer;
                fileClass += buffer.ToString();

                reader.Close();
                fs.Close();
                //255216是jpg;7173是gif;6677是BMP,13780是PNG;7790是exe,8297是rar 
                if (fileClass == "255216" || fileClass == "7173"
                    || fileClass == "6677" || fileClass == "13780")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 生成缩略图,原图直接缩略
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        public static Image MakeThumbnail(string originalImagePath, int width, int height)
        {
            if (!File.Exists(originalImagePath))
            {
                return null;
            }
            Image originalImage = GetImage(originalImagePath);
            if (originalImage == null)
            {
                return null;
            }
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            //新建一个bmp图片
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight, originalImage.PixelFormat);
            //新建一个画板
            Graphics g = System.Drawing.Graphics.FromImage(bitmap);
            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);
            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);
            g.Dispose();
            return bitmap;
        }

        /// <summary>
        /// 生成缩略图，可按需进行缩放
        /// </summary>
        /// <param name="originalImage">源图路径（物理路径）</param>
        /// <param name="width">缩略图宽度,此值为0代表按高度自适应</param>
        /// <param name="height">缩略图高度,此值为0代表按宽度自适应</param>
        public static Image MakeThumbnail(Image originalImage, int width, int height)
        {
            if (originalImage == null)
            {
                return null;
            }
            int towidth = 0;
            int toheight = 0;
            if (width == 0 && height == 0)
            {
                towidth = originalImage.Width;
                toheight = originalImage.Height;
            }
            else if (width == 0)
            {
                toheight = height;
                towidth = originalImage.Width * height / originalImage.Height;
            }
            else if (height == 0)
            {
                towidth = width;
                toheight = originalImage.Height * width / originalImage.Width;
            }
            else
            {
                towidth = width;
                toheight = height;
            }

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;
            Image bit = originalImage;
            if (IsPixelFormatIndexed(originalImage.PixelFormat))
            {
                bit = new Bitmap(originalImage.Width, originalImage.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bit))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.DrawImage(originalImage, 0, 0);
                }
            }
            //新建一个bmp图片
            Image bitmap = new System.Drawing.Bitmap(towidth, toheight, bit.PixelFormat);
            using (Graphics g = System.Drawing.Graphics.FromImage(bitmap))
            {
                //设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //清空画布并以透明背景色填充
                g.Clear(Color.Transparent);
                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                    new Rectangle(x, y, ow, oh),
                    GraphicsUnit.Pixel);
            }
            return bitmap;
        }

        /// <summary>
        /// 根据imagebounds把原图部分区域缩略，缩略图大小 小于或等于controlSize
        /// </summary>
        /// <param name="img">原图</param>
        /// <param name="imageBounds">运动物体矩形框</param>
        /// <param name="controlSize">外部展示控件大小</param>
        /// <returns>裁剪并缩略后的图</returns>
        public static Image MakeThumbnail(Image img, Rectangle imageBounds, Size controlSize)
        {
            return MakeThumbnail(img, imageBounds, controlSize.Width, controlSize.Height);
        }

        /// <summary>
        /// 根据imagebounds把原图部分区域缩略，缩略图大小 小于或等于controlSize
        /// </summary>
        /// <param name="imgPath">原图路径</param>
        /// <param name="imageBounds">运动物体矩形框</param>
        /// <param name="controlSize">外部展示控件大小</param>
        /// <returns>裁剪并缩略后的图</returns>
        public static Image MakeThumbnail(string imgPath, Rectangle imageBounds, Size controlSize)
        {
            Image img = GetImage(imgPath);
            if (img == null)
            {
                return null;
            }
            Image tImg = MakeThumbnail(img, imageBounds, controlSize.Width, controlSize.Height);
            img.Dispose();
            return tImg;
        }

        /// <summary>
        /// 根据imagebounds把原图部分区域缩略，缩略图大小 小于或等于controlWidth controlHeight
        /// </summary>
        /// <param name="imgPath">原图路径</param>
        /// <param name="imageBounds">运动物体矩形框</param>
        /// <param name="controlWidth">外部展示控件大小 宽</param>
        /// <param name="controlHeight">外部展示控件大小 高</param>
        /// <returns>裁剪并缩略后的图</returns>
        public static Image MakeThumbnail(string imgPath, Rectangle imageBounds, int controlWidth, int controlHeight)
        {
            Image img = GetImage(imgPath);
            if (img == null)
            {
                return null;
            }
            Image tImg = MakeThumbnail(img, imageBounds, controlWidth, controlHeight);
            img.Dispose();
            return tImg;
        }

        /// <summary>
        /// 根据imagebounds把原图部分区域缩略，缩略图大小 小于或等于controlWidth controlHeight
        /// </summary>
        /// <param name="img">原图</param>
        /// <param name="imageBounds">运动物体矩形框</param>
        /// <param name="controlWidth">外部展示控件大小 宽</param>
        /// <param name="controlHeight">外部展示控件大小 高</param>
        /// <returns>裁剪并缩略后的图</returns>
        public static Image MakeThumbnail(Image imgSource, Rectangle imageBounds, int controlWidth, int controlHeight)
        {
            //如果矩形够大，直接返回原图片
            if (imageBounds.Width >= imgSource.Width && imageBounds.Height >= imgSource.Height)
            {
                return (Image)imgSource.Clone();
            }            
            //按控件的比例来裁减
            //裁减后的宽度
            int reduceWidth = imageBounds.Height * controlWidth / controlHeight;
            int reduceHeight = imageBounds.Width * controlHeight / controlWidth;
            //取较大的值，来保证截取的图片可以看到全部内容
            if (reduceWidth > imageBounds.Width)
            {
                imageBounds.Width = reduceWidth;
            }
            else
            {
                imageBounds.Height = reduceHeight;
            }

            //默认膨胀原矩形的2倍
            int moveX = imageBounds.X;
            int moveY = imageBounds.Y;
            //需要膨胀的大小
            int expandWidth = imageBounds.Width;
            int expandHeigth = imageBounds.Height;

            //膨胀后的值需要小于图片值
            int afterExpandWidth = imageBounds.Width + expandWidth;
            int afterExpandHeight = imageBounds.Height + expandHeigth;
            //图片最小值必须为原图的1/36
            int minWidth = imgSource.Width / 6;
            int minHeight = imgSource.Height / 6;
            if (afterExpandWidth < minWidth)
            {
                afterExpandWidth = minWidth;
                afterExpandHeight = minWidth * controlHeight / controlWidth;
            }
            else if (afterExpandHeight < minHeight)
            {
                afterExpandHeight = minHeight;
                afterExpandWidth = minHeight * controlWidth / controlHeight;
            }
            //涨膨胀的一半
            moveX -= (afterExpandWidth - imageBounds.Width) / 2;
            moveY -= (afterExpandHeight - imageBounds.Height) / 2;
            moveX = Math.Max(0, moveX);
            moveY = Math.Max(0, moveY);

            imageBounds.Width = afterExpandWidth > imgSource.Width ? imgSource.Width : afterExpandWidth;
            imageBounds.Height = afterExpandHeight > imgSource.Height ? imgSource.Height : afterExpandHeight;

            imageBounds.X = moveX;
            imageBounds.Y = moveY;
            //不仅膨胀，上面做长宽比时也改变了矩形大小，可能导致矩形大小正确，但是却出了图片范围
            //如图片352 288，转换后矩形 100，100 ，200，200，导致高度越界
            if (imageBounds.X + imageBounds.Width > imgSource.Width)
            {
                imageBounds.X = imgSource.Width - imageBounds.Width;
            }
            if (imageBounds.Y + imageBounds.Height > imgSource.Height)
            {
                imageBounds.Y = imgSource.Height - imageBounds.Height;
            }

            int width, height;
            Bitmap reduceBmp = null;
            //显示检索结果的最大宽高
            if (imageBounds.Width > controlWidth || imageBounds.Height > controlHeight)
            {
                width = (imageBounds.Width > controlWidth) ? controlWidth : imageBounds.Width;
                height = (imageBounds.Height > controlHeight) ? controlHeight : imageBounds.Height;
                System.Diagnostics.Debug.WriteLine("图片格式：{0}", imgSource.PixelFormat);
                reduceBmp = new Bitmap(width, height, imgSource.PixelFormat);
                Graphics gx = Graphics.FromImage(reduceBmp);
                gx.DrawImage(imgSource, new Rectangle(
                    0, 0, width, height), imageBounds, GraphicsUnit.Pixel);
                gx.Dispose();
            }
            else
            {
                reduceBmp = new Bitmap(imageBounds.Width, imageBounds.Height, imgSource.PixelFormat);
                Graphics gx = Graphics.FromImage(reduceBmp);
                gx.DrawImage(imgSource, new Rectangle(
                    0, 0, imageBounds.Width, imageBounds.Height), imageBounds, GraphicsUnit.Pixel);
                gx.Dispose();
            }
            reduceBmp = Format24bppRgb((Bitmap)reduceBmp);
            return reduceBmp;
        }

        /// <summary>
        /// 转换为 24位Rgb格式标准位图
        /// </summary>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static Bitmap Format24bppRgb(Bitmap bit)
        {
            //转换为 24位深位图
            Bitmap Bmp = new Bitmap(bit.Width, bit.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Graphics.FromImage(Bmp).DrawImage(bit, new Rectangle(0, 0, Bmp.Width, Bmp.Height));
            return Bmp;
        }

        /// <summary>
        /// 从控件上点击的点转换到对应图片上的点，图片在控件上显示一定要是按控件宽或者高自适应的
        /// </summary>
        /// <param name="iW">图片宽度</param>
        /// <param name="iH">图片高度</param>
        /// <param name="controlSize">控件大小</param>
        /// <param name="cX">控件点击的点X坐标</param>
        /// <param name="cY">控件点击的点Y坐标</param>
        /// <returns>对应图片的点为，x或者y可能有负值，代表未点击到图片上</returns>
        public static Point ClientPointToImage(int iW, int iH, Size controlSize, int cX, int cY)
        {
            int cW = controlSize.Width;
            int cH = controlSize.Height;
            //算出横向增量
            int ax = cH * iW / iH - cW;
            ax = ax > 0 ? 0 : Math.Abs(ax) / 2;
            //算出图片在控件内的宽度
            int iInW = cW - ax * 2;
            //算出纵向增量
            int ay = cW * iH / iW - cH;
            ay = ay > 0 ? 0 : Math.Abs(ay) / 2;
            //算出图片在控件内的高度
            int iInH = cH - ay * 2;
            //点击的点相对于原大小图片的位置
            int xValue = (cX - ax) * iW / iInW;
            int yValue = (cY - ay) * iH / iInH;

            return new Point(xValue, yValue);
        }

        /// <summary>
        /// 判断图片的PixelFormat 是否在 引发异常的 PixelFormat 之中
        /// </summary>
        /// <param name="imgPixelFormat">原图片的PixelFormat</param>
        /// <returns></returns>
        public static bool IsPixelFormatIndexed(PixelFormat imgPixelFormat)
        {
            foreach (PixelFormat pf in indexedPixelFormats)
            {
                if (pf.Equals(imgPixelFormat)) return true;
            }
            return false;
        }
    }
}
