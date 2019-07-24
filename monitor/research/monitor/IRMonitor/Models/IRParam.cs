using System;
using System.Runtime.Serialization;

namespace Models
{
    [DataContract]
    public class IRParam
    {
        /// <summary>
        /// 索引
        /// </summary>
        [DataMember(Name = "Id")]
        public Int64 mId;

        /// <summary>
        /// 辐射率
        /// </summary>
        [DataMember(Name = "Emissivity")]
        public Single mEmissivity;

        /// <summary>
        /// 翻转表象温度
        /// </summary>
        [DataMember(Name = "ReflectedTemperature")]
        public Single mReflectedTemperature;

        /// <summary>
        /// 透过率
        /// </summary>
        [DataMember(Name = "Transmission")]
        public Single mTransmission;

        /// <summary>
        /// 大气温度
        /// </summary>
        [DataMember(Name = "AtmosphericTemperature")]
        public Single mAtmosphericTemperature;

        /// <summary>
        /// 相对湿度
        /// </summary>
        [DataMember(Name = "RelativeHumidity")]
        public Single mRelativeHumidity;

        /// <summary>
        /// 距离
        /// </summary>
        [DataMember(Name = "Distance")]
        public Single mDistance;
    }
}
