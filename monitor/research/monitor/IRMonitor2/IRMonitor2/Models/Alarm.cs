using Common;
using System;
using System.Runtime.Serialization;

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
        /// 历史温度队列
        /// </summary>
        [NonSerialized]
        public FixedLengthQueue<float> temperatures = new FixedLengthQueue<float>(queueLength);
    }
}
