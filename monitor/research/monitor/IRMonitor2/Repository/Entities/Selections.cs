using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace Repository.Entities
{
    /// <summary>
    /// 选区配置
    /// </summary>
    [DataContract]
    public class Selections
    {
        /// <summary>
        /// 选区类型
        /// </summary>
        public enum SelectionType
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// 点选区
            /// </summary>
            Point,

            /// <summary>
            /// 线选区
            /// </summary>
            Line,

            /// <summary>
            /// 矩形选区
            /// </summary>
            Rectangle,

            /// <summary>
            /// 椭圆选区
            /// </summary>
            Ellipse
        }

        /// <summary>
        /// 温度类型
        /// </summary>
        public enum TemperatureType
        {
            /// <summary>
            /// 最低温度
            /// </summary>
            min,

            /// <summary>
            /// 最高温度
            /// </summary>
            max,

            /// <summary>
            /// 平均温度
            /// </summary>
            average
        }

        /// <summary>
        /// 选区
        /// </summary>
        [DataContract]
        public abstract class Selection
        {
            /// <summary>
            /// 类型
            /// </summary>
            [DataMember]
            public SelectionType type;

            /// <summary>
            /// 名称
            /// </summary>
            [DataMember]
            public string name;

            /// <summary>
            /// 是否为全局选区
            /// </summary>
            [DataMember]
            public bool isGlobal;

            /// <summary>
            /// 高温警告配置
            /// </summary>
            [DataMember]
            public List<Alarm.AlarmConfiguration> alarmConfigurations;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="SelectionType">选区类型</param>
            public Selection(SelectionType SelectionType)
            {
                type = SelectionType;
            }
        }

        /// <summary>
        /// 点选区
        /// </summary>
        [DataContract]
        public class PointSelection : Selection
        {
            [DataMember]
            public Point point;

            public PointSelection() : base(SelectionType.Point) { }
        }

        /// <summary>
        /// 线选区
        /// </summary>
        [DataContract]
        public class LineSelection : Selection
        {
            [DataMember]
            public Point start = new Point();

            [DataMember]
            public Point end = new Point();

            public LineSelection() : base(SelectionType.Line) { }
        }

        /// <summary>
        /// 矩形选区
        /// </summary>
        [DataContract]
        public class RectangleSelection : Selection
        {
            [DataMember]
            public Rectangle rectangle;

            public RectangleSelection() : base(SelectionType.Rectangle) { }
        }

        /// <summary>
        /// 椭圆选区
        /// </summary>
        [DataContract]
        public class EllipseSelection : Selection
        {
            [DataMember]
            public Rectangle rectangle;

            public EllipseSelection() : base(SelectionType.Ellipse) { }
        }

        /// <summary>
        /// 选区列表
        /// </summary>     
        [DataMember]
        public List<Selection> selections;
    }
}
