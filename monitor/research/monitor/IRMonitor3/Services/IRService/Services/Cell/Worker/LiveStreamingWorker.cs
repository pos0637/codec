using Codec;
using Common;
using Devices;
using Miscs;
using System;
using System.Collections.Generic;
using System.Threading;

namespace IRService.Services.Cell.Worker
{
    /// <summary>
    /// 直播推流工作线程
    /// </summary>
    public class LiveStreamingWorker : BaseWorker
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

        /// <summary>
        /// 编码器
        /// </summary>
        private RTMPEncoder encoder;

        public override ARESULT Initialize(Dictionary<string, object> arguments)
        {
            cell = arguments["cell"] as CellService;
            device = arguments["device"] as IDevice;
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
            try {
                encoder = new RTMPEncoder();
                encoder.Initialize(width, height, frameRate);
                encoder.Start(uri);
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return ARESULT.E_FAIL;
            }

            EventEmitter.Instance.Subscribe(eventName, onReceiveImage);
            return base.Start();
        }

        public override void Discard()
        {
            encoder.Stop();
            encoder.Dispose();
            encoder = null;

            EventEmitter.Instance.Unsubscribe(eventName, onReceiveImage);
            base.Discard();
        }

        protected override void Run()
        {
            PinnedBuffer<byte> image = null;
            int size = width * height;

            while (!IsTerminated()) {
                // 克隆数据
                image = Arrays.Clone(this.image.buffer, image, sizeof(byte));

                try {
                    // 编码
                    encoder.Encode(image.ptr, image.ptr + size, image.ptr + size + size / 4);
                }
                catch (Exception e) {
                    Tracker.LogE(e);
                }

                Thread.Sleep(duration);
            }
        }
    }
}
