
namespace Devices
{
    /// <summary>
    /// 设备类型
    /// </summary>
    public enum DeviceType
    {
        Unknown = 0, // 未知
        Camera, // 可见光摄像头
        IrCamera, // 红外热像仪
        Spot, // 点温
        Platform // 云台
    };
}
