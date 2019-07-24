namespace Devices
{
    /// <summary>
    /// 设备状态
    /// </summary>
    public enum DeviceStatus
    {
        Idle = 0, // 待机
        Running, // 运行中
        Fault // 不可恢复故障中
    }
}
