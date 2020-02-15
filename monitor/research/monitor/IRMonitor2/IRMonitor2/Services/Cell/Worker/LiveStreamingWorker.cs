using Common;
using Devices;
using IRMonitor2.Common;
using Miscs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IRMonitor2.Services.Cell.Worker
{
    /// <summary>
    /// 直播推流工作线程
    /// </summary>
    public class LiveStreamingWorker : BaseWorker
    {
        /// <summary>
        /// 设备单元服务
        /// </summary>
        private CellService service;

        /// <summary>
        /// 设备
        /// </summary>
        private IDevice device;

        /// <summary>
        /// 红外摄像机参数
        /// </summary>
        private Repository.Entities.Configuration.IrCameraParameters irCameraParameters;

        /// <summary>
        /// 可见光摄像机参数
        /// </summary>
        private Repository.Entities.Configuration.CameraParameters cameraParameters;

        /// <summary>
        /// 红外图像
        /// </summary>
        private PinnedBuffer<float> temperature;

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

        /// <summary>
        /// 接收温度事件处理函数
        /// </summary>
        private EventEmitter.EventHandler onReceiveTemperature;

        /// <summary>
        /// 接收可见光图像事件处理函数
        /// </summary>
        private EventEmitter.EventHandler onReceiveImage;

        public override ARESULT Initialize(Dictionary<string, object> arguments)
        {
            service = arguments["service"] as CellService;
            device = arguments["device"] as IDevice;

            // 读取配置信息
            if (!device.Read(ReadMode.IrCameraParameters, null, out object outData, out _)) {
                return ARESULT.E_INVALIDARG;
            }

            irCameraParameters = outData as Repository.Entities.Configuration.IrCameraParameters;
            tempertureDuration = 1000 / irCameraParameters.temperatureFrameRate;

            // 读取配置信息
            if (!device.Read(ReadMode.CameraParameters, null, out outData, out _)) {
                return ARESULT.E_INVALIDARG;
            }

            cameraParameters = outData as Repository.Entities.Configuration.CameraParameters;
            videoDuration = 1000 / cameraParameters.videoFrameRate;

            // 声明事件处理函数
            onReceiveTemperature = (arguments) => {
                if (arguments[0] == device) {
                    temperature = Arrays.Clone(arguments[1] as float[], temperature);
                }
            };

            onReceiveImage = (arguments) => {
                if (arguments[0] == device) {
                    image = Arrays.Clone(arguments[1] as byte[], image);
                }
            };

            return base.Initialize(arguments);
        }

        public override ARESULT Start()
        {
            EventEmitter.Instance.Subscribe(Constants.EVENT_RECEIVE_TEMPERATURE, onReceiveTemperature);
            EventEmitter.Instance.Subscribe(Constants.EVENT_RECEIVE_IMAGE, onReceiveImage);
            return base.Start();
        }

        public override void Discard()
        {
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_RECEIVE_TEMPERATURE, onReceiveTemperature);
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_RECEIVE_IMAGE, onReceiveImage);
            base.Discard();
        }

        protected override void Run()
        {
            float[] temperature = null;

            while (!IsTerminated()) {
                // 克隆数据
                var selections = service.selections.Clone();
                temperature = Arrays.Clone(this.temperature, temperature);

                // 计算选取温度
                CalculateTemperature(selections, temperature);
                // 检查选区告警
                CheckSelectionAlarm(selections);

                Thread.Sleep(tempertureDuration);
            }
        }

        /// <summary>
        /// 创建视频数据
        /// </summary>
        /// <param name="size">原始数据大小</param>
        private void CreateImageBuffer(int size)
        {
            if (imageGCHandle.IsAllocated) {
                imageGCHandle.Free();
            }

            // 分配YUV视频数据空间
            imageSize = size;
            image = new byte[imageSize * 3 / 2];
            imageGCHandle = GCHandle.Alloc(image, GCHandleType.Pinned);
        }
    }
}
