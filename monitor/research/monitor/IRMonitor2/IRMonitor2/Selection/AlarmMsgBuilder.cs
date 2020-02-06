using System;

namespace IRMonitor2
{
    public static class AlarmMsgBuilder
    {
        /// <summary>
        /// 获取告警短信内容
        /// </summary>
        public static String GetShowMessage(
           String substation,
           String deviceName,
           Int32 alarmMode,
           Int32 alarmType,
           Int32 alarmLevel,
           Single temperature,
           DateTime time)
        {
            String msg = String.Format("变电站:{0}\n", substation);
            msg += String.Format("设备:{0}\n", deviceName);

            AlarmMode mode = (AlarmMode)alarmMode;
            if (mode == AlarmMode.Selection) {
                SelectionAlarmType type = (SelectionAlarmType)alarmType;
                switch (type) {
                    case SelectionAlarmType.MaxTemp:
                        msg += "监控类型:最高温度\n";
                        break;
                    case SelectionAlarmType.MinTemp:
                        msg += "监控类型:最低温度\n";
                        break;
                    case SelectionAlarmType.AvgTemp:
                        msg += "监控类型:平均温度\n";
                        break;
                }
                msg += String.Format("温度值:{0:F2}℃\n", temperature);
            }
            else if (mode == AlarmMode.GroupSelection) {
                GroupAlarmType type = (GroupAlarmType)alarmType;
                switch (type) {
                    case GroupAlarmType.MaxTemperature:
                        msg += "监控类型:最高温度(组)\n";
                        break;
                    case GroupAlarmType.MaxTempDif:
                        msg += "监控类型:最大温差(组)\n";
                        break;
                    case GroupAlarmType.MaxTempRise:
                        msg += "监控类型:最大温升(组)\n";
                        break;
                    case GroupAlarmType.RelativeTempDif:
                        msg += "监控类型:相对温差(组)\n";
                        break;
                }

                if (type == GroupAlarmType.RelativeTempDif)
                    msg += String.Format("百分比:{0:F2}%\n", temperature);
                else
                    msg += String.Format("温度值:{0:F2}℃\n", temperature);
            }

            AlarmLevel level = (AlarmLevel)alarmLevel;
            switch (level) {
                case AlarmLevel.General:
                    msg += "告警等级:一般\n";
                    break;
                case AlarmLevel.Serious:
                    msg += "告警等级:严重\n";
                    break;
                case AlarmLevel.Critical:
                    msg += "告警等级:危急\n";
                    break;
            }

            msg += String.Format("时间:{0}", DateTime.Now.ToString("HH:mm:ss"));

            return msg;
        }

        /// <summary>
        /// 获取告警原因
        /// </summary>
        public static String GetAlarmDetail(
            String sName,
            Single alarmTemp,
            Int32 alarmMode,
            Int32 alarmType,
            Int32 alarmReason,
            Int32 alarmLevel,
            String condition)
        {
            AlarmMode mode = (AlarmMode)alarmMode;

            String type = "";
            String name = "";
            if (mode == AlarmMode.Selection) {
                name = sName + "(选区)";
                switch ((SelectionAlarmType)alarmType) {
                    case SelectionAlarmType.MaxTemp:
                        type = "最高温度告警";
                        break;
                    case SelectionAlarmType.MinTemp:
                        type = "最低温度告警";
                        break;
                    case SelectionAlarmType.AvgTemp:
                        type = "平均温度告警";
                        break;
                }
            }
            else if (mode == AlarmMode.GroupSelection) {
                name = sName + "(选区组)";
                switch ((GroupAlarmType)alarmType) {
                    case GroupAlarmType.MaxTemperature:
                        type = "最高温度告警(组)";
                        break;
                    case GroupAlarmType.MaxTempDif:
                        type = "最大温差告警(组)";
                        break;
                    case GroupAlarmType.MaxTempRise:
                        type = "最大温升告警(组)";
                        break;
                    case GroupAlarmType.RelativeTempDif:
                        type = "相对温差告警(组)";
                        break;
                }
            }

            String Reason = "";
            switch ((AlarmReason)alarmReason) {
                case AlarmReason.High:
                    Reason = "高于阈值";
                    break;
                case AlarmReason.Low:
                    Reason = "低于阈值";
                    break;
            }

            String level = "";
            switch ((AlarmLevel)alarmLevel) {
                case AlarmLevel.General:
                    level = "一般告警";
                    break;
                case AlarmLevel.Serious:
                    level = "严重告警";
                    break;
                case AlarmLevel.Critical:
                    level = "危急告警";
                    break;
            }

            String detail = String.Format("{0}|温度:{1:F2}|{2}|{3}|{4}|{5}",
                name, alarmTemp, type, Reason, level, condition);

            return detail;
        }
    }
}
