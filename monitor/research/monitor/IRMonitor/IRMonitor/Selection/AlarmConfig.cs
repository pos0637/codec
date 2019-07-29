using System;
using System.Runtime.Serialization;

namespace IRMonitor
{
    /// <summary>
    /// 告警原因
    /// </summary>
    public enum AlarmReason
    {
        /// <summary>
        /// 未知告警
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// 无告警
        /// </summary>
        None,
        /// <summary>
        /// 高于阈值
        /// </summary>
        High,
        /// <summary>
        /// 低温告警
        /// </summary>
        Low
    }

    /// <summary>
    /// 告警设置
    /// </summary>
    [DataContract]
    public class AlarmConfigData
    {
        /// <summary>
        /// 告警类型
        /// </summary>
        [DataMember(Name = "AlarmReason")]
        public AlarmReason mAlarmReason = AlarmReason.None;

        /// <summary>
        /// 一般阀值
        /// </summary>
        [DataMember(Name = "GeneralThreshold")]
        public Single mGeneralThreshold;

        /// <summary>
        /// 严重阀值
        /// </summary>
        [DataMember(Name = "SeriousThreshold")]
        public Single mSeriousThreshold;

        /// <summary>
        /// 危急阀值
        /// </summary>
        [DataMember(Name = "CriticalThreshold")]
        public Single mCriticalThreshold;

        public override Boolean Equals(Object obj)
        {
            if (obj == null)
                return false;

            AlarmConfigData temp = obj as AlarmConfigData;
            if (temp == null)
                return false;

            if ((this.mAlarmReason != temp.mAlarmReason)
                || (this.mGeneralThreshold != temp.mGeneralThreshold)
                || (this.mSeriousThreshold != temp.mSeriousThreshold)
                || (this.mCriticalThreshold != temp.mCriticalThreshold))
                return false;

            return true;
        }

        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// 选区告警配置
    /// </summary>
    [DataContract]
    public class SelectionAlarmConfig
    {
        /// <summary>
        /// 高温警告配置
        /// </summary>
        [DataMember(Name = "MaxTempConfig")]
        public AlarmConfigData mMaxTempConfig = new AlarmConfigData();

        /// <summary>
        /// 低温警告配置
        /// </summary>
        [DataMember(Name = "MinTempConfig")]
        public AlarmConfigData mMinTempConfig = new AlarmConfigData();

        /// <summary>
        /// 平均温警告配置
        /// </summary>
        [DataMember(Name = "AvgTempConfig")]
        public AlarmConfigData mAvgTempConfig = new AlarmConfigData();
    }

    /// <summary>
    /// 组选区告警设置
    /// </summary>
    [DataContract]
    public class GroupAlarmConfig
    {
        /// <summary>
        /// 高温警告配置
        /// </summary>
        [DataMember(Name = "MaxTempConfig")]
        public AlarmConfigData mMaxTempConfig = new AlarmConfigData();

        /// <summary>
        /// 最大温升警告配置
        /// </summary>
        [DataMember(Name = "MaxTempeRiseConfig")]
        public AlarmConfigData mMaxTempeRiseConfig = new AlarmConfigData();

        /// <summary>
        /// 相对温差警告配置
        /// </summary>
        [DataMember(Name = "RelativeTempDifConfig")]
        public AlarmConfigData mRelativeTempDifConfig = new AlarmConfigData();

        /// <summary>
        ///  最大温差警告配置
        /// </summary>
        [DataMember(Name = "MaxTempDifConfig")]
        public AlarmConfigData mMaxTempDifConfig = new AlarmConfigData();
    }
}
