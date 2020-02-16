using OpenCvSharp;

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
        public static void ShowYV12Image(string name, int width, int height, dynamic buffer)
        {
            var src = new Mat(height + height / 2, width, MatType.CV_8UC1, buffer);
            var dst = src.CvtColor(ColorConversionCodes.YUV2BGR_YV12);
            Cv2.ImShow(name, dst);
            Cv2.WaitKey(0);
        }

        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="buffer">数据</param>
        public static void ShowYImage(string name, int width, int height, dynamic buffer)
        {
            var src = new Mat(height, width, MatType.CV_8UC1, buffer);
            Cv2.ImShow(name, src);
            Cv2.WaitKey(0);
        }
    }
}
