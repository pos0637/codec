using Common;
using Devices;
using IRMonitor2.Common;
using Miscs;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace IRMonitor2.Services.Cell.Worker
{
    /// <summary>
    /// 获取数据工作线程
    /// </summary>
    public class CaptureVideoWorker : BaseWorker
    {
        /// <summary>
        /// 设备
        /// </summary>
        private IDevice device;

        /// <summary>
        /// 红外图像
        /// </summary>
        private float[] temperature;

        /// <summary>
        /// 红外图像句柄
        /// </summary>
        private GCHandle temperatureHandle;

        /// <summary>
        /// 红外图像地址
        /// </summary>
        private IntPtr temperaturePtr;

        /// <summary>
        /// 可见光图像
        /// </summary>
        private byte[] image;

        /// <summary>
        /// 可见光图像句柄
        /// </summary>
        private GCHandle imageHandle;

        /// <summary>
        /// 可见光图像地址
        /// </summary>
        private IntPtr imagePtr;

        /// <summary>
        /// 红外图像读取间隔
        /// </summary>
        private int tempertureDuration;

        /// <summary>
        /// 可见光读取间隔
        /// </summary>
        private int videoDuration;

        public override void Dispose()
        {
            base.Dispose();

            if (temperaturePtr != IntPtr.Zero) {
                temperatureHandle.Free();
            }

            if (imagePtr != IntPtr.Zero) {
                imageHandle.Free();
            }
        }

        public override ARESULT Initialize(Dictionary<string, object> arguments)
        {
            device = arguments["device"] as IDevice;

            // 读取配置信息
            if (!device.Read(ReadMode.IrCameraParameters, null, out object outData, out _)) {
                return ARESULT.E_INVALIDARG;
            }

            // 创建资源
            Repository.Entities.Configuration.IrCameraParameters irCameraParameters = outData as Repository.Entities.Configuration.IrCameraParameters;
            temperature = new float[irCameraParameters.width * irCameraParameters.height];
            temperatureHandle = GCHandle.Alloc(temperature, GCHandleType.Pinned);
            temperaturePtr = temperatureHandle.AddrOfPinnedObject();
            tempertureDuration = 1000 / irCameraParameters.temperatureFrameRate;

            // 读取配置信息
            if (!device.Read(ReadMode.CameraParameters, null, out outData, out _)) {
                return ARESULT.E_INVALIDARG;
            }

            // 创建资源
            Repository.Entities.Configuration.CameraParameters cameraParameters = outData as Repository.Entities.Configuration.CameraParameters;
            image = new byte[cameraParameters.width * cameraParameters.height];
            imageHandle = GCHandle.Alloc(image, GCHandleType.Pinned);
            imagePtr = imageHandle.AddrOfPinnedObject();
            videoDuration = 1000 / cameraParameters.videoFrameRate;

            return base.Initialize(arguments);
        }

        protected override void Run()
        {
            // 默认间隔20ms
            var duration = 20;
            var duration1 = 0;
            var duration2 = 0;

            while (!IsTerminated()) {
                // 检查设备运行状态
                if (device.GetDeviceStatus() != DeviceStatus.Running) {
                    // 尝试再次打开设备
                    device.Open();
                    Thread.Sleep(1000);
                    continue;
                }

                // 读取红外图像
                if (duration1 > tempertureDuration) {
                    if (!device.Read(ReadMode.TemperatureArray, temperaturePtr, temperature.Length * sizeof(float))) {
                        device.Read(ReadMode.TemperatureArray, temperature, out _, out _);
                    }

                    EventEmitter.Instance.Publish(Constants.EVENT_RECEIVE_TEMPERATURE, device, temperature);
                    duration1 = 0;
                }

                // 读取可见光图像
                if (duration2 > videoDuration) {
                    if (!device.Read(ReadMode.ImageArray, imagePtr, image.Length * sizeof(byte))) {
                        device.Read(ReadMode.ImageArray, image, out _, out _);
                    }

                    EventEmitter.Instance.Publish(Constants.EVENT_RECEIVE_IMAGE, device, image);
                    duration2 = 0;
                }

                duration1 += duration;
                duration2 += duration;
                Thread.Sleep(duration);
            }
        }
    }
}
