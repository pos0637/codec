using System.Collections.Generic;
using System.Drawing;

namespace Repository.Entities
{
    /// <summary>
    /// 人脸信息
    /// </summary>
    public class DetectionResult
    {
        /// <summary>
        /// 人脸矩形区域
        /// </summary>
        public List<Rectangle> rectangles = new List<Rectangle>();

        /// <summary>
        /// 新增人脸矩形区域
        /// </summary>
        public List<Rectangle> addRectangles = new List<Rectangle>();
    }
}
