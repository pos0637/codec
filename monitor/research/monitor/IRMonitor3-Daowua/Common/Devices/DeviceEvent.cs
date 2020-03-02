namespace Devices
{
    /// <summary>
    /// 设备事件
    /// </summary>
    public enum DeviceEvent
    {
        Alarm = 0, // 告警
        HumanHighTemperatureAlarm, // 人体体温超温告警
        Fault // 故障
    }
}
