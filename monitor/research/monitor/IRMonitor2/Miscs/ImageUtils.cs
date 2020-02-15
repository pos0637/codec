using OpenCvSharp;
using System;

namespace Miscs
{
    /// <summary>
    /// 图像工具类
    /// </summary>
    public static class ImageUtils
    {
        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="buffer">数据</param>
        public static void ShowImage(string name, int width, int height, IntPtr buffer)
        {
            var src = new Mat(height + height / 2, width, MatType.CV_8UC1, buffer);
            var dst = src.CvtColor(ColorConversionCodes.YUV2BGR_YV12);
            Cv2.ImShow(name, dst);
            Cv2.WaitKey(0);
        }
    }
}
