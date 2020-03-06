using Codec;
using Common;
using Devices;
using Miscs;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace IRService.Services.Cell.Worker
{
    /// <summary>
    /// 录像工作线程
    /// </summary>
    public class RecordingWorker : BaseWorker
    {
        /// <summary>
        /// 配置信息
        /// </summary>
        public Configuration configuration;

        /// <summary>
        /// 设备单元服务
        /// </summary>
        private CellService cell;

        /// <summary>
        /// 设备
        /// </summary>
        private IDevice device;

        /// <summary>
        /// 通道
        /// </summary>
        private int channel;

        /// <summary>
        /// 服务器资源地址
        /// </summary>
        private string uri;

        /// <summary>
        /// 宽度
        /// </summary>
        private int width;

        /// <summary>
        /// 高度
        /// </summary>
        private int height;

        /// <summary>
        /// 帧率
        /// </summary>
        private int frameRate;

        /// <summary>
        /// 事件名称
        /// </summary>
        private string eventName;

        /// <summary>
        /// 图像缓存
        /// </summary>
        private PinnedBuffer<byte> image;

        /// <summary>
        /// 推送间隔
        /// </summary>
        private int duration;

        /// <summary>
        /// 接收图像事件处理函数
        /// </summary>
        private EventEmitter.EventHandler onReceiveImage;

        public override ARESULT Initialize(Dictionary<string, object> arguments)
        {
            configuration = Repository.Repository.LoadConfiguation();
            cell = arguments["cell"] as CellService;
            device = arguments["device"] as IDevice;
            channel = (int)arguments["channel"];
            uri = arguments["uri"] as string;
            width = (int)arguments["width"];
            height = (int)arguments["height"];
            frameRate = (int)arguments["frameRate"];
            eventName = arguments["eventName"] as string;
            duration = 1000 / frameRate;

            onReceiveImage = (args) => {
                if ((args[0] == cell) && (args[1] == device)) {
                    image = Arrays.Clone(args[2] as PinnedBuffer<byte>, image, sizeof(byte));
                }
            };

            return base.Initialize(arguments);
        }

        public override ARESULT Start()
        {
            EventEmitter.Instance.Subscribe(eventName, onReceiveImage);
            return base.Start();
        }

        public override void Discard()
        {
            EventEmitter.Instance.Unsubscribe(eventName, onReceiveImage);
            base.Discard();
        }

        protected override void Run()
        {
            Encoder encoder = null;
            PinnedBuffer<byte> image = null;
            var size = width * height;
            var recordingDuration = configuration.information.recordingDuration;

            while (!IsTerminated()) {
                DateTime start = DateTime.Now;
                var uri = this.uri;

                try {
                    // 自动生成文件名
                    if (uri == null) {
                        uri = GenerateRecordingFilename($"{cell.cell.name}-{device.Name}-{channel}");
                    }

                    // TODO: 检查存储空间
                    encoder = new Encoder();
                    encoder.Initialize(width, height, frameRate);
                    encoder.Start(uri);
                }
                catch (Exception e) {
                    Tracker.LogE(e);
                    Thread.Sleep(3000);
                    continue;
                }

                while (!IsTerminated()) {
                    // 检查分片录像是否结束
                    if ((DateTime.Now - start).TotalSeconds > recordingDuration) {
                        encoder?.Stop();
                        encoder?.Dispose();
                        encoder = null;

                        // 保存录像信息
                        Repository.Repository.AddRecording(new Recording() {
                            cellName = cell.cell.name,
                            deviceName = device.Name,
                            channelName = channel.ToString(),
                            startTime = start,
                            endTime = DateTime.Now,
                            type = Recording.RecordingType.Local,
                            url = uri,
                            snapshotUrl = Repository.Repository.SaveYV12Image(image)
                        });
                        break;
                    }

                    // 克隆数据
                    var temp = this.image;
                    if (temp == null) {
                        Thread.Sleep(duration);
                        continue;
                    }

                    try {
                        // 编码
                        image = Arrays.Clone(temp, image, sizeof(byte));
                        encoder.Encode(image.ptr, image.ptr + size, image.ptr + size + size / 4);
                    }
                    catch (Exception e) {
                        Tracker.LogE(e);
                        Thread.Sleep(3000);
                        break;
                    }

                    Thread.Sleep(duration);
                }
            }

            encoder?.Stop();
            encoder?.Dispose();
        }

        /// <summary>
        /// 生成录像文件名
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>录像文件名</returns>
        private string GenerateRecordingFilename(string tag)
        {
            var now = DateTime.Now;
            var folder = $"{AppDomain.CurrentDomain.BaseDirectory}/{configuration.information.saveVideoPath}/{now.ToString("yyyyMMdd")}";
            var filename = $"{folder}/{now.ToString("yyyyMMddHHmmss")}-{tag}.mp4";
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            return filename;
        }
    }
}
