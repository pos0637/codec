
namespace Devices
{
    /// <summary>
    /// 控制类型
    /// </summary>
    public enum ControlMode
    {
        AutoFocus, // 红外热像仪专用
        FocusFar, // 调远焦
        FocusNear // 调近焦
    };

    /// <summary>
    /// 写类型
    /// </summary>
    public enum WriteMode
    {
        URI = 0, // 资源地址
        FrameRate, // 帧率
        AtmosphericTemperature, // 大气温度
        RelativeHumidity, // 相对湿度
        ReflectedTemperature, // 相对温度
        ObjectDistance, // 距离
        Emissivity, // 辐射率
        Transmission, // 透过率
        IrCameraParameters, // 红外摄像机参数
        CameraParameters // 可见光摄像机参数
    };

    /// <summary>
    /// 读类型
    /// </summary>
    public enum ReadMode
    {
        TemperatureArray = 0, // 温度矩阵
        IrImage, // 红外图像
        Image, // 可见光图像
        FrameRate, // 帧率
        AtmosphericTemperature, // 大气温度
        RelativeHumidity, // 相对湿度
        ReflectedTemperature, // 翻转表面温度
        ObjectDistance, // 距离
        Emissivity, // 辐射率
        Transmission, // 透过率
        IrCameraParameters, // 红外摄像机参数
        CameraParameters // 可见光摄像机参数
    };
}
