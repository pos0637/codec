using Devices;
using SDK;
using System;
using System.Runtime.InteropServices;
using ThermoGroupSample;

namespace MagnityDevice
{
    public class MagnityDevice : IDevice
    {
        #region 常量

        /// <summary>
        /// 连接相机超时时间
        /// </summary>
        private const Int32 LINK_TIMEOUT = 2000;

        #endregion

        #region 成员变量

        /// <summary>
        /// 设备状态
        /// </summary>
        private DeviceStatus mStatus = DeviceStatus.Idle;

        /// <summary>
        /// IP地址
        /// </summary>
        private String mIp;

        /// <summary>
        /// 宽度
        /// </summary>
        private const Int32 mWidth = 384;

        /// <summary>
        /// 高度
        /// </summary>
        private const Int32 mHeight = 288;

        /// <summary>
        /// 帧缓存
        /// </summary>
        private Byte[] mFrameBuffer;

        /// <summary>
        /// 温度缓存
        /// </summary>
        private Single[] mTemperatureBuffer;

        /// <summary>
        /// 温度缓存
        /// </summary>
        private Int32[] mTempBuffer;

        /// <summary>
        /// 服务
        /// </summary>
        private MagService mMagService = new MagService(IntPtr.Zero);

        /// <summary>
        /// 设备
        /// </summary>
        private MagDevice mMagDevice = new MagDevice(IntPtr.Zero);

        /// <summary>
        /// 接收帧回调函数
        /// </summary>
        private GroupSDK.DelegateNewFrame mOnNewFrame;

        /// <summary>
        /// 红外参数
        /// </summary>
        private GroupSDK.FIX_PARAM mParams = new GroupSDK.FIX_PARAM();

        #endregion

        ~MagnityDevice()
        {
            DestroyDevice();
        }

        #region 接口方法

        public override Boolean Close()
        {
            if (!Stop())
                return false;

            if (!Disconnect())
                return false;

            lock (this) {
                mStatus = DeviceStatus.Idle;
            }

            return true;
        }

        public override Boolean Control(ControlMode mode)
        {
            switch (mode) {
                case ControlMode.AutoFocus:
                    return mMagDevice.AutoFocus();

                case ControlMode.FocusNear:
                    return mMagDevice.SetPTZCmd(GroupSDK.PTZIRCMD.PTZFocusNear, 100);

                case ControlMode.FocusFar:
                    return mMagDevice.SetPTZCmd(GroupSDK.PTZIRCMD.PTZFocusFar, 100);

                default:
                    break;
            }

            return false;
        }

        public override void Dispose()
        {
            DestroyDevice();
        }

        public override DeviceStatus GetDeviceStatus()
        {
            lock (this) {
                return mStatus;
            }
        }

        public override DeviceType GetDeviceType()
        {
            return DeviceType.IrCamera;
        }

        public override Boolean Initialize()
        {
            return InitializeDevice();
        }

        public override Boolean Open()
        {
            mFrameBuffer = new Byte[mWidth * mHeight];
            mTemperatureBuffer = new Single[mWidth * mHeight];
            mTempBuffer = new Int32[mWidth * mHeight];

            if (!Connect(mIp))
                return false;

            if (!Config(mParams))
                return false;

            if (!Start()) {
                Disconnect();
                return false;
            }

            lock (this) {
                mStatus = DeviceStatus.Running;
            }

            return true;
        }

        public override Boolean Read(ReadMode mode, IntPtr dataAddr, Int32 bufferLen)
        {
            return false;
        }

        public override Boolean Read(ReadMode mode, Object data, out Int32 useLen)
        {
            useLen = 0;
            if (data == null)
                return false;

            switch (mode) {
                case ReadMode.ObjectDistance: {
                        Single? result = data as Single?;
                        result = mParams.fDistance;
                        break;
                    }

                case ReadMode.Emissivity: {
                        Single? result = data as Single?;
                        result = mParams.fEmissivity;
                        break;
                    }

                case ReadMode.AtmosphericTemperature: {
                        Single? result = data as Single?;
                        result = mParams.fTemp;
                        break;
                    }

                case ReadMode.RelativeHumidity: {
                        Single? result = data as Single?;
                        result = mParams.fRH;
                        break;
                    }

                case ReadMode.Transmission: {
                        Single? result = data as Single?;
                        result = mParams.fTaoFilter;
                        break;
                    }

                case ReadMode.TemperatureArray: {
                        IntPtr pIrData = IntPtr.Zero;
                        IntPtr pIrInfo = IntPtr.Zero;

                        mMagDevice.Lock();

                        if (!mMagDevice.GetTemperatureDataRaw(mTempBuffer, (UInt32)mTempBuffer.Length * sizeof(Int32), 1)) {
                            mMagDevice.Unlock();
                            return false;
                        }

                        mMagDevice.Unlock();

                        Single[] dst = (Single[])data;
                        for (Int32 y = 0, i = 0; y < mHeight; ++y) {
                            Int32 startOffset = (mHeight - y - 1) * mWidth;
                            for (Int32 x = 0; x < mWidth; ++x) {
                                dst[i++] = ((Single)(mTempBuffer[startOffset + x])) / 1000.0F;
                            }
                        }

                        return true;
                    }

                case ReadMode.ImageArray: {
                        IntPtr pIrData = IntPtr.Zero;
                        IntPtr pIrInfo = IntPtr.Zero;

                        mMagDevice.Lock();

                        if (!mMagDevice.GetOutputBMPdata(ref pIrData, ref pIrInfo)) {
                            mMagDevice.Unlock();
                            return false;
                        }

                        Marshal.Copy(pIrData, mFrameBuffer, 0, mFrameBuffer.Length);

                        mMagDevice.Unlock();

                        Byte[] dst = (Byte[])data;
                        for (Int32 y = 0, i = 0; y < mHeight; ++y) {
                            Int32 startOffset = (mHeight - y - 1) * mWidth;
                            for (Int32 x = 0; x < mWidth; ++x) {
                                dst[i++] = mFrameBuffer[startOffset + x];
                            }
                        }

                        return true;
                    }

                default:
                    break;
            }

            return false;
        }

        public override Boolean Write(WriteMode mode, Object data)
        {
            switch (mode) {
                case WriteMode.ConnectionString:
                    mIp = data as String;
                    break;

                case WriteMode.ObjectDistance:
                    mParams.fDistance = (Single)data;
                    if (GetDeviceStatus() == DeviceStatus.Running)
                        return Config(mParams);
                    break;

                case WriteMode.Emissivity:
                    mParams.fEmissivity = (Single)data;
                    if (GetDeviceStatus() == DeviceStatus.Running)
                        return Config(mParams);
                    break;

                case WriteMode.AtmosphericTemperature:
                    mParams.fTemp = (Single)data;
                    if (GetDeviceStatus() == DeviceStatus.Running)
                        return Config(mParams);
                    break;

                case WriteMode.RelativeHumidity:
                    mParams.fRH = (Single)data;
                    if (GetDeviceStatus() == DeviceStatus.Running)
                        return Config(mParams);
                    break;

                case WriteMode.Transmission:
                    mParams.fTaoFilter = (Single)data;
                    if (GetDeviceStatus() == DeviceStatus.Running)
                        return Config(mParams);
                    break;

                default:
                    break;
            }

            return true;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns>是否成功</returns>
        private Boolean InitializeDevice()
        {
            if (mMagService.IsInitialized() || !mMagService.Initialize())
                return false;

            if (mMagDevice.IsInitialized() || !mMagDevice.Initialize())
                return false;

            mMagService.EnableAutoReConnect(true);
            mOnNewFrame = new GroupSDK.DelegateNewFrame(OnFrameReceived);

            return true;
        }

        /// <summary>
        /// 销毁设备
        /// </summary>
        private void DestroyDevice()
        {
            mMagDevice.StopProcessImage();

            if (mMagDevice.IsInitialized())
                mMagDevice.DeInitialize();

            if (mMagService.IsInitialized())
                mMagService.DeInitialize();

            lock (this) {
                mStatus = DeviceStatus.Idle;
            }
        }

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>是否成功</returns>
        private Boolean Connect(String ip)
        {
            if (!mMagDevice.IsInitialized())
                return false;

            return mMagDevice.LinkCamera(ip, LINK_TIMEOUT);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns>是否成功</returns>
        private Boolean Disconnect()
        {
            if (!mMagDevice.IsInitialized())
                return false;

            if (!mMagDevice.IsLinked())
                return true;

            mMagDevice.DisLinkCamera();

            return true;
        }

        /// <summary>
        /// 启动设备
        /// </summary>
        /// <returns>是否成功</returns>
        private Boolean Start()
        {
            GroupSDK.CAMERA_INFO info = mMagDevice.GetCamInfo();
            GroupSDK.OUTPUT_PARAM param = new GroupSDK.OUTPUT_PARAM();
            param.intFPAWidth = (UInt32)info.intFPAWidth;
            param.intFPAHeight = (UInt32)info.intFPAHeight;
            param.intBMPWidth = (UInt32)info.intVideoWidth;
            param.intBMPHeight = (UInt32)info.intVideoHeight;
            param.intColorbarWidth = 20;
            param.intColorbarHeight = 100;

            mMagDevice.AutoFocus();

            if (!mMagDevice.StartProcessImage(param, mOnNewFrame, (UInt32)GroupSDK.STREAM_TYPE.STREAM_TEMPERATURE, IntPtr.Zero))
                return false;

            mMagDevice.SetColorPalette(GroupSDK.COLOR_PALETTE.IRONBOW);

            return true;
        }

        /// <summary>
        /// 停止设备
        /// </summary>
        /// <returns>是否成功</returns>
        private Boolean Stop()
        {
            mMagDevice.StopProcessImage();

            return true;
        }

        /// <summary>
        /// 设置红外参数
        /// </summary>
        /// <param name="param">红外参数</param>
        /// <returns>是否成功</returns>
        private Boolean Config(GroupSDK.FIX_PARAM param)
        {
            mMagDevice.SetFixPara(param, 1);

            return true;
        }

        /// <summary>
        /// 接收帧响应函数
        /// </summary>
        /// <param name="hDevice">The device.</param>
        /// <param name="intCamTemp">The cam temporary.</param>
        /// <param name="intFFCCounter">The FFC counter.</param>
        /// <param name="intCamState">State of the cam.</param>
        /// <param name="intStreamType">Type of the stream.</param>
        /// <param name="pUserData">The user data.</param>
        private void OnFrameReceived(UInt32 hDevice, Int32 intCamTemp, Int32 intFFCCounter, Int32 intCamState, Int32 intStreamType, IntPtr pUserData)
        {
            if (!mMagDevice.IsProcessingImage())
                return;
        }

        #endregion
    }
}
