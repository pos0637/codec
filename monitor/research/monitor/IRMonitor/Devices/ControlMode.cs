
namespace Devices
{
    /// <summary>
    /// 控制类型
    /// </summary>
    public enum ControlMode
    {
        AutoFocus, // 红外热像仪专用
        FocusFar, // 调远焦
        FocusNear, // 调近焦
    };

    /// <summary>
    /// 写类型
    /// </summary>
    public enum WriteMode
    {
        ConnectionString = 0,
        FrameRate, // 设置帧率
        AtmosphericTemperature, // 红外热像仪专用 设置大气温度
        RelativeHumidity, // 红外热像仪专用 设置相对湿度
        ReflectedTemperature, // 红外热像仪专用 设置相对温度
        ObjectDistance, // 红外热像仪专用 设置距离
        Emissivity, // 红外热像仪专用 设置辐射率
        Transmission, // 红外热像仪专用 设置透过率
        SetIrCameraParametersToDevice
    };

    /// <summary>
    /// 读类型
    /// </summary>
    public enum ReadMode
    {
        TemperatureArray = 0, // 温度矩阵
        ImageArray, // 图像矩阵
        FrameRate,
        AtmosphericTemperature, // 读取大气温度
        RelativeHumidity, // 读取相对湿度
        ReflectedTemperature, // 读取翻转表面温度
        ObjectDistance, // 读取距离
        Emissivity, // 读取辐射率
        Transmission, // 读取透过率
        Parameters // 读取参数
    };
}
