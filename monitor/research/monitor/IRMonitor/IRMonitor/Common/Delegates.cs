using Common;
using IRMonitor.Common;
using System;
using System.Collections.Generic;

namespace IRMonitor
{
    /// <summary>
    /// 代理声明仓库
    /// </summary>
    public static class Delegates
    {
        /// <summary>
        ///  添加告警
        /// </summary>
        public delegate ARESULT DgOnAddAlarmInfo(
            Int32 alarmMode,
            Int32 alarmType,
            Int32 alarmReason,
            Int32 alarmLevel,
            String alarmCondition,
            Single alarmTemperature,
            Int64 selectionId,
            String selectionName,
            String selectionData,
            String temperatureInfo,
            Single maxTemperature,
            Single minTemperature,
            List<Selection> selections,
            AlarmInfo alarmInfo);

        /// <summary>
        /// 更新告警
        /// </summary>
        public delegate ARESULT DgOnUpdateAlarmInfo(
            Int32 alarmMode,
            Int32 alarmType,
            Int64 selectionId,
            String selectionData,
            AlarmInfo alarmInfo);

        /// <summary>
        /// 告警超时
        /// </summary>
        public delegate ARESULT DgOnAlarmTimeout(
            Int32 alarmMode,
            Int32 alarmType,
            Int64 selectionId,
            String selectionData,
            AlarmInfo alarmInfo);

        /// <summary>
        ///  图像数据回调
        /// </summary>
        public delegate void DgOnImageCallback(Byte[] buffer);

        /// <summary>
        ///  温度数据回调
        /// </summary>
        public delegate void DgOnTemperatureCallback(Single[] buffer);

        /// <summary>
        /// 设备连接失败回调
        /// </summary>
        public delegate void DgOnIrDisconnected();

        /// <summary>
        /// 添加录像信息
        /// </summary>
        public delegate ARESULT DgOnAddRecordInfo(
            String videoFileName,
            String selectionFileName);

        /// <summary>
        /// 接收到短信
        /// </summary>
        public delegate void DgOnAutoReplyShortMessage(String phone);

        /// <summary>
        /// 平台短信
        /// </summary>
        public delegate void DgOnSendShortMessage(ShortMessageData messageData);

        /// <summary>
        /// 添加普通选区温度信息
        /// </summary>
        public delegate ARESULT DgOnAddSelectionTemperature(
            Int64 selectionId,
            DateTime time,
            Single maxTemperature,
            Single minTemperature,
            Single avgTemperature);

        /// <summary>
        /// 添加选区组温度信息
        /// </summary>
        public delegate ARESULT DgOnAddGroupSelectionTemperature(
            Int64 groupSelectionId,
            DateTime time,
            Single maxTemperature,
            Single temperaturedifference,
            Single temperaturerise);

        /// <summary>
        /// 选区温度数据回调
        /// </summary>
        public delegate void DgOnRealtimeSelectionTemperature(
            String allSelectionData,
            String allGroupData);

        /// <summary>
        /// 整点发送
        /// </summary>
        public delegate ARESULT DgOnHourSend();

        /// <summary>
        /// 定时发送
        /// </summary>
        public delegate ARESULT DgOnRegularSend();

        /// <summary>
        /// 回放完成
        /// </summary>
        public delegate void DgOnReplayCompleted();

        /// <summary>
        /// 回放图像数据回调
        /// </summary>
        /// <param name="seconds">时间戳</param>
        /// <param name="IsRender">是否渲染</param>
        /// <param name="buf">数据</param>
        public delegate void DgOnReplayImageCallback(Double seconds, Boolean IsRender, Byte[] buf);

        /// <summary>
        /// 回放选区数据回调
        /// </summary>
        public delegate void DgOnReplaySelectionCallback(String selection);

        public delegate void DgOnReportDataCallback(ReportData reportData);

        public delegate void DgOnGetTempCurve(TempCurve curve);

        /// <summary>
        /// 定时发送
        /// </summary>
        public delegate void DgOnSendReportData(Int64 selectionId);

        /// <summary>
        /// 实时温度信息
        /// </summary>
        public delegate void DgOnSendRealTemperature(
            DateTime time,
            Single maxTemperature,
            Single minTemperature,
            Single avgTemperature);
    }
}
