using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace IRMonitor2.Models
{
    /// <summary>
    /// 选区
    /// </summary>
    public class Selections
    {
        /// <summary>
        /// 选区
        /// </summary>
        [DataContract]
        public abstract class Selection
        {
            /// <summary>
            /// 选区最低温度
            /// </summary>
            [DataMember]
            public float minTemperature;

            /// <summary>
            /// 选区最低温度位置
            /// </summary>
            [DataMember]
            public Point minPoint;

            /// <summary>
            /// 选区最高温度
            /// </summary>
            [DataMember]
            public float maxTemperature;

            /// <summary>
            /// 选区最高温度位置
            /// </summary>
            [DataMember]
            public Point maxPoint;

            /// <summary>
            /// 选区平均温度
            /// </summary>
            [DataMember]
            public float avgTemperature;

            /// <summary>
            /// 高温警告
            /// </summary>
            [DataMember]
            public Alarm maxTemperatureAlarm;

            /// <summary>
            /// 低温警告
            /// </summary>
            [DataMember]
            public Alarm minTemperatureAlarm;

            /// <summary>
            /// 平均温警告
            /// </summary>
            [DataMember]
            public Alarm averageTemperatureAlarm;

            /// <summary>
            /// 区域内像素点的数量
            /// </summary>
            [NonSerialized]
            public int pixels;

            /// <summary>
            /// 选区配置
            /// </summary>
            public abstract Repository.Entities.Selections.Selection Entity { get; }

            /// <summary>
            /// 初始化
            /// </summary>
            public abstract void Initialize();

            /// <summary>
            /// 计算温度
            /// </summary>
            /// <param name="data">温度数据</param>
            /// <param name="width">宽度</param>
            /// <param name="stride">对齐宽度</param>
            /// <param name="height">高度</param>
            /// <returns>计算结果</returns>
            public abstract void Calculate(float[] data, int width, int stride, int height);
        }

        /// <summary>
        /// 点选区
        /// </summary>
        [DataContract]
        public class PointSelection : Selection
        {
            /// <summary>
            /// 实体
            /// </summary>
            [DataMember]
            public Repository.Entities.Selections.PointSelection entity;

            public override Repository.Entities.Selections.Selection Entity { get { return entity; } }

            public override void Initialize()
            {
                pixels = 1;
            }

            public override void Calculate(float[] data, int width, int stride, int height)
            {
                var point = entity.point;
                var temp = data[stride * point.Y + point.X];
                minTemperature = maxTemperature = avgTemperature = temp;
                minPoint = maxPoint = point;
            }
        }

        /// <summary>
        /// 线选区
        /// </summary>
        [DataContract]
        public class LineSelection : Selection
        {
            /// <summary>
            /// 实体
            /// </summary>
            [DataMember]
            public Repository.Entities.Selections.LineSelection entity;

            public override Repository.Entities.Selections.Selection Entity { get { return entity; } }

            public override void Initialize()
            {
                var start = entity.start;
                var end = entity.end;
                var x = end.X - start.X;
                var y = end.Y - start.Y;
                pixels = (int)(Math.Sqrt(x * x + y * y) + 1);
            }

            public override void Calculate(float[] data, int width, int stride, int height)
            {
                var start = entity.start;
                var end = entity.end;
                var position = stride * start.Y + start.X;
                var min = data[position];
                var max = data[position];
                var minPoint = new Point(start.X, start.Y);
                var maxPoint = new Point(start.X, start.Y);

                // 直线斜率
                float a;
                if (start.X != end.X) {
                    a = (start.Y - end.Y) * 1.0F / (start.X - end.X);
                }
                else {
                    a = 0;
                }

                // 直线截距
                var b = end.Y - a * end.X;
                var beginX = start.X > end.X ? end.X : start.X;
                var endX = start.X < end.X ? end.X : start.X;
                var beginY = start.Y > end.Y ? end.Y : start.Y;
                var endY = start.Y < end.Y ? end.Y : start.Y;
                var pitch = stride * beginY;
                var total = 0.0F;

                for (var m = beginY; m <= endY; ++m) {
                    for (var n = beginX; n <= endX; ++n) {
                        if ((int)(a * n + b) == m) {
                            var val = data[pitch + n];
                            if (min > val) {
                                min = val;
                                minPoint.X = n;
                                minPoint.Y = m;
                            }
                            else if (max < val) {
                                max = val;
                                maxPoint.X = n;
                                maxPoint.Y = m;
                            }
                            total += val;
                        }
                    }
                    pitch += stride;
                }

                minTemperature = min;
                maxTemperature = max;
                avgTemperature = total / pixels;
                this.minPoint = minPoint;
                this.maxPoint = maxPoint;
            }
        }

        /// <summary>
        /// 矩形选区
        /// </summary>
        [DataContract]
        public class RectangleSelection : Selection
        {
            /// <summary>
            /// 实体
            /// </summary>
            [DataMember]
            public Repository.Entities.Selections.RectangleSelection entity;

            public override Repository.Entities.Selections.Selection Entity { get { return entity; } }

            public override void Initialize()
            {
                var rectangle = entity.rectangle;
                pixels = rectangle.Width * rectangle.Height;
            }

            public override void Calculate(float[] data, int width, int stride, int height)
            {
                var rectangle = entity.rectangle;
                var position = stride * rectangle.Top + rectangle.Left;
                var min = data[position];
                var max = data[position];
                var minPoint = new Point(rectangle.Left, rectangle.Top);
                var maxPoint = new Point(rectangle.Left, rectangle.Top);

                var total = 0.0F;
                var pitch = stride * rectangle.Top;

                for (var m = rectangle.Top; m < rectangle.Bottom; ++m) {
                    for (var n = rectangle.Left; n < rectangle.Right; ++n) {
                        var temp = data[pitch + n];
                        if (min > temp) {
                            min = temp;
                            minPoint.X = n;
                            minPoint.Y = m;
                        }
                        else if (max < temp) {
                            max = temp;
                            maxPoint.X = n;
                            maxPoint.Y = m;
                        }
                        total += temp;
                    }
                    pitch += stride;
                }

                minTemperature = min;
                maxTemperature = max;
                avgTemperature = total / pixels;
                this.minPoint = minPoint;
                this.maxPoint = maxPoint;
            }
        }

        /// <summary>
        /// 椭圆选区
        /// </summary>
        [DataContract]
        public class EllipseSelection : Selection
        {
            /// <summary>
            /// 线段的结构体
            /// </summary>
            protected struct Segment
            {
                public int start;
                public int end;
                public int y;
            }

            /// <summary>
            /// 实体
            /// </summary>
            [DataMember]
            public Repository.Entities.Selections.EllipseSelection entity;

            public override Repository.Entities.Selections.Selection Entity { get { return entity; } }

            /// <summary>
            /// X轴
            /// </summary>
            [NonSerialized]
            protected int xAxis;

            /// <summary>
            /// Y轴
            /// </summary>
            [NonSerialized]
            protected int yAxis;

            /// <summary>
            /// 圆心
            /// </summary>
            [NonSerialized]
            protected Point center;

            /// <summary>
            /// 椭圆所在区域点的数组
            /// </summary>
            [NonSerialized]
            protected Segment[] area;

            public override void Initialize()
            {
                var rectangle = entity.rectangle;
                xAxis = rectangle.Width;
                yAxis = rectangle.Height;
                center.X = rectangle.X + rectangle.Width / 2;
                center.Y = rectangle.Y + rectangle.Height / 2;

                var length = yAxis + 1;
                var startY = center.Y - (yAxis / 2);
                double ellipseA, ellipseB, ellipseX, ellipseY;

                ellipseA = xAxis / 2.0F;
                ellipseA *= ellipseA;
                ellipseB = yAxis / 2.0F;
                ellipseB *= ellipseB;

                pixels = 0;
                area = new Segment[length];

                // 获取椭圆图形区域
                for (var i = 0; i < length; ++i) {
                    ellipseY = center.Y - startY - i;
                    ellipseY *= ellipseY;
                    ellipseX = ellipseA - ((ellipseY * ellipseA) / ellipseB);
                    ellipseX = Math.Sqrt(ellipseX);
                    area[i].start = (int)(center.X - ellipseX);
                    area[i].end = (int)(center.X + ellipseX);
                    area[i].y = startY + i;
                    pixels += area[i].end - area[i].start;
                }
            }

            public override void Calculate(float[] data, int width, int stride, int height)
            {
                var length = yAxis + 1;
                var position = area[0].start + stride * area[0].y;
                var min = data[position];
                var max = data[position];
                var minPoint = new Point { X = area[0].start, Y = area[0].y };
                var maxPoint = new Point { X = area[0].start, Y = area[0].y };
                var total = 0.0F;
                var pitch = stride * area[0].y;

                for (int m = 0; m < length; ++m) {
                    for (int n = area[m].start; n < area[m].end; ++n) {
                        float temp = data[n + pitch];
                        if (min > temp) {
                            min = temp;
                            minPoint.X = n;
                            minPoint.Y = area[m].y;
                        }
                        else if (max < temp) {
                            max = temp;
                            maxPoint.X = n;
                            maxPoint.Y = area[m].y;
                        }
                        total += temp;
                    }
                    pitch += stride;
                }

                minTemperature = min;
                maxTemperature = max;
                avgTemperature = total / pixels;
                this.minPoint = minPoint;
                this.maxPoint = maxPoint;
            }
        }
    }
}
