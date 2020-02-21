﻿using Common;
using Devices;
using IRService.Common;
using IRService.Models;
using Miscs;
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
        private Repository.Entities.Configuration.IrCameraParameters irCameraParameters;

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
        /// 先入先出告警队列
        /// </summary>
        private FixedLengthQueue<Alarm> alarms = new FixedLengthQueue<Alarm>(100);

        public override ARESULT Initialize(Dictionary<string, object> arguments)
        {
            cell = arguments["cell"] as CellService;
            device = arguments["device"] as IDevice;

            // 读取配置信息
            if (!device.Read(ReadMode.IrCameraParameters, null, out object outData, out _)) {
                return ARESULT.E_INVALIDARG;
            }

            irCameraParameters = outData as Repository.Entities.Configuration.IrCameraParameters;
            device.handler += OnDeviceAlarm;

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

            return base.Initialize(arguments);
        }

        public override ARESULT Start()
        {
            EventEmitter.Instance.Subscribe(Constants.EVENT_RECEIVE_TEMPERATURE, onReceiveTemperature);
            EventEmitter.Instance.Subscribe(Constants.EVENT_RECEIVE_IRIMAGE, onReceiveIrImage);
            EventEmitter.Instance.Subscribe(Constants.EVENT_RECEIVE_IMAGE, onReceiveImage);
            return base.Start();
        }

        public override void Discard()
        {
            alarms.Notify();
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_RECEIVE_TEMPERATURE, onReceiveTemperature);
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_RECEIVE_IRIMAGE, onReceiveIrImage);
            EventEmitter.Instance.Unsubscribe(Constants.EVENT_RECEIVE_IMAGE, onReceiveImage);
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
        /// 设备告警事件处理函数
        /// </summary>
        /// <param name="deviceEvent">事件</param>
        /// <param name="arguments">参数</param>
        private void OnDeviceAlarm(DeviceEvent deviceEvent, params object[] arguments)
        {
            Alarm alarm;
            switch (deviceEvent) {
                case DeviceEvent.HumanHighTemperatureAlarm: {
                    alarm = new Alarm() {
                        type = Repository.Entities.Alarm.Type.High,
                        temperatureType = Repository.Entities.Selections.TemperatureType.max,
                        level = Repository.Entities.Alarm.Level.General,
                        cellName = cell.cell.name,
                        deviceName = device.Name,
                        selectionName = null,
                        startTime = DateTime.Now,
                        area = (RectangleF)arguments[0],
                        detail = arguments[1] as string
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
        private void AddAlarm(Selections.Selection selection, Alarm alarm)
        {
            var data = new Repository.Entities.Alarm() {
                cellName = cell.cell.name,
                selectionName = selection?.Entity.name ?? null,
                startTime = DateTime.Now,
                // AlarmType = alarm.type,
                // TemperatureType = alarm.temperatureType,
                // Alarmlevel = alarm.level,
                // Area = alarm.area,
                // Point = alarm.point,
                detail = alarm.detail,
                temperatureUrl = Repository.Repository.SaveAlarmTemperature(alarm.temperature.buffer),
                irImageUrl = Repository.Repository.SaveAlarmYV12Image(alarm.irImage.width, alarm.irImage.height, alarm.irImage.buffer),
                imageUrl = Repository.Repository.SaveAlarmYV12Image(alarm.image.width, alarm.image.height, alarm.image.buffer),
                videoUrl = null,
                // IrCameraParameters = irCameraParameters
            };

            Repository.Repository.AddAlarm(data);
        }
    }
}
