using Common;
using System;
using System.Drawing;
using System.Runtime.Serialization;
using static Repository.Entities.Alarm;

namespace IRMonitor2.Models
{
    /// <summary>
    /// 告警
    /// </summary>
    [DataContract]
    public class Alarm
    {
        /// <summary>
        /// 历史温度队列长度
        /// </summary>
        private const int queueLength = 3;

        /// <summary>
        /// 告警类型
        /// </summary>
        [DataMember]
        public Repository.Entities.Alarm.Type type;

        /// <summary>
        /// 温度类型
        /// </summary>
        [DataMember]
        public Repository.Entities.Selections.TemperatureType temperatureType;

        /// <summary>
        /// 告警等级
        /// </summary>
        [DataMember]
        public Level level;

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        public DateTime startTime;

        /// <summary>
        /// 告警区域
        /// </summary>
        [DataMember]
        public Rectangle area;

        /// <summary>
        /// 告警点
        /// </summary>
        [DataMember]
        public Point point;

        /// <summary>
        /// 历史温度队列
        /// </summary>
        [NonSerialized]
        public FixedLengthQueue<float> temperatures = new FixedLengthQueue<float>(queueLength);
    }
}
