using System;
using System.Runtime.Serialization;

namespace IRMonitor
{
    /// <summary>
    /// 选区温度信息
    /// </summary>
    [DataContract]
    public class GroupTempData
    {
        // ID
        [DataMember(Name = "SelectionId")]
        public Int64 mGroupId;

        // 最高温度
        [DataMember(Name = "MaxTemperature")]
        public Single mMaxTemperature;

        // 温升
        [DataMember(Name = "TemperatureRise")]
        public Single mTemperatureRise;

        // 温差
        [DataMember(Name = "TemperatureDif")]
        public Single mTemperatureDif;

        // 相对温差
        [DataMember(Name = "RelTemperatureDif")]
        public Single mRelTemperatureDif;
    }
}
