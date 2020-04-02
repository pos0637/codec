using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace DrawTools2
{
    public class GraphicsData
    {
        /// <summary>
        /// 是否是椭圆
        /// </summary>
        public Boolean IsEllipse;

        /// <summary>
        /// 点坐标列表,仅用于非椭圆
        /// </summary>
        public List<Point> PointList;

        /// <summary>
        /// 椭圆圆心坐标,仅用于椭圆
        /// </summary>
        public Point Center;

        /// <summary>
        /// 轴的长度,仅用于椭圆
        /// </summary>
        public Size Axes;

        /// <summary>
        /// 偏转的角度
        /// </summary>
        public double Angle;
    }
}
