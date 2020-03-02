using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IRApplication.Components
{
    /// <summary>
    /// 可渲染控件
    /// </summary>
    public class GDIRenderableControl : IRenderableControl
    {
        /// <summary>
        /// 视图宽度
        /// </summary>
        private int viewWidth;

        /// <summary>
        /// 视图对齐宽度
        /// </summary>
        private int viewStride;

        /// <summary>
        /// 视图高度
        /// </summary>
        private int viewHeight;

        /// <summary>
        /// YV12位图
        /// </summary>
        protected Mat yuv;

        /// <summary>
        /// BGRA位图
        /// </summary>
        protected Mat bgra;

        /// <summary>
        /// 位图
        /// </summary>
        private Bitmap bitmap;

        /// <summary>
        /// 绘图区域
        /// </summary>
        private Rectangle area;

        public bool InitializeComponent(Control parent, int width, int stride, int height)
        {
            viewWidth = width;
            viewStride = stride;
            viewHeight = height;
            yuv = new Mat(viewHeight + viewHeight / 2, viewWidth, MatType.CV_8UC1);
            bgra = new Mat(viewHeight, viewWidth, MatType.CV_8UC4);
            bitmap = new Bitmap(width, height);
            area = parent.ClientRectangle;
            parent.Paint += Parent_Paint;

            return true;
        }

        public CreateParams GetParams(CreateParams createParams)
        {
            // 防止控件闪烁
            createParams.ExStyle |= 0x02000000;
            return createParams;
        }

        public void Dispose()
        {
        }

        public unsafe void DrawYV12Image(IntPtr image, int length)
        {
            System.Buffer.MemoryCopy(image.ToPointer(), yuv.Data.ToPointer(), length, length);
            Cv2.CvtColor(yuv, bgra, ColorConversionCodes.YUV2BGRA_YV12);
        }

        public unsafe void DrawYImage(IntPtr image, int length)
        {
            System.Buffer.MemoryCopy(image.ToPointer(), yuv.Data.ToPointer(), length, length);
            Cv2.CvtColor(yuv, bgra, ColorConversionCodes.YUV2BGRA_YV12);
        }

        public unsafe void DrawYImage(byte[] image, int length)
        {
            Marshal.Copy(image, 0, yuv.Data, length);
            Cv2.CvtColor(yuv, bgra, ColorConversionCodes.YUV2BGRA_YV12);
        }

        public unsafe void DrawRGBAImage(IntPtr image, int length)
        {
            System.Buffer.MemoryCopy(image.ToPointer(), bgra.Data.ToPointer(), length, length);
        }

        public void Render(Control control)
        {
            BitmapConverter.ToBitmap(bgra, bitmap);
            control.Invalidate();
        }

        public virtual void OnSizeChanged(Control control)
        {
            area = control.ClientRectangle;
        }

        private void Parent_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bitmap, area);
        }
    }
}
