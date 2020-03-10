using System;
using System.Text;
using System.Windows.Forms;

namespace IRApplication.Components.HIKDevice
{
    /// <summary>
    /// 可渲染控件
    /// </summary>
    public class HIKVisionRenderableControl : IRenderableControl
    {
        /// <summary>
        /// 使用海康播放组件
        /// </summary>
        public static readonly bool UseHIKDevice = false;

        /// <summary>
        /// 用户句柄
        /// </summary>
        private int m_lUserID = -1;

        /// <summary>
        /// 实时播放句柄
        /// </summary>
        private int m_lRealHandle = -1;

        /// <summary>
        /// 通道
        /// </summary>
        private readonly int channel;

        static HIKVisionRenderableControl()
        {
            CHCNetSDK.NET_DVR_Init();
        }

        public HIKVisionRenderableControl(int channel)
        {
            this.channel = channel;
        }

        public bool InitializeComponent(Control parent, int width, int stride, int height)
        {
            if (m_lUserID >= 0) {
                return false;
            }

            var struLogInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();

            // 设备IP地址或者域名
            byte[] byIP = Encoding.Default.GetBytes("192.168.1.64");
            struLogInfo.sDeviceAddress = new byte[129];
            byIP.CopyTo(struLogInfo.sDeviceAddress, 0);

            // 设备用户名
            byte[] byUserName = Encoding.Default.GetBytes("admin");
            struLogInfo.sUserName = new byte[64];
            byUserName.CopyTo(struLogInfo.sUserName, 0);

            // 设备密码
            byte[] byPassword = Encoding.Default.GetBytes("abcd1234");
            struLogInfo.sPassword = new byte[64];
            byPassword.CopyTo(struLogInfo.sPassword, 0);

            struLogInfo.wPort = ushort.Parse("8000");//设备服务端口号
            struLogInfo.bUseAsynLogin = false; //是否异步登录：0- 否，1- 是

            var DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();

            //登录设备 Login the device
            m_lUserID = CHCNetSDK.NET_DVR_Login_V40(ref struLogInfo, ref DeviceInfo);
            if (m_lUserID < 0) {
                return false;
            }

            return StartRealPlay(channel, parent.Handle);
        }

        public CreateParams GetParams(CreateParams createParams)
        {
            return createParams;
        }

        public void Dispose()
        {
            StopRealPlay();
        }

        public void DrawRGBAImage(IntPtr image, int length)
        {
        }

        public void DrawYImage(IntPtr image, int length)
        {
        }

        public void DrawYImage(byte[] image, int length)
        {
        }

        public void DrawYV12Image(IntPtr image, int length)
        {
        }

        public void OnSizeChanged(Control control)
        {
        }

        public void Render(Control control)
        {
        }

        /// <summary>
        /// 开启实时播放
        /// </summary>
        /// <param name="channel">通道</param>
        /// <param name="handle">控件句柄</param>
        /// <returns>是否成功</returns>
        private bool StartRealPlay(int channel, IntPtr handle)
        {
            if (m_lRealHandle >= 0) {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
                m_lRealHandle = -1;
            }

            CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO {
                hPlayWnd = handle,//预览窗口
                lChannel = channel,//预览的设备通道
                dwStreamType = 1,//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                dwLinkMode = 0,//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                bBlocked = true, //0- 非阻塞取流，1- 阻塞取流
                dwDisplayBufNum = 1, //播放库播放缓冲区最大缓冲帧数
                byProtoType = 0,
                byPreviewMode = 0
            };

            IntPtr pUser = new IntPtr();//用户数据

            // 打开预览 Start live view 
            m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);
            if (m_lRealHandle < 0) {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 停止实时播放
        /// </summary>
        private void StopRealPlay()
        {
            if (m_lRealHandle >= 0) {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
            }

            if (m_lUserID >= 0) {
                CHCNetSDK.NET_DVR_Logout(m_lUserID);
            }
        }
    }
}
