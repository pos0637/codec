using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace Repository.Entities
{
    /// <summary>
    /// 告警配置
    /// </summary>
    public class Alarm
    {
        /// <summary>
        /// 告警类型
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 无告警
            /// </summary>
            None = 0,

            /// <summary>
            /// 高于阈值告警
            /// </summary>
            High,

            /// <summary>
            /// 低于阈值告警
            /// </summary>
            Low
        }

        /// <summary>
        /// 告警等级
        /// </summary>
        public enum Level
        {
            /// <summary>
            /// 无告警
            /// </summary>
            None = 0,

            /// <summary>
            /// 一般告警
            /// </summary>
            General,

            /// <summary>
            /// 严重告警
            /// </summary>
            Serious,

            /// <summary>
            /// 危急告警
            /// </summary>
            Critical
        }

        /// <summary>
        /// 告警配置
        /// </summary>
        [DataContract]
        public class AlarmConfiguration
        {
            /// <summary>
            /// 告警类型
            /// </summary>
            [DataMember]
            public Type type;

            /// <summary>
            /// 温度类型
            /// </summary>
            [DataMember]
            public Selections.TemperatureType temperatureType;

            /// <summary>
            /// 一般阀值
            /// </summary>
            [DataMember]
            public float generalThreshold;

            /// <summary>
            /// 严重阀值
            /// </summary>
            [DataMember]
            public float seriousThreshold;

            /// <summary>
            /// 危急阀值
            /// </summary>
            [DataMember]
            public float criticalThreshold;
        }

        /// <summary>
        /// 索引
        /// </summary>
        public long id;

        /// <summary>
        /// 设备单元名称
        /// </summary>
        public string cellName;

        /// <summary>
        /// 设备名称
        /// </summary>
        public string deviceName;

        /// <summary>
        /// 选区名称
        /// </summary>
        public string selectionName;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime startTime;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime endTime;

        /// <summary>
        /// 告警类型
        /// </summary>
        public Type alarmType;

        /// <summary>
        /// 温度类型
        /// </summary>
        public Selections.TemperatureType temperatureType;

        /// <summary>
        /// 告警等级
        /// </summary>
        public Level level;

        /// <summary>
        /// 告警区域
        /// </summary>
        public RectangleF? area;

        /// <summary>
        /// 告警点
        /// </summary>
        public PointF? point;

        /// <summary>
        /// 告警详情
        /// </summary>
        public string detail;

        /// <summary>
        /// 处理意见
        /// </summary>
        public string comment;

        /// <summary>
        /// 温度矩阵快照资源地址
        /// </summary>
        public string temperatureUrl;

        /// <summary>
        /// 红外图像快照资源地址
        /// </summary>
        public string irImageUrl;

        /// <summary>
        /// 可见光图像快照资源地址
        /// </summary>
        public string imageUrl;

        /// <summary>
        /// 告警录像资源地址
        /// </summary>
        public string videoUrl;

        /// <summary>
        /// 红外摄像机参数
        /// </summary>
        public Configuration.IrCameraParameters irCameraParameters;
    }
}
