using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace SDK
{
    public class GroupSDK
    {
        #region struct

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct ENUM_INFO
        {
            /// <summary>
            /// 相机名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sName;

            /// <summary>
            /// 协议版本号
            /// </summary>
            public UInt32 intVersion;

            /// <summary>
            /// 相机IP
            /// </summary>
            public UInt32 intCamIp;

            /// <summary>
            /// 控制端IP
            /// </summary>
            public UInt32 intUsrIp;

            /// <summary>
            /// 相机MAC地址
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] byCamMAC;

            /// <summary>
            /// 填充字符
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] byPad;

            /// <summary>
            /// 列表更新参数
            /// </summary>
            public UInt32 intRefreshAck;

            /// <summary>
            /// 组播/单播
            /// </summary>
            public UInt32 bIsMulticast;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct OUTPUT_PARAM
        {
            /// <summary>
            /// FPA宽度，必须为FPAWIDTH
            /// </summary>
            public UInt32 intFPAWidth;

            /// <summary>
            /// FPA高度，必须为FPAHEIGHT
            /// </summary>
            public UInt32 intFPAHeight;

            /// <summary>
            /// 输出图像宽度, 必须为4的整数倍, 且不小于FPA的宽度(FPAWIDTH)
            /// </summary>
            public UInt32 intBMPWidth;

            /// <summary>
            /// 输出图像高度, 不小于FPA的高度(FPAHEIGHT)
            /// </summary>
            public UInt32 intBMPHeight;

            /// <summary>
            /// 颜色条图像宽度, 必须为4的整数倍, 最小为4
            /// </summary>
            public UInt32 intColorbarWidth;

            /// <summary>
            /// 颜色条图像高度, 最小为1
            /// </summary>
            public UInt32 intColorbarHeight;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RECT_ROI
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sROIName;

            /// <summary>
            /// 以左下角为原点, x0必须小于x1
            /// </summary>
            public int x0;

            /// <summary>
            /// 以左下角为原点, y0必须小于y1
            /// </summary>
            public int y0;
            public int x1;
            public int y1;

            /// <summary>
            /// 发射率*100，例如90代表0.9
            /// </summary>
            public int intEmissivity;

            /// <summary>
            /// 报警温度，单位mC，报警方式为IO输出和画面上数字闪烁
            /// </summary>
            public int intAlarmTemp;

            /// <summary>
            /// 显示选项
            /// </summary>
            public int intDrawOpt;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public int[] byPad;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct USER_ROI
        {
            public int intValidRectROI;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public RECT_ROI[] ROI;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct CAMERA_INFO
        {
            /// <summary>
            /// 探测器尺寸宽度
            /// </summary>
            public int intFPAWidth;

            /// <summary>
            /// 探测器尺寸高度
            /// </summary>
            public int intFPAHeight;

            /// <summary>
            /// 填充字符，无实际意义
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public int[] pad;

            /// <summary>
            /// 相机名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sCamName;

            /// <summary>
            /// 类型名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string sTypeName;

            /// <summary>
            /// 本型号的最高帧率
            /// </summary>
            public int intMaxFps;

            /// <summary>
            /// 当前实际输出帧率
            /// </summary>
            public int intCurrFps;

            /// <summary>
            /// (以HDMI, H.264或MPEG输出的)数字视频像素
            /// </summary>
            public int intVideoWidth;

            /// <summary>
            /// (以HDMI, H.264或MPEG输出的)数字视频像素
            /// </summary>
            public int intVideoHeight;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct CAMERA_REGCONTENT
        {
            /// <summary>
            /// 填充字符
            /// </summary>
            public int pad0;

            /// <summary>
            /// 热像仪名称
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sName;

            /// <summary>
            /// 是否使用静态IP 0-未使用 1-使用
            /// </summary>
            public int intIsUseStaticIP;

            /// <summary>
            /// 静态IP，网络字节序
            /// </summary>
            public int intStaticIp;

            /// <summary>
            /// 静态IP的子网掩码, 网络字节序
            /// </summary>
            public int intStaticNetMask;

            /// <summary>
            /// 图像数据是否组播 0 不 1组播
            /// </summary>
            public int intIsMulticastImg;

            /// <summary>
            /// 图像组播的ip，网络字节序
            /// </summary>
            public int intMulticastIp;

            /// <summary>
            /// 填充字符
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public int[] pad1;

            /// <summary>
            /// 序列号，只读
            /// </summary>
            public int intSerianNo;

            /// <summary>
            /// 填充字符
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public int[] pad2;

            /// <summary>
            /// 静态网关
            /// </summary>
            public int intStaticGateWay;

            /// <summary>
            /// 填充字符
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public int[] pad3;

            /// <summary>
            /// 当前镜头编号
            /// </summary>
            public int intCurrLensIndex;

            /// <summary>
            /// 填充字符
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public int[] pad4;

            /// <summary>
            /// FFC帧数触发器，单位帧
            /// </summary>
            public int intFFCFrameTrigger;

            /// <summary>
            /// FFC温度触发器，单位mC
            /// </summary>
            public int intFFCTemperatureTrigger;

            /// <summary>
            /// 填充字符
            /// </summary>
            public int pad5;

            /// <summary>
            /// 多帧累加，单位帧, 1,2,4,8,16
            /// </summary>
            public int intAccResponse;

            /// <summary>
            /// 填充字符
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public int[] pad6;

            /// <summary>
            /// //输入Io功能, 0-FFC, 1-MGT, 2-BMP,3-自定义
            /// </summary>
            public int intInputIoFunction;

            #region 模拟视频控制，不影响PC机上看到的效果

            /// <summary>
            /// 调色板
            /// </summary>
            public COLOR_PALETTE intPaletteIndex;

            /// <summary>
            /// 是否叠加颜色条 0 不叠加 1 叠加
            /// </summary>
            public int intIsShowColorBar;

            /// <summary>
            /// 是否使用人工拉伸
            /// </summary>
            public int intIsUseManualEnlarge;

            /// <summary>
            /// 人工拉伸节点X1, 单位mC
            /// </summary>
            public int intEnlargeX1;

            /// <summary>
            /// 人工拉伸节点X2, 单位mC
            /// </summary>
            public int intEnlargeX2;

            /// <summary>
            /// 人工拉伸节点Y1, 单位为灰度，范围0~254
            /// </summary>
            public int intEnlargeY1;

            /// <summary>
            /// 人工拉伸节点Y2, 单位为灰度，范围1~255
            /// </summary>
            public int intEnlargeY2;

            /// <summary>
            /// 自动拉伸最小温度范围，单位C，人工分段灰度拉伸比自动拉伸优先，必须大于2，建议不小于5
            /// </summary>
            public int intAutoEnlargeRange;

            #endregion

            #region IO

            /// <summary>
            /// 模拟视频标记, 0-无,1-中心,2-高温,3-测温对象
            /// </summary>
            public int intAnalogPlot;

            /// <summary>
            /// IO报警温度
            /// </summary>
            public int intAlarmTemp;

            #endregion

            #region TV standard

            /// <summary>
            /// 模拟视频制式
            /// </summary>
            public TV_STANDARD intTVStandard;

            /// <summary>
            /// 是否启用心跳检测, 0 不适用 1 使用
            /// </summary>
            public int intIsCheckHeartBeat;

            /// <summary>
            /// 是否始终输出模拟视频，若否，则以太网传输图像时模拟视频关闭，以降低功耗 0 否 1 是
            /// </summary>
            public int intIsAlwaysAnalogOutput;

            /// <summary>
            /// 填充字符
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public int[] pad8;

            /// <summary>
            /// 模拟视频电子倍焦
            /// </summary>
            public EX EXLevel;

            /// <summary>
            /// DTXX搭档的可见光相机IP，MAGXX未定义
            /// </summary>
            public int intPartnerVisibleIp;

            /// <summary>
            /// 模拟视频DDE强度设置, 0~32
            /// </summary>
            public int intDDE;

            /// <summary>
            /// 填充字符
            /// </summary>
            public int pad9;

            /// <summary>
            /// 串口波特率
            /// </summary>
            public int intSerialBaudRate;

            /// <summary>
            /// 串口参数，格式为 (使能<<24) | (数据位<<16) | (停止位<<8) | 校验位。其中停止位 0-1, 1-1.5, 2-2，校验位0-None, 1-Odd, 2-Even, 3-Mark, 4-Space
            /// </summary>
            public int intSerialFeature;

            #endregion
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TEMP_STATE
        {
            /// <summary>
            /// 保留字段
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] pad0;

            /// <summary>
            /// 全相面最高温, mc
            /// </summary>
            public int intMaxTemp;

            /// <summary>
            /// 全相面最低温, mc
            /// </summary>
            public int intMinTemp;

            /// <summary>
            /// 全相面平均温, mc
            /// </summary>
            public int intAveTemp;

            /// <summary>
            /// 全相面标准差
            /// </summary>
            public int intStdTemp;

            /// <summary>
            /// 最高温度出现位置, y=int(intPosMax/FPAWIDTH), x=intPosMax-FPAWIDTH*y, 原点在图像左下角
            /// </summary>
            public int intPosMax;

            /// <summary>
            /// 最低温度出现位置, y=int(intPosMin/FPAWIDTH), x=intPosMin-FPAWIDTH*y, 原点在图像左下角
            /// </summary>
            public int intPosMin;

            /// <summary>
            /// 保留字段
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public int[] pad1;

            /// <summary>
            /// 全像面温度时域标准差, 单位mC, 包含景物温度变化和相机噪声两个部分，该数值对部分型号的高温热像仪不适用
            /// </summary>
            public int intAveNETDt;

            /// <summary>
            /// 保留字段
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public int[] pad2;

            /// <summary>
            /// 红外直方图
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public int[] intHistTemperature;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct FIX_PARAM
        {
            /// <summary>
            /// 目标距离, 单位m, 范围(2*fFocalLength, ∞)
            /// </summary>
            public float fDistance;

            /// <summary>
            /// 全局缺省发射率，范围(0,1]
            /// </summary>
            public float fEmissivity;

            /// <summary>
            /// 气温, 单位C
            /// </summary>
            public float fTemp;

            /// <summary>
            /// 相对湿度，范围(0~1)
            /// </summary>
            public float fRH;

            /// <summary>
            /// 能见度, 单位km, 范围(0, ∞)
            /// </summary>
            public float fVisDistance;

            /// <summary>
            /// 降雨强度, 单位mm/h, 范围(0, ∞)
            /// </summary>
            public float fRain;

            /// <summary>
            /// 降雪强度, 单位mm/h, 范围(0, ∞)
            /// </summary>
            public float fSnow;

            /// <summary>
            /// 修正参数1 镜头相机相关
            /// </summary>
            public float fExtrapara1;

            /// <summary>
            /// 修正参数2 镜头相机相关
            /// </summary>
            public float fExtrapara2;

            /// <summary>
            /// 大气透过率, 只读
            /// </summary>
            public float fTaoAtm;

            /// <summary>
            /// (热像仪外部的)窗口/滤光片透过率
            /// </summary>
            public float fTaoFilter;
        }

        #endregion

        #region enum

        public enum STREAM_TYPE
        {
            STREAM_TEMPERATURE = 2,
            STREAM_VIDEO = 4,
            STREAM_HYBRID = 6,
        };

        public enum COLOR_PALETTE
        {
            /// <summary>
            /// 白热
            /// </summary>
            GRAY1 = 0,

            /// <summary>
            /// 黑热
            /// </summary>
            GRAY2 = 1,

            /// <summary>
            /// 铁红
            /// </summary>
            IRONBOW = 2,

            /// <summary>
            /// 彩虹
            /// </summary>
            RAINBOW = 3,

            /// <summary>
            /// 琥珀
            /// </summary>
            GLOWBOW = 4,

            /// <summary>
            /// 金秋
            /// </summary>
            AUTUMN = 5,

            /// <summary>
            /// 寒冬
            /// </summary>
            WINTER = 6,

            /// <summary>
            /// 热金属
            /// </summary>
            HOTMETAL = 7,

            /// <summary>
            /// 喷射
            /// </summary>
            JET = 8,

            /// <summary>
            /// 红色饱和
            /// </summary>
            REDSATURATION = 9,
        };

        public enum TV_STANDARD
        {
            NTSC_M = 0,
            NTSC_J,
            PAL_BDGHI,
            PAL_M,
            PAL_NC,
        };

        public enum EX
        {
            E1X = 0,
            E2X,
            E4X,
        };

        public enum STANDARD_MESSAGE
        {
            WM_SETREDRAW = 0xB,
        };

        public enum ROI_TYPE
        {
            ROI_NONE = -1,
            ROI_POINT = 0,
            ROI_LINE = 1,
            ROI_RECTANGLE = 2,
            ROI_ELLIPSE = 3,
            ROI_TRIPHASE = 4,
        };

        public enum SOURCE_FORMAT
        {
            YV12 = 0,
            RGB_8 = 1,
            RGB_555 = 2,
            RGB_565 = 3,
            RGB_24 = 4,
        };

        public enum PTZIRCMD
        {
            PTZStop = 0,    //方位或预置位停止 参数为0
            PTZRight,       //方位 参数为运动速度0~63
            PTZLeft,        
            PTZUp,
            PTZDown,
            PTZUpRight = 5,
            PTZUpLeft,
            PTZDownRight,
            PTZDownLeft,
            PTZSetPreset = 9,//预置位 参数为预置位编号0~255
            PTZCallPreset,
            PTZClearPreset,
            PTZSetAuxiliary = 12,//辅助开关 参数为辅助开关编号0~255
            PTZClearAuxiliary,
            PTZZoomStop = 14,//镜头停止变倍 参数为0
            PTZZoomIn,//镜头放大 参数为马达运动时间
            PTZZoomOut,//镜头缩小
            PTZFocusStop = 17,//镜头停止调焦 参数为0
            PTZFocusAuto,//镜头自动对焦 参数为0
            PTZFocusFar,//镜头看远 参数为马达运动时间
            PTZFocusNear,//镜头看近
            PTZFocusGoto,//镜头调焦绝对位置 参数为调焦位置
        };

        public enum PTZQuery
        {
            PTZQueryPan = 0,//查询云台上下角度
            PTZQueryTilt,//查询云台左右角度
            PTZQueryZoomPosition,//查询zoom绝对位置
            PTZQueryZoomState,//查询是否正在zoom
            PTZQueryFocusPosition,//查询focus绝对位置
            PTZQueryFocusState,//查询是否正在执行自动对焦
        };

        public enum SDStorageFileType
        {
            SDFileBMP = 0,//保存BMP文件
            SDFileDDT,//保存DDT文件
            SDFileMGT,//保存MGT文件
            SDFileAVI,//保存AVI文件
            SDFileMGS,//保存MGS文件
        };

        #endregion

        #region delegate

        ///<summary>
        ///播放回调
        ///void CALLBACK NewFrame(UINT intChannelIndex, int intCameraTemperature, DWORD dwFFCCounterdown, DWORD dwCamState, DWORD dwStreamType, void * pUserData);
        ///</summary>
        /// <param name="intChannelIndex">表示相机通道号</param>
        /// <returns></returns>
        public delegate void DelegateNewFrame(uint hDevice, int intCamTemp, int intFFCCounter, int intCamState, int intStreamType, IntPtr pUserData);


        /// <summary>
        /// void (CALLBACK * MAG_RECONNECTCALLBACK)(UINT intChannelIndex, UINT intRecentHeartBeatTick, int intState, void * pUserData);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intRecentHeartBeatTick"></param>
        /// <param name="intState"></param>
        /// <param name="pUserData"></param>
        public delegate void DelegateReconnect(uint hDevice, uint intRecentHeartBeatTick, int intState, IntPtr pUserData);

        /// <summary>
        /// 串口回调
        /// void (CALLBACK * MAG_SERIALCALLBACK)(UINT intChannelIndex, void * pData, UINT intDataLength, void * pUserData);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="szData"></param>
        /// <param name="intDataLen"></param>
        /// <param name="pUserData"></param>
        public delegate void DelegateSerial(uint hDevice, IntPtr pRealData, uint intDataLen, IntPtr pUserData);

        ///DDT压缩回调
        ///void (CALLBACK * MAG_DDTCOMPRESSCALLBACK)(UINT intChannelIndex, void * pData, UINT intDataLength, void * pUserData);
        ///</summary>
        /// <param name="intChannelIndex">表示相机通道号</param>
        /// <returns></returns>
        public delegate void DelegateDDTCompressComplete(uint hDevice, IntPtr pData, uint intDataLength, IntPtr pUserData);

        #endregion

        /// <summary>
        /// 不包括0通道,最大连接相机数
        /// </summary>
        public const int MAX_INSTANCE = 32;

        ///<summary>
        ///通信初始化
        ///_stdcall BOOL MAG_Initialize(UINT intChannelIndex, HWND hWndMsg);
        ///</summary>
        /// <param name="intChannelIndex">相机对应的通道号</param>
        /// <param name="hWnd">接收消息窗口句柄。
        /// 热像仪主动断开连接时向hwnd发送message=WM_COMMAND, wParam=WM_APP+1001的消息；
        /// 热像仪列表更新时向hWndMsg发送message=WM_COMMAND, wParam=WM_APP+1002的消息。</param>
        /// <returns>返回TRUE表示成功，返回FALSE表示失败。</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_Initialize")]
        public static extern bool MAG_Initialize(uint hDevice, IntPtr hWnd);

        ///<summary>
        ///是否已经通信初始化成功
        ///_stdcall BOOL WINAPI MAG_IsInitialized(UINT intChannelIndex);
        ///</summary>
        /// <param name="intChannelIndex">相机对应的通道号</param>
        /// <returns>返回TRUE表示成功，返回FALSE表示失败。</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsInitialized")]
        public static extern bool MAG_IsInitialized(uint hDevice);

        ///<summary>
        ///释放资源
        ///_stdcall BOOL MAG_Free(UINT intChannelIndex\);
        ///</summary>
        /// <param name="intChannelIndex">相机对应的通道号</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_Free")]
        public static extern void MAG_Free(uint hDevice);

        ///<summary>
        ///枚举局域网中的相机
        ///_stdcall BOOL MAG_EnumCameras();
        ///</summary>
        /// <returns>返回TRUE表示成功，返回FALSE表示失败。</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_EnumCameras")]
        public static extern bool MAG_EnumCameras();

        ///<summary>
        ///连接相机
        ///_stdcall BOOL MAG_LinkCamera(UINT intChannelIndex, UINT intIP, UINT intTimeoutMS);
        ///</summary>
        /// <param name="intChannelIndex">相机对应的通道号</param>
        /// <param name="intIP">相机IP，网络字节序。</param>
        /// <param name="intTimeoutMS">连接超时时间，毫秒单位。</param>
        /// <returns>返回TRUE表示成功，返回FALSE表示失败。</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_LinkCamera")]
        public static extern bool MAG_LinkCamera(uint hDevice, uint ip, uint timeout);


	    //_stdcall BOOL MAG_LinkCameraEx(UINT intChannelIndex, UINT intIP, USHORT shortPort, const char * charCloudUser, const char * charCloudPwd, UINT intCamSN, const char * charCamUser, const char * charCamPwd, UINT intTimeoutMS);
        ///</summary>
        /// <param name="intChannelIndex">相机对应的通道号</param>
        /// <param name="intIP">相机IP，网络字节序。</param>
        /// <param name="shortPort">相机端口号。</param>
        /// <param name="charCloudUser">ThermoAny用户名。</param>
        /// <param name="charCloudPwd">ThermoAny用户密码。</param>
        /// <param name="intCamSN">通过ThermoAny连接的热像仪序列号。</param>
        /// <param name="charCamUser">热像仪用户名。</param>
        /// <param name="charCamPwd">热像仪用户密码。</param>
        /// <param name="intTimeoutMS">连接超时时间，毫秒单位。</param>
        /// <returns>返回TRUE表示成功，返回FALSE表示失败。</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_LinkCameraEx")]
        public static extern bool MAG_LinkCameraEx(uint hDevice, uint ip, ushort port, [MarshalAs(UnmanagedType.LPStr)] string charCloudUser, 
            [MarshalAs(UnmanagedType.LPStr)] string charCloudPwd, uint intCamSN, [MarshalAs(UnmanagedType.LPStr)] string charCamUser,
           [MarshalAs(UnmanagedType.LPStr)] string charCamPwd, uint timeout);

        ///<summary>
        ///与相机断开连接
        ///_stdcall void MAG_DisLinkCamera(UINT intChannelIndex);
        ///</summary>
        /// <param name="intChannelIndex">相机对应的通道号</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_DisLinkCamera")]
        public static extern void MAG_DisLinkCamera(uint hDevice);

        ///<summary>
        ///是否已经连接成功
        ///_stdcall BOOL MAG_IsLinked(UINT intChannelIndex);
        /// </summary>
        /// <param name="intChannelIndex">相机对应的通道号</param>
        /// <returns>返回TRUE表示成功，返回FALSE表示失败。</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsLinked")]
        public static extern bool MAG_IsLinked(uint hDevice);

        /// <summary>
        /// 是否使用了静态IP
        /// </summary>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsUsingStaticIp")]
        public static extern bool MAG_IsUsingStaticIp();

        ///<summary>
        ///得到枚举到的所有相机列表
        ///_stdcall MAG_API DWORD MAG_GetTerminalList(struct_TerminalList * pList, DWORD dwBufferSize);
        /// </summary>
        /// <param name="list">相机列表</param>
        /// <param name="dwBufferSize">所提供缓冲区尺寸，字节。</param>
        /// <returns>返回枚举到的相机个数。</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetTerminalList")]
        public static extern UInt32 MAG_GetTerminalList(IntPtr list, uint bufSize);

        ///<summary>
        ///开始传输红外数据
        ///_stdcall BOOL MAG_StartProcessImage(UINT intChannelIndex, const OutputPara * paraOut, MAG_FRAMECALLBACK funcFrame, DWORD dwStreamType, void * pUserData);
        ///</summary>
        /// <param name="intChannelIndex">相机对应的通道号</param>
        /// <param name="paraOut">相机的输出参数。</param>
        /// <param name="funcFrame">回调函数。</param>
        /// <param name="dwStreamType">相机的输出数据类型。</param>
        /// <param name="pUserData">用户数据。</param>
        /// <returns>返回枚举到的相机个数。</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StartProcessImage")]
        public static extern bool MAG_StartProcessImage(uint hDevice, ref OUTPUT_PARAM outputParam, DelegateNewFrame newFrame, uint intStreamtype, IntPtr pUserData);

        /// <summary>
        /// 停止传输红外数据
        /// _stdcall void MAG_StopProcessImage(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StopProcessImage")]
        public static extern void MAG_StopProcessImage(uint hDevice);

        /// <summary>
        /// 判断是否正在传输红外图像
        /// _stdcall bool MAG_IsProcessingImage(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsProcessingImage")]
        public static extern bool MAG_IsProcessingImage(uint hDevice);

        ///<summary>
        ///应用新的伪彩色
        ///_stdcall void MAG_SetColorPalette(UINT intChannelIndex, enum ColorPalette ColorPaletteIndex);
        ///</summary>
        /// <param name="intChannelIndex">相机对应的通道号</param>
        /// <param name="ColorPaletteIndex">所采用的伪彩色。</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetColorPalette")]
        public static extern void MAG_SetColorPalette(uint hDevice, COLOR_PALETTE paletteIndex);

        ///<summary>
        ///新建一个通道
        ///_stdcall BOOL MAG_NewChannel(UINT intChannelIndex);
        ///</summary>
        /// <param name="intChannelIndex">相机对应的通道号</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_NewChannel")]
        public static extern bool MAG_NewChannel(uint hDevice);

        ///<summary>
        ///删除一个通道
        ///_stdcall void MAG_DelChannel(UINT intChannelIndex);
        ///</summary>
        /// <param name="intChannelIndex">相机对应的通道号</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_DelChannel")]
        public static extern void MAG_DelChannel(uint hDevice);

        ///<summary>
        ///判断通道是否已可用
        ///_stdcall BOOL MAG_IsChannelAvailable(UINT intChannelIndex);
        ///</summary>
        /// <param name="intChannelIndex">相机对应的通道号</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsChannelAvailable")]
        public static extern bool MAG_IsChannelAvailable(uint hDevice);

        /// <summary>
        /// DHCP是否已经正常工作
        /// </summary>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsDHCPServerRunning")]
        public static extern bool MAG_IsDHCPServerRunning();

        /// <summary>
        /// 关闭DHCP
        /// </summary>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StopDHCPServer")]
        public static extern bool MAG_StopDHCPServer();

        /// <summary>
        /// 开启DHCP
        /// </summary>
        /// <param name="hWndMsg"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StartDHCPServer")]
        public static extern bool MAG_StartDHCPServer(IntPtr hWndMsg);

        /// <summary>
        /// 开启自动重连功能
        /// </summary>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_EnableAutoReConnect")]
        public static extern bool MAG_EnableAutoReConnect(bool bEnable);

        /// <summary>
        /// 获取相机信息
        /// _stdcall void MAG_GetCamInfo(UINT intChannelIndex, struct_CamInfo * pInfo, UINT intSize);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="camInfo"></param>
        /// <param name="intSize"></param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetCamInfo")]
        public static extern void MAG_GetCamInfo(uint hDevice, ref CAMERA_INFO CamInfo, int intSize);

        /// <summary>
        /// 获取远程注册表参数
        /// _stdcall BOOL MAG_ReadCameraRegContent(UINT intChannelIndex, struct_CeRegContent * pContent, DWORD dwTimeoutMS, BOOL bReadDefaultValue);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="Content">存储在相机端注册表的内容</param>
        /// <param name="intTimeOut">超时时间</param>
        /// <param name="isReadDefaultValue">是否读取出厂设置并覆盖当前设置</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_ReadCameraRegContent")]
        public static extern bool MAG_ReadCameraRegContent(uint hDevice, ref CAMERA_REGCONTENT Content, uint intTimeOut, int isReadDefaultValue);

        /// <summary>
        /// 设置远程注册表参数
        /// MAG_API BOOL WINAPI MAG_SetCameraRegContent(UINT intChannelIndex, const struct_CeRegContent * pContent);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="Content">结构体参数</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetCameraRegContent")]
        public static extern bool MAG_SetCameraRegContent(uint hDevice, ref CAMERA_REGCONTENT Content);

        /// <summary>
        /// 获取本机IP
        /// _stdcall DWORD MAG_GetLocalIp();
        /// </summary>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetLocalIp")]
        public static extern uint MAG_GetLocalIp();

        /// <summary>
        /// 获取8位伪彩色图像
        /// MAG_API BOOL WINAPI MAG_GetOutputBMPdata(UINT intChannelIndex, UCHAR const **  pData, BITMAPINFO const ** pInfo);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="pData">数据指针，缓冲区可用尺寸BMPWidth*BMPHeight字节，out</param>
        /// <param name="pInfo">缓冲区可用尺寸1064字节，包括BITMAPINFOHEADER和256*RGBQUAD，out</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetOutputBMPdata")]
        public static extern bool MAG_GetOutputBMPdata(uint hDevice, ref IntPtr pData, ref IntPtr pInfo);

        /// <summary>
        /// 获取Colorbar数据
        /// MAG_API BOOL WINAPI MAG_GetOutputColorBardata(UINT intChannelIndex, UCHAR const ** pData, BITMAPINFO const ** pInfo);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="pData"></param>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetOutputColorBardata")]
        public static extern bool MAG_GetOutputColorBardata(uint hDevice, ref IntPtr pData, ref IntPtr pInfo);

        /// <summary>
        /// 获取视频数据
        /// MAG_API BOOL WINAPI MAG_GetOutputVideoData(UINT intChannelIndex, UCHAR const **  pData, BITMAPINFO const ** pInfo);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="pData"></param>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetOutputVideoData")]
        public static extern bool MAG_GetOutputVideoData(uint hDevice, ref IntPtr pData, ref IntPtr pInfo);

        /// <summary>
        /// 获取某一点的温度值
        /// MAG_API int WINAPI MAG_GetTemperatureProbe(UINT intChannelIndex, DWORD dwPosX, DWORD dwPosY, UINT intSize);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size">平均范围，一般去1、3、5、7</param>
        /// <returns>平均温度</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetTemperatureProbe")]
        public static extern int MAG_GetTemperatureProbe(uint hDevice, uint x, uint y, uint size);

        /// <summary>
        /// 修正温度
        /// MAG_API int WINAPI MAG_FixTemperature(UINT intChannelIndex, int intT, float fEmissivity, DWORD dwPosX, DWORD dwPosY);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="temp">待修正温度</param>
        /// <param name="fEmissivity">发射率，范围(0, 1]</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_FixTemperature")]
        public static extern int MAG_FixTemperature(uint hDevice, int temp, float fEmissivity, uint x, uint y);

        /// <summary>
        /// 获取校正参数，应用程序退出后将不保留
        /// MAG_API void WINAPI MAG_GetFixPara(UINT intChannelIndex, struct_FixPara * pBuffer);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="pBuf"></param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetFixPara")]
        public static extern void MAG_GetFixPara(uint hDevice, ref FIX_PARAM pParam);

        /// <summary>
        /// 设置温度修正参数
        /// MAG_API float WINAPI MAG_SetFixPara(UINT intChannelIndex, const struct_FixPara * pBuffer, BOOL bEnableCameraCorrect);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="pParam">修正参数</param>
        /// <param name="bEnableCameraCorrect">是否对相机端同时应用温度修正功能</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetFixPara")]
        public static extern float MAG_SetFixPara(uint hDevice, ref FIX_PARAM pParam, int bEnableCameraCorrect);

        /// <summary>
        /// 获取相关数据
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetFrameStatisticalData")]
        public static extern IntPtr MAG_GetFrameStatisticalData(uint hDevice);

        /// <summary>
        /// 获取线上温度统计
        /// MAG_API int WINAPI MAG_GetLineTemperatureInfo(UINT intChannelIndex, int * buffer, UINT intBufferSizeByte, int info[3], UINT x0, UINT y0, UINT x1, UINT y1);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="pBuf">缓冲区</param>
        /// <param name="size">缓冲区尺寸， byte单位</param>
        /// <param name="info">统计信息， min max ave</param>
        /// <param name="x0">FPA坐标</param>
        /// <param name="y0">FPA坐标</param>
        /// <param name="x1">FPA坐标</param>
        /// <param name="y1">FPA坐标</param>
        /// <returns>非零值-缓冲区内有效像素数；0-失败。</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetLineTemperatureInfo")]
        public static extern int MAG_GetLineTemperatureInfo(uint hDevice, [MarshalAs(UnmanagedType.LPArray)] int[] pBuf, uint size, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] int[] info, uint x0, uint y0, uint x1, uint y1);

        /// <summary>
        /// 获取Rect区域温度
        /// MAG_API BOOL WINAPI MAG_GetRectTemperatureInfo(UINT intChannelIndex, UINT x0, UINT y0, UINT x1, UINT y1, int info[5]);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="x0">FPA坐标</param>
        /// <param name="y0">FPA坐标</param>
        /// <param name="x1">FPA坐标</param>
        /// <param name="y1">FPA坐标</param>
        /// <param name="info">min max ave  posMin posMax</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetRectTemperatureInfo")]
        public static extern bool MAG_GetRectTemperatureInfo(uint hDevice, uint x0, uint y0, uint x1, uint y1, [MarshalAs(UnmanagedType.LPArray, SizeConst = 5)] int[] info);

        /// <summary>
        /// 获取圆形及椭圆形区域温度
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="x0">FPA坐标</param>
        /// <param name="y0">FPA坐标</param>
        /// <param name="x1">FPA坐标</param>
        /// <param name="y1">FPA坐标</param>
        /// <param name="info">min max ave  posMin posMax</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetEllipseTemperatureInfo")]
        public static extern bool MAG_GetEllipseTemperatureInfo(uint hDevice, uint x0, uint y0, uint x1, uint y1, [MarshalAs(UnmanagedType.LPArray, SizeConst = 5)] int[] info);

        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetRgnTemperatureInfo")]
        public static extern bool MAG_GetRgnTemperatureInfo(uint hDevice, uint[] pos, uint intPosNum, [MarshalAs(UnmanagedType.LPArray, SizeConst = 5)] int[] info);

        /// <summary>
        /// 保存温度数据到SD卡
        /// MAG_SDStorageMGT(m_pDoc->m_intChannelIndex
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SDStorageMGT")]
        public static extern bool MAG_SDStorageMGT(uint hDevice);

        /// <summary>
        /// 开始录制MGS
        /// MAG_SDStorageMGSStart(m_pDoc->m_intChannelIndex)
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SDStorageMGSStart")]
        public static extern bool MAG_SDStorageMGSStart(uint hDevice);

        /// <summary>
        /// 停止录制MGS
        /// MAG_SDStorageMGSStop(m_pDoc->m_intChannelIndex)
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SDStorageMGSStop")]
        public static extern bool MAG_SDStorageMGSStop(uint hDevice);

        /// <summary>
        /// 开始录制AVI
        /// MAG_SDStorageAviStart(m_pDoc->m_intChannelIndex)
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SDStorageAviStart")]
        public static extern bool MAG_SDStorageAviStart(uint hDevice);

        /// <summary>
        /// 停止录制AVI
        /// MAG_SDStorageAviStop(m_pDoc->m_intChannelIndex)
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SDStorageAviStop")]
        public static extern bool MAG_SDStorageAviStop(uint hDevice);

        /// <summary>
        /// 保存灰度图到SD卡
        /// MAG_SDStorageBMP(m_pDoc->m_intChannelIndex)
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SDStorageBMP")]
        public static extern bool MAG_SDStorageBMP(uint hDevice);

        //MAG_API BOOL WINAPI MAG_SDCardStorage(UINT intChannelIndex, enum SDStorageFileType filetype, UINT para);
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SDCardStorage")]
        public static extern bool MAG_SDCardStorage(uint hDevice, SDStorageFileType filetype, uint para);


        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetReConnectCallBack")]
        public static extern bool MAG_SetReConnectCallBack(uint hDevice, DelegateReconnect funcReconnect, IntPtr pUser);

        /// <summary>
        /// 复位热像仪
        /// MAG_ResetCamera(m_pDoc->m_intChannelIndex)
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_ResetCamera")]
        public static extern bool MAG_ResetCamera(uint hDevice);

        /// <summary>
        /// 线程锁,运算线程使用该锁
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_LockFrame")]
        public static extern void MAG_LockFrame(uint hDevice);

        /// <summary>
        /// 线程锁,运算线程使用该锁
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_UnLockFrame")]
        public static extern void MAG_UnLockFrame(uint hDevice);

        /// <summary>
        /// 保存位图到文件
        /// MAG_API BOOL WINAPI MAG_SaveBMP(UINT intChannelIndex, DWORD dwIndex, const WCHAR * charFilename);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="index">0 - FPA, 1 - BMP, 2 - bar</param>
        /// <param name="sFileName">文件名</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SaveBMP", CharSet = CharSet.Unicode)]
        public static extern bool MAG_SaveBMP(uint hDevice, uint index, [MarshalAs(UnmanagedType.LPWStr)] string sFileName);

        /// <summary>
        /// 保存MGT文件
        /// MAG_API BOOL WINAPI MAG_SaveMGT(UINT intChannelIndex, const WCHAR * charFilename);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="sFileName">文件名</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SaveMGT", CharSet = CharSet.Unicode)]
        public static extern bool MAG_SaveMGT(uint hDevice, [MarshalAs(UnmanagedType.LPWStr)] string sFileName);

        /// <summary>
        /// 转发云台指令
        /// MAG_API BOOL WINAPI MAG_SetPTZCmd(UINT intChannelIndex, enum PTZCmd cmd, DWORD dwPara);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="cmd">云台控制命令</param>
        /// <param name="param">指令参数，对于方向类指令，本参数为云台速度(0~63)，
        ///                     对于预置位和辅助开关类指令，
        ///                     本参数为预置位编号或辅助开关编号</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetPTZCmd")]
        public static extern bool MAG_SetPTZCmd(uint hDevice, PTZIRCMD cmd, uint param);

        /// <summary>
        /// 转发串口指令
        /// MAG_API BOOL WINAPI MAG_SetSerialCmd(UINT intChannelIndex, const BYTE * buffer, UINT intBufferLen);
        /// </summary>
        /// <param name="hDevice">相机句柄</param>
        /// <param name="sCmd">命令字符串</param>
        /// <param name="intCmdLen">命令长度</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetSerialCmd")]
        public static extern bool MAG_SetSerialCmd(uint hDevice, byte[] cmd, uint intCmdLen);

        /// <summary>
        /// 自动对焦
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns>true 成功发送指令 false 发送指令失败</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_ExecAutoFocus")]
        public static extern bool MAG_ExecAutoFocus(uint hDevice);

        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetFilter")]
        public static extern bool MAG_SetFilter(uint intFilter);

        /// <summary>
        /// 获取心跳包
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetRecentHeartBeat")]
        public static extern uint MAG_GetRecentHeartBeat(uint hDevice);

        /// <summary>
        /// 微调镜头位置
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="isFar">1 为向外调，看近处</param>
        /// <param name="ms">马达运转时间,ms</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_MoveLens")]
        public static extern bool MAG_MoveLens(uint hDevice, int isFar, uint ms);

        /// <summary>
        /// 调焦马达紧急停止
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StopLensMotor")]
        public static extern bool MAG_StopLensMotor(uint hDevice);

        /// <summary>
        /// 设置用户ROI到相机
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="roi"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetUserROIs")]
        public static extern bool MAG_SetUserROIs(uint hDevice, ref USER_ROI roi);

        /// <summary>
        /// 保存DDT文件
        /// BOOL MAG_SaveDDT(UINT intChannelIndex, const WCHAR * charFilename);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="sFileName"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SaveDDT", CharSet = CharSet.Unicode)]
        public static extern bool MAG_SaveDDT(uint hDevice, [MarshalAs(UnmanagedType.LPWStr)] string sFileName);

        /// <summary>
        /// 保存DDT数据到缓冲区
        /// int MAG_SaveDDT2Buffer(UINT intChannelIndex, void * pBuffer, UINT intBufferSize);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="pBuffer"></param>
	    /// <param name="intBufferSize"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SaveDDT2Buffer", CharSet = CharSet.Unicode)]
        public static extern int MAG_SaveDDT2Buffer(uint hDevice, byte[] pBuffer, uint intBufferSize);

        /// <summary>
        /// 加载DDT文件
        /// BOOL MAG_LoadDDT(UINT intChannelIndex, OutputPara * paraOut, const WCHAR * charFilename, MAG_FRAMECALLBACK funcFrame, void * pUserData);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="param"></param>
        /// <param name="sFileName"></param>
        /// <param name="newFrame"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_LoadDDT", CharSet = CharSet.Unicode)]
        public static extern bool MAG_LoadDDT(uint hDevice, ref OUTPUT_PARAM param, [MarshalAs(UnmanagedType.LPWStr)] string sFileName, DelegateNewFrame newFrame, IntPtr pUserData);

        /// <summary>
        /// MAG_API BOOL WINAPI MAG_LoadBufferedDDT(UINT intChannelIndex, OutputPara * paraOut, const void * pBuffer, UINT intBufferSize, MAG_FRAMECALLBACK funcFrame, void * pUserData);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="param"></param>
        /// <param name="pBuffer"></param>
        /// <param name="intBufferSize"></param>
        /// <param name="newFrame"></param>
        /// <param name="pUserData"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_LoadBufferedDDT")]
        public static extern bool MAG_LoadBufferedDDT(uint hDevice, ref OUTPUT_PARAM param, IntPtr pBuffer, uint intBufferSize, DelegateNewFrame newFrame, IntPtr pUserData);

	    /// <summary>
        /// MAG_API int WINAPI MAG_CompressDDT(void * pDstBuffer, UINT intDstBufferSize, const void * pSrcBuffer, UINT intSrcBufferSize, UINT intQuality);
        /// </summary>
        /// <param name="pDstBuffer"></param>
        /// <param name="intDstBufferSize"></param>
        /// <param name="pSrcBuffer"></param>
        /// <param name="intSrcBufferSize"></param>
        /// <param name="intQuality"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_CompressDDT")]
        public static extern int MAG_CompressDDT(IntPtr pDstBuffer, uint intDstBufferSize, IntPtr pSrcBuffer, uint intSrcBufferSize, uint intQuality);

	    /// <summary>
        /// MAG_API int WINAPI MAG_DeCompressDDT(void * pDstBuffer, UINT intDstBufferSize, const void * pSrcBuffer, UINT intSrcBufferSize);
        /// </summary>
        /// <param name="pDstBuffer"></param>
        /// <param name="intDstBufferSize"></param>
        /// <param name="pSrcBuffer"></param>
        /// <param name="intSrcBufferSize"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_DeCompressDDT")]
        public static extern int MAG_DeCompressDDT(IntPtr pDstBuffer, uint intDstBufferSize, IntPtr pSrcBuffer, uint intSrcBufferSize);

        
        //MAG_API BOOL WINAPI MAG_SetAsyncCompressCallBack(UINT intChannelIndex, MAG_DDTCOMPRESSCALLBACK pCallBack, int intQuality);
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetAsyncCompressCallBack")]
        public static extern bool MAG_SetAsyncCompressCallBack(uint intChannelIndex, DelegateDDTCompressComplete funcDDTCompressComplete, int intQuality);

        //MAG_API BOOL WINAPI MAG_GrabAndAsyncCompressDDT(UINT intChannelIndex, void * pUserData);
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GrabAndAsyncCompressDDT")]
        public static extern bool MAG_GrabAndAsyncCompressDDT(uint intChannelIndex, IntPtr pUserData);


        /// <summary>
        /// MAG_API BOOL WINAPI MAG_IsLanConnected();
        /// </summary>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsLanConnected")]
        public static extern bool MAG_IsLanConnected();

        /// <summary>
        /// BOOL MAG_GetMulticastState(UINT intTargetIp, UINT * intMulticastIp, UINT * intMulticastPort, UINT intTimeoutMS);
        /// </summary>
        /// <param name="intTargetIp"></param>
        /// <param name="intMulticastIp"></param>
        /// <param name="intMulticastPort"></param>
        /// <param name="intTimeout">ms</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetMulticastState")]
        public static extern bool MAG_GetMulticastState(uint intTargetIp, ref uint intMulticastIp, ref uint intMulticastPort, uint intTimeout);

        /// <summary>
        /// 是否正在收听
        /// BOOL MAG_IsListening(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsListening")]
        public static extern bool MAG_IsListening(uint hDevice);

        /// <summary>
        /// 停止收听
        /// void WINAPI MAG_StopListen(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice"></param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StopListen")]
        public static extern void MAG_StopListen(uint hDevice);

        /// <summary>
        /// 开始收听
        /// BOOL MAG_ListenTo(UINT intChannelIndex, UINT intTargetIp);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intTargetIP">被收听的相机IP</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_ListenTo")]
        public static extern bool MAG_ListenTo(uint hDevice, uint intTargetIP);

        /// <summary>
        /// 触发FFC
        /// BOOL MAG_TriggerFFC(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_TriggerFFC")]
        public static extern bool MAG_TriggerFFC(uint hDevice);

        /// <summary>
        /// 查询云台状态
        /// BOOL MAG_QueryPTZState(UINT intChannelIndex, enum PTZQuery query, int * intValue, UINT intTimeoutMS);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="query"></param>
        /// <param name="intValue"></param>
        /// <param name="intTimeout"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_QueryPTZState")]
        public static extern bool MAG_QueryPTZState(uint hDevice, PTZQuery query, ref int intValue, uint intTimeout);

        /// <summary>
        /// 获取相机温度
        /// BOOL MAG_GetCameraTemperature(UINT intChannelIndex, int intT[4], UINT intTimeoutMS);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intT"></param>
        /// <param name="intTimeout"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetCameraTemperature")]
        public static extern bool MAG_GetCameraTemperature(uint hDevice, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]int[] intT, uint intTimeout);

        /// <summary>
        /// 开始脉冲传输
        /// BOOL MAG_StartProcessPulseImage(UINT intChannelIndex, const OutputPara * paraOut, MAG_FRAMECALLBACK funcFrame, DWORD dwStreamType, void * pUserData);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="outputParam"></param>
        /// <param name="newFrame"></param>
        /// <param name="intStreamtype"></param>
        /// <param name="pUserData"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StartProcessPulseImage")]
        public static extern bool MAG_StartProcessPulseImage(uint hDevice, ref OUTPUT_PARAM outputParam, DelegateNewFrame newFrame, uint intStreamtype, IntPtr pUserData);

        /// <summary>
        /// 传输一帧数据
        /// BOOL MAG_TransferPulseImage(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_TransferPulseImage")]
        public static extern bool MAG_TransferPulseImage(uint hDevice);

        /// <summary>
        /// 设置人工拉伸
        /// MAG_API BOOL MAG_SetSubsectionEnlargePara(UINT intChannelIndex, int intX1, int intX2, UCHAR byteY1, UCHAR byteY2);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetSubsectionEnlargePara")]
        public static extern bool MAG_SetSubsectionEnlargePara(uint hDevice, int x1, int x2, byte y1, byte y2);

        /// <summary>
        /// 设置自动拉伸
        /// void WINAPI MAG_SetAutoEnlargePara(UINT intChannelIndex, DWORD dwAutoEnlargeRange, int intBrightOffset, int intContrastOffset);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intAutoEnlargeRange"></param>
        /// <param name="intBrightOffset"></param>
        /// <param name="intContrastOffset"></param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetAutoEnlargePara")]
        public static extern void MAG_SetAutoEnlargePara(uint hDevice, uint intAutoEnlargeRange, int intBrightOffset, int intContrastOffset);

        /// <summary>
        /// 设置电子放大倍率
        /// void MAG_SetEXLevel(UINT intChannelIndex, enum EX ExLevel, int intCenterX, int intCenterY);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="ex"></param>
        /// <param name="intCenterX"></param>
        /// <param name="intCenterY"></param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetEXLevel")]
        public static extern void MAG_SetEXLevel(uint hDevice, EX ex, int intCenterX, int intCenterY);

        /// <summary>
        /// 获取电子放大倍率
        /// enum EX MAG_GetEXLevel(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetEXLevel")]
        public static extern EX MAG_GetEXLevel(uint hDevice);

        /// <summary>
        /// 设置DDE强度
        /// void MAG_SetDetailEnhancement(UINT intChannelIndex, int intDDE, BOOL bQuickDDE);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intDDE"></param>
        /// <param name="isQuickDDE"></param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetDetailEnhancement")]
        public static extern void MAG_SetDetailEnhancement(uint hDevice, int intDDE, int isQuickDDE);

        /// <summary>
        /// 设置对比度
        /// BOOL MAG_SetVideoContrast(UINT intChannelIndex, int intContrastOffset);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intContrastOffset"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetVideoContrast")]
        public static extern bool MAG_SetVideoContrast(uint hDevice, int intContrastOffset);

        /// <summary>
        /// 设置亮度
        /// BOOL WINAPI MAG_SetVideoBrightness(UINT intChannelIndex, int intBrightnessOffset);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intBrightnessOffset"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetVideoBrightness")]
        public static extern bool MAG_SetVideoBrightness(uint hDevice, int intBrightnessOffset);

        /// <summary>
        /// 获取温度数据
        ///  BOOL MAG_GetTemperatureData(UINT intChannelIndex, int * pData, UINT intBufferSize, BOOL bEnableExtCorrect);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="data"></param>
        /// <param name="intBufferSize"></param>
        /// <param name="isEnableExtCorrect"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetTemperatureData")]
        public static extern bool MAG_GetTemperatureData(uint hDevice, int[] data, uint intBufferSize, int isEnableExtCorrect);

        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetTemperatureData_Raw")]
        public static extern bool MAG_GetTemperatureData_Raw(uint hDevice, int[] data, uint intBufferSize, int isEnableExtCorrect);

        /// <summary>
        /// 开启温度mask
        /// BOOL MAG_UseTemperatureMask(UINT intChannelIndex, BOOL bUse);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="isUse"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_UseTemperatureMask")]
        public static extern bool MAG_UseTemperatureMask(uint hDevice, int isUse);

        /// <summary>
        /// 是否使用温度mask
        /// BOOL MAG_IsUsingTemperatureMask(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsUsingTemperatureMask")]
        public static extern bool MAG_IsUsingTemperatureMask(uint hDevice);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="sReferenceDDT"></param>
        /// <param name="offsetx"></param>
        /// <param name="offsety"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetCurrentOffset", CharSet = CharSet.Unicode)]
        public static extern bool MAG_GetCurrentOffset(uint hDevice, [MarshalAs(UnmanagedType.LPWStr)] string sReferenceDDT, ref int offsetx, ref int offsety);

        /// <summary>
        /// 设置等温区域,将上限温度与下限温度之间的温度用绿色来替代
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetIsothermalPara")]
        public static extern void MAG_SetIsothermalPara(uint hDevice, int intLowerLimit, int intUpperLimit);

        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetSerialCallBack")]
        public static extern bool MAG_SetSerialCallBack(uint hDevice, GroupSDK.DelegateSerial cb, IntPtr pUserData);
    }

    public class WINAPI
    {
        public enum StretchMode
        {
            STRETCH_ANDSCANS = 1,
            STRETCH_ORSCANS = 2,
            STRETCH_DELETESCANS = 3,
            STRETCH_HALFTONE = 4,
        }

        public enum PaletteMode
        {
            DIB_RGB_COLORS = 0,
            DIB_PAL_COLORS = 1
        }

        public enum ExecuteOption
        {
            SRCCOPY = 0x00CC0020, /* dest = source*/
            SRCPAINT = 0x00EE0086, /* dest = source OR dest*/
            SRCAND = 0x008800C6, /* dest = source AND dest*/
            SRCINVERT = 0x00660046, /* dest = source XOR dest*/
            SRCERASE = 0x00440328, /* dest = source AND (NOT dest )*/
            NOTSRCCOPY = 0x00330008, /* dest = (NOT source)*/
            NOTSRCERASE = 0x001100A6, /* dest = (NOT src) AND (NOT dest) */
            MERGECOPY = 0x00C000CA, /* dest = (source AND pattern)*/
            MERGEPAINT = 0x00BB0226, /* dest = (NOT source) OR dest*/
            PATCOPY = 0x00F00021, /* dest = pattern*/
            PATPAINT = 0x00FB0A09, /* dest = DPSnoo*/
            PATINVERT = 0x005A0049, /* dest = pattern XOR dest*/
            DSTINVERT = 0x00550009, /* dest = (NOT dest)*/
            BLACKNESS = 0x00000042, /* dest = BLACK*/
            WHITENESS = 0x00FF0062, /* dest = WHITE*/
        }

        [DllImport("Ws2_32.dll")]
        public static extern uint inet_addr(string ip);

        [DllImport("gdi32.dll", EntryPoint = "StretchDIBits")]
        public static extern int StretchDIBits([In] IntPtr hdc,
            int xDest, int yDest, int DestWidth, int DestHeight,
            int xSrc, int ySrc, int SrcWidth, int SrcHeight,
            [In] IntPtr lpBits, [In] IntPtr lpbmi, uint iUsage, uint rop);

        [DllImport("gdi32.dll", EntryPoint = "SetStretchBltMode")]
        public static extern int SetStretchBltMode(IntPtr hdc, StretchMode nStretchMode);
    }
}