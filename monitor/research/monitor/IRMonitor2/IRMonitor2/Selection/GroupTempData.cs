using System.Runtime.Serialization;

namespace IRMonitor2
{
    /// <summary>
    /// 选区温度信息
    /// </summary>
    [DataContract]
    public class GroupTempData
    {
        // ID
        [DataMember(Name = "SelectionId")]
        public long mGroupId;

        // 最高温度
        [DataMember(Name = "MaxTemperature")]
        public float mMaxTemperature;

        // 温升
        [DataMember(Name = "TemperatureRise")]
        public float mTemperatureRise;

        // 温差
        [DataMember(Name = "TemperatureDif")]
        public float mTemperatureDif;

        // 相对温差
        [DataMember(Name = "RelTemperatureDif")]
        public float mRelTemperatureDif;
    }
}
