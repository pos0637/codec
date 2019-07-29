using System;
using System.Runtime.Serialization;

namespace IRMonitor.Common
{
    public enum CommandType
    {
        /// <summary>
        /// 获取全部Cell信息
        /// 参数:无
        /// 返回值:List<Cell>对象JSON
        /// </summary>
        GetAllCellInfo = 0,

        /// <summary>
        /// 获取所有选区信息
        /// 参数:无
        /// 返回值:List<Selection>对象JSON
        /// </summary>
        GetAllSelectionInfo,

        /// <summary>
        /// 添加选区
        /// 参数:选区对象JSON
        /// 返回值:选区id
        /// </summary>
        AddSelection,

        /// <summary>
        /// 更新选区
        /// 参数:选区对象JSON
        /// 返回值:无
        /// </summary>
        UpdateSelection,

        /// <summary>
        /// 更新选区配置
        /// </summary>
        UpdateSelectionConfig,

        /// <summary>
        /// 删除选区
        /// 参数:选区id
        /// 返回值:无
        /// </summary>
        RemoveSelection,

        /// <summary>
        /// 获取所有组选区信息
        /// 参数:无
        /// 返回值:List<GroupSelection>对象JSON
        /// </summary>
        GetAllGroupSelectionInfo,

        /// <summary>
        /// 添加组选区
        /// 参数:组选区对象JSON
        /// 返回值:组选区id
        /// </summary>
        AddGroupSelection,

        /// <summary>
        /// 更新组选区
        /// 参数:组选区对象JSON
        /// 返回值:无
        /// </summary>
        UpdateGroupSelectionConfig,

        /// <summary>
        /// 删除组选区
        /// 参数:组选区id
        /// 返回值:无
        /// </summary>
        RemoveGroupSelection,

        /// <summary>
        /// 获取告警通知配置
        /// 参数:无
        /// 返回值:AlarmNoticeConfig对象JSON
        /// </summary>
        GetAlarmNoticeConfig,

        /// <summary>
        /// 更新告警通知配置
        /// 参数:告警通知配置JSON
        /// 返回值:无
        /// </summary>
        UpdateAlarmNoticeConfig,

        /// <summary>
        /// 获取红外参数配置
        /// 参数:无
        /// 返回值:IRParam对象JSON
        /// </summary>
        GetIRParam,

        /// <summary>
        /// 更新红外参数
        /// 参数:IRParam对象JSON
        /// 返回值:无
        /// </summary>
        UpdateIRParam,

        /// <summary>
        /// 获取温度采样
        /// 参数:无
        /// 返回值:TempCurveSample对象JSON
        /// </summary>
        GetTempCurveSample,

        /// <summary>
        /// 更新温度采样
        /// 参数:TempCurveSample对象JSON
        /// 返回值:无
        /// </summary>
        UpdateTempCurveSample,

        /// <summary>
        /// 获取用户常规配置 
        /// 参数:无
        /// 返回值:UserRoutineConfig对象JSON
        /// </summary>
        GetUserRoutineConfig,

        /// <summary>
        /// 更新用户常规配置
        /// 参数:UserRoutineConfig对象JSON
        /// 返回值:无
        /// </summary>
        UpdateUserRoutineConfig,

        /// <summary>
        /// 开启手动录像
        /// 参数:无
        /// 返回值:无
        /// </summary>
        StartManaulRecord,

        /// <summary>
        /// 关闭手动录像
        /// 参数:无
        /// 返回值:无
        /// </summary>
        StopManaulRecord,

        /// <summary>
        /// 获取关于硬件信息
        /// 参数:无
        /// 返回值:DeviceInfo对象JSON
        /// </summary>
        GetDeviceInfo,

        /// <summary>
        /// 获取未解决告警
        /// 参数:无
        /// 返回值:List<Alarm>对象JSON
        /// </summary>
        GetUnresolvedAlarms,

        /// <summary>
        /// 获取所有告警信息
        /// 参数:SearchAlarm对象JSON
        /// 返回值:List<Alarm>对象JSON
        /// </summary>
        GetAllAlarmInfo,

        /// <summary>
        /// 获取所有手动录像信息
        /// 参数:SearchRecord对象JSON
        /// 返回值:List<Record>对象JSON
        /// </summary>
        GetAllManualVideoInfo,

        /// <summary>
        /// 获取所有告警录像信息
        /// 参数:SearchRecord对象JSON
        /// 返回值:List<Record>对象JSON
        /// </summary>
        GetAllAlarmVideoInfo,

        /// <summary>
        /// 获取选区温度曲线
        /// 参数:SearchCurve对象JSON
        /// 返回值:List<SelectionTempCurve>对象JSON
        /// </summary>
        GetSelectionTempCurve,

        /// <summary>
        /// 获取组选区温度曲线
        /// 参数:SearchCurve对象JSON
        /// 返回值:List<GroupSelectionTempCurve>对象JSON
        /// </summary>
        GetGroupSelectionTempCurve,

        /// <summary>
        /// 红外自动调焦
        /// 参数:无
        /// 返回值:无
        /// </summary>
        IRCameraAutoFocus,

        /// <summary>
        /// 红外远焦
        /// 参数:无
        /// 返回值:无
        /// </summary>
        IRCameraFocusFar,

        /// <summary>
        /// 红外近焦
        /// 参数:无
        /// 返回值:无
        /// </summary>
        IRCameraFocusNear,

        /// <summary>
        /// 删除告警信息
        /// 参数:RemoveAlarm对象JSON
        /// 返回值:无
        /// </summary>
        RemoveAlarmInfo,

        /// <summary>
        /// 删除告警录像
        /// 参数:RemoveRecord对象JSON
        /// 返回值:无
        /// </summary>
        RemoveAlarmVideo,

        /// <summary>
        /// 删除手动录像 
        /// 参数:RemoveRecord对象JSON
        /// 返回值:无
        /// </summary>
        RemoveManaulVideo,

        /// <summary>
        /// 获取告警报表
        /// 参数:获取条件
        /// 返回值:
        /// </summary>
        GetAlarmReport,

        /// <summary>
        /// 获取分析报表
        /// 参数:获取条件
        /// 返回值:
        /// </summary>
        GetAnalysisReport,

        /// <summary>
        /// 获取定时报表
        /// 参数:获取条件
        /// </summary>
        GetRegularTimeReport,

        GetReplayInfo,

        /// <summary>
        /// 回放
        /// </summary>
        PlayReplay,

        /// <summary>
        /// 暂停
        /// </summary>
        PauseReplay,

        /// <summary>
        /// 继续播放
        /// </summary>
        ResumeReplay,

        /// <summary>
        /// 跳帧
        /// </summary>
        SkipReplay,

        /// <summary>
        /// 停止
        /// </summary>
        StopReplay,

        /// <summary>
        /// 加速
        /// </summary>
        SpeedUp,

        /// <summary>
        /// 减速
        /// </summary>
        SpeedDown,

        /// <summary>
        /// 获取下载文件列表
        /// </summary>
        GetDownloadFiles,

        /// <summary>
        /// 获取Info数据
        /// </summary>
        GetDownloadInfoData,

        /// <summary>
        /// 获取帧数据
        /// </summary>
        GetDownloadFrameData,

        /// <summary>
        /// 获取二次解析Image
        /// </summary>
        GetSecondAnalysisImage,

        /// <summary>
        /// 获取普通Image
        /// </summary>
        GetCommonImage,

        /// <summary>
        /// 远程关机
        /// </summary>
        RemoteShutdown,

        /// <summary>
        /// 注册设备
        /// </summary>
        RegisterDevice,

        /// <summary>
        /// 发送短信
        /// </summary>
        SendSMS,

        /// <summary>
        /// 发送回复的短信
        /// </summary>
        SendReplySMS
    }

    [DataContract]
    public class Command
    {
        [DataMember(Name = "SessionID")]
        public Int64 mSessionID = DateTime.Now.Ticks;

        [DataMember(Name = "CellId")]
        public Int64 mCellId;

        [DataMember(Name = "Command")]
        public CommandType mCommand;

        [DataMember(Name = "CommandParam")]
        public String mCommandParam;

        /// <summary>
        /// 返回值
        /// AS_OK:返回成功
        /// E_FAIL:返回失败
        /// ACK:返回确认
        /// E_TIMEOUT:返回超时
        /// </summary>
        [DataMember(Name = "ResponeCode")]
        public String mResponeCode;

        [DataMember(Name = "DateTime")]
        public DateTime mDateTime = DateTime.Now;

        [NonSerialized]
        public Int32 mResendCount;
    }
}
