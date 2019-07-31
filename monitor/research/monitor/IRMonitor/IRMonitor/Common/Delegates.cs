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
            int alarmMode,
            int alarmType,
            int alarmReason,
            int alarmLevel,
            string alarmCondition,
            float alarmTemperature,
            long selectionId,
            string selectionName,
            string selectionData,
            string temperatureInfo,
            float maxTemperature,
            float minTemperature,
            List<Selection> selections,
            AlarmInfo alarmInfo);

        /// <summary>
        /// 更新告警
        /// </summary>
        public delegate ARESULT DgOnUpdateAlarmInfo(
            int alarmMode,
            int alarmType,
            long selectionId,
            string selectionData,
            AlarmInfo alarmInfo);

        /// <summary>
        /// 告警超时
        /// </summary>
        public delegate ARESULT DgOnAlarmTimeout(
            int alarmMode,
            int alarmType,
            long selectionId,
            string selectionData,
            AlarmInfo alarmInfo);

        /// <summary>
        ///  图像数据回调
        /// </summary>
        public delegate void DgOnImageCallback(byte[] data);

        /// <summary>
        ///  温度数据回调
        /// </summary>
        public delegate void DgOnTemperatureCallback(float[] data);

        /// <summary>
        /// 设备连接失败回调
        /// </summary>
        public delegate void DgOnIrDisconnected();

        /// <summary>
        /// 添加录像信息
        /// </summary>
        public delegate ARESULT DgOnAddRecordInfo(
            string videoFileName,
            string selectionFileName);

        /// <summary>
        /// 接收到短信
        /// </summary>
        public delegate void DgOnAutoReplyShortMessage(string phone);

        /// <summary>
        /// 平台短信
        /// </summary>
        public delegate void DgOnSendShortMessage(ShortMessageData messageData);

        /// <summary>
        /// 添加普通选区温度信息
        /// </summary>
        public delegate ARESULT DgOnAddSelectionTemperature(
            long selectionId,
            DateTime time,
            float maxTemperature,
            float minTemperature,
            float avgTemperature);

        /// <summary>
        /// 添加选区组温度信息
        /// </summary>
        public delegate ARESULT DgOnAddGroupSelectionTemperature(
            long groupSelectionId,
            DateTime time,
            float maxTemperature,
            float temperaturedifference,
            float temperaturerise);

        /// <summary>
        /// 选区温度数据回调
        /// </summary>
        public delegate void DgOnRealtimeSelectionTemperature(
            string allSelectionData,
            string allGroupData);

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
        /// <param name="data">数据</param>
        public delegate void DgOnReplayImageCallback(double seconds, bool IsRender, byte[] data);

        /// <summary>
        /// 回放选区数据回调
        /// </summary>
        public delegate void DgOnReplaySelectionCallback(string selection);

        /// <summary>
        /// 报表数据回调
        /// </summary>
        /// <param name="reportData">报表数据</param>
        public delegate void DgOnReportDataCallback(ReportData reportData);

        /// <summary>
        /// 温度曲线回调
        /// </summary>
        /// <param name="curve">温度曲线</param>
        public delegate void DgOnGetTempCurve(TempCurve curve);

        /// <summary>
        /// 定时发送
        /// </summary>
        public delegate void DgOnSendReportData(long selectionId);

        /// <summary>
        /// 实时温度信息
        /// </summary>
        public delegate void DgOnSendRealTemperature(
            DateTime time,
            float maxTemperature,
            float minTemperature,
            float avgTemperature);
    }
}
