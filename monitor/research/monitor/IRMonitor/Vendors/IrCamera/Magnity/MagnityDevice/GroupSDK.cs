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
            /// �������
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sName;

            /// <summary>
            /// Э��汾��
            /// </summary>
            public UInt32 intVersion;

            /// <summary>
            /// ���IP
            /// </summary>
            public UInt32 intCamIp;

            /// <summary>
            /// ���ƶ�IP
            /// </summary>
            public UInt32 intUsrIp;

            /// <summary>
            /// ���MAC��ַ
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] byCamMAC;

            /// <summary>
            /// ����ַ�
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] byPad;

            /// <summary>
            /// �б���²���
            /// </summary>
            public UInt32 intRefreshAck;

            /// <summary>
            /// �鲥/����
            /// </summary>
            public UInt32 bIsMulticast;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct OUTPUT_PARAM
        {
            /// <summary>
            /// FPA��ȣ�����ΪFPAWIDTH
            /// </summary>
            public UInt32 intFPAWidth;

            /// <summary>
            /// FPA�߶ȣ�����ΪFPAHEIGHT
            /// </summary>
            public UInt32 intFPAHeight;

            /// <summary>
            /// ���ͼ����, ����Ϊ4��������, �Ҳ�С��FPA�Ŀ��(FPAWIDTH)
            /// </summary>
            public UInt32 intBMPWidth;

            /// <summary>
            /// ���ͼ��߶�, ��С��FPA�ĸ߶�(FPAHEIGHT)
            /// </summary>
            public UInt32 intBMPHeight;

            /// <summary>
            /// ��ɫ��ͼ����, ����Ϊ4��������, ��СΪ4
            /// </summary>
            public UInt32 intColorbarWidth;

            /// <summary>
            /// ��ɫ��ͼ��߶�, ��СΪ1
            /// </summary>
            public UInt32 intColorbarHeight;

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RECT_ROI
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sROIName;

            /// <summary>
            /// �����½�Ϊԭ��, x0����С��x1
            /// </summary>
            public int x0;

            /// <summary>
            /// �����½�Ϊԭ��, y0����С��y1
            /// </summary>
            public int y0;
            public int x1;
            public int y1;

            /// <summary>
            /// ������*100������90����0.9
            /// </summary>
            public int intEmissivity;

            /// <summary>
            /// �����¶ȣ���λmC��������ʽΪIO����ͻ�����������˸
            /// </summary>
            public int intAlarmTemp;

            /// <summary>
            /// ��ʾѡ��
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
            /// ̽�����ߴ���
            /// </summary>
            public int intFPAWidth;

            /// <summary>
            /// ̽�����ߴ�߶�
            /// </summary>
            public int intFPAHeight;

            /// <summary>
            /// ����ַ�����ʵ������
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public int[] pad;

            /// <summary>
            /// �������
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sCamName;

            /// <summary>
            /// ��������
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string sTypeName;

            /// <summary>
            /// ���ͺŵ����֡��
            /// </summary>
            public int intMaxFps;

            /// <summary>
            /// ��ǰʵ�����֡��
            /// </summary>
            public int intCurrFps;

            /// <summary>
            /// (��HDMI, H.264��MPEG�����)������Ƶ����
            /// </summary>
            public int intVideoWidth;

            /// <summary>
            /// (��HDMI, H.264��MPEG�����)������Ƶ����
            /// </summary>
            public int intVideoHeight;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct CAMERA_REGCONTENT
        {
            /// <summary>
            /// ����ַ�
            /// </summary>
            public int pad0;

            /// <summary>
            /// ����������
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string sName;

            /// <summary>
            /// �Ƿ�ʹ�þ�̬IP 0-δʹ�� 1-ʹ��
            /// </summary>
            public int intIsUseStaticIP;

            /// <summary>
            /// ��̬IP�������ֽ���
            /// </summary>
            public int intStaticIp;

            /// <summary>
            /// ��̬IP����������, �����ֽ���
            /// </summary>
            public int intStaticNetMask;

            /// <summary>
            /// ͼ�������Ƿ��鲥 0 �� 1�鲥
            /// </summary>
            public int intIsMulticastImg;

            /// <summary>
            /// ͼ���鲥��ip�������ֽ���
            /// </summary>
            public int intMulticastIp;

            /// <summary>
            /// ����ַ�
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public int[] pad1;

            /// <summary>
            /// ���кţ�ֻ��
            /// </summary>
            public int intSerianNo;

            /// <summary>
            /// ����ַ�
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public int[] pad2;

            /// <summary>
            /// ��̬����
            /// </summary>
            public int intStaticGateWay;

            /// <summary>
            /// ����ַ�
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public int[] pad3;

            /// <summary>
            /// ��ǰ��ͷ���
            /// </summary>
            public int intCurrLensIndex;

            /// <summary>
            /// ����ַ�
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public int[] pad4;

            /// <summary>
            /// FFC֡������������λ֡
            /// </summary>
            public int intFFCFrameTrigger;

            /// <summary>
            /// FFC�¶ȴ���������λmC
            /// </summary>
            public int intFFCTemperatureTrigger;

            /// <summary>
            /// ����ַ�
            /// </summary>
            public int pad5;

            /// <summary>
            /// ��֡�ۼӣ���λ֡, 1,2,4,8,16
            /// </summary>
            public int intAccResponse;

            /// <summary>
            /// ����ַ�
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public int[] pad6;

            /// <summary>
            /// //����Io����, 0-FFC, 1-MGT, 2-BMP,3-�Զ���
            /// </summary>
            public int intInputIoFunction;

            #region ģ����Ƶ���ƣ���Ӱ��PC���Ͽ�����Ч��

            /// <summary>
            /// ��ɫ��
            /// </summary>
            public COLOR_PALETTE intPaletteIndex;

            /// <summary>
            /// �Ƿ������ɫ�� 0 ������ 1 ����
            /// </summary>
            public int intIsShowColorBar;

            /// <summary>
            /// �Ƿ�ʹ���˹�����
            /// </summary>
            public int intIsUseManualEnlarge;

            /// <summary>
            /// �˹�����ڵ�X1, ��λmC
            /// </summary>
            public int intEnlargeX1;

            /// <summary>
            /// �˹�����ڵ�X2, ��λmC
            /// </summary>
            public int intEnlargeX2;

            /// <summary>
            /// �˹�����ڵ�Y1, ��λΪ�Ҷȣ���Χ0~254
            /// </summary>
            public int intEnlargeY1;

            /// <summary>
            /// �˹�����ڵ�Y2, ��λΪ�Ҷȣ���Χ1~255
            /// </summary>
            public int intEnlargeY2;

            /// <summary>
            /// �Զ�������С�¶ȷ�Χ����λC���˹��ֶλҶ�������Զ��������ȣ��������2�����鲻С��5
            /// </summary>
            public int intAutoEnlargeRange;

            #endregion

            #region IO

            /// <summary>
            /// ģ����Ƶ���, 0-��,1-����,2-����,3-���¶���
            /// </summary>
            public int intAnalogPlot;

            /// <summary>
            /// IO�����¶�
            /// </summary>
            public int intAlarmTemp;

            #endregion

            #region TV standard

            /// <summary>
            /// ģ����Ƶ��ʽ
            /// </summary>
            public TV_STANDARD intTVStandard;

            /// <summary>
            /// �Ƿ������������, 0 ������ 1 ʹ��
            /// </summary>
            public int intIsCheckHeartBeat;

            /// <summary>
            /// �Ƿ�ʼ�����ģ����Ƶ����������̫������ͼ��ʱģ����Ƶ�رգ��Խ��͹��� 0 �� 1 ��
            /// </summary>
            public int intIsAlwaysAnalogOutput;

            /// <summary>
            /// ����ַ�
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public int[] pad8;

            /// <summary>
            /// ģ����Ƶ���ӱ���
            /// </summary>
            public EX EXLevel;

            /// <summary>
            /// DTXX��Ŀɼ������IP��MAGXXδ����
            /// </summary>
            public int intPartnerVisibleIp;

            /// <summary>
            /// ģ����ƵDDEǿ������, 0~32
            /// </summary>
            public int intDDE;

            /// <summary>
            /// ����ַ�
            /// </summary>
            public int pad9;

            /// <summary>
            /// ���ڲ�����
            /// </summary>
            public int intSerialBaudRate;

            /// <summary>
            /// ���ڲ�������ʽΪ (ʹ��<<24) | (����λ<<16) | (ֹͣλ<<8) | У��λ������ֹͣλ 0-1, 1-1.5, 2-2��У��λ0-None, 1-Odd, 2-Even, 3-Mark, 4-Space
            /// </summary>
            public int intSerialFeature;

            #endregion
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TEMP_STATE
        {
            /// <summary>
            /// �����ֶ�
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] pad0;

            /// <summary>
            /// ȫ���������, mc
            /// </summary>
            public int intMaxTemp;

            /// <summary>
            /// ȫ���������, mc
            /// </summary>
            public int intMinTemp;

            /// <summary>
            /// ȫ����ƽ����, mc
            /// </summary>
            public int intAveTemp;

            /// <summary>
            /// ȫ�����׼��
            /// </summary>
            public int intStdTemp;

            /// <summary>
            /// ����¶ȳ���λ��, y=int(intPosMax/FPAWIDTH), x=intPosMax-FPAWIDTH*y, ԭ����ͼ�����½�
            /// </summary>
            public int intPosMax;

            /// <summary>
            /// ����¶ȳ���λ��, y=int(intPosMin/FPAWIDTH), x=intPosMin-FPAWIDTH*y, ԭ����ͼ�����½�
            /// </summary>
            public int intPosMin;

            /// <summary>
            /// �����ֶ�
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public int[] pad1;

            /// <summary>
            /// ȫ�����¶�ʱ���׼��, ��λmC, ���������¶ȱ仯����������������֣�����ֵ�Բ����ͺŵĸ��������ǲ�����
            /// </summary>
            public int intAveNETDt;

            /// <summary>
            /// �����ֶ�
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public int[] pad2;

            /// <summary>
            /// ����ֱ��ͼ
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public int[] intHistTemperature;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct FIX_PARAM
        {
            /// <summary>
            /// Ŀ�����, ��λm, ��Χ(2*fFocalLength, ��)
            /// </summary>
            public float fDistance;

            /// <summary>
            /// ȫ��ȱʡ�����ʣ���Χ(0,1]
            /// </summary>
            public float fEmissivity;

            /// <summary>
            /// ����, ��λC
            /// </summary>
            public float fTemp;

            /// <summary>
            /// ���ʪ�ȣ���Χ(0~1)
            /// </summary>
            public float fRH;

            /// <summary>
            /// �ܼ���, ��λkm, ��Χ(0, ��)
            /// </summary>
            public float fVisDistance;

            /// <summary>
            /// ����ǿ��, ��λmm/h, ��Χ(0, ��)
            /// </summary>
            public float fRain;

            /// <summary>
            /// ��ѩǿ��, ��λmm/h, ��Χ(0, ��)
            /// </summary>
            public float fSnow;

            /// <summary>
            /// ��������1 ��ͷ������
            /// </summary>
            public float fExtrapara1;

            /// <summary>
            /// ��������2 ��ͷ������
            /// </summary>
            public float fExtrapara2;

            /// <summary>
            /// ����͸����, ֻ��
            /// </summary>
            public float fTaoAtm;

            /// <summary>
            /// (�������ⲿ��)����/�˹�Ƭ͸����
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
            /// ����
            /// </summary>
            GRAY1 = 0,

            /// <summary>
            /// ����
            /// </summary>
            GRAY2 = 1,

            /// <summary>
            /// ����
            /// </summary>
            IRONBOW = 2,

            /// <summary>
            /// �ʺ�
            /// </summary>
            RAINBOW = 3,

            /// <summary>
            /// ����
            /// </summary>
            GLOWBOW = 4,

            /// <summary>
            /// ����
            /// </summary>
            AUTUMN = 5,

            /// <summary>
            /// ����
            /// </summary>
            WINTER = 6,

            /// <summary>
            /// �Ƚ���
            /// </summary>
            HOTMETAL = 7,

            /// <summary>
            /// ����
            /// </summary>
            JET = 8,

            /// <summary>
            /// ��ɫ����
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
            PTZStop = 0,    //��λ��Ԥ��λֹͣ ����Ϊ0
            PTZRight,       //��λ ����Ϊ�˶��ٶ�0~63
            PTZLeft,        
            PTZUp,
            PTZDown,
            PTZUpRight = 5,
            PTZUpLeft,
            PTZDownRight,
            PTZDownLeft,
            PTZSetPreset = 9,//Ԥ��λ ����ΪԤ��λ���0~255
            PTZCallPreset,
            PTZClearPreset,
            PTZSetAuxiliary = 12,//�������� ����Ϊ�������ر��0~255
            PTZClearAuxiliary,
            PTZZoomStop = 14,//��ͷֹͣ�䱶 ����Ϊ0
            PTZZoomIn,//��ͷ�Ŵ� ����Ϊ����˶�ʱ��
            PTZZoomOut,//��ͷ��С
            PTZFocusStop = 17,//��ͷֹͣ���� ����Ϊ0
            PTZFocusAuto,//��ͷ�Զ��Խ� ����Ϊ0
            PTZFocusFar,//��ͷ��Զ ����Ϊ����˶�ʱ��
            PTZFocusNear,//��ͷ����
            PTZFocusGoto,//��ͷ��������λ�� ����Ϊ����λ��
        };

        public enum PTZQuery
        {
            PTZQueryPan = 0,//��ѯ��̨���½Ƕ�
            PTZQueryTilt,//��ѯ��̨���ҽǶ�
            PTZQueryZoomPosition,//��ѯzoom����λ��
            PTZQueryZoomState,//��ѯ�Ƿ�����zoom
            PTZQueryFocusPosition,//��ѯfocus����λ��
            PTZQueryFocusState,//��ѯ�Ƿ�����ִ���Զ��Խ�
        };

        public enum SDStorageFileType
        {
            SDFileBMP = 0,//����BMP�ļ�
            SDFileDDT,//����DDT�ļ�
            SDFileMGT,//����MGT�ļ�
            SDFileAVI,//����AVI�ļ�
            SDFileMGS,//����MGS�ļ�
        };

        #endregion

        #region delegate

        ///<summary>
        ///���Żص�
        ///void CALLBACK NewFrame(UINT intChannelIndex, int intCameraTemperature, DWORD dwFFCCounterdown, DWORD dwCamState, DWORD dwStreamType, void * pUserData);
        ///</summary>
        /// <param name="intChannelIndex">��ʾ���ͨ����</param>
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
        /// ���ڻص�
        /// void (CALLBACK * MAG_SERIALCALLBACK)(UINT intChannelIndex, void * pData, UINT intDataLength, void * pUserData);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="szData"></param>
        /// <param name="intDataLen"></param>
        /// <param name="pUserData"></param>
        public delegate void DelegateSerial(uint hDevice, IntPtr pRealData, uint intDataLen, IntPtr pUserData);

        ///DDTѹ���ص�
        ///void (CALLBACK * MAG_DDTCOMPRESSCALLBACK)(UINT intChannelIndex, void * pData, UINT intDataLength, void * pUserData);
        ///</summary>
        /// <param name="intChannelIndex">��ʾ���ͨ����</param>
        /// <returns></returns>
        public delegate void DelegateDDTCompressComplete(uint hDevice, IntPtr pData, uint intDataLength, IntPtr pUserData);

        #endregion

        /// <summary>
        /// ������0ͨ��,������������
        /// </summary>
        public const int MAX_INSTANCE = 32;

        ///<summary>
        ///ͨ�ų�ʼ��
        ///_stdcall BOOL MAG_Initialize(UINT intChannelIndex, HWND hWndMsg);
        ///</summary>
        /// <param name="intChannelIndex">�����Ӧ��ͨ����</param>
        /// <param name="hWnd">������Ϣ���ھ����
        /// �����������Ͽ�����ʱ��hwnd����message=WM_COMMAND, wParam=WM_APP+1001����Ϣ��
        /// �������б����ʱ��hWndMsg����message=WM_COMMAND, wParam=WM_APP+1002����Ϣ��</param>
        /// <returns>����TRUE��ʾ�ɹ�������FALSE��ʾʧ�ܡ�</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_Initialize")]
        public static extern bool MAG_Initialize(uint hDevice, IntPtr hWnd);

        ///<summary>
        ///�Ƿ��Ѿ�ͨ�ų�ʼ���ɹ�
        ///_stdcall BOOL WINAPI MAG_IsInitialized(UINT intChannelIndex);
        ///</summary>
        /// <param name="intChannelIndex">�����Ӧ��ͨ����</param>
        /// <returns>����TRUE��ʾ�ɹ�������FALSE��ʾʧ�ܡ�</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsInitialized")]
        public static extern bool MAG_IsInitialized(uint hDevice);

        ///<summary>
        ///�ͷ���Դ
        ///_stdcall BOOL MAG_Free(UINT intChannelIndex\);
        ///</summary>
        /// <param name="intChannelIndex">�����Ӧ��ͨ����</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_Free")]
        public static extern void MAG_Free(uint hDevice);

        ///<summary>
        ///ö�پ������е����
        ///_stdcall BOOL MAG_EnumCameras();
        ///</summary>
        /// <returns>����TRUE��ʾ�ɹ�������FALSE��ʾʧ�ܡ�</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_EnumCameras")]
        public static extern bool MAG_EnumCameras();

        ///<summary>
        ///�������
        ///_stdcall BOOL MAG_LinkCamera(UINT intChannelIndex, UINT intIP, UINT intTimeoutMS);
        ///</summary>
        /// <param name="intChannelIndex">�����Ӧ��ͨ����</param>
        /// <param name="intIP">���IP�������ֽ���</param>
        /// <param name="intTimeoutMS">���ӳ�ʱʱ�䣬���뵥λ��</param>
        /// <returns>����TRUE��ʾ�ɹ�������FALSE��ʾʧ�ܡ�</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_LinkCamera")]
        public static extern bool MAG_LinkCamera(uint hDevice, uint ip, uint timeout);


	    //_stdcall BOOL MAG_LinkCameraEx(UINT intChannelIndex, UINT intIP, USHORT shortPort, const char * charCloudUser, const char * charCloudPwd, UINT intCamSN, const char * charCamUser, const char * charCamPwd, UINT intTimeoutMS);
        ///</summary>
        /// <param name="intChannelIndex">�����Ӧ��ͨ����</param>
        /// <param name="intIP">���IP�������ֽ���</param>
        /// <param name="shortPort">����˿ںš�</param>
        /// <param name="charCloudUser">ThermoAny�û�����</param>
        /// <param name="charCloudPwd">ThermoAny�û����롣</param>
        /// <param name="intCamSN">ͨ��ThermoAny���ӵ����������кš�</param>
        /// <param name="charCamUser">�������û�����</param>
        /// <param name="charCamPwd">�������û����롣</param>
        /// <param name="intTimeoutMS">���ӳ�ʱʱ�䣬���뵥λ��</param>
        /// <returns>����TRUE��ʾ�ɹ�������FALSE��ʾʧ�ܡ�</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_LinkCameraEx")]
        public static extern bool MAG_LinkCameraEx(uint hDevice, uint ip, ushort port, [MarshalAs(UnmanagedType.LPStr)] string charCloudUser, 
            [MarshalAs(UnmanagedType.LPStr)] string charCloudPwd, uint intCamSN, [MarshalAs(UnmanagedType.LPStr)] string charCamUser,
           [MarshalAs(UnmanagedType.LPStr)] string charCamPwd, uint timeout);

        ///<summary>
        ///������Ͽ�����
        ///_stdcall void MAG_DisLinkCamera(UINT intChannelIndex);
        ///</summary>
        /// <param name="intChannelIndex">�����Ӧ��ͨ����</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_DisLinkCamera")]
        public static extern void MAG_DisLinkCamera(uint hDevice);

        ///<summary>
        ///�Ƿ��Ѿ����ӳɹ�
        ///_stdcall BOOL MAG_IsLinked(UINT intChannelIndex);
        /// </summary>
        /// <param name="intChannelIndex">�����Ӧ��ͨ����</param>
        /// <returns>����TRUE��ʾ�ɹ�������FALSE��ʾʧ�ܡ�</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsLinked")]
        public static extern bool MAG_IsLinked(uint hDevice);

        /// <summary>
        /// �Ƿ�ʹ���˾�̬IP
        /// </summary>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsUsingStaticIp")]
        public static extern bool MAG_IsUsingStaticIp();

        ///<summary>
        ///�õ�ö�ٵ�����������б�
        ///_stdcall MAG_API DWORD MAG_GetTerminalList(struct_TerminalList * pList, DWORD dwBufferSize);
        /// </summary>
        /// <param name="list">����б�</param>
        /// <param name="dwBufferSize">���ṩ�������ߴ磬�ֽڡ�</param>
        /// <returns>����ö�ٵ������������</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetTerminalList")]
        public static extern UInt32 MAG_GetTerminalList(IntPtr list, uint bufSize);

        ///<summary>
        ///��ʼ�����������
        ///_stdcall BOOL MAG_StartProcessImage(UINT intChannelIndex, const OutputPara * paraOut, MAG_FRAMECALLBACK funcFrame, DWORD dwStreamType, void * pUserData);
        ///</summary>
        /// <param name="intChannelIndex">�����Ӧ��ͨ����</param>
        /// <param name="paraOut">��������������</param>
        /// <param name="funcFrame">�ص�������</param>
        /// <param name="dwStreamType">���������������͡�</param>
        /// <param name="pUserData">�û����ݡ�</param>
        /// <returns>����ö�ٵ������������</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StartProcessImage")]
        public static extern bool MAG_StartProcessImage(uint hDevice, ref OUTPUT_PARAM outputParam, DelegateNewFrame newFrame, uint intStreamtype, IntPtr pUserData);

        /// <summary>
        /// ֹͣ�����������
        /// _stdcall void MAG_StopProcessImage(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice">������</param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StopProcessImage")]
        public static extern void MAG_StopProcessImage(uint hDevice);

        /// <summary>
        /// �ж��Ƿ����ڴ������ͼ��
        /// _stdcall bool MAG_IsProcessingImage(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice">������</param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsProcessingImage")]
        public static extern bool MAG_IsProcessingImage(uint hDevice);

        ///<summary>
        ///Ӧ���µ�α��ɫ
        ///_stdcall void MAG_SetColorPalette(UINT intChannelIndex, enum ColorPalette ColorPaletteIndex);
        ///</summary>
        /// <param name="intChannelIndex">�����Ӧ��ͨ����</param>
        /// <param name="ColorPaletteIndex">�����õ�α��ɫ��</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetColorPalette")]
        public static extern void MAG_SetColorPalette(uint hDevice, COLOR_PALETTE paletteIndex);

        ///<summary>
        ///�½�һ��ͨ��
        ///_stdcall BOOL MAG_NewChannel(UINT intChannelIndex);
        ///</summary>
        /// <param name="intChannelIndex">�����Ӧ��ͨ����</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_NewChannel")]
        public static extern bool MAG_NewChannel(uint hDevice);

        ///<summary>
        ///ɾ��һ��ͨ��
        ///_stdcall void MAG_DelChannel(UINT intChannelIndex);
        ///</summary>
        /// <param name="intChannelIndex">�����Ӧ��ͨ����</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_DelChannel")]
        public static extern void MAG_DelChannel(uint hDevice);

        ///<summary>
        ///�ж�ͨ���Ƿ��ѿ���
        ///_stdcall BOOL MAG_IsChannelAvailable(UINT intChannelIndex);
        ///</summary>
        /// <param name="intChannelIndex">�����Ӧ��ͨ����</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsChannelAvailable")]
        public static extern bool MAG_IsChannelAvailable(uint hDevice);

        /// <summary>
        /// DHCP�Ƿ��Ѿ���������
        /// </summary>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsDHCPServerRunning")]
        public static extern bool MAG_IsDHCPServerRunning();

        /// <summary>
        /// �ر�DHCP
        /// </summary>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StopDHCPServer")]
        public static extern bool MAG_StopDHCPServer();

        /// <summary>
        /// ����DHCP
        /// </summary>
        /// <param name="hWndMsg"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StartDHCPServer")]
        public static extern bool MAG_StartDHCPServer(IntPtr hWndMsg);

        /// <summary>
        /// �����Զ���������
        /// </summary>
        /// <param name="bEnable"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_EnableAutoReConnect")]
        public static extern bool MAG_EnableAutoReConnect(bool bEnable);

        /// <summary>
        /// ��ȡ�����Ϣ
        /// _stdcall void MAG_GetCamInfo(UINT intChannelIndex, struct_CamInfo * pInfo, UINT intSize);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="camInfo"></param>
        /// <param name="intSize"></param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetCamInfo")]
        public static extern void MAG_GetCamInfo(uint hDevice, ref CAMERA_INFO CamInfo, int intSize);

        /// <summary>
        /// ��ȡԶ��ע������
        /// _stdcall BOOL MAG_ReadCameraRegContent(UINT intChannelIndex, struct_CeRegContent * pContent, DWORD dwTimeoutMS, BOOL bReadDefaultValue);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="Content">�洢�������ע��������</param>
        /// <param name="intTimeOut">��ʱʱ��</param>
        /// <param name="isReadDefaultValue">�Ƿ��ȡ�������ò����ǵ�ǰ����</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_ReadCameraRegContent")]
        public static extern bool MAG_ReadCameraRegContent(uint hDevice, ref CAMERA_REGCONTENT Content, uint intTimeOut, int isReadDefaultValue);

        /// <summary>
        /// ����Զ��ע������
        /// MAG_API BOOL WINAPI MAG_SetCameraRegContent(UINT intChannelIndex, const struct_CeRegContent * pContent);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="Content">�ṹ�����</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetCameraRegContent")]
        public static extern bool MAG_SetCameraRegContent(uint hDevice, ref CAMERA_REGCONTENT Content);

        /// <summary>
        /// ��ȡ����IP
        /// _stdcall DWORD MAG_GetLocalIp();
        /// </summary>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetLocalIp")]
        public static extern uint MAG_GetLocalIp();

        /// <summary>
        /// ��ȡ8λα��ɫͼ��
        /// MAG_API BOOL WINAPI MAG_GetOutputBMPdata(UINT intChannelIndex, UCHAR const **  pData, BITMAPINFO const ** pInfo);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="pData">����ָ�룬���������óߴ�BMPWidth*BMPHeight�ֽڣ�out</param>
        /// <param name="pInfo">���������óߴ�1064�ֽڣ�����BITMAPINFOHEADER��256*RGBQUAD��out</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetOutputBMPdata")]
        public static extern bool MAG_GetOutputBMPdata(uint hDevice, ref IntPtr pData, ref IntPtr pInfo);

        /// <summary>
        /// ��ȡColorbar����
        /// MAG_API BOOL WINAPI MAG_GetOutputColorBardata(UINT intChannelIndex, UCHAR const ** pData, BITMAPINFO const ** pInfo);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="pData"></param>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetOutputColorBardata")]
        public static extern bool MAG_GetOutputColorBardata(uint hDevice, ref IntPtr pData, ref IntPtr pInfo);

        /// <summary>
        /// ��ȡ��Ƶ����
        /// MAG_API BOOL WINAPI MAG_GetOutputVideoData(UINT intChannelIndex, UCHAR const **  pData, BITMAPINFO const ** pInfo);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="pData"></param>
        /// <param name="pInfo"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetOutputVideoData")]
        public static extern bool MAG_GetOutputVideoData(uint hDevice, ref IntPtr pData, ref IntPtr pInfo);

        /// <summary>
        /// ��ȡĳһ����¶�ֵ
        /// MAG_API int WINAPI MAG_GetTemperatureProbe(UINT intChannelIndex, DWORD dwPosX, DWORD dwPosY, UINT intSize);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size">ƽ����Χ��һ��ȥ1��3��5��7</param>
        /// <returns>ƽ���¶�</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetTemperatureProbe")]
        public static extern int MAG_GetTemperatureProbe(uint hDevice, uint x, uint y, uint size);

        /// <summary>
        /// �����¶�
        /// MAG_API int WINAPI MAG_FixTemperature(UINT intChannelIndex, int intT, float fEmissivity, DWORD dwPosX, DWORD dwPosY);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="temp">�������¶�</param>
        /// <param name="fEmissivity">�����ʣ���Χ(0, 1]</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_FixTemperature")]
        public static extern int MAG_FixTemperature(uint hDevice, int temp, float fEmissivity, uint x, uint y);

        /// <summary>
        /// ��ȡУ��������Ӧ�ó����˳��󽫲�����
        /// MAG_API void WINAPI MAG_GetFixPara(UINT intChannelIndex, struct_FixPara * pBuffer);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="pBuf"></param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetFixPara")]
        public static extern void MAG_GetFixPara(uint hDevice, ref FIX_PARAM pParam);

        /// <summary>
        /// �����¶���������
        /// MAG_API float WINAPI MAG_SetFixPara(UINT intChannelIndex, const struct_FixPara * pBuffer, BOOL bEnableCameraCorrect);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="pParam">��������</param>
        /// <param name="bEnableCameraCorrect">�Ƿ�������ͬʱӦ���¶���������</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetFixPara")]
        public static extern float MAG_SetFixPara(uint hDevice, ref FIX_PARAM pParam, int bEnableCameraCorrect);

        /// <summary>
        /// ��ȡ�������
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetFrameStatisticalData")]
        public static extern IntPtr MAG_GetFrameStatisticalData(uint hDevice);

        /// <summary>
        /// ��ȡ�����¶�ͳ��
        /// MAG_API int WINAPI MAG_GetLineTemperatureInfo(UINT intChannelIndex, int * buffer, UINT intBufferSizeByte, int info[3], UINT x0, UINT y0, UINT x1, UINT y1);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="pBuf">������</param>
        /// <param name="size">�������ߴ磬 byte��λ</param>
        /// <param name="info">ͳ����Ϣ�� min max ave</param>
        /// <param name="x0">FPA����</param>
        /// <param name="y0">FPA����</param>
        /// <param name="x1">FPA����</param>
        /// <param name="y1">FPA����</param>
        /// <returns>����ֵ-����������Ч��������0-ʧ�ܡ�</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetLineTemperatureInfo")]
        public static extern int MAG_GetLineTemperatureInfo(uint hDevice, [MarshalAs(UnmanagedType.LPArray)] int[] pBuf, uint size, [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)] int[] info, uint x0, uint y0, uint x1, uint y1);

        /// <summary>
        /// ��ȡRect�����¶�
        /// MAG_API BOOL WINAPI MAG_GetRectTemperatureInfo(UINT intChannelIndex, UINT x0, UINT y0, UINT x1, UINT y1, int info[5]);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="x0">FPA����</param>
        /// <param name="y0">FPA����</param>
        /// <param name="x1">FPA����</param>
        /// <param name="y1">FPA����</param>
        /// <param name="info">min max ave  posMin posMax</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetRectTemperatureInfo")]
        public static extern bool MAG_GetRectTemperatureInfo(uint hDevice, uint x0, uint y0, uint x1, uint y1, [MarshalAs(UnmanagedType.LPArray, SizeConst = 5)] int[] info);

        /// <summary>
        /// ��ȡԲ�μ���Բ�������¶�
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="x0">FPA����</param>
        /// <param name="y0">FPA����</param>
        /// <param name="x1">FPA����</param>
        /// <param name="y1">FPA����</param>
        /// <param name="info">min max ave  posMin posMax</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetEllipseTemperatureInfo")]
        public static extern bool MAG_GetEllipseTemperatureInfo(uint hDevice, uint x0, uint y0, uint x1, uint y1, [MarshalAs(UnmanagedType.LPArray, SizeConst = 5)] int[] info);

        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetRgnTemperatureInfo")]
        public static extern bool MAG_GetRgnTemperatureInfo(uint hDevice, uint[] pos, uint intPosNum, [MarshalAs(UnmanagedType.LPArray, SizeConst = 5)] int[] info);

        /// <summary>
        /// �����¶����ݵ�SD��
        /// MAG_SDStorageMGT(m_pDoc->m_intChannelIndex
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SDStorageMGT")]
        public static extern bool MAG_SDStorageMGT(uint hDevice);

        /// <summary>
        /// ��ʼ¼��MGS
        /// MAG_SDStorageMGSStart(m_pDoc->m_intChannelIndex)
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SDStorageMGSStart")]
        public static extern bool MAG_SDStorageMGSStart(uint hDevice);

        /// <summary>
        /// ֹͣ¼��MGS
        /// MAG_SDStorageMGSStop(m_pDoc->m_intChannelIndex)
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SDStorageMGSStop")]
        public static extern bool MAG_SDStorageMGSStop(uint hDevice);

        /// <summary>
        /// ��ʼ¼��AVI
        /// MAG_SDStorageAviStart(m_pDoc->m_intChannelIndex)
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SDStorageAviStart")]
        public static extern bool MAG_SDStorageAviStart(uint hDevice);

        /// <summary>
        /// ֹͣ¼��AVI
        /// MAG_SDStorageAviStop(m_pDoc->m_intChannelIndex)
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SDStorageAviStop")]
        public static extern bool MAG_SDStorageAviStop(uint hDevice);

        /// <summary>
        /// ����Ҷ�ͼ��SD��
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
        /// ��λ������
        /// MAG_ResetCamera(m_pDoc->m_intChannelIndex)
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_ResetCamera")]
        public static extern bool MAG_ResetCamera(uint hDevice);

        /// <summary>
        /// �߳���,�����߳�ʹ�ø���
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_LockFrame")]
        public static extern void MAG_LockFrame(uint hDevice);

        /// <summary>
        /// �߳���,�����߳�ʹ�ø���
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_UnLockFrame")]
        public static extern void MAG_UnLockFrame(uint hDevice);

        /// <summary>
        /// ����λͼ���ļ�
        /// MAG_API BOOL WINAPI MAG_SaveBMP(UINT intChannelIndex, DWORD dwIndex, const WCHAR * charFilename);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="index">0 - FPA, 1 - BMP, 2 - bar</param>
        /// <param name="sFileName">�ļ���</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SaveBMP", CharSet = CharSet.Unicode)]
        public static extern bool MAG_SaveBMP(uint hDevice, uint index, [MarshalAs(UnmanagedType.LPWStr)] string sFileName);

        /// <summary>
        /// ����MGT�ļ�
        /// MAG_API BOOL WINAPI MAG_SaveMGT(UINT intChannelIndex, const WCHAR * charFilename);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="sFileName">�ļ���</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SaveMGT", CharSet = CharSet.Unicode)]
        public static extern bool MAG_SaveMGT(uint hDevice, [MarshalAs(UnmanagedType.LPWStr)] string sFileName);

        /// <summary>
        /// ת����ָ̨��
        /// MAG_API BOOL WINAPI MAG_SetPTZCmd(UINT intChannelIndex, enum PTZCmd cmd, DWORD dwPara);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="cmd">��̨��������</param>
        /// <param name="param">ָ����������ڷ�����ָ�������Ϊ��̨�ٶ�(0~63)��
        ///                     ����Ԥ��λ�͸���������ָ�
        ///                     ������ΪԤ��λ��Ż������ر��</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetPTZCmd")]
        public static extern bool MAG_SetPTZCmd(uint hDevice, PTZIRCMD cmd, uint param);

        /// <summary>
        /// ת������ָ��
        /// MAG_API BOOL WINAPI MAG_SetSerialCmd(UINT intChannelIndex, const BYTE * buffer, UINT intBufferLen);
        /// </summary>
        /// <param name="hDevice">������</param>
        /// <param name="sCmd">�����ַ���</param>
        /// <param name="intCmdLen">�����</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetSerialCmd")]
        public static extern bool MAG_SetSerialCmd(uint hDevice, byte[] cmd, uint intCmdLen);

        /// <summary>
        /// �Զ��Խ�
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns>true �ɹ�����ָ�� false ����ָ��ʧ��</returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_ExecAutoFocus")]
        public static extern bool MAG_ExecAutoFocus(uint hDevice);

        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetFilter")]
        public static extern bool MAG_SetFilter(uint intFilter);

        /// <summary>
        /// ��ȡ������
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetRecentHeartBeat")]
        public static extern uint MAG_GetRecentHeartBeat(uint hDevice);

        /// <summary>
        /// ΢����ͷλ��
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="isFar">1 Ϊ�������������</param>
        /// <param name="ms">�����תʱ��,ms</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_MoveLens")]
        public static extern bool MAG_MoveLens(uint hDevice, int isFar, uint ms);

        /// <summary>
        /// ����������ֹͣ
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StopLensMotor")]
        public static extern bool MAG_StopLensMotor(uint hDevice);

        /// <summary>
        /// �����û�ROI�����
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="roi"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetUserROIs")]
        public static extern bool MAG_SetUserROIs(uint hDevice, ref USER_ROI roi);

        /// <summary>
        /// ����DDT�ļ�
        /// BOOL MAG_SaveDDT(UINT intChannelIndex, const WCHAR * charFilename);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="sFileName"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SaveDDT", CharSet = CharSet.Unicode)]
        public static extern bool MAG_SaveDDT(uint hDevice, [MarshalAs(UnmanagedType.LPWStr)] string sFileName);

        /// <summary>
        /// ����DDT���ݵ�������
        /// int MAG_SaveDDT2Buffer(UINT intChannelIndex, void * pBuffer, UINT intBufferSize);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="pBuffer"></param>
	    /// <param name="intBufferSize"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SaveDDT2Buffer", CharSet = CharSet.Unicode)]
        public static extern int MAG_SaveDDT2Buffer(uint hDevice, byte[] pBuffer, uint intBufferSize);

        /// <summary>
        /// ����DDT�ļ�
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
        /// �Ƿ���������
        /// BOOL MAG_IsListening(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_IsListening")]
        public static extern bool MAG_IsListening(uint hDevice);

        /// <summary>
        /// ֹͣ����
        /// void WINAPI MAG_StopListen(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice"></param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_StopListen")]
        public static extern void MAG_StopListen(uint hDevice);

        /// <summary>
        /// ��ʼ����
        /// BOOL MAG_ListenTo(UINT intChannelIndex, UINT intTargetIp);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intTargetIP">�����������IP</param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_ListenTo")]
        public static extern bool MAG_ListenTo(uint hDevice, uint intTargetIP);

        /// <summary>
        /// ����FFC
        /// BOOL MAG_TriggerFFC(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_TriggerFFC")]
        public static extern bool MAG_TriggerFFC(uint hDevice);

        /// <summary>
        /// ��ѯ��̨״̬
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
        /// ��ȡ����¶�
        /// BOOL MAG_GetCameraTemperature(UINT intChannelIndex, int intT[4], UINT intTimeoutMS);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intT"></param>
        /// <param name="intTimeout"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetCameraTemperature")]
        public static extern bool MAG_GetCameraTemperature(uint hDevice, [MarshalAs(UnmanagedType.LPArray, SizeConst = 4)]int[] intT, uint intTimeout);

        /// <summary>
        /// ��ʼ���崫��
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
        /// ����һ֡����
        /// BOOL MAG_TransferPulseImage(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_TransferPulseImage")]
        public static extern bool MAG_TransferPulseImage(uint hDevice);

        /// <summary>
        /// �����˹�����
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
        /// �����Զ�����
        /// void WINAPI MAG_SetAutoEnlargePara(UINT intChannelIndex, DWORD dwAutoEnlargeRange, int intBrightOffset, int intContrastOffset);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intAutoEnlargeRange"></param>
        /// <param name="intBrightOffset"></param>
        /// <param name="intContrastOffset"></param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetAutoEnlargePara")]
        public static extern void MAG_SetAutoEnlargePara(uint hDevice, uint intAutoEnlargeRange, int intBrightOffset, int intContrastOffset);

        /// <summary>
        /// ���õ��ӷŴ���
        /// void MAG_SetEXLevel(UINT intChannelIndex, enum EX ExLevel, int intCenterX, int intCenterY);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="ex"></param>
        /// <param name="intCenterX"></param>
        /// <param name="intCenterY"></param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetEXLevel")]
        public static extern void MAG_SetEXLevel(uint hDevice, EX ex, int intCenterX, int intCenterY);

        /// <summary>
        /// ��ȡ���ӷŴ���
        /// enum EX MAG_GetEXLevel(UINT intChannelIndex);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_GetEXLevel")]
        public static extern EX MAG_GetEXLevel(uint hDevice);

        /// <summary>
        /// ����DDEǿ��
        /// void MAG_SetDetailEnhancement(UINT intChannelIndex, int intDDE, BOOL bQuickDDE);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intDDE"></param>
        /// <param name="isQuickDDE"></param>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetDetailEnhancement")]
        public static extern void MAG_SetDetailEnhancement(uint hDevice, int intDDE, int isQuickDDE);

        /// <summary>
        /// ���öԱȶ�
        /// BOOL MAG_SetVideoContrast(UINT intChannelIndex, int intContrastOffset);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intContrastOffset"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetVideoContrast")]
        public static extern bool MAG_SetVideoContrast(uint hDevice, int intContrastOffset);

        /// <summary>
        /// ��������
        /// BOOL WINAPI MAG_SetVideoBrightness(UINT intChannelIndex, int intBrightnessOffset);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="intBrightnessOffset"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_SetVideoBrightness")]
        public static extern bool MAG_SetVideoBrightness(uint hDevice, int intBrightnessOffset);

        /// <summary>
        /// ��ȡ�¶�����
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
        /// �����¶�mask
        /// BOOL MAG_UseTemperatureMask(UINT intChannelIndex, BOOL bUse);
        /// </summary>
        /// <param name="hDevice"></param>
        /// <param name="isUse"></param>
        /// <returns></returns>
        [DllImport("ThermoGroupSDKLib.dll", EntryPoint = "MAG_UseTemperatureMask")]
        public static extern bool MAG_UseTemperatureMask(uint hDevice, int isUse);

        /// <summary>
        /// �Ƿ�ʹ���¶�mask
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
        /// ���õ�������,�������¶��������¶�֮����¶�����ɫ�����
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