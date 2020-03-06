using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace DrawTools
{
    /// <summary>
    /// Line graphic object
    /// </summary>
    //[Serializable]
    public class Draw
    {
        public Draw()
        {

        }

        /// <summary>
        /// 绘制文字
        /// </summary>
        /// <param name="image">输入图像</param>
        /// <param name="textList">文字集合</param>
        /// <param name="xList">绘制x坐标集合</param>
        /// <param name="yList">绘制y坐标集合</param>
        /// <param name="fontSize">绘制文字大小</param>
        public void DrawText(Mat image, List<string> textList, List<int> xList, List<int> yList, Scalar color, int fontSize)
        {
            Bitmap bmap = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmap);
            SolidBrush drawBrush = new SolidBrush(Color.FromArgb((int)color.Val2, (int)color.Val1, (int)color.Val0));
            Font drawFont = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Millimeter);
            for (int i = 0; i < textList.Count; i++) {
                g.DrawString(textList[i], drawFont, drawBrush, xList[i], yList[i]);
            }

            Mat font = BitmapConverter.ToMat(bmap);
            Mat mask = font.Clone();
            Cv2.CvtColor(mask, mask, ColorConversionCodes.BGR2GRAY);
            Mat roi = new Mat(image, new Rect(0, 0, font.Cols, font.Rows));
            font.CopyTo(roi, mask);

            bmap = null;
        }


        /// <summary>
        /// opencv绘制矩形
        /// </summary>
        /// <param name="image">输入图像</param>
        /// <param name="x">矩形x坐标</param>
        /// <param name="y">矩形y坐标</param>
        /// <param name="width">矩形宽</param>
        /// <param name="height">矩形高</param>
        public void DrawRect(Mat image, int x, int y, int width, int height, float xScale, float yScale, Scalar color, int thickness)
        {
            Cv2.Rectangle(image, new Rect((int)(x * xScale), (int)(y * yScale), (int)(width * xScale), (int)(height * yScale)), color, thickness);
        }

    }
}