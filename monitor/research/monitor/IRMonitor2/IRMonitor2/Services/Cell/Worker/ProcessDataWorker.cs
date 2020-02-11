using Common;
using Devices;
using IRMonitor2.Common;
using Miscs;
using System;
using System.Collections.Generic;
using System.Threading;

namespace IRMonitor2.Services.Cell.Worker
{
    /// <summary>
    /// 处理数据工作线程
    /// </summary>
    public class ProcessDataWorker : BaseWorker
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
        /// 红外图像
        /// </summary>
        private float[] temperature;

        /// 可见光图像
        /// </summary>
        private byte[] image;

        /// <summary>
        /// 接收温度事件处理函数
        /// </summary>
        private EventEmitter.EventHandler onReceiveTemperature;

        /// <summary>
        /// 接收可见光图像事件处理函数
        /// </summary>
        private EventEmitter.EventHandler onReceiveImage;

        /// <summary>
        /// 红外图像读取间隔
        /// </summary>
        private int tempertureDuration;

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

                Thread.Sleep(tempertureDuration);
            }
        }

        /// <summary>
        /// 计算选取温度
        /// </summary>
        /// <param name="selections">选区列表</param>
        /// <param name="temperature">温度</param>
        private void CalculateTemperature(List<Models.Selections.Selection> selections, float[] temperature)
        {
            foreach (var selection in selections) {
                selection.Calculate(temperature, irCameraParameters.width, irCameraParameters.stride, irCameraParameters.height);
            }
        }

        /// <summary>
        /// 检查选区告警
        /// </summary>
        /// <param name="selection"></param>
        private void CheckSelectionAlarm(Models.Selections.Selection selection)
        {
        }
    }
}
