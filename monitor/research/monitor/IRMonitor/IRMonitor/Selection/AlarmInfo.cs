using System;

namespace IRMonitor
{
    /// <summary>
    /// 告警模式
    /// </summary>
    public enum AlarmMode
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// 普通选区告警
        /// </summary>
        Selection,
        /// <summary>
        /// 组告警(664告警)
        /// </summary>
        GroupSelection
    }

    /// <summary>
    /// 选区告警类型
    /// </summary>
    public enum SelectionAlarmType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// 最高温度
        /// </summary>
        MaxTemp = 0,
        /// <summary>
        /// 最低温度
        /// </summary>
        MinTemp,
        /// <summary>
        /// 平均温度
        /// </summary>
        AvgTemp
    }

    public enum GroupAlarmType
    {
        /// <summary>
        /// 
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// 最大温度
        /// </summary>
        MaxTemperature = 0,
        /// <summary>
        /// 最大温升
        /// </summary>
        MaxTempRise,
        /// <summary>
        /// 最大温差
        /// </summary>
        MaxTempDif,
        /// <summary>
        /// 相对温差
        /// </summary>
        RelativeTempDif,
    }

    /// <summary>
    /// 告警状态
    /// </summary>
    public enum AlarmStatus
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 告警准备开始
        /// </summary>
        AlarmReadyStart,
        /// <summary>
        /// 告警开始
        /// </summary>
        AlarmStart,
        /// <summary>
        /// 正在告警
        /// </summary>
        Alarming,
        /// <summary>
        /// 告警等级准备改变
        /// </summary>
        AlarmingReadyChanged,
        /// <summary>
        /// 告警等级改变
        /// </summary>
        AlarmingChanged,
        /// <summary>
        /// 告警准备结束
        /// </summary>
        AlarmReadyEnd,
        /// <summary>
        /// 告警结束
        /// </summary>
        AlarmEnd
    }

    /// <summary>
    /// 告警等级
    /// </summary>
    public enum AlarmLevel
    {
        /// <summary>
        /// 未知告警
        /// </summary>
        Unknown = -1,
        /// <summary>
        /// 一般告警
        /// </summary>
        General = 0,
        /// <summary>
        /// 严重告警
        /// </summary>
        Serious,
        /// <summary>
        /// 危急告警
        /// </summary>
        Critical,
    }

    /// <summary>
    /// 告警信息结构体
    /// </summary>
    public class AlarmInfo
    {
        /// <summary>
        /// 告警状态
        /// </summary>
        public AlarmStatus mAlarmStatus = AlarmStatus.Unknown;

        /// <summary>
        /// 最近的告警等级
        /// </summary>
        public AlarmLevel mLastAlarmLevel = AlarmLevel.Unknown;

        /// <summary>
        /// 当前告警等级
        /// </summary>
        public AlarmLevel mCurrentAlarmLevel = AlarmLevel.Unknown;

        /// <summary>
        /// 告警时的等级
        /// </summary>
        public AlarmLevel mAlarmingLevel = AlarmLevel.Unknown;

        /// <summary>
        /// 延迟告警次数
        /// </summary>
        public Int32 mReadyAlarmNum = 0;

        /// <summary>
        /// 是否在录像
        /// </summary>
        public Boolean mIsRecord = false;

        /// <summary>
        /// 是否超时
        /// </summary>
        public Boolean mIsTimeout = false;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime mBeginTime;

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            mAlarmStatus = AlarmStatus.Unknown;
            mLastAlarmLevel = AlarmLevel.Unknown;
            mCurrentAlarmLevel = AlarmLevel.Unknown;
            mReadyAlarmNum = 0;
            mIsRecord = false;
            mIsTimeout = false;
        }
    }

    /// <summary>
    /// 告警信息结构体
    /// </summary>
    public class SelectionAlarmInfo
    {
        /// <summary>
        /// 高温告警信息
        /// </summary>
        public AlarmInfo mMaxTempAlarmInfo = new AlarmInfo();

        /// <summary>
        /// 低温告警信息
        /// </summary>
        public AlarmInfo mMinTempAlarmInfo = new AlarmInfo();

        /// <summary>
        /// 平均温告警信息
        /// </summary>
        public AlarmInfo mAvgTempAlarmInfo = new AlarmInfo();
    }
}
