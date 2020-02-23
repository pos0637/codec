using Common;
using Devices;
using IRService.Common;
using IRService.Services.Cell.Worker;
using Miscs;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.IO;
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
        /// 录像工作线程列表
        /// </summary>
        public readonly List<BaseWorker> recordingWorkers = new List<BaseWorker>();

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
                if (!device.enabled) {
                    continue;
                }

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
        /// <param name="streams">推流资源地址列表</param>
        /// <returns>是否成功</returns>
        private bool StartLiveStreaming(Dictionary<int, string> streams)
        {
            StopLiveStreaming();

            foreach (var device in devices) {
                try {
                    var category = Enum.Parse(typeof(DeviceCategory), device.Category);
                    switch (category) {
                        case DeviceCategory.Camera: {
                            if (!device.Read(ReadMode.CameraParameters, null, out object outData, out _)) {
                                return false;
                            }

                            var cameraParameters = outData as Configuration.CameraParameters;
                            if (!CreateLiveStreamingWorker(device, streams[cameraParameters.channel], cameraParameters.width, cameraParameters.height, cameraParameters.videoFrameRate, Constants.EVENT_SERVICE_RECEIVE_IMAGE)) {
                                return false;
                            }

                            break;
                        }

                        case DeviceCategory.IrCamera: {
                            if (!device.Read(ReadMode.CameraParameters, null, out object outData, out _)) {
                                return false;
                            }

                            var cameraParameters = outData as Configuration.CameraParameters;
                            if (!CreateLiveStreamingWorker(device, streams[cameraParameters.channel], cameraParameters.width, cameraParameters.height, cameraParameters.videoFrameRate, Constants.EVENT_SERVICE_RECEIVE_IMAGE)) {
                                return false;
                            }

                            if (!device.Read(ReadMode.IrCameraParameters, null, out outData, out _)) {
                                return false;
                            }

                            var irCameraParameters = outData as Configuration.IrCameraParameters;
                            if (!CreateLiveStreamingWorker(device, streams[irCameraParameters.channel], irCameraParameters.width, irCameraParameters.height, irCameraParameters.videoFrameRate, Constants.EVENT_SERVICE_RECEIVE_IRIMAGE)) {
                                return false;
                            }

                            break;
                        }

                        default: {
                            Tracker.LogE($"StartLiveStreaming fail: {device.Model}");
                            return false;
                        }
                    }
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
            // 提高停止直播推流速度,不等待推流线程结束
            liveStreamingWorkers.ForEach(worker => worker.Discard());
            liveStreamingWorkers.Clear();
        }

        /// <summary>
        /// 创建直播推流工作线程
        /// </summary>
        /// <param name="device">设备</param>
        /// <param name="uri">资源地址</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="frameRate">帧率</param>
        /// <param name="eventName">事件名称</param>
        /// <returns>是否成功</returns>
        private bool CreateLiveStreamingWorker(IDevice device, string uri, int width, int height, int frameRate, string eventName)
        {
            var worker = new LiveStreamingWorker();
            if (ARESULT.AFAILED(worker.Initialize(new Dictionary<string, object>() {
                { "cell", this },
                { "device", device },
                { "uri", uri },
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

            return true;
        }

        /// <summary>
        /// 开启录像
        /// </summary>
        /// <returns>是否成功</returns>
        private bool StartRecording()
        {
            StopRecording();

            foreach (var device in devices) {
                try {
                    var category = Enum.Parse(typeof(DeviceCategory), device.Category);
                    switch (category) {
                        case DeviceCategory.Camera: {
                            if (!device.Read(ReadMode.CameraParameters, null, out object outData, out _)) {
                                return false;
                            }

                            var cameraParameters = outData as Configuration.CameraParameters;
                            if (!CreateRecordingWorker(device, GenerateRecordingFilename($"{device.Model}-{cameraParameters.channel}"), cameraParameters.width, cameraParameters.height, cameraParameters.videoFrameRate, Constants.EVENT_SERVICE_RECEIVE_IMAGE)) {
                                return false;
                            }

                            break;
                        }

                        case DeviceCategory.IrCamera: {
                            if (!device.Read(ReadMode.CameraParameters, null, out object outData, out _)) {
                                return false;
                            }

                            var cameraParameters = outData as Configuration.CameraParameters;
                            if (!CreateRecordingWorker(device, GenerateRecordingFilename($"{device.Model}-{cameraParameters.channel}"), cameraParameters.width, cameraParameters.height, cameraParameters.videoFrameRate, Constants.EVENT_SERVICE_RECEIVE_IMAGE)) {
                                return false;
                            }

                            if (!device.Read(ReadMode.IrCameraParameters, null, out outData, out _)) {
                                return false;
                            }

                            var irCameraParameters = outData as Configuration.IrCameraParameters;
                            if (!CreateRecordingWorker(device, GenerateRecordingFilename($"{device.Model}-{irCameraParameters.channel}"), irCameraParameters.width, irCameraParameters.height, irCameraParameters.videoFrameRate, Constants.EVENT_SERVICE_RECEIVE_IRIMAGE)) {
                                return false;
                            }

                            break;
                        }

                        default: {
                            Tracker.LogE($"StartLiveStreaming fail: {device.Model}");
                            return false;
                        }
                    }
                }
                catch (Exception e) {
                    Tracker.LogE($"StartLiveStreaming fail: {device.Model}", e);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 停止录像
        /// </summary>
        private void StopRecording()
        {
            // 提高停止录像速度,不等待录像线程结束
            recordingWorkers.ForEach(worker => worker.Discard());
            recordingWorkers.Clear();
        }

        /// <summary>
        /// 生成录像文件名
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>录像文件名</returns>
        private string GenerateRecordingFilename(string tag)
        {
            var configuation = Repository.Repository.LoadConfiguation();
            var now = DateTime.Now;
            var folder = $"{configuation.information.saveVideoPath}/{now.Year}-{now.Month}-{now.Day}";
            var filename = $"{folder}/{now.ToString("yyyyMMddHHmmss")}-{tag}.mp4";
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            return filename;
        }

        /// <summary>
        /// 创建录像工作线程
        /// </summary>
        /// <param name="device">设备</param>
        /// <param name="uri">资源地址</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="frameRate">帧率</param>
        /// <param name="eventName">事件名称</param>
        /// <returns>是否成功</returns>
        private bool CreateRecordingWorker(IDevice device, string uri, int width, int height, int frameRate, string eventName)
        {
            var worker = new RecordingWorker();
            if (ARESULT.AFAILED(worker.Initialize(new Dictionary<string, object>() {
                { "cell", this },
                { "device", device },
                { "uri", uri },
                { "width", width },
                { "height", height },
                { "frameRate", frameRate },
                { "eventName", eventName },
            }))) {
                Tracker.LogE($"RecordingWorker initialize fail: {device.Model}");
                return false;
            }

            if (ARESULT.AFAILED(worker.Start())) {
                Tracker.LogE($"RecordingWorker start fail: {device.Model}");
                return false;
            }

            liveStreamingWorkers.Add(worker);
            Tracker.LogE($"StartRecording succeed: {device.Model}");

            return true;
        }
    }
}
