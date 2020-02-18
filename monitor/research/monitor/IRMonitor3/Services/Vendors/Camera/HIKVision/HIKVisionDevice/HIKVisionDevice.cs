using Common;
using Devices;
using Miscs;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace HIKVisionDevice
{
    public class HIKVisionDevice : IDevice
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
        /// 可见光摄像机参数
        /// </summary>
        private Repository.Entities.Configuration.CameraParameters cameraParameters;

        /// <summary>
        /// 可见光摄像机通道
        /// </summary>
        private int cameraChannel;

        /// <summary>
        /// 用户索引
        /// </summary>
        private int userId;

        /// <summary>
        /// 可见光摄像机实时播放句柄
        /// </summary>
        private int cameraRealPlayHandle;

        /// <summary>
        /// 接收数据回调函数
        /// </summary>
        private CHCNetSDK.REALDATACALLBACK readDataCallback;

        /// <summary>
        /// 可见光摄像机实时播放端口
        /// </summary>
        private int cameraRealPlayPort = -1;

        /// <summary>
        /// 解码器回调函数
        /// </summary>
        private PlayCtrl.DECCBFUN decoderCallback;

        /// <summary>
        /// 图像帧缓存
        /// </summary>
        private TripleByteBuffer imageBuffer;

        #endregion

        ~HIKVisionDevice()
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
            imageBuffer = new TripleByteBuffer(cameraParameters.width * cameraParameters.height * 3 / 2);

            CHCNetSDK.NET_DVR_Init();

            if (!Login(mIp, mPort, mUserName, mPassword)) {
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
            return false;
        }

        public override DeviceType GetDeviceType()
        {
            return DeviceType.Camera;
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
        /// 启动实时播放
        /// </summary>
        /// <returns></returns>
        private bool StartRealPlay()
        {
            var lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO {
                hPlayWnd = IntPtr.Zero, // 预览窗口
                lChannel = cameraChannel, // 预览的设备通道
                dwStreamType = 0, // 码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                dwLinkMode = 0, // 连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                bBlocked = true, // 0- 非阻塞取流，1- 阻塞取流
                dwDisplayBufNum = FRAME_BUFFER_COUNT // 播放库显示缓冲区最大帧数
            };

            readDataCallback = new CHCNetSDK.REALDATACALLBACK((int lRealHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr pUser) => {
                onFrameReceived(lRealHandle, dwDataType, pBuffer, dwBufSize, pUser, ref cameraRealPlayPort);
            });

            decoderCallback = new PlayCtrl.DECCBFUN((int nPort, IntPtr pBuf, int nSize, ref PlayCtrl.FRAME_INFO pFrameInfo, int nReserved1, int nReserved2) => {
                if (nPort == cameraRealPlayPort) {
                    var length = cameraParameters.width * cameraParameters.height * 3 / 2;
                    if (nSize != length) {
                        Tracker.LogE("invalid cameraParameters");
                        return;
                    }

                    imageBuffer.GetWritableBuffer().Push(pBuf, length);
                    imageBuffer.SwapWritableBuffer();
                }
            });

            cameraRealPlayHandle = CHCNetSDK.NET_DVR_RealPlay_V40(userId, ref lpPreviewInfo, readDataCallback, IntPtr.Zero);
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
            return CHCNetSDK.NET_DVR_SerialStop(cameraChannel);
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

        #endregion
    }
}
