using Common;
using Devices;
using IRService.Common;
using Miscs;
using System.Collections.Generic;
using System.Threading;

namespace IRService.Services.Cell.Worker
{
    /// <summary>
    /// 获取数据工作线程
    /// </summary>
    public class CaptureWorker : BaseWorker
    {
        /// <summary>
        /// 设备单元服务
        /// </summary>
        private CellService cell;

        /// <summary>
        /// 设备
        /// </summary>
        private IDevice device;

        /// <summary>
        /// 温度矩阵
        /// </summary>
        private PinnedBuffer<float> temperature;

        /// <summary>
        /// 红外图像
        /// </summary>
        private PinnedBuffer<byte> irImage;

        /// <summary>
        /// 可见光图像
        /// </summary>
        private PinnedBuffer<byte> image;

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

            temperature?.Dispose();
            image?.Dispose();
        }

        public override ARESULT Initialize(Dictionary<string, object> arguments)
        {
            cell = arguments["cell"] as CellService;
            device = arguments["device"] as IDevice;

            // 读取配置信息
            if (device.Read(ReadMode.IrCameraParameters, null, out object outData, out _)) {
                // 创建资源
                var irCameraParameters = outData as Repository.Entities.Configuration.IrCameraParameters;
                temperature = PinnedBuffer<float>.Alloc(irCameraParameters.temperatureWidth * irCameraParameters.temperatureHeight, irCameraParameters.temperatureWidth, irCameraParameters.temperatureHeight);
                irImage = PinnedBuffer<byte>.Alloc(irCameraParameters.width * irCameraParameters.height * 3 / 2, irCameraParameters.width, irCameraParameters.height);
                tempertureDuration = 1000 / irCameraParameters.temperatureFrameRate;
            }

            // 读取配置信息
            if (device.Read(ReadMode.CameraParameters, null, out outData, out _)) {
                // 创建资源
                var cameraParameters = outData as Repository.Entities.Configuration.CameraParameters;
                image = PinnedBuffer<byte>.Alloc(cameraParameters.width * cameraParameters.height * 3 / 2, cameraParameters.width, cameraParameters.height);
                videoDuration = 1000 / cameraParameters.videoFrameRate;
            }

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

                // 读取温度
                if (duration1 > tempertureDuration) {
                    if ((temperature != null)
                        && (device.Read(ReadMode.TemperatureArray, temperature.ptr, temperature.Length * sizeof(float))
                        || device.Read(ReadMode.TemperatureArray, temperature.buffer, out _, out _))) {                        
                        EventEmitter.Instance.Publish(Constants.EVENT_SERVICE_RECEIVE_TEMPERATURE, cell, device, temperature);
                    }

                    duration1 = 0;
                }

                // 读取可见光与红外图像
                if (duration2 > videoDuration) {
                    if ((irImage != null)
                        && (device.Read(ReadMode.IrImage, irImage.ptr, irImage.Length * sizeof(float))
                        || device.Read(ReadMode.IrImage, irImage.buffer, out _, out _))) {
                        EventEmitter.Instance.Publish(Constants.EVENT_SERVICE_RECEIVE_IRIMAGE, cell, device, irImage);
                    }

                    if ((image != null)
                        && (device.Read(ReadMode.Image, image.ptr, image.Length * sizeof(byte))
                        || device.Read(ReadMode.Image, image.buffer, out _, out _))) {
                        EventEmitter.Instance.Publish(Constants.EVENT_SERVICE_RECEIVE_IMAGE, cell, device, image);
                    }

                    duration2 = 0;
                }

                duration1 += duration;
                duration2 += duration;
                Thread.Sleep(duration);
            }
        }
    }
}
