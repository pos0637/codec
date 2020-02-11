using Common;
using Devices;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace HIKVisionDevice
{
    public class HIKVisionDevice : IDevice
    {
        #region 常量

        // 温度偏置
        private const int FSMT_TEMP_OFFSET = 200;

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
        private const short mPort = 8000;

        /// <summary>
        /// 用户名
        /// </summary>
        private const string mUserName = "admin";

        /// <summary>
        /// 密码
        /// </summary>
        private const string mPassword = "abcd1234";

        /// <summary>
        /// 宽度
        /// </summary>
        private const int mWidth = 384;

        /// <summary>
        /// 高度
        /// </summary>
        private const int mHeight = 288;

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
        private int mUserId;

        /// <summary>
        /// 串口句柄
        /// </summary>
        private int mRealPlayHandle;

        /// <summary>
        /// 帧缓冲
        /// </summary>
        private byte[] mFrameBuffer;

        /// <summary>
        /// 温度缓冲区
        /// </summary>
        private float[] mTemperatureBuffer;

        /// <summary>
        /// 临时帧缓冲
        /// </summary>
        private byte[] mTempBuffer;

        /// <summary>
        /// 缓冲区已使用长度
        /// </summary>
        private int mBufferUsed;

        /// <summary>
        /// 缓冲区长度
        /// </summary>
        private int mBufferLength;

        /// <summary>
        /// 接收数据回调函数
        /// </summary>
        private CHCNetSDK.REALDATACALLBACK mOnReceived;

        private bool mHasHeader = false;

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
            mBufferLength = 4 + mWidth * mHeight * sizeof(float);
            mTempBuffer = new byte[mBufferLength];
            mFrameBuffer = new byte[mBufferLength];
            mTemperatureBuffer = new float[mWidth * mHeight];
            mBufferUsed = 0;
            mHasHeader = false;

            CHCNetSDK.NET_DVR_Init();

            if (!Login(mIp, mPort, mUserName, mPassword))
                return false;

            if (!Config(mDistance, mEmissivity, mReflectedTemperature)) {
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
                    return CHCNetSDK.NET_DVR_FocusOnePush(mUserId, 1);

                case ControlMode.FocusFar:
                    if (!CHCNetSDK.NET_DVR_PTZControl_Other(mUserId, 1, CHCNetSDK.FOCUS_FAR, 0))
                        return false;

                    Thread.Sleep(1000);
                    return CHCNetSDK.NET_DVR_PTZControl_Other(mUserId, 1, CHCNetSDK.FOCUS_FAR, 1);

                case ControlMode.FocusNear:
                    if (!CHCNetSDK.NET_DVR_PTZControl_Other(mUserId, 1, CHCNetSDK.FOCUS_NEAR, 0))
                        return false;

                    Thread.Sleep(1000);
                    return CHCNetSDK.NET_DVR_PTZControl_Other(mUserId, 1, CHCNetSDK.FOCUS_NEAR, 1);

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
                    lock (mFrameBuffer) {
                        for (int i = 0, j = 4; i < mTemperatureBuffer.Length; ++i, j += 4) {
                            mTemperatureBuffer[i] = BitConverter.ToSingle(mFrameBuffer, j);
                        }
                    }

                    var dst = (float[])inData;
                    var scale = BitConverter.ToInt32(mFrameBuffer, 4);
                    for (var i = 0; i < mTemperatureBuffer.Length; ++i) {
                        dst[i] = mTemperatureBuffer[i];
                    }

                    return true;
                }

                case ReadMode.ImageArray: {
                    lock (mFrameBuffer) {
                        for (int i = 0, j = 4; i < mTemperatureBuffer.Length; ++i, j += 4) {
                            mTemperatureBuffer[i] = BitConverter.ToSingle(mFrameBuffer, j);
                        }
                    }

                    var min = float.MaxValue;
                    var max = 0.0F;
                    for (var i = 0; i < mTemperatureBuffer.Length; ++i) {
                        min = Math.Min(mTemperatureBuffer[i], min);
                        max = Math.Max(mTemperatureBuffer[i], max);
                    }

                    var dst = (byte[])inData;
                    float span = max - min;
                    if (span <= float.Epsilon) {
                        Array.Clear(dst, 0, dst.Length);
                        return true;
                    }

                    for (var i = 0; i < mTemperatureBuffer.Length; ++i) {
                        dst[i] = (byte)((mTemperatureBuffer[i] - min) * 255.0F / span);
                    }

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
                case WriteMode.URI:
                    mIp = data as string;
                    break;

                case WriteMode.ObjectDistance:
                    mDistance = (float)data;
                    if (GetDeviceStatus() == DeviceStatus.Running) {
                        return Config(mDistance, mEmissivity, mReflectedTemperature);
                    }
                    break;

                case WriteMode.Emissivity:
                    mEmissivity = (float)data;
                    if (GetDeviceStatus() == DeviceStatus.Running) {
                        return Config(mDistance, mEmissivity, mReflectedTemperature);
                    }
                    break;

                case WriteMode.ReflectedTemperature:
                    mReflectedTemperature = (float)data;
                    if (GetDeviceStatus() == DeviceStatus.Running) {
                        return Config(mDistance, mEmissivity, mReflectedTemperature);
                    }
                    break;

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

            mUserId = CHCNetSDK.NET_DVR_Login_V30(ip, port, userName, password, ref deviceInfo);
            if (mUserId < 0) {
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
            return CHCNetSDK.NET_DVR_Logout(mUserId);
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="distance">距离</param>
        /// <param name="emissivity">发射率</param>
        /// <param name="reflectedTemperature">反射温度</param>
        /// <returns></returns>
        private bool Config(float distance, float emissivity, float reflectedTemperature)
        {
            int channelId = 1;
            IntPtr lpOutputXml = Marshal.AllocHGlobal(1024 * 1024);

            CHCNetSDK.NET_DVR_STD_ABILITY struSTDAbility = new CHCNetSDK.NET_DVR_STD_ABILITY();
            struSTDAbility.lpCondBuffer = new IntPtr(channelId);
            struSTDAbility.dwCondSize = sizeof(int);

            struSTDAbility.lpOutBuffer = lpOutputXml;
            struSTDAbility.dwOutSize = 1024 * 1024;
            struSTDAbility.lpStatusBuffer = lpOutputXml;
            struSTDAbility.dwStatusSize = 1024 * 1024;

            if (!CHCNetSDK.NET_DVR_GetSTDAbility(mUserId, CHCNetSDK.NET_DVR_GET_THERMAL_CAPABILITIES, ref struSTDAbility)) {
                Marshal.FreeHGlobal(lpOutputXml);
                lpOutputXml = IntPtr.Zero;
                return false;
            }

            CHCNetSDK.NET_DVR_XML_CONFIG_INPUT struInput = new CHCNetSDK.NET_DVR_XML_CONFIG_INPUT();
            CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT struOuput = new CHCNetSDK.NET_DVR_XML_CONFIG_OUTPUT();
            struInput.dwSize = (uint)Marshal.SizeOf(struInput);
            struOuput.dwSize = (uint)Marshal.SizeOf(struOuput);
            struOuput.lpOutBuffer = lpOutputXml;
            struOuput.dwOutBufferSize = 1024 * 1024;

            string szUrl = "GET /ISAPI/Thermal/channels/1/streamParam/capabilities";
            struInput.lpRequestUrl = szUrl;
            struInput.dwRequestUrlLen = (uint)szUrl.Length;
            struInput.lpInBuffer = null;
            struInput.dwInBufferSize = 0;
            if (!CHCNetSDK.NET_DVR_STDXMLConfig(mUserId, ref struInput, ref struOuput)) {
                Marshal.FreeHGlobal(lpOutputXml);
                lpOutputXml = IntPtr.Zero;
                return false;
            }

            szUrl = "GET /ISAPI/Thermal/channels/1/streamParam";
            struInput.lpRequestUrl = szUrl;
            struInput.dwRequestUrlLen = (uint)szUrl.Length;
            struInput.lpInBuffer = null;
            struInput.dwInBufferSize = 0;
            if (!CHCNetSDK.NET_DVR_STDXMLConfig(mUserId, ref struInput, ref struOuput)) {
                Marshal.FreeHGlobal(lpOutputXml);
                lpOutputXml = IntPtr.Zero;
                return false;
            }

            szUrl = "PUT /ISAPI/Thermal/channels/1/streamParam";
            struInput.lpRequestUrl = szUrl;
            struInput.dwRequestUrlLen = (uint)szUrl.Length;
            string szThermalStreamParam =
                "<ThermalStreamParam version=\"2.0\" xmlns=\"http://www.isapi.org/ver20/XMLSchema\"><videoCodingType>pixel-to-pixel_thermometry_data</videoCodingType></ThermalStreamParam>";
            struInput.lpInBuffer = szThermalStreamParam;
            struInput.dwInBufferSize = (uint)szThermalStreamParam.Length;
            if (!CHCNetSDK.NET_DVR_STDXMLConfig(mUserId, ref struInput, ref struOuput)) {
                Marshal.FreeHGlobal(lpOutputXml);
                lpOutputXml = IntPtr.Zero;
                return false;
            }

            szUrl = "GET /ISAPI/Thermal/channels/1/thermometry/pixelToPixelParam/capabilities";
            struInput.lpRequestUrl = szUrl;
            struInput.dwRequestUrlLen = (uint)szUrl.Length;
            struInput.lpInBuffer = null;
            struInput.dwInBufferSize = 0;
            if (!CHCNetSDK.NET_DVR_STDXMLConfig(mUserId, ref struInput, ref struOuput)) {
                Marshal.FreeHGlobal(lpOutputXml);
                lpOutputXml = IntPtr.Zero;
                return false;
            }

            szUrl = "GET /ISAPI/Thermal/channels/1/thermometry/pixelToPixelParam";
            struInput.lpRequestUrl = szUrl;
            struInput.dwRequestUrlLen = (uint)szUrl.Length;
            struInput.lpInBuffer = null;
            struInput.dwInBufferSize = 0;
            if (!CHCNetSDK.NET_DVR_STDXMLConfig(mUserId, ref struInput, ref struOuput)) {
                Marshal.FreeHGlobal(lpOutputXml);
                lpOutputXml = IntPtr.Zero;
                return false;
            }

            szUrl = "GET /ISAPI/Thermal/channels/1/thermometry/pixelToPixelParam";
            struInput.lpRequestUrl = szUrl;
            struInput.dwRequestUrlLen = (uint)szUrl.Length;
            string szPixelToPixelParamFormat =
                "<?xml version=\"1.0\" encoding=\"UTF - 8\"?>" +
                "<PixelToPixelParam version = \"2.0\" xmlns = \"http://www.hikvision.com/ver20/XMLSchema\">" +
                "<id>1</id>" +
                "<maxFrameRate>0</maxFrameRate>" +
                "<reflectiveEnable>false</reflectiveEnable>" +
                "<reflectiveTemperature>{0:F2}</reflectiveTemperature>" +
                "<emissivity>{1:F2}</emissivity>" +
                "<distance>{2:D}</distance>" +
                "<refreshInterval>0</refreshInterval>" +
                "</PixelToPixelParam>";

            string szPixelToPixelParam = string.Format(szPixelToPixelParamFormat, reflectedTemperature, emissivity, (int)distance);
            struInput.lpInBuffer = szPixelToPixelParam;
            struInput.dwInBufferSize = (uint)szPixelToPixelParam.Length;
            if (!CHCNetSDK.NET_DVR_STDXMLConfig(mUserId, ref struInput, ref struOuput)) {
                Marshal.FreeHGlobal(lpOutputXml);
                lpOutputXml = IntPtr.Zero;
                return false;
            }

            Marshal.FreeHGlobal(lpOutputXml);
            lpOutputXml = IntPtr.Zero;

            CHCNetSDK.NET_DVR_FOCUSMODE_CFG cfg = new CHCNetSDK.NET_DVR_FOCUSMODE_CFG();
            uint returnBytes = 0;
            int outSize = Marshal.SizeOf(cfg);
            IntPtr outBuffer = Marshal.AllocHGlobal(outSize);
            Marshal.StructureToPtr(cfg, outBuffer, false);
            if (!CHCNetSDK.NET_DVR_GetDVRConfig(mUserId, CHCNetSDK.NET_DVR_GET_FOCUSMODECFG, 1, outBuffer, (uint)outSize, ref returnBytes)) {
                Marshal.FreeHGlobal(outBuffer);
                outBuffer = IntPtr.Zero;
                return false;
            }

            cfg.byFocusMode = 2;
            bool ret = CHCNetSDK.NET_DVR_SetDVRConfig(mUserId, CHCNetSDK.NET_DVR_SET_FOCUSMODECFG, 1, outBuffer, (uint)outSize);
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
            mOnReceived = (int lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser) => {
                if ((dwDataType == CHCNetSDK.NET_DVR_STREAMDATA) && (dwBufSize > 0)) {
                    CHCNetSDK.STREAM_FRAME_INFO_S st_frame_info = (CHCNetSDK.STREAM_FRAME_INFO_S)Marshal.PtrToStructure(pBuffer, typeof(CHCNetSDK.STREAM_FRAME_INFO_S));

                    bool hasHeader = false;
                    int iIndex = 0;
                    if (st_frame_info.u32MagicNo == 0x70827773) {
                        iIndex = (int)st_frame_info.u32HeaderSize;
                        hasHeader = true;
                    }

                    lock (mTempBuffer) {
                        int length = (int)dwBufSize - iIndex;
                        if (((!mHasHeader) && (!hasHeader)) || ((mBufferUsed + length) > mBufferLength)) {
                            mBufferUsed = 0;
                            mHasHeader = false;
                            return;
                        }

                        if (hasHeader)
                            mHasHeader = true;

                        Marshal.Copy(pBuffer + iIndex, mTempBuffer, mBufferUsed, length);
                        mBufferUsed += length;

                        if (mBufferUsed == mBufferLength) {
                            lock (mFrameBuffer) {
                                Array.Copy(mTempBuffer, mFrameBuffer, mBufferLength);
                            }
                            mBufferUsed = 0;
                            mHasHeader = false;
                        }
                    }
                }
            };

            CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
            lpPreviewInfo.hPlayWnd = IntPtr.Zero; // 预览窗口
            lpPreviewInfo.lChannel = 1; // 预览的设备通道
            lpPreviewInfo.dwStreamType = 0; // 码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
            lpPreviewInfo.dwLinkMode = 0; // 连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
            lpPreviewInfo.bBlocked = true; // 0- 非阻塞取流，1- 阻塞取流
            lpPreviewInfo.byVideoCodingType = 1;

            mRealPlayHandle = CHCNetSDK.NET_DVR_RealPlay_V40(mUserId, ref lpPreviewInfo, mOnReceived, IntPtr.Zero);
            if (mRealPlayHandle < 0) {
                Tracker.LogE("NET_DVR_RealPlay_V40 failed, error code= " + CHCNetSDK.NET_DVR_GetLastError());
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
            return CHCNetSDK.NET_DVR_SerialStop(mRealPlayHandle);
        }

        #endregion
    }
}
