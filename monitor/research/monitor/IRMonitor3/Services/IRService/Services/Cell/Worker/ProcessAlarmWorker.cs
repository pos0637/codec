using Common;
using Devices;
using IRService.Common;
using IRService.Miscs;
using Miscs;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace IRService.Services.Cell.Worker
{
    /// <summary>
    /// 处理告警工作线程
    /// </summary>
    public class ProcessAlarmWorker : BaseWorker
    {
        /// <summary>
        /// TODO: delete 配置信息
        /// </summary>
        private Configuration configuration;

        /// <summary>
        /// 设备单元服务
        /// </summary>
        private CellService cell;

        /// <summary>
        /// 设备
        /// </summary>
        private IDevice device;

        /// <summary>
        /// 红外摄像机参数
        /// </summary>
        private Configuration.IrCameraParameters irCameraParameters;

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
        /// 接收温度事件处理函数
        /// </summary>
        private EventEmitter.EventHandler onReceiveTemperature;

        /// <summary>
        /// 接收红外图像事件处理函数
        /// </summary>
        private EventEmitter.EventHandler onReceiveIrImage;

        /// <summary>
        /// 接收可见光图像事件处理函数
        /// </summary>
        private EventEmitter.EventHandler onReceiveImage;

        /// <summary>
        /// 触发告警事件处理函数
        /// </summary>
        private EventEmitter.EventHandler onAlarm;

        /// <summary>
        /// 先入先出告警队列
        /// </summary>
        private readonly FixedLengthQueue<Models.Alarm> alarms = new FixedLengthQueue<Models.Alarm>(100);

        public override ARESULT Initialize(Dictionary<string, object> arguments)
        {
            // TODO: delete 读取配置信息
            configuration = Repository.Repository.LoadConfiguation();

            cell = arguments["cell"] as CellService;
            device = arguments["device"] as IDevice;
            device.Handler += OnDeviceEvent;

            // 读取配置信息
            if (!device.Read(ReadMode.IrCameraParameters, null, out object outData, out _)) {
                return ARESULT.E_INVALIDARG;
            }

            irCameraParameters = outData as Configuration.IrCameraParameters;

            // 声明事件处理函数
            onReceiveTemperature = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    temperature = Arrays.Clone(args[2] as PinnedBuffer<float>, temperature, sizeof(float));
                }
            };

            onReceiveIrImage = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    irImage = Arrays.Clone(args[2] as PinnedBuffer<byte>, irImage, sizeof(byte));
                }
            };

            onReceiveImage = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    image = Arrays.Clone(args[2] as PinnedBuffer<byte>, image, sizeof(byte));
                }
            };

            onAlarm = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    OnAlarm(args);
                }
            };

            return base.Initialize(arguments);
        }

        public override ARESULT Start()
        {
            EventEmitter.Instance.Subscribe(Constants.EVENT_SERVICE_RECEIVE_TEMPERATURE, onReceiveTemperature);
            EventEmitter.Instance.Subscribe(Constants.EVENT_SERVICE_RECEIVE_IRIMAGE, onReceiveIrImage);
            EventEmitter.Instance.Subscribe(Constants.EVENT_SERVICE_RECEIVE_IMAGE, onReceiveImage);
            EventEmitter.Instance.Subscribe(Constants.EVENT_SERVICE_ON_ALARM, onAlarm);
            return base.Start();
        }

        public override void Discard()
        {
            alarms.Notify();
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_SERVICE_RECEIVE_TEMPERATURE, onReceiveTemperature);
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_SERVICE_RECEIVE_IRIMAGE, onReceiveIrImage);
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_SERVICE_RECEIVE_IMAGE, onReceiveImage);
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_SERVICE_ON_ALARM, onAlarm);
            base.Discard();
        }

        protected override void Run()
        {
            while (!IsTerminated()) {
                alarms.Wait(1000);

                var alarm = alarms.Dequeue();
                if (alarm != null) {
                    AddAlarm(null, alarm);
                }
            }
        }

        /// <summary>
        /// 设备事件处理函数
        /// </summary>
        /// <param name="deviceEvent">事件</param>
        /// <param name="arguments">参数</param>
        private void OnDeviceEvent(DeviceEvent deviceEvent, params object[] arguments)
        {
            switch (deviceEvent) {
                case DeviceEvent.HumanHighTemperatureAlarm: {
                    var detail = $"体温异常: {arguments[1]}";
                    OnAlarm(cell, device, Alarm.AlarmType.HumanHighTemperature, arguments[0], detail);
                    break;
                }
                default:
                    return;
            }
        }

        /// <summary>
        /// 告警事件处理函数
        /// </summary>
        /// <param name="arguments">参数</param>
        private void OnAlarm(params object[] arguments)
        {
            var type = (Alarm.AlarmType)arguments[2];
            var rect = (RectangleF?)arguments[3];
            var detail = arguments[4] as string;

            Models.Alarm alarm;
            switch (type) {
                case Alarm.AlarmType.HumanHighTemperature: {
                    alarm = new Models.Alarm() {
                        type = type,
                        temperatureType = Selections.TemperatureType.max,
                        level = Alarm.Level.General,
                        cellName = cell.cell.name,
                        deviceName = device.Name,
                        selectionName = null,
                        startTime = DateTime.Now,
                        area = rect,
                        detail = detail
                    };
                    break;
                }
                case Alarm.AlarmType.Manual: {
                    alarm = new Models.Alarm() {
                        type = type,
                        temperatureType = Selections.TemperatureType.max,
                        level = Alarm.Level.General,
                        cellName = cell.cell.name,
                        deviceName = device.Name,
                        selectionName = null,
                        startTime = DateTime.Now,
                        area = rect,
                        detail = detail
                    };
                    break;
                }
                default:
                    return;
            }

            alarm.temperature = Arrays.Clone(this.temperature, alarm.temperature, sizeof(float));
            alarm.irImage = Arrays.Clone(this.irImage, alarm.irImage, sizeof(byte));
            alarm.image = Arrays.Clone(this.image, alarm.image, sizeof(byte));

            alarms.Enqueue(alarm);
        }

        /// <summary>
        /// 添加选区告警
        /// </summary>
        /// <param name="selection">选区</param>
        /// <param name="alarm">告警</param>
        private void AddAlarm(Models.Selections.Selection selection, Models.Alarm alarm)
        {
            // TODO: 触发告警录像

            var data = new Alarm() {
                cellName = cell.cell.name,
                selectionName = selection?.Entity.name ?? null,
                startTime = DateTime.Now,
                type = alarm.type,
                temperatureType = alarm.temperatureType,
                level = alarm.level,
                area = JsonUtils.ObjectToJson(alarm.area),
                point = JsonUtils.ObjectToJson(alarm.point),
                detail = alarm.detail,
                temperatureUrl = Repository.Repository.SaveAlarmTemperature(alarm.temperature),
                irImageUrl = Repository.Repository.SaveYV12Image(alarm.irImage),
                imageUrl = Repository.Repository.SaveYV12Image(alarm.image),
                videoUrl = null,
                irCameraParameters = JsonUtils.ObjectToJson(irCameraParameters)
            };

            if (!Repository.Repository.AddAlarm(data)) {
                alarms.Enqueue(alarm);
            }

            WebMethod.AddAlarm(new WebMethod.Alarm() {
                serialNumber = configuration.information.clientId,
                datetime = alarm.startTime.ToString(),
                image = ImageUtils.ImageFileToBase64(data.imageUrl),
                irImage = ImageUtils.ImageFileToBase64(data.irImageUrl),
                temperature = ImageUtils.ImageFileToBase64(data.temperatureUrl),
                data = alarm.detail
            });
        }
    }
}
