using Common;
using Devices;
using Miscs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace HIKVisionIrDevice
{
    public class HIKVisionIrDevice : IDevice
    {
        #region 常量

        // 解码器帧缓冲数量
        private const int FRAME_BUFFER_COUNT = 6;

        #endregion

        #region 成员变量

        /// <summary>
        /// 设备状态
        /// </summary>
        private DeviceStatus mStatus = DeviceStatus.Idle;

        /// <summary>
        /// IP地址
        /// </summary>
        private string mIp;

        /// <summary>
        /// 端口
        /// </summary>
        private short mPort;

        /// <summary>
        /// 用户名
        /// </summary>
        private string mUserName;

        /// <summary>
        /// 密码
        /// </summary>
        private string mPassword;

        /// <summary>
        /// 红外摄像机参数
        /// </summary>
        private Repository.Entities.Configuration.IrCameraParameters irCameraParameters;

        /// <summary>
        /// 可见光摄像机参数
        /// </summary>
        private Repository.Entities.Configuration.CameraParameters cameraParameters;

        /// <summary>
        /// 红外摄像机通道
        /// </summary>
        private int irCameraChannel;

        /// <summary>
        /// 可见光摄像机通道
        /// </summary>
        private int cameraChannel;

        /// <summary>
        /// 距离
        /// </summary>
        private float mDistance = 0.0F;

        /// <summary>
        /// 发射率
        /// </summary>
        private float mEmissivity = 0.0F;

        /// <summary>
        /// 反射温度
        /// </summary>
        private float mReflectedTemperature = 0.0F;

        /// <summary>
        /// 用户索引
        /// </summary>
        private int userId;

        /// <summary>
        /// 布防句柄
        /// </summary>
        private int alarmId = -1;

        /// <summary>
        /// 告警回调函数
        /// </summary>
        private CHCNetSDK.MSGCallBack_V31 alarmCallback;

        /// <summary>
        /// 红外摄像机温度实时播放句柄
        /// </summary>
        private int temperatureRealPlayHandle;

        /// <summary>
        /// 红外摄像机实时播放句柄
        /// </summary>
        private int irCameraRealPlayHandle;

        /// <summary>
        /// 可见光摄像机实时播放句柄
        /// </summary>
        private int cameraRealPlayHandle;

        /// <summary>
        /// 接收数据回调函数
        /// </summary>
        private CHCNetSDK.REALDATACALLBACK readTemperatureDataCallback;

        /// <summary>
        /// 接收数据回调函数
        /// </summary>
        private CHCNetSDK.REALDATACALLBACK readIrCameraDataCallback;

        /// <summary>
        /// 接收数据回调函数
        /// </summary>
        private CHCNetSDK.REALDATACALLBACK readCameraDataCallback;

        /// <summary>
        /// 红外摄像机实时播放端口
        /// </summary>
        private int irCameraRealPlayPort = -1;

        /// <summary>
        /// 可见光摄像机实时播放端口
        /// </summary>
        private int cameraRealPlayPort = -1;

        /// <summary>
        /// 解码器回调函数
        /// </summary>
        private PlayCtrl.DECCBFUN decoderCallback;

        /// <summary>
        /// 温度帧缓存
        /// </summary>
        private TripleByteBuffer temperatureBuffer;

        /// <summary>
        /// 红外图像帧缓存
        /// </summary>
        private TripleByteBuffer irImageBuffer;

        /// <summary>
        /// 图像帧缓存
        /// </summary>
        private TripleByteBuffer imageBuffer;

        /// <summary>
        /// 是否解析到温度帧头部
        /// </summary>
        private bool mHasHeader = false;

        #endregion

        ~HIKVisionIrDevice()
        {
            CHCNetSDK.NET_DVR_Cleanup();
        }

        #region 接口方法

        public override void Dispose()
        {
            CHCNetSDK.NET_DVR_Cleanup();
        }

        public override bool Initialize()
        {
            return true;
        }

        public override bool Open()
        {
            temperatureBuffer = new TripleByteBuffer(4 + irCameraParameters.temperatureWidth * irCameraParameters.temperatureHeight * sizeof(float));
            irImageBuffer = new TripleByteBuffer(irCameraParameters.width * irCameraParameters.height * 3 / 2);
            imageBuffer = new TripleByteBuffer(cameraParameters.width * cameraParameters.height * 3 / 2);
            mHasHeader = false;

            CHCNetSDK.NET_DVR_Init();

            if (!Login(mIp, mPort, mUserName, mPassword)) {
                return false;
            }

            if (!Config(irCameraChannel, mDistance, mEmissivity, mReflectedTemperature)) {
                Logout();
                return false;
            }

            if (!RegisterAlarmEvent()) {
                Logout();
                return false;
            }

            if (!StartRealPlay()) {
                Logout();
                return false;
            }

            lock (this) {
                mStatus = DeviceStatus.Running;
            }

            return true;
        }

        public override bool Close()
        {
            StopRealPlay();
            UnregisterAlarmEvent();
            Logout();

            lock (this) {
                mStatus = DeviceStatus.Idle;
            }

            return true;
        }

        public override bool Control(ControlMode mode, object data)
        {
            switch (mode) {
                case ControlMode.FocusFar: {
                    if (!CHCNetSDK.NET_DVR_PTZControl_Other(userId, irCameraChannel, CHCNetSDK.FOCUS_FAR, 0))
                        return false;

                    Thread.Sleep(1000);
                    return CHCNetSDK.NET_DVR_PTZControl_Other(userId, irCameraChannel, CHCNetSDK.FOCUS_FAR, 1);
                }

                case ControlMode.FocusNear: {
                    if (!CHCNetSDK.NET_DVR_PTZControl_Other(userId, irCameraChannel, CHCNetSDK.FOCUS_NEAR, 0))
                        return false;

                    Thread.Sleep(1000);
                    return CHCNetSDK.NET_DVR_PTZControl_Other(userId, irCameraChannel, CHCNetSDK.FOCUS_NEAR, 1);
                }

                case ControlMode.SetFaceThermometryEnabled: {
                    return SetFaceThermometryEnabled((bool)data);
                }

                default:
                    return false;
            }
        }

        public override DeviceCategory GetDeviceType()
        {
            return DeviceCategory.IrCamera;
        }

        public override DeviceStatus GetDeviceStatus()
        {
            lock (this) {
                return mStatus;
            }
        }

        public override bool Read(ReadMode mode, IntPtr dataAddr, int bufferLength)
        {
            return false;
        }

        public override bool Read(ReadMode mode, in object inData, out object outData, out int used)
        {
            used = 0;
            outData = null;

            switch (mode) {
                case ReadMode.ObjectDistance: {
                    outData = mDistance;
                    break;
                }

                case ReadMode.Emissivity: {
                    outData = mEmissivity;
                    break;
                }

                case ReadMode.ReflectedTemperature: {
                    outData = mReflectedTemperature;
                    break;
                }

                case ReadMode.TemperatureArray: {
                    var dst = (float[])inData;
                    var length = irCameraParameters.temperatureWidth * irCameraParameters.temperatureHeight;
                    if (dst.Length != length) {
                        Tracker.LogE("invalid irCameraParameters");
                        return false;
                    }

                    var buffer = temperatureBuffer.SwapReadableBuffer().ToArray();
                    var scale = BitConverter.ToInt32(buffer, 4);
                    for (int i = 0, j = 4; i < length; ++i, j += 4) {
                        dst[i] = BitConverter.ToSingle(buffer, j);
                    }

                    return true;
                }

                case ReadMode.IrImage: {
                    var dst = (byte[])inData;
                    var length = irCameraParameters.width * irCameraParameters.height * 3 / 2;
                    if (dst.Length != length) {
                        Tracker.LogE("invalid irCameraParameters");
                        return false;
                    }

                    var buffer = irImageBuffer.SwapReadableBuffer().ToArray();
                    Buffer.BlockCopy(buffer, 0, dst, 0, length);

                    return true;
                }

                case ReadMode.Image: {
                    var dst = (byte[])inData;
                    var length = cameraParameters.width * cameraParameters.height * 3 / 2;
                    if (dst.Length != length) {
                        Tracker.LogE("invalid cameraParameters");
                        return false;
                    }

                    var buffer = imageBuffer.SwapReadableBuffer().ToArray();
                    Buffer.BlockCopy(buffer, 0, dst, 0, length);

                    return true;
                }

                case ReadMode.IrCameraParameters: {
                    outData = irCameraParameters;
                    return true;
                }

                case ReadMode.CameraParameters: {
                    outData = cameraParameters;
                    return true;
                }

                case ReadMode.PaletteMode: {
                    outData = GetPaletteMode();
                    return true;
                }

                case ReadMode.FaceThermometryRegion: {
                    outData = GetFaceThermometryRegion();
                    return true;
                }

                case ReadMode.FaceThermometryBasicParameter: {
                    outData = GetFaceThermometryBasicParameter();
                    return true;
                }

                case ReadMode.BodyTemperatureCompensation: {
                    outData = GetBodyTemperatureCompensation();
                    return true;
                }

                case ReadMode.BlackBody: {
                    outData = GetBlackBody();
                    return true;
                }

                default:
                    break;
            }

            return false;
        }

        public override bool Write(WriteMode mode, object data)
        {
            switch (mode) {
                case WriteMode.URI: {
                    var dict = (data as string).ParseQueryString();
                    mIp = dict["ip"];
                    mPort = short.Parse(dict["port"]);
                    mUserName = dict["username"];
                    mPassword = dict["password"];
                    break;
                }

                case WriteMode.ObjectDistance: {
                    mDistance = (float)data;
                    if (GetDeviceStatus() == DeviceStatus.Running) {
                        return Config(irCameraChannel, mDistance, mEmissivity, mReflectedTemperature);
                    }
                    break;
                }

                case WriteMode.Emissivity: {
                    mEmissivity = (float)data;
                    if (GetDeviceStatus() == DeviceStatus.Running) {
                        return Config(irCameraChannel, mDistance, mEmissivity, mReflectedTemperature);
                    }
                    break;
                }

                case WriteMode.ReflectedTemperature: {
                    mReflectedTemperature = (float)data;
                    if (GetDeviceStatus() == DeviceStatus.Running) {
                        return Config(irCameraChannel, mDistance, mEmissivity, mReflectedTemperature);
                    }
                    break;
                }

                case WriteMode.IrCameraParameters: {
                    irCameraParameters = data as Repository.Entities.Configuration.IrCameraParameters;
                    irCameraChannel = int.Parse(irCameraParameters.uri.ParseQueryString()["channel"]);
                    return true;
                }

                case WriteMode.CameraParameters: {
                    cameraParameters = data as Repository.Entities.Configuration.CameraParameters;
                    cameraChannel = int.Parse(cameraParameters.uri.ParseQueryString()["channel"]);
                    return true;
                }

                case WriteMode.PaletteMode: {
                    return SetPaletteMode(data.ToString());
                }

                case WriteMode.FaceThermometryRegion: {
                    return SetFaceThermometryRegion(data as Dictionary<string, object>);
                }

                case WriteMode.FaceThermometryBasicParameter: {
                    return SetFaceThermometryBasicParameter(data as Dictionary<string, object>);
                }

                case WriteMode.BodyTemperatureCompensation: {
                    return SetBodyTemperatureCompensation(data as Dictionary<string, object>);
                }

                case WriteMode.BlackBody: {
                    return SetBlackBody(data as Dictionary<string, object>);
                }

                default:
                    break;
            }

            return true;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="port">端口</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        private bool Login(string ip, short port, string userName, string password)
        {
            CHCNetSDK.NET_DVR_DEVICEINFO_V30 deviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

            userId = CHCNetSDK.NET_DVR_Login_V30(ip, port, userName, password, ref deviceInfo);
            if (userId < 0) {
                Tracker.LogE("NET_DVR_Login_V30 failed, error code= " + CHCNetSDK.NET_DVR_GetLastError());
                return false;
            }

            return true;
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        private bool Logout()
        {
            return CHCNetSDK.NET_DVR_Logout(userId);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="channel">通道</param>
        /// <param name="distance">距离</param>
        /// <param name="emissivity">发射率</param>
        /// <param name="reflectedTemperature">反射温度</param>
        /// <returns></returns>
        private bool Config(int channel, float distance, float emissivity, float reflectedTemperature)
        {
            var configuration = GetConfiguration($"/ISAPI/Thermal/channels/{channel}/streamParam/capabilities");
            Tracker.LogD($"get streamParam capabilities: {configuration}");

            configuration = GetConfiguration($"/ISAPI/Thermal/channels/{channel}/streamParam");
            Tracker.LogD($"get streamParam: {configuration}");

            configuration = SetConfiguration($"/ISAPI/Thermal/channels/{channel}/streamParam", "<ThermalStreamParam version=\"2.0\" xmlns=\"http://www.isapi.org/ver20/XMLSchema\"><videoCodingType>pixel-to-pixel_thermometry_data</videoCodingType></ThermalStreamParam>");
            Tracker.LogD($"set streamParam: {configuration}");

            configuration = GetConfiguration($"/ISAPI/Thermal/channels/{channel}/thermometry/pixelToPixelParam/capabilities");
            Tracker.LogD($"get pixelToPixelParam capabilities: {configuration}");

            configuration = GetConfiguration($"/ISAPI/Thermal/channels/{channel}/thermometry/pixelToPixelParam");
            Tracker.LogD($"get pixelToPixelParam: {configuration}");

            string szPixelToPixelParamFormat =
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<PixelToPixelParam version=\"2.0\" xmlns=\"http://www.hikvision.com/ver20/XMLSchema\">" +
                "<id>1</id>" +
                "<maxFrameRate>0</maxFrameRate>" +
                "<reflectiveEnable>false</reflectiveEnable>" +
                "<reflectiveTemperature>{0:F2}</reflectiveTemperature>" +
                "<emissivity>{1:F2}</emissivity>" +
                "<distance>{2:D}</distance>" +
                "<refreshInterval>0</refreshInterval>" +
                "</PixelToPixelParam>";
            string szPixelToPixelParam = string.Format(szPixelToPixelParamFormat, reflectedTemperature, emissivity, (int)distance);
            configuration = GetConfiguration($"/ISAPI/Thermal/channels/{channel}/thermometry/pixelToPixelParam", szPixelToPixelParam);
            Tracker.LogD($"get pixelToPixelParam: {configuration}");

            return true;
        }

        /// <summary>
        /// 启用人脸检测 
        /// </summary>
        /// <param name="enabled">是否启用</param>
        /// <returns>是否成功</returns>
        public bool SetFaceThermometryEnabled(bool enabled)
        {
            try {
                var url = $"/ISAPI/Thermal/channels/{cameraChannel}/faceThermometry";
                var configuration = GetConfiguration(url);
                var doc = XDocument.Parse(configuration);
                var element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("faceThermometryEnabled"));
                element.Value = enabled.ToString().ToLower();

                var response = SetConfiguration(url, doc.ToString());
                doc = XDocument.Parse(response);
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("statusCode"));
                return element.Value.Equals("1");
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return false;
            }
        }

        /// <summary>
        /// 获取人脸测温配置规则
        /// </summary>
        /// <returns>人脸测温配置规则</returns>
        public Dictionary<string, object> GetFaceThermometryRegion()
        {
            try {
                var result = new Dictionary<string, object>();

                // 第一区域始终存在
                var region = 1;
                var url = $"/ISAPI/Thermal/channels/{cameraChannel}/faceThermometry/regions/{region}";
                var configuration = GetConfiguration(url);
                var doc = XDocument.Parse(configuration);
                var element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("sensitivity"));
                result["sensitivity"] = int.Parse(element.Value);
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("targetSpeed"));
                result["targetSpeed"] = int.Parse(element.Value);
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("alarmTemperature"));
                result["alarmTemperature"] = float.Parse(element.Value);

                var rectangle = new Rectangle();
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("Region"));
                element = element.Elements().First(e => e.Name.LocalName.Equals("RegionCoordinatesList"));
                var elements = element.Elements().Where(e => e.Name.LocalName.Equals("RegionCoordinates")).ToList();

                element = elements[3].Elements().First(e => e.Name.LocalName.Equals("positionX"));
                rectangle.X = int.Parse(element.Value);
                element = elements[3].Elements().First(e => e.Name.LocalName.Equals("positionY"));
                rectangle.Y = int.Parse(element.Value);

                element = elements[1].Elements().First(e => e.Name.LocalName.Equals("positionX"));
                rectangle.Width = int.Parse(element.Value) - rectangle.X;
                element = elements[1].Elements().First(e => e.Name.LocalName.Equals("positionY"));
                rectangle.Height = int.Parse(element.Value) - rectangle.Y;

                // 转向屏幕坐标系,归一化高度默认为1000
                rectangle.Y = 1000 - rectangle.Y;
                rectangle.Height = -rectangle.Height;
                result["rectangle"] = rectangle;

                return result;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 设置人脸测温配置规则
        /// </summary>
        /// <param name="arguments">人脸测温配置规则</param>
        /// <returns>是否成功</returns>
        public bool SetFaceThermometryRegion(Dictionary<string, object> arguments)
        {
            try {
                // 第一区域始终存在
                var region = 1;
                var url = $"/ISAPI/Thermal/channels/{cameraChannel}/faceThermometry/regions/{region}";
                var configuration = GetConfiguration(url);
                var doc = XDocument.Parse(configuration);
                var element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("sensitivity"));
                element.Value = arguments["sensitivity"].ToString().ToLower();
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("targetSpeed"));
                element.Value = arguments["targetSpeed"].ToString().ToLower();
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("alarmTemperature"));
                element.Value = arguments["alarmTemperature"].ToString().ToLower();

                // 转向笛卡尔坐标系,归一化高度默认为1000
                var rectangle = (Rectangle)arguments["rectangle"];
                rectangle.Y = 1000 - rectangle.Y;
                rectangle.Height = -rectangle.Height;

                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("Region"));
                element = element.Elements().First(e => e.Name.LocalName.Equals("RegionCoordinatesList"));
                var elements = element.Elements().Where(e => e.Name.LocalName.Equals("RegionCoordinates")).ToList();

                element = elements[0].Elements().First(e => e.Name.LocalName.Equals("positionX"));
                element.Value = rectangle.Left.ToString();
                element = elements[0].Elements().First(e => e.Name.LocalName.Equals("positionY"));
                element.Value = rectangle.Bottom.ToString();

                element = elements[1].Elements().First(e => e.Name.LocalName.Equals("positionX"));
                element.Value = rectangle.Right.ToString();
                element = elements[1].Elements().First(e => e.Name.LocalName.Equals("positionY"));
                element.Value = rectangle.Bottom.ToString();

                element = elements[2].Elements().First(e => e.Name.LocalName.Equals("positionX"));
                element.Value = rectangle.Right.ToString();
                element = elements[2].Elements().First(e => e.Name.LocalName.Equals("positionY"));
                element.Value = rectangle.Top.ToString();

                element = elements[3].Elements().First(e => e.Name.LocalName.Equals("positionX"));
                element.Value = rectangle.Left.ToString();
                element = elements[3].Elements().First(e => e.Name.LocalName.Equals("positionY"));
                element.Value = rectangle.Top.ToString();

                var response = SetConfiguration(url, doc.ToString());
                doc = XDocument.Parse(response);
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("statusCode"));
                return element.Value.Equals("1");
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return false;
            }
        }

        /// <summary>
        /// 获取人脸测温基本参数配置
        /// </summary>
        /// <returns>人脸测温基本参数配置</returns>
        public Dictionary<string, object> GetFaceThermometryBasicParameter()
        {
            try {
                var result = new Dictionary<string, object>();
                var url = $"/ISAPI/Thermal/channels/{irCameraChannel}/thermometry/basicParam";
                var configuration = GetConfiguration(url);
                var doc = XDocument.Parse(configuration);
                var element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("emissivity"));
                result["emissivity"] = float.Parse(element.Value);

                // 默认距离单位为厘米
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("distance"));
                result["distance"] = float.Parse(element.Value) / 100;

                return result;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 设置人脸测温基本参数配置
        /// </summary>
        /// <param name="arguments">人脸测温基本参数配置</param>
        /// <returns>是否成功</returns>
        public bool SetFaceThermometryBasicParameter(Dictionary<string, object> arguments)
        {
            try {
                var url = $"/ISAPI/Thermal/channels/{irCameraChannel}/thermometry/basicParam";
                var configuration = GetConfiguration(url);
                var doc = XDocument.Parse(configuration);
                var element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("emissivity"));
                element.Value = arguments["emissivity"].ToString().ToLower();

                // 默认距离单位为厘米
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("distance"));
                element.Value = ((float)arguments["distance"] * 100).ToString().ToLower();

                var response = SetConfiguration(url, doc.ToString());
                doc = XDocument.Parse(response);
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("statusCode"));
                return element.Value.Equals("1");
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return false;
            }
        }

        /// <summary>
        /// 获取体温温度补偿配置
        /// </summary>
        /// <returns>体温温度补偿配置</returns>
        public Dictionary<string, object> GetBodyTemperatureCompensation()
        {
            try {
                var result = new Dictionary<string, object>();
                var url = $"/ISAPI/Thermal/channels/{irCameraChannel}/bodyTemperatureCompensation";
                var configuration = GetConfiguration(url);
                var doc = XDocument.Parse(configuration);
                var element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("type"));
                result["type"] = element.Value;

                if (element.Value.Equals("auto")) {
                    // 自动补偿
                    element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("AutoParam"));
                    var element1 = element.Elements().First(e => e.Name.LocalName.Equals("compensationValue"));
                    result["compensationValue"] = float.Parse(element1.Value);
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("smartCorrection"));
                    result["smartCorrection"] = float.Parse(element1.Value);
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("environmentalTemperature"));
                    result["environmentalTemperature"] = float.Parse(element1.Value);
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("temperatureCompensation"));
                    result["temperatureCompensation"] = bool.Parse(element1.Value);
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("environmentalTemperatureMode"));
                    result["environmentalTemperatureMode"] = element1.Value;
                }
                else {
                    // 手动补偿
                    element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("ManualParam"));
                    var element1 = element.Elements().First(e => e.Name.LocalName.Equals("compensationValue"));
                    result["compensationValue"] = float.Parse(element1.Value);
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("smartCorrection"));
                    result["smartCorrection"] = float.Parse(element1.Value);
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("environmentalTemperature"));
                    result["environmentalTemperature"] = float.Parse(element1.Value);
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("temperatureCompensation"));
                    result["temperatureCompensation"] = bool.Parse(element1.Value);
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("environmentalTemperatureMode"));
                    result["environmentalTemperatureMode"] = element1.Value;
                }

                return result;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 设置体温温度补偿配置
        /// </summary>
        /// <param name="arguments">体温温度补偿配置</param>
        /// <returns>是否成功</returns>
        public bool SetBodyTemperatureCompensation(Dictionary<string, object> arguments)
        {
            try {
                var url = $"/ISAPI/Thermal/channels/{irCameraChannel}/bodyTemperatureCompensation";
                var configuration = GetConfiguration(url);
                var doc = XDocument.Parse(configuration);
                var element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("type"));
                element.Value = arguments["type"].ToString().ToLower();

                if (element.Value.Equals("auto")) {
                    // 自动补偿
                    element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("AutoParam"));
                    var element1 = element.Elements().First(e => e.Name.LocalName.Equals("compensationValue"));
                    element1.Value = arguments["compensationValue"].ToString().ToLower();
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("smartCorrection"));
                    element1.Value = arguments["smartCorrection"].ToString().ToLower();
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("environmentalTemperature"));
                    element1.Value = arguments["environmentalTemperature"].ToString().ToLower();
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("temperatureCompensation"));
                    element1.Value = arguments["temperatureCompensation"].ToString().ToLower();
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("environmentalTemperatureMode"));
                    element1.Value = arguments["environmentalTemperatureMode"].ToString().ToLower();
                }
                else {
                    // 手动补偿
                    element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("ManualParam"));
                    var element1 = element.Elements().First(e => e.Name.LocalName.Equals("compensationValue"));
                    element1.Value = arguments["compensationValue"].ToString().ToLower();
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("smartCorrection"));
                    element1.Value = arguments["smartCorrection"].ToString().ToLower();
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("environmentalTemperature"));
                    element1.Value = arguments["environmentalTemperature"].ToString().ToLower();
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("temperatureCompensation"));
                    element1.Value = arguments["temperatureCompensation"].ToString().ToLower();
                    element1 = element.Elements().First(e => e.Name.LocalName.Equals("environmentalTemperatureMode"));
                    element1.Value = arguments["environmentalTemperatureMode"].ToString().ToLower();
                }

                var response = SetConfiguration(url, doc.ToString());
                doc = XDocument.Parse(response);
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("statusCode"));
                return element.Value.Equals("1");
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return false;
            }
        }

        /// <summary>
        /// 获取黑体配置
        /// </summary>
        /// <returns>黑体配置</returns>
        public Dictionary<string, object> GetBlackBody()
        {
            try {
                var result = new Dictionary<string, object>();
                var url = $"/ISAPI/Thermal/channels/{irCameraChannel}/blackBody";
                var configuration = GetConfiguration(url);
                var doc = XDocument.Parse(configuration);
                var element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("enabled"));
                result["enabled"] = bool.Parse(element.Value);
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("emissivity"));
                result["emissivity"] = float.Parse(element.Value);
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("distance"));
                result["distance"] = float.Parse(element.Value);
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("temperature"));
                result["temperature"] = float.Parse(element.Value);

                var point = new Point();
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("CentrePoint"));
                element = element.Elements().First(e => e.Name.LocalName.Equals("CalibratingCoordinates"));
                var element1 = element.Elements().First(e => e.Name.LocalName.Equals("positionX"));
                point.X = int.Parse(element1.Value);
                element1 = element.Elements().First(e => e.Name.LocalName.Equals("positionY"));
                point.Y = int.Parse(element1.Value);

                // 转向屏幕坐标系,归一化高度默认为1000
                point.Y = 1000 - point.Y;
                result["point"] = point;
                return result;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 设置黑体配置
        /// </summary>
        /// <param name="arguments"黑体配置</param>
        /// <returns>是否成功</returns>
        public bool SetBlackBody(Dictionary<string, object> arguments)
        {
            try {
                var url = $"/ISAPI/Thermal/channels/{irCameraChannel}/blackBody";
                var configuration = GetConfiguration(url);
                var doc = XDocument.Parse(configuration);
                var element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("enabled"));
                element.Value = arguments["enabled"].ToString().ToLower();
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("emissivity"));
                element.Value = arguments["emissivity"].ToString().ToLower();
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("distance"));
                element.Value = arguments["distance"].ToString().ToLower();
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("temperature"));
                element.Value = arguments["temperature"].ToString().ToLower();

                // 转向笛卡尔坐标系,归一化高度默认为1000
                var point = (Point)arguments["point"];
                point.Y = 1000 - point.Y;

                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("CentrePoint"));
                element = element.Elements().First(e => e.Name.LocalName.Equals("CalibratingCoordinates"));
                var element1 = element.Elements().First(e => e.Name.LocalName.Equals("positionX"));
                element1.Value = point.X.ToString().ToLower();
                element1 = element.Elements().First(e => e.Name.LocalName.Equals("positionY"));
                element1.Value = point.Y.ToString().ToLower();

                var response = SetConfiguration(url, doc.ToString());
                doc = XDocument.Parse(response);
                element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("statusCode"));
                return element.Value.Equals("1");
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return false;
            }
        }

        /// <summary>
        /// 获取调色板模式
        /// </summary>
        /// <returns>调色板模式</returns>
        private string GetPaletteMode()
        {
            try {
                var url = $"/ISAPI/Image/channels/{irCameraChannel}/Palettes";
                var configuration = GetConfiguration(url);
                var doc = XDocument.Parse(configuration);
                var element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("mode"));
                return element.Value;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 设置调色板模式
        /// </summary>
        /// <param name="mode">调色板模式</param>
        /// <returns>是否成功</returns>
        private bool SetPaletteMode(string mode)
        {
            try {
                var url = $"/ISAPI/Image/channels/{irCameraChannel}/Palettes";
                string configuration = null;
                if (mode.Equals("WhiteHot") || mode.Equals("BlackHot")) {
                    configuration = $"<Palettes version=\"2.0\" xmlns=\"http://www.hikvision.com/ver20/XMLSchema\"><mode>{mode}</mode><ColorateTarget><ColorateTargetModeList><ColorateTargetMode><id>1</id><mode>colorateHotAreae</mode><enabled>true</enabled><TemperatureLimit><minTemperature>37.5</minTemperature></TemperatureLimit><Color><R>241</R><G>108</G><B>77</B></Color></ColorateTargetMode><ColorateTargetMode><id>2</id><mode>colorateIntervalArea</mode><enabled>false</enabled><TemperatureLimit><minTemperature>30</minTemperature><maxTemperature>30</maxTemperature></TemperatureLimit><Color><R>255</R><G>165</G><B>0</B></Color></ColorateTargetMode><ColorateTargetMode><id>3</id><mode>colorateColdArea</mode><enabled>false</enabled><TemperatureLimit><maxTemperature>30</maxTemperature></TemperatureLimit><Color><R>0</R><G>255</G><B>0</B></Color></ColorateTargetMode></ColorateTargetModeList></ColorateTarget></Palettes>";
                }
                else {
                    configuration = GetConfiguration(url);
                    var doc = XDocument.Parse(configuration);
                    var element = doc.Root.Elements().First(e => e.Name.LocalName.Equals("mode"));
                    element.Value = mode;
                    configuration = doc.ToString();
                }

                var response = SetConfiguration(url, configuration);
                var doc1 = XDocument.Parse(response);
                var element1 = doc1.Root.Elements().First(e => e.Name.LocalName.Equals("statusCode"));
                return element1.Value.Equals("1");
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return false;
            }
        }

        /// <summary>
        /// 启动实时播放
        /// </summary>
        /// <returns></returns>
        private bool StartRealPlay()
        {
            decoderCallback = new PlayCtrl.DECCBFUN(DecoderCallback);

            if ((irCameraParameters.temperatureWidth != 0) && (irCameraParameters.temperatureHeight != 0)) {
                var lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO {
                    hPlayWnd = IntPtr.Zero, // 预览窗口
                    lChannel = irCameraChannel, // 预览的设备通道
                    dwStreamType = 0, // 码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    dwLinkMode = 0, // 连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    bBlocked = false, // 0- 非阻塞取流，1- 阻塞取流
                    byVideoCodingType = 1
                };

                readTemperatureDataCallback = new CHCNetSDK.REALDATACALLBACK(OnTemperatureReceived);
                temperatureRealPlayHandle = CHCNetSDK.NET_DVR_RealPlay_V40(userId, ref lpPreviewInfo, readTemperatureDataCallback, IntPtr.Zero);
                if (temperatureRealPlayHandle < 0) {
                    Tracker.LogE($"NET_DVR_RealPlay_V40 failed, channel={irCameraChannel} error code={CHCNetSDK.NET_DVR_GetLastError()}");
                    return false;
                }
            }

            if ((irCameraParameters.width != 0) && (irCameraParameters.height != 0)) {
                var lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO {
                    hPlayWnd = IntPtr.Zero, // 预览窗口
                    lChannel = irCameraChannel, // 预览的设备通道
                    dwStreamType = 1, // 码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    dwLinkMode = 0, // 连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    bBlocked = false, // 0- 非阻塞取流，1- 阻塞取流
                    dwDisplayBufNum = FRAME_BUFFER_COUNT // 播放库显示缓冲区最大帧数
                };

                readIrCameraDataCallback = new CHCNetSDK.REALDATACALLBACK(OnIrCameraReceived);
                irCameraRealPlayHandle = CHCNetSDK.NET_DVR_RealPlay_V40(userId, ref lpPreviewInfo, readIrCameraDataCallback, IntPtr.Zero);
                if (irCameraRealPlayHandle < 0) {
                    Tracker.LogE($"NET_DVR_RealPlay_V40 failed, channel={irCameraChannel} error code={CHCNetSDK.NET_DVR_GetLastError()}");
                    return false;
                }

                // 主码流动态产生一个关键帧
                CHCNetSDK.NET_DVR_MakeKeyFrame(userId, irCameraChannel);
            }

            if ((cameraParameters.width != 0) && (cameraParameters.height != 0)) {
                var lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO {
                    hPlayWnd = IntPtr.Zero, // 预览窗口
                    lChannel = cameraChannel, // 预览的设备通道
                    dwStreamType = 1, // 码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    dwLinkMode = 0, // 连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    bBlocked = false, // 0- 非阻塞取流，1- 阻塞取流
                    dwDisplayBufNum = FRAME_BUFFER_COUNT // 播放库显示缓冲区最大帧数
                };

                readCameraDataCallback = new CHCNetSDK.REALDATACALLBACK(OnCameraReceived);
                cameraRealPlayHandle = CHCNetSDK.NET_DVR_RealPlay_V40(userId, ref lpPreviewInfo, readCameraDataCallback, IntPtr.Zero);
                if (cameraRealPlayHandle < 0) {
                    Tracker.LogE($"NET_DVR_RealPlay_V40 failed, channel={cameraChannel} error code={CHCNetSDK.NET_DVR_GetLastError()}");
                    return false;
                }

                // 主码流动态产生一个关键帧
                CHCNetSDK.NET_DVR_MakeKeyFrame(userId, cameraChannel);
            }

            return true;
        }

        /// <summary>
        /// 停止实时播放
        /// </summary>
        /// <returns></returns>
        private bool StopRealPlay()
        {
            if (!CHCNetSDK.NET_DVR_StopRealPlay(temperatureRealPlayHandle)) {
                Tracker.LogE($"NET_DVR_RealPlay_V40 failed, channel={irCameraChannel} error code={CHCNetSDK.NET_DVR_GetLastError()}");
            }

            if (!CHCNetSDK.NET_DVR_StopRealPlay(irCameraRealPlayHandle)) {
                Tracker.LogE($"NET_DVR_RealPlay_V40 failed, channel={irCameraChannel} error code={CHCNetSDK.NET_DVR_GetLastError()}");
            }

            if (!CHCNetSDK.NET_DVR_StopRealPlay(cameraRealPlayHandle)) {
                Tracker.LogE($"NET_DVR_RealPlay_V40 failed, channel={irCameraChannel} error code={CHCNetSDK.NET_DVR_GetLastError()}");
            }

            if (!PlayCtrl.PlayM4_Stop(irCameraRealPlayPort)) {
                Tracker.LogE($"PlayM4_Stop failed, port={irCameraRealPlayPort} error code={PlayCtrl.PlayM4_GetLastError(irCameraRealPlayPort)}");
            }

            if (!PlayCtrl.PlayM4_CloseStream(irCameraRealPlayPort)) {
                Tracker.LogE($"PlayM4_CloseStream failed, port={irCameraRealPlayPort} error code={PlayCtrl.PlayM4_GetLastError(irCameraRealPlayPort)}");
            }

            if (!PlayCtrl.PlayM4_FreePort(irCameraRealPlayPort)) {
                Tracker.LogE($"PlayM4_FreePort failed, port={irCameraRealPlayPort} error code={PlayCtrl.PlayM4_GetLastError(irCameraRealPlayPort)}");
            }

            if (!PlayCtrl.PlayM4_Stop(cameraRealPlayPort)) {
                Tracker.LogE($"PlayM4_Stop failed, port={cameraRealPlayPort} error code={PlayCtrl.PlayM4_GetLastError(cameraRealPlayPort)}");
            }

            if (!PlayCtrl.PlayM4_CloseStream(cameraRealPlayPort)) {
                Tracker.LogE($"PlayM4_CloseStream failed, port={cameraRealPlayPort} error code={PlayCtrl.PlayM4_GetLastError(cameraRealPlayPort)}");
            }

            if (!PlayCtrl.PlayM4_FreePort(cameraRealPlayPort)) {
                Tracker.LogE($"PlayM4_FreePort failed, port={cameraRealPlayPort} error code={PlayCtrl.PlayM4_GetLastError(cameraRealPlayPort)}");
            }

            return true;
        }

        /// <summary>
        /// 开启布防
        /// </summary>
        /// <returns>是否成功</returns>
        private bool RegisterAlarmEvent()
        {
            var alarmId = CHCNetSDK.NET_DVR_SetupAlarmChan_V30(userId);
            if (alarmId < 0) {
                return false;
            }

            // 设置报警回调函数
            alarmCallback = new CHCNetSDK.MSGCallBack_V31(OnAlarm);
            if (!CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V31(alarmCallback, IntPtr.Zero)) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 撤销布防
        /// </summary>
        /// <returns>是否成功</returns>
        private bool UnregisterAlarmEvent()
        {
            if (alarmId >= 0) {
                if (!CHCNetSDK.NET_DVR_CloseAlarmChan_V30(alarmId)) {
                    return false;
                }

                alarmId = -1;
            }

            return true;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="url">请求信令</param>
        /// <param name="configuration">配置</param>
        /// <param name="outputBufferSize">输出缓冲区大小</param>
        /// <returns>配置</returns>
        private string GetConfiguration(string url, string configuration = null, int outputBufferSize = 3 * 1024 * 1024)
        {
            IntPtr lpUrl = IntPtr.Zero;
            IntPtr lpInBuffer = IntPtr.Zero;
            IntPtr lpOutputXml = IntPtr.Zero;
            IntPtr lpStatusBuffer = IntPtr.Zero;
            url = $"GET {url}";

            try {
                lpUrl = Marshal.StringToHGlobalAnsi(url);
                lpInBuffer = Marshal.StringToHGlobalAnsi(configuration);
                lpOutputXml = Marshal.AllocHGlobal(outputBufferSize);
                lpStatusBuffer = Marshal.AllocHGlobal(4096 * 4);

                var struInput = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();
                var struOutput = new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();

                struInput.dwSize = (uint)Marshal.SizeOf(struInput);
                struInput.lpRequestUrl = lpUrl;
                struInput.dwRequestUrlLen = (uint)url.Length;
                struInput.lpInBuffer = lpInBuffer;
                struInput.dwInBufferSize = (uint)(configuration?.Length ?? 0);

                struOutput.dwSize = (uint)Marshal.SizeOf(struOutput);
                struOutput.lpOutBuffer = lpOutputXml;
                struOutput.dwOutBufferSize = (uint)outputBufferSize;
                struOutput.lpStatusBuffer = lpStatusBuffer;
                struOutput.dwStatusSize = 4096 * 4;

                if (!CHCNetSDK.NET_DVR_STDXMLConfig(userId, ref struInput, ref struOutput)) {
                    Tracker.LogE($"GetConfiguration fail: {CHCNetSDK.NET_DVR_GetLastError()}");
                    return null;
                }

                var response = Marshal.PtrToStringAnsi(struOutput.lpOutBuffer);
                return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(response));
            }
            finally {
                if (lpUrl != IntPtr.Zero) {
                    Marshal.FreeHGlobal(lpUrl);
                }

                if (lpInBuffer != IntPtr.Zero) {
                    Marshal.FreeHGlobal(lpInBuffer);
                }

                if (lpOutputXml != IntPtr.Zero) {
                    Marshal.FreeHGlobal(lpOutputXml);
                }

                if (lpStatusBuffer != IntPtr.Zero) {
                    Marshal.FreeHGlobal(lpStatusBuffer);
                }
            }
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="url">请求信令</param>
        /// <param name="configuration">配置</param>
        /// <param name="outputBufferSize">输出缓冲区大小</param>
        /// <returns>配置</returns>
        private string SetConfiguration(string url, string configuration, int outputBufferSize = 3 * 1024 * 1024)
        {
            IntPtr lpUrl = IntPtr.Zero;
            IntPtr lpInBuffer = IntPtr.Zero;
            IntPtr lpOutputXml = IntPtr.Zero;
            IntPtr lpStatusBuffer = IntPtr.Zero;
            url = $"PUT {url}";

            try {
                lpUrl = Marshal.StringToHGlobalAnsi(url);
                lpInBuffer = Marshal.StringToHGlobalAnsi(configuration);
                lpOutputXml = Marshal.AllocHGlobal(outputBufferSize);
                lpStatusBuffer = Marshal.AllocHGlobal(4096 * 4);

                var struInput = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();
                var struOutput = new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();

                struInput.dwSize = (uint)Marshal.SizeOf(struInput);
                struInput.lpRequestUrl = lpUrl;
                struInput.dwRequestUrlLen = (uint)url.Length;
                struInput.lpInBuffer = lpInBuffer;
                struInput.dwInBufferSize = (uint)configuration.Length;

                struOutput.dwSize = (uint)Marshal.SizeOf(struOutput);
                struOutput.lpOutBuffer = lpOutputXml;
                struOutput.dwOutBufferSize = (uint)outputBufferSize;
                struOutput.lpStatusBuffer = lpStatusBuffer;
                struOutput.dwStatusSize = 4096 * 4;

                if (!CHCNetSDK.NET_DVR_STDXMLConfig(userId, ref struInput, ref struOutput)) {
                    Tracker.LogE($"SetConfiguration fail: {CHCNetSDK.NET_DVR_GetLastError()}");
                    return null;
                }

                var response = Marshal.PtrToStringAnsi(struOutput.lpStatusBuffer);
                return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(response));
            }
            finally {
                if (lpUrl != IntPtr.Zero) {
                    Marshal.FreeHGlobal(lpUrl);
                }

                if (lpInBuffer != IntPtr.Zero) {
                    Marshal.FreeHGlobal(lpInBuffer);
                }

                if (lpOutputXml != IntPtr.Zero) {
                    Marshal.FreeHGlobal(lpOutputXml);
                }

                if (lpStatusBuffer != IntPtr.Zero) {
                    Marshal.FreeHGlobal(lpStatusBuffer);
                }
            }
        }

        /// <summary>
        /// 接收红外摄像机温度数据回调函数
        /// </summary>
        /// <param name="lRealHandle">句柄</param>
        /// <param name="dwDataType">数据类型</param>
        /// <param name="pBuffer">数据</param>
        /// <param name="dwBufSize">大小</param>
        /// <param name="pUser">用户数据</param>
        void OnTemperatureReceived(int lRealHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr pUser)
        {
            if ((dwDataType != CHCNetSDK.NET_DVR_STREAMDATA) || (dwBufSize <= 0)) {
                return;
            }

            var hasHeader = false;
            var headerSize = 0;
            var st_frame_info = (CHCNetSDK._STREAM_FRAME_INFO_S_)Marshal.PtrToStructure(pBuffer, typeof(CHCNetSDK._STREAM_FRAME_INFO_S_));
            if (st_frame_info.u32MagicNo == 0x70827773) {
                // 检查红外摄像机参数是否一致
                if ((st_frame_info.DataInfo.stRTDataInfo.u32Width != irCameraParameters.temperatureWidth) || (st_frame_info.DataInfo.stRTDataInfo.u32Height != irCameraParameters.temperatureHeight)) {
                    Tracker.LogE("invalid irCameraParameters");
                    return;
                }
                headerSize = (int)st_frame_info.u32HeaderSize;
                mHasHeader = hasHeader = true;
            }

            var buffer = temperatureBuffer.GetWritableBuffer();
            var length = (int)dwBufSize - headerSize;
            if (((!mHasHeader) && (!hasHeader)) || ((buffer.Used + length) > buffer.Capacity)) {
                buffer.Reset();
                mHasHeader = false;
                return;
            }

            buffer.Push(pBuffer + headerSize, length);
            if (buffer.IsFull()) {
                temperatureBuffer.SwapWritableBuffer();
                mHasHeader = false;
            }
        }

        /// <summary>
        /// 接收红外摄像机数据回调函数
        /// </summary>
        /// <param name="lRealHandle">句柄</param>
        /// <param name="dwDataType">数据类型</param>
        /// <param name="pBuffer">数据</param>
        /// <param name="dwBufSize">大小</param>
        /// <param name="pUser">用户数据</param>
        void OnIrCameraReceived(int lRealHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr pUser)
        {
            OnFrameReceived(lRealHandle, dwDataType, pBuffer, dwBufSize, pUser, ref irCameraRealPlayPort);
        }

        /// <summary>
        /// 接收可见光摄像机数据回调函数
        /// </summary>
        /// <param name="lRealHandle">句柄</param>
        /// <param name="dwDataType">数据类型</param>
        /// <param name="pBuffer">数据</param>
        /// <param name="dwBufSize">大小</param>
        /// <param name="pUser">用户数据</param>
        void OnCameraReceived(int lRealHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr pUser)
        {
            OnFrameReceived(lRealHandle, dwDataType, pBuffer, dwBufSize, pUser, ref cameraRealPlayPort);
        }

        /// <summary>
        /// 接收数据回调函数
        /// </summary>
        /// <param name="lRealHandle">句柄</param>
        /// <param name="dwDataType">数据类型</param>
        /// <param name="pBuffer">数据</param>
        /// <param name="dwBufSize">大小</param>
        /// <param name="pUser">用户数据</param>
        /// <param name="port">实时播放端口</param>
        void OnFrameReceived(int lRealHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr pUser, ref int port)
        {
            switch (dwDataType) {
                case CHCNetSDK.NET_DVR_SYSHEAD: {
                    if (dwBufSize <= 0) {
                        break;
                    }

                    // 同一路码流不需要多次调用开流接口
                    if (port >= 0) {
                        break;
                    }

                    // 获取播放句柄
                    if (!PlayCtrl.PlayM4_GetPort(ref port)) {
                        Tracker.LogE($"PlayM4_GetPort fail: {PlayCtrl.PlayM4_GetLastError(port)}");
                        break;
                    }

                    // 设置流播放模式
                    if (!PlayCtrl.PlayM4_SetStreamOpenMode(port, PlayCtrl.STREAME_REALTIME)) {
                        Tracker.LogE($"Set STREAME_REALTIME mode fail: {PlayCtrl.PlayM4_GetLastError(port)}");
                        break;
                    }

                    // 打开码流,送入头数据
                    if (!PlayCtrl.PlayM4_OpenStream(port, pBuffer, dwBufSize, 16 * 1024 * 1024)) {
                        Tracker.LogE($"PlayM4_OpenStream fail: {PlayCtrl.PlayM4_GetLastError(port)}");
                        break;
                    }

                    // 设置显示缓冲区个数
                    if (!PlayCtrl.PlayM4_SetDisplayBuf(port, FRAME_BUFFER_COUNT)) {
                        Tracker.LogE($"PlayM4_SetDisplayBuf fail: {PlayCtrl.PlayM4_GetLastError(port)}");
                        break;
                    }

                    // 设置显示模式
                    if (!PlayCtrl.PlayM4_SetOverlayMode(port, 0, 0)) {
                        Tracker.LogE($"PlayM4_SetOverlayMode fail: {PlayCtrl.PlayM4_GetLastError(port)}");
                        break;
                    }

                    // 设置解码回调函数
                    if (!PlayCtrl.PlayM4_SetDecCallBackEx(port, decoderCallback, IntPtr.Zero, 0)) {
                        Tracker.LogE($"PlayM4_SetDecCallBackEx fail: {PlayCtrl.PlayM4_GetLastError(port)}");
                        break;
                    }

                    // 开始解码
                    if (!PlayCtrl.PlayM4_Play(port, IntPtr.Zero)) {
                        Tracker.LogE($"PlayM4_Play fail: {PlayCtrl.PlayM4_GetLastError(port)}");
                        break;
                    }

                    break;
                }

                case CHCNetSDK.NET_DVR_STREAMDATA: {
                    if (dwBufSize <= 0) {
                        break;
                    }

                    if (port < 0) {
                        break;
                    }

                    // 送入其他数据
                    PlayCtrl.PlayM4_InputData(port, pBuffer, dwBufSize);

                    break;
                }

                default:
                    break;
            }
        }

        private void DecoderCallback(int nPort, IntPtr pBuf, int nSize, ref PlayCtrl.FRAME_INFO pFrameInfo, int nReserved1, int nReserved2)
        {
            if (nPort == irCameraRealPlayPort) {
                var length = irCameraParameters.width * irCameraParameters.height * 3 / 2;
                if (nSize != length) {
                    Tracker.LogE("invalid irCameraParameters");
                    return;
                }

                irImageBuffer.GetWritableBuffer().Push(pBuf, length);
                irImageBuffer.SwapWritableBuffer();
            }
            else if (nPort == cameraRealPlayPort) {
                var length = cameraParameters.width * cameraParameters.height * 3 / 2;
                if (nSize != length) {
                    Tracker.LogE("invalid cameraParameters");
                    return;
                }

                imageBuffer.GetWritableBuffer().Push(pBuf, length);
                imageBuffer.SwapWritableBuffer();
            }
        }

        private bool OnAlarm(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            // 判断报警是否为温度异常
            if (lCommand != 0x5212) {
                return false;
            }

            var struThermAlarm = (CHCNetSDK.NET_DVR_THERMOMETRY_ALARM)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_DVR_THERMOMETRY_ALARM));
            RectangleF rect = new RectangleF(struThermAlarm.struRegion.struPos[0].fX, struThermAlarm.struRegion.struPos[0].fY, struThermAlarm.struRegion.struPos[1].fX - struThermAlarm.struRegion.struPos[0].fX, struThermAlarm.struRegion.struPos[2].fY - struThermAlarm.struRegion.struPos[0].fY);
            RaiseEvent(DeviceEvent.HumanHighTemperatureAlarm, rect, struThermAlarm.fCurrTemperature);

            return true;
        }

        #endregion
    }
}
