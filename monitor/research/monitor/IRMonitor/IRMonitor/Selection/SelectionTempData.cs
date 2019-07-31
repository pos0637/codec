﻿using SixLabors.Primitives;
using System.Runtime.Serialization;

namespace IRMonitor
{
    /// <summary>
    /// 选区温度信息
    /// </summary>
    [DataContract]
    public class SelectionTempData
    {
        // 所属选区ID
        [DataMember(Name = "SelectionId")]
        public long mSelectionId;

        // 选区最低温度
        [DataMember(Name = "MinTemperature")]
        public float mMinTemperature;

        // 选区最低温度位置
        [DataMember(Name = "MinPoint")]
        public Point mMinPoint;

        // 选区最高温度
        [DataMember(Name = "MaxTemperature")]
        public float mMaxTemperature;

        // 选区最高温度位置
        [DataMember(Name = "MaxPoint")]
        public Point mMaxPoint;

        // 选区平均温度
        [DataMember(Name = "AvgTemperature")]
        public float mAvgTemperature;
    }
}
