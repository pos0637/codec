using SixLabors.Primitives;
using System.Runtime.Serialization;

namespace IRMonitor
{
    /// <summary>
    /// 选区温度
    /// </summary>
    [DataContract]
    public class SelectionTemperature
    {
        /// <summary>
        /// 所属选区ID
        /// </summary>
        [DataMember(Name = "SelectionId")]
        public long mSelectionId;

        /// <summary>
        /// 选区最低温度
        /// </summary>
        [DataMember(Name = "MinTemperature")]
        public float mMinTemperature;

        /// <summary>
        /// 选区最低温度位置
        /// </summary>
        [DataMember(Name = "MinPoint")]
        public Point mMinPoint;

        /// <summary>
        /// 选区最高温度
        /// </summary>
        [DataMember(Name = "MaxTemperature")]
        public float mMaxTemperature;

        /// <summary>
        /// 选区最高温度位置
        /// </summary>
        [DataMember(Name = "MaxPoint")]
        public Point mMaxPoint;

        /// <summary>
        /// 选区平均温度
        /// </summary>
        [DataMember(Name = "AvgTemperature")]
        public float mAvgTemperature;
    }
}
