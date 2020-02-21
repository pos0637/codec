using Common;
using Devices;
using IRService.Common;
using IRService.Services.Cell.Worker;
using Miscs;
using Repository.Entities;
using System;
using System.Collections.Generic;
using static IRService.Miscs.MethodUtils;

namespace IRService.Services.Cell
{
    /// <summary>
    /// 设备单元服务设备操作
    /// </summary>
    public partial class CellService
    {
        /// <summary>
        /// 设备列表
        /// </summary>
        public readonly List<IDevice> devices = new List<IDevice>();

        /// <summary>
        /// 工作线程列表
        /// </summary>
        public readonly List<BaseWorker> workers = new List<BaseWorker>();

        /// <summary>
        /// 直播推流工作线程列表
        /// </summary>
        public readonly List<BaseWorker> liveStreamingWorkers = new List<BaseWorker>();

        /// <summary>
        /// 接收图像事件处理函数
        /// </summary>
        private EventEmitter.EventHandler onStartLiveStreaming;

        /// <summary>
        /// 加载设备列表
        /// </summary>
        /// <returns>是否成功</returns>
        [Synchronized("devices")]
        private bool LoadDevices()
        {
            foreach (var device in cell.devices) {
                var instance = DeviceFactory.Instance.GetDevice(UUID.GenerateUUID(), device.category, device.model, device.serialNumber);
                if (instance == null) {
                    Tracker.LogE($"Load device fail: {device.model}");
                    return false;
                }

                Tracker.LogI($"Load device succeed: {device.model}");

                if (!InitializeDevice(instance, device)) {
                    Tracker.LogE($"Initialize device fail: {device.model}");
                    return false;
                }

                Tracker.LogI($"Initialize device succeed: {device.model}");

                devices.Add(instance);
            }

            return true;
        }

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <param name="instance">设备</param>
        /// <param name="device">设备配置</param>
        /// <returns>是否成功</returns>
        private bool InitializeDevice(IDevice instance, Configuration.Device device)
        {
            try {
                var category = Enum.Parse(typeof(DeviceCategory), device.category);
                switch (category) {
                    case DeviceCategory.Camera: {
                        // 启动服务线程并设置参数
                        if (!instance.Write(WriteMode.URI, device.uri)) {
                            Tracker.LogE($"Write parameters fail: {device.model}");
                            return false;
                        }

                        if (!instance.Write(WriteMode.CameraParameters, device.cameraParameters)) {
                            Tracker.LogE($"Write parameters fail: {device.model}");
                            return false;
                        }

                        // 启动获取数据工作线程
                        var worker = new CaptureVideoWorker();
                        if (ARESULT.AFAILED(worker.Initialize(new Dictionary<string, object>() { { "cell", this }, { "device", instance } }))) {
                            Tracker.LogE($"CaptureVideoWorker initialize fail: {device.model}");
                            return false;
                        }

                        if (ARESULT.AFAILED(worker.Start())) {
                            Tracker.LogE($"CaptureVideoWorker start fail: {device.model}");
                            return false;
                        }

                        workers.Add(worker);
                        Tracker.LogE($"CaptureVideoWorker start succeed: {device.model}");

                        break;
                    }
                    case DeviceCategory.IrCamera: {
                        // 启动服务线程并设置参数
                        if (!instance.Write(WriteMode.URI, device.uri)) {
                            Tracker.LogE($"Write parameters fail: {device.model}");
                            return false;
                        }

                        if (!instance.Write(WriteMode.IrCameraParameters, device.irCameraParameters)) {
                            Tracker.LogE($"Write parameters fail: {device.model}");
                            return false;
                        }

                        if (!instance.Write(WriteMode.CameraParameters, device.cameraParameters)) {
                            Tracker.LogE($"Write parameters fail: {device.model}");
                            return false;
                        }

                        // 启动获取数据工作线程
                        dynamic worker = new CaptureVideoWorker();
                        if (ARESULT.AFAILED(worker.Initialize(new Dictionary<string, object>() { { "cell", this }, { "device", instance } }))) {
                            Tracker.LogE($"CaptureVideoWorker initialize fail: {device.model}");
                            return false;
                        }

                        if (ARESULT.AFAILED(worker.Start())) {
                            Tracker.LogE($"CaptureVideoWorker start fail: {device.model}");
                            return false;
                        }

                        workers.Add(worker);
                        Tracker.LogE($"CaptureVideoWorker start succeed: {device.model}");

                        // 启动处理告警工作线程
                        worker = new ProcessAlarmWorker();
                        if (ARESULT.AFAILED(worker.Initialize(new Dictionary<string, object>() { { "cell", this }, { "device", instance } }))) {
                            Tracker.LogE($"ProcessAlarmWorker initialize fail: {device.model}");
                            return false;
                        }

                        if (ARESULT.AFAILED(worker.Start())) {
                            Tracker.LogE($"ProcessAlarmWorker start fail: {device.model}");
                            return false;
                        }

                        workers.Add(worker);
                        Tracker.LogE($"ProcessAlarmWorker start succeed: {device.model}");

                        break;
                    }
                    default:
                        return false;
                }

                return true;
            }
            catch (Exception e) {
                Tracker.LogE($"Initialize device fail: {device.model}", e);
                return false;
            }
        }

        [Synchronized("devices")]
        private void CloseDevices()
        {
            StopLiveStreaming();

            foreach (var worker in workers) {
                worker.Dispose();
            }

            foreach (var device in devices) {
                device.Close();
                device.Dispose();
            }
        }

        /// <summary>
        /// 开启直播推流
        /// </summary>
        private bool StartLiveStreaming(Dictionary<string, string> streams)
        {
            StopLiveStreaming();

            foreach (var device in devices) {
                try {
                    int width;
                    int height;
                    int frameRate;
                    string eventName;

                    var category = Enum.Parse(typeof(DeviceCategory), device.Category);
                    switch (category) {
                        case DeviceCategory.Camera: {
                            if (!device.Read(ReadMode.CameraParameters, null, out object outData, out _)) {
                                return false;
                            }

                            var cameraParameters = outData as Configuration.CameraParameters;
                            width = cameraParameters.width;
                            height = cameraParameters.height;
                            frameRate = cameraParameters.videoFrameRate;
                            eventName = Constants.EVENT_SERVICE_RECEIVE_IMAGE;
                            break;
                        }

                        case DeviceCategory.IrCamera: {
                            if (!device.Read(ReadMode.IrCameraParameters, null, out object outData, out _)) {
                                return false;
                            }

                            var irCameraParameters = outData as Configuration.IrCameraParameters;
                            width = irCameraParameters.width;
                            height = irCameraParameters.height;
                            frameRate = irCameraParameters.videoFrameRate;
                            eventName = Constants.EVENT_SERVICE_RECEIVE_IMAGE;
                            break;
                        }

                        default: {
                            Tracker.LogE($"StartLiveStreaming fail: {device.Model}");
                            return false;
                        }
                    }

                    var worker = new LiveStreamingWorker();
                    if (ARESULT.AFAILED(worker.Initialize(new Dictionary<string, object>() {
                        { "cell", this },
                        { "device", device },
                        { "uri", streams[device.Name] },
                        { "width", width },
                        { "height", height },
                        { "frameRate", frameRate },
                        { "eventName", eventName },
                    }))) {
                        Tracker.LogE($"LiveStreamingWorker initialize fail: {device.Model}");
                        return false;
                    }

                    if (ARESULT.AFAILED(worker.Start())) {
                        Tracker.LogE($"LiveStreamingWorker start fail: {device.Model}");
                        return false;
                    }

                    liveStreamingWorkers.Add(worker);
                    Tracker.LogE($"StartLiveStreaming succeed: {device.Model}");
                }
                catch (Exception e) {
                    Tracker.LogE($"StartLiveStreaming fail: {device.Model}", e);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 停止直播推流
        /// </summary>
        private void StopLiveStreaming()
        {
            // 提高停止直播流速度,不等待推流线程结束
            liveStreamingWorkers.ForEach(worker => worker.Discard());
            liveStreamingWorkers.Clear();
        }
    }
}
