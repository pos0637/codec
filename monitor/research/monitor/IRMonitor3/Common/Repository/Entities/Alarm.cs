using System;
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
        public int id { get; set; }

        /// <summary>
        /// 设备单元名称
        /// </summary>
        public string cellName { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string deviceName { get; set; }

        /// <summary>
        /// 选区名称
        /// </summary>
        public string selectionName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? startTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? endTime { get; set; }

        /// <summary>
        /// 告警类型
        /// </summary>
        public Type alarmType { get; set; }

        /// <summary>
        /// 温度类型
        /// </summary>
        public Selections.TemperatureType temperatureType { get; set; }

        /// <summary>
        /// 告警等级
        /// </summary>
        public Level level { get; set; }

        /// <summary>
        /// 告警区域
        /// </summary>
        public string area { get; set; }

        /// <summary>
        /// 告警点
        /// </summary>
        public string point { get; set; }

        /// <summary>
        /// 告警详情
        /// </summary>
        public string detail { get; set; }

        /// <summary>
        /// 处理意见
        /// </summary>
        public string comment { get; set; }

        /// <summary>
        /// 温度矩阵快照资源地址
        /// </summary>
        public string temperatureUrl { get; set; }

        /// <summary>
        /// 红外图像快照资源地址
        /// </summary>
        public string irImageUrl { get; set; }

        /// <summary>
        /// 可见光图像快照资源地址
        /// </summary>
        public string imageUrl { get; set; }

        /// <summary>
        /// 告警录像资源地址
        /// </summary>
        public string videoUrl { get; set; }

        /// <summary>
        /// 红外摄像机参数
        /// </summary>
        public string irCameraParameters { get; set; }
    }
}
