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
        public enum AlarmType
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
        /// 告警配置
        /// </summary>
        [DataContract]
        public class AlarmConfiguration
        {
            /// <summary>
            /// 告警类型
            /// </summary>
            [DataMember]
            public AlarmType type;

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
    }
}
