using Common;
using Devices;
using Miscs;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace HIKVisionIrDevice
{
    public class HIKVisionIrDevice : IDevice
    {
        #region 常量

        // 解码器帧缓冲数量
        private const int FRAME_BUFFER_COUNT = 15;

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

            if (!Login(mIp, mPort, mUserName, mPassword))
                return false;

            if (!Config(irCameraChannel, mDistance, mEmissivity, mReflectedTemperature)) {
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
            Logout();

            lock (this) {
                mStatus = DeviceStatus.Idle;
            }

            return true;
        }

        public override bool Control(ControlMode mode)
        {
            switch (mode) {
                case ControlMode.AutoFocus:
                    return CHCNetSDK.NET_DVR_FocusOnePush(userId, irCameraChannel);

                case ControlMode.FocusFar:
                    if (!CHCNetSDK.NET_DVR_PTZControl_Other(userId, irCameraChannel, CHCNetSDK.FOCUS_FAR, 0))
                        return false;

                    Thread.Sleep(1000);
                    return CHCNetSDK.NET_DVR_PTZControl_Other(userId, irCameraChannel, CHCNetSDK.FOCUS_FAR, 1);

                case ControlMode.FocusNear:
                    if (!CHCNetSDK.NET_DVR_PTZControl_Other(userId, irCameraChannel, CHCNetSDK.FOCUS_NEAR, 0))
                        return false;

                    Thread.Sleep(1000);
                    return CHCNetSDK.NET_DVR_PTZControl_Other(userId, irCameraChannel, CHCNetSDK.FOCUS_NEAR, 1);

                default:
                    return false;
            }
        }

        public override DeviceType GetDeviceType()
        {
            return DeviceType.IrCamera;
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
                        dst[i] = BitConverter.ToSingle(buffer, j) * scale;
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
            IntPtr lpOutputXml = Marshal.AllocHGlobal(1024 * 1024);

            CHCNetSDK.NET_DVR_STD_ABILITY struSTDAbility = new CHCNetSDK.NET_DVR_STD_ABILITY();
            struSTDAbility.lpCondBuffer = new IntPtr(channel);
            struSTDAbility.dwCondSize = sizeof(int);

            struSTDAbility.lpOutBuffer = lpOutputXml;
            struSTDAbility.dwOutSize = 1024 * 1024;
            struSTDAbility.lpStatusBuffer = lpOutputXml;
            struSTDAbility.dwStatusSize = 1024 * 1024;

            if (!CHCNetSDK.NET_DVR_GetSTDAbility(userId, CHCNetSDK.NET_DVR_GET_THERMAL_CAPABILITIES, ref struSTDAbility)) {
                Marshal.FreeHGlobal(lpOutputXml);
                lpOutputXml = IntPtr.Zero;
                return false;
            }

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

            CHCNetSDK.NET_DVR_FOCUSMODE_CFG cfg = new CHCNetSDK.NET_DVR_FOCUSMODE_CFG();
            uint returnBytes = 0;
            int outSize = Marshal.SizeOf(cfg);
            IntPtr outBuffer = Marshal.AllocHGlobal(outSize);
            Marshal.StructureToPtr(cfg, outBuffer, false);
            if (!CHCNetSDK.NET_DVR_GetDVRConfig(userId, CHCNetSDK.NET_DVR_GET_FOCUSMODECFG, channel, outBuffer, (uint)outSize, ref returnBytes)) {
                Marshal.FreeHGlobal(outBuffer);
                outBuffer = IntPtr.Zero;
                return false;
            }

            cfg.byFocusMode = 2;
            bool ret = CHCNetSDK.NET_DVR_SetDVRConfig(userId, CHCNetSDK.NET_DVR_SET_FOCUSMODECFG, channel, outBuffer, (uint)outSize);
            Marshal.FreeHGlobal(outBuffer);
            outBuffer = IntPtr.Zero;

            return ret;
        }

        /// <summary>
        /// 启动实时播放
        /// </summary>
        /// <returns></returns>
        private bool StartRealPlay()
        {
            decoderCallback = new PlayCtrl.DECCBFUN(DecoderCallback);

            var lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO {
                hPlayWnd = IntPtr.Zero, // 预览窗口
                lChannel = irCameraChannel, // 预览的设备通道
                dwStreamType = 0, // 码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                dwLinkMode = 0, // 连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                bBlocked = true, // 0- 非阻塞取流，1- 阻塞取流
                byVideoCodingType = 1
            };

            readTemperatureDataCallback = new CHCNetSDK.REALDATACALLBACK(OnTemperatureReceived);
            temperatureRealPlayHandle = CHCNetSDK.NET_DVR_RealPlay_V40(userId, ref lpPreviewInfo, readTemperatureDataCallback, IntPtr.Zero);
            if (temperatureRealPlayHandle < 0) {
                Tracker.LogE($"NET_DVR_RealPlay_V40 failed, channel={irCameraChannel} error code={CHCNetSDK.NET_DVR_GetLastError()}");
                return false;
            }

            lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO {
                hPlayWnd = IntPtr.Zero, // 预览窗口
                lChannel = irCameraChannel, // 预览的设备通道
                dwStreamType = 0, // 码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                dwLinkMode = 0, // 连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                bBlocked = true, // 0- 非阻塞取流，1- 阻塞取流
                dwDisplayBufNum = FRAME_BUFFER_COUNT // 播放库显示缓冲区最大帧数
            };

            readIrCameraDataCallback = new CHCNetSDK.REALDATACALLBACK(OnIrCameraReceived);
            irCameraRealPlayHandle = CHCNetSDK.NET_DVR_RealPlay_V40(userId, ref lpPreviewInfo, OnIrCameraReceived, IntPtr.Zero);
            if (irCameraRealPlayHandle < 0) {
                Tracker.LogE($"NET_DVR_RealPlay_V40 failed, channel={irCameraChannel} error code={CHCNetSDK.NET_DVR_GetLastError()}");
                return false;
            }

            lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO {
                hPlayWnd = IntPtr.Zero, // 预览窗口
                lChannel = cameraChannel, // 预览的设备通道
                dwStreamType = 0, // 码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                dwLinkMode = 0, // 连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                bBlocked = true, // 0- 非阻塞取流，1- 阻塞取流
                dwDisplayBufNum = FRAME_BUFFER_COUNT // 播放库显示缓冲区最大帧数
            };

            readCameraDataCallback = new CHCNetSDK.REALDATACALLBACK(OnCameraReceived);
            cameraRealPlayHandle = CHCNetSDK.NET_DVR_RealPlay_V40(userId, ref lpPreviewInfo, readCameraDataCallback, IntPtr.Zero);
            if (cameraRealPlayHandle < 0) {
                Tracker.LogE($"NET_DVR_RealPlay_V40 failed, channel={cameraChannel} error code={CHCNetSDK.NET_DVR_GetLastError()}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 停止实时播放
        /// </summary>
        /// <returns></returns>
        private bool StopRealPlay()
        {
            return CHCNetSDK.NET_DVR_SerialStop(temperatureRealPlayHandle) || CHCNetSDK.NET_DVR_SerialStop(irCameraRealPlayHandle) || CHCNetSDK.NET_DVR_SerialStop(cameraChannel);
        }

        /// <summary>
        /// 获取人脸测温配置
        /// </summary>
        /// <param name="channel">通道</param>
        /// <returns>人脸测温配置</returns>
        private bool GetFaceThermometry(int channel)
        {
            var faceThermometry = GetConfiguration($"/ISAPI/Thermal/channels/{channel}/faceThermometry");
            Tracker.LogI(faceThermometry);

            return true;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="url">请求信令</param>
        /// <param name="configuration">配置</param>
        /// <param name="outputBufferSize">输出缓冲区大小</param>
        /// <returns>配置</returns>
        private string GetConfiguration(string url, string configuration = null, int outputBufferSize = 1024 * 1024)
        {
            IntPtr lpOutputXml = IntPtr.Zero;
            url = $"GET {url}";

            try {
                lpOutputXml = Marshal.AllocHGlobal(outputBufferSize);
                var struInput = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();
                var struOutput = new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();

                struInput.dwSize = (uint)Marshal.SizeOf(struInput);
                struInput.lpRequestUrl = url;
                struInput.dwRequestUrlLen = (uint)url.Length;
                struInput.lpInBuffer = configuration;
                struInput.dwInBufferSize = (uint)(configuration?.Length ?? 0);

                struOutput.dwSize = (uint)Marshal.SizeOf(struOutput);
                struOutput.lpOutBuffer = lpOutputXml;
                struOutput.dwOutBufferSize = (uint)outputBufferSize;

                if (!CHCNetSDK.NET_DVR_STDXMLConfig(userId, ref struInput, ref struOutput)) {
                    Tracker.LogE($"GetConfiguration fail: {CHCNetSDK.NET_DVR_GetLastError()}");
                    return null;
                }

                return Marshal.PtrToStringAnsi(struOutput.lpOutBuffer);
            }
            finally {
                if (lpOutputXml != IntPtr.Zero) {
                    Marshal.FreeHGlobal(lpOutputXml);
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
        private string SetConfiguration(string url, string configuration, int outputBufferSize = 1024 * 1024)
        {
            IntPtr lpOutputXml = IntPtr.Zero;
            url = $"PUT {url}";

            try {
                lpOutputXml = Marshal.AllocHGlobal(outputBufferSize);
                var struInput = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();
                var struOutput = new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();

                struInput.dwSize = (uint)Marshal.SizeOf(struInput);
                struInput.lpRequestUrl = url;
                struInput.dwRequestUrlLen = (uint)url.Length;
                struInput.lpInBuffer = configuration;
                struInput.dwInBufferSize = (uint)configuration.Length;

                struOutput.dwSize = (uint)Marshal.SizeOf(struOutput);
                struOutput.lpOutBuffer = lpOutputXml;
                struOutput.dwOutBufferSize = (uint)outputBufferSize;

                if (!CHCNetSDK.NET_DVR_STDXMLConfig(userId, ref struInput, ref struOutput)) {
                    Tracker.LogE($"SetConfiguration fail: {CHCNetSDK.NET_DVR_GetLastError()}");
                    return null;
                }

                return Marshal.PtrToStringAnsi(struOutput.lpOutBuffer);
            }
            finally {
                if (lpOutputXml != IntPtr.Zero) {
                    Marshal.FreeHGlobal(lpOutputXml);
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
            var st_frame_info = (CHCNetSDK.STREAM_FRAME_INFO_S)Marshal.PtrToStructure(pBuffer, typeof(CHCNetSDK.STREAM_FRAME_INFO_S));
            if (st_frame_info.u32MagicNo == 0x70827773) {
                // 检查红外摄像机参数是否一致
                if ((st_frame_info.stRTDataInfo.u32Width != irCameraParameters.temperatureWidth) || (st_frame_info.stRTDataInfo.u32Height != irCameraParameters.temperatureHeight)) {
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
            onFrameReceived(lRealHandle, dwDataType, pBuffer, dwBufSize, pUser, ref irCameraRealPlayPort);
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
            onFrameReceived(lRealHandle, dwDataType, pBuffer, dwBufSize, pUser, ref cameraRealPlayPort);
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
        void onFrameReceived(int lRealHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr pUser, ref int port)
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
                    if (!PlayCtrl.PlayM4_OpenStream(port, pBuffer, dwBufSize, 2 * 1024 * 1024)) {
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
                    for (var i = 0; i < 999; i++) {
                        if (!PlayCtrl.PlayM4_InputData(port, pBuffer, dwBufSize)) {
                            Thread.Sleep(2);
                        }
                        else {
                            break;
                        }
                    }

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

        #endregion
    }
}
