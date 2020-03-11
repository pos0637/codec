using Common;
using Devices;
using Miscs;
using SDK;
using System;
using ThermoGroupSample;

namespace MagnityIrDevice
{
    public class MagnityIrDevice : IDevice
    {
        #region 常量

        /// <summary>
        /// 连接相机超时时间
        /// </summary>
        private const int LINK_TIMEOUT = 2000;

        #endregion

        #region 成员变量

        /// <summary>
        /// IP地址
        /// </summary>
        private string ip;

        /// <summary>
        /// 红外摄像机参数
        /// </summary>
        private Repository.Entities.Configuration.IrCameraParameters irCameraParameters;

        /// <summary>
        /// 服务
        /// </summary>
        private readonly MagService service = new MagService(IntPtr.Zero);

        /// <summary>
        /// 设备
        /// </summary>
        private readonly MagDevice device = new MagDevice(IntPtr.Zero);

        /// <summary>
        /// 接收帧回调函数
        /// </summary>
        private GroupSDK.DelegateNewFrame onNewFrame;

        /// <summary>
        /// 红外参数
        /// </summary>
        private GroupSDK.FIX_PARAM parameters = new GroupSDK.FIX_PARAM();

        /// <summary>
        /// 温度帧缓存
        /// </summary>
        private PinnedBuffer<int> temperatureBuffer;

        /// <summary>
        /// 红外图像帧缓存
        /// </summary>
        private PinnedBuffer<byte> irImageBuffer;

        #endregion

        ~MagnityIrDevice()
        {
            DestroyDevice();
        }

        #region 接口方法

        public override void Dispose()
        {
            DestroyDevice();
        }

        public override DeviceStatus GetDeviceStatus()
        {
            lock (this) {
                return status;
            }
        }

        public override DeviceCategory GetDeviceType()
        {
            return DeviceCategory.IrCamera;
        }

        public override bool Initialize()
        {
            return InitializeDevice();
        }

        public override bool Open()
        {
            temperatureBuffer = PinnedBuffer<int>.Alloc(irCameraParameters.temperatureWidth * irCameraParameters.temperatureHeight);
            irImageBuffer = PinnedBuffer<byte>.Alloc(irCameraParameters.width * irCameraParameters.height * 3 / 2);

            if (!Connect(ip)) {
                return false;
            }

            if (!Config(parameters)) {
                return false;
            }

            if (!Start()) {
                Disconnect();
                return false;
            }

            lock (this) {
                status = DeviceStatus.Running;
            }

            return true;
        }

        public override bool Close()
        {
            if (!Stop()) {
                return false;
            }

            if (!Disconnect()) {
                return false;
            }

            lock (this) {
                status = DeviceStatus.Idle;
            }

            return true;
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
                    outData = parameters.fDistance;
                    break;
                }

                case ReadMode.Emissivity: {
                    outData = parameters.fEmissivity;
                    break;
                }

                case ReadMode.AtmosphericTemperature: {
                    outData = parameters.fTemp;
                    break;
                }

                case ReadMode.RelativeHumidity: {
                    outData = parameters.fRH;
                    break;
                }

                case ReadMode.Transmission: {
                    outData = parameters.fTaoFilter;
                    break;
                }

                case ReadMode.TemperatureArray: {
                    var dst = (float[])inData;
                    var length = irCameraParameters.temperatureWidth * irCameraParameters.temperatureHeight;
                    if (dst.Length != length) {
                        Tracker.LogE("invalid irCameraParameters");
                        return false;
                    }

                    GetTemperatures(ref temperatureBuffer);

                    var buffer = temperatureBuffer.buffer;
                    for (var i = 0; i < length; ++i) {
                        dst[i] = buffer[i] / 1000.0F;
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

                    GetIrImage(ref irImageBuffer);
                    Buffer.BlockCopy(irImageBuffer.buffer, 0, dst, 0, length);

                    return true;
                }

                case ReadMode.IrCameraParameters: {
                    outData = irCameraParameters;
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
                    ip = dict["ip"];
                    break;
                }

                case WriteMode.ObjectDistance: {
                    parameters.fDistance = (float)data;
                    if (GetDeviceStatus() == DeviceStatus.Running) {
                        return Config(parameters);
                    }
                    break;
                }

                case WriteMode.Emissivity: {
                    parameters.fEmissivity = (float)data;
                    if (GetDeviceStatus() == DeviceStatus.Running) {
                        return Config(parameters);
                    }
                    break;
                }

                case WriteMode.AtmosphericTemperature: {
                    parameters.fTemp = (float)data;
                    if (GetDeviceStatus() == DeviceStatus.Running) {
                        return Config(parameters);
                    }
                    break;
                }

                case WriteMode.RelativeHumidity: {
                    parameters.fRH = (float)data;
                    if (GetDeviceStatus() == DeviceStatus.Running) {
                        return Config(parameters);
                    }
                    break;
                }

                case WriteMode.Transmission: {
                    parameters.fTaoFilter = (Single)data;
                    if (GetDeviceStatus() == DeviceStatus.Running) {
                        return Config(parameters);
                    }
                    break;
                }

                case WriteMode.IrCameraParameters: {
                    irCameraParameters = data as Repository.Entities.Configuration.IrCameraParameters;
                    return true;
                }

                default:
                    break;
            }

            return true;
        }

        public override bool Control(ControlMode mode, object data)
        {
            switch (mode) {
                case ControlMode.AutoFocus: {
                    return device.AutoFocus();
                }

                case ControlMode.FocusFar: {
                    return device.SetPTZCmd(GroupSDK.PTZIRCMD.PTZFocusNear, 100);
                }

                case ControlMode.FocusNear: {
                    return device.SetPTZCmd(GroupSDK.PTZIRCMD.PTZFocusFar, 100);
                }

                default:
                    return false;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns>是否成功</returns>
        private bool InitializeDevice()
        {
            if (service.IsInitialized() || !service.Initialize()) {
                return false;
            }

            if (device.IsInitialized() || !device.Initialize()) {
                return false;
            }

            service.EnableAutoReConnect(true);
            onNewFrame = new GroupSDK.DelegateNewFrame(OnFrameReceived);

            return true;
        }

        /// <summary>
        /// 销毁设备
        /// </summary>
        private void DestroyDevice()
        {
            device.StopProcessImage();

            if (device.IsInitialized()) {
                device.DeInitialize();
            }

            if (service.IsInitialized()) {
                service.DeInitialize();
            }
        }

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns>是否成功</returns>
        private bool Connect(string ip)
        {
            if (!device.IsInitialized()) {
                return false;
            }

            return device.LinkCamera(ip, LINK_TIMEOUT);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns>是否成功</returns>
        private bool Disconnect()
        {
            if (!device.IsInitialized()) {
                return false;
            }

            if (!device.IsLinked()) {
                return true;
            }

            device.DisLinkCamera();

            return true;
        }

        /// <summary>
        /// 启动设备
        /// </summary>
        /// <returns>是否成功</returns>
        private bool Start()
        {
            var info = device.GetCamInfo();
            var param = new GroupSDK.OUTPUT_PARAM {
                intFPAWidth = (uint)info.intFPAWidth,
                intFPAHeight = (uint)info.intFPAHeight,
                intBMPWidth = (uint)info.intVideoWidth,
                intBMPHeight = (uint)info.intVideoHeight,
                intColorbarWidth = 20,
                intColorbarHeight = 100
            };

            device.AutoFocus();

            if (!device.StartProcessImage(param, onNewFrame, (uint)GroupSDK.STREAM_TYPE.STREAM_TEMPERATURE, IntPtr.Zero)) {
                return false;
            }

            device.SetColorPalette(GroupSDK.COLOR_PALETTE.IRONBOW);

            return true;
        }

        /// <summary>
        /// 停止设备
        /// </summary>
        /// <returns>是否成功</returns>
        private bool Stop()
        {
            device.StopProcessImage();

            return true;
        }

        /// <summary>
        /// 设置红外参数
        /// </summary>
        /// <param name="param">红外参数</param>
        /// <returns>是否成功</returns>
        private bool Config(GroupSDK.FIX_PARAM param)
        {
            device.SetFixPara(param, 1);

            return true;
        }

        /// <summary>
        /// 获取温度数据
        /// </summary>
        /// <param name="temperatureBuffer">温度帧缓存</param>
        private void GetTemperatures(ref PinnedBuffer<int> temperatureBuffer)
        {
            device.Lock();

            if (!device.GetTemperatureDataRaw(temperatureBuffer.buffer, (uint)temperatureBuffer.Length * sizeof(int), 1)) {
                Tracker.LogE("GetTemperatureDataRaw fail");
            }

            device.Unlock();
        }

        /// <summary>
        /// 获取红外图像
        /// </summary>
        /// <param name="irImageBuffer">红外图像帧缓存</param>
        private unsafe void GetIrImage(ref PinnedBuffer<byte> irImageBuffer)
        {
            var data = IntPtr.Zero;
            var info = IntPtr.Zero;

            device.Lock();

            if (!device.GetOutputBMPdata(ref data, ref info)) {
                Tracker.LogE("GetOutputBMPdata fail");
                device.Unlock();
                return;
            }
            else {
                Buffer.MemoryCopy(data.ToPointer(), irImageBuffer.ptr.ToPointer(), irImageBuffer.Length, irImageBuffer.Length);
            }

            device.Unlock();
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
        private void OnFrameReceived(uint hDevice, int intCamTemp, int intFFCCounter, int intCamState, int intStreamType, IntPtr pUserData)
        {
            if (!device.IsProcessingImage()) {
                return;
            }
        }

        #endregion
    }
}
