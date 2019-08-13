using Codec;
using Common;
using IRMonitor.Common;
using IRMonitor.Miscs;
using IRMonitor.Services.Cell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using static Common.BaseWorker;

namespace IRMonitor.Services.LiveStreaming
{
    /// <summary>
    /// 直播推流服务
    /// </summary>
    public class LiveStreamingService : Service, IExecutor, IDisposable
    {
        /// <summary>
        /// 用户数据
        /// </summary>
        private struct UserData
        {
            /// <summary>
            /// 温度数据
            /// </summary>
            public string temperature;

            /// <summary>
            /// 选区
            /// </summary>
            public string selections;

            /// <summary>
            /// 组选区
            /// </summary>
            public string groupSelections;
        }

        /// <summary>
        /// 设备单元服务
        /// </summary>
        private CellService cell;

        /// <summary>
        /// 视频数据
        /// </summary>
        private byte[] image;

        /// <summary>
        /// 视频数据大小
        /// </summary>
        private int imageSize;

        /// <summary>
        /// GCHandle
        /// </summary>
        private GCHandle imageGCHandle;

        /// <summary>
        /// 温度数据
        /// </summary>
        private byte[] temperature;

        /// <summary>
        /// 编码器
        /// </summary>
        private RTMPEncoder encoder = new RTMPEncoder();

        /// <summary>
        /// 流索引
        /// </summary>
        private string streamId;

        /// <summary>
        /// 工作线程
        /// </summary>
        private BaseWorker worker;

        public override void Dispose()
        {
            if (worker != null) {
                worker.Discard();
                worker.Join();
            }

            if (cell != null) {
                cell.OnImageCallback -= OnImageCallback;
            }

            if (imageGCHandle.IsAllocated) {
                imageGCHandle.Free();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Initialize(Dictionary<string, object> arguments)
        {
            cell = CellServiceManager.gIRServiceList[(int)arguments["CellId"]];
            streamId = arguments["StreamId"] as string;

            cell.OnImageCallback += OnImageCallback;
            cell.OnTempertureCallback += OnTemperatureCallback;

            CreateImageBuffer(cell.mCell.mIRCameraWidth * cell.mCell.mIRCameraHeight);
            encoder.Initialize(cell.mCell.mIRCameraWidth, cell.mCell.mIRCameraHeight, cell.mCell.mIRCameraVideoFrameRate);

            base.Initialize(arguments);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnStart()
        {
            worker = new BaseWorker(this);
            worker.Start();
            base.OnStart();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Stop()
        {
            worker.Discard();
            base.Stop();
        }

        public void Run(BaseWorker worker)
        {
            try {
                encoder.Stop();
                encoder.Start($"rtmp://{Global.gCloudRtmpIP}:{Global.gCloudRtmpPort}/live/{streamId}");
            }
            catch (Exception e) {
                Tracker.LogE(e);
                OnFault();
                return;
            }

            while (!worker.IsTerminated()) {
                try {
                    IntPtr addr = imageGCHandle.AddrOfPinnedObject();
                    encoder.Encode(addr, addr + imageSize, addr + imageSize + imageSize / 4);
                }
                catch (Exception e) {
                    Tracker.LogE(e);
                    encoder.Stop();
                    OnFault();
                    return;
                }

                Thread.Sleep(1000 / cell.mCell.mIRCameraVideoFrameRate);
            }

            try {
                encoder.Encode(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                encoder.Stop();
                Stop();
            }
            catch (Exception e) {
                Tracker.LogE(e);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnImageCallback(byte[] data)
        {
            if ((image == null) || (image.Length != data.Length * 3 / 2)) {
                CreateImageBuffer(data.Length);
            }

            data.CopyTo(image, 0);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnTemperatureCallback(float[] data)
        {
            if ((temperature == null) || (temperature.Length != data.Length)) {
                temperature = new byte[data.Length * 4];
            }

            Buffer.BlockCopy(data, 0, temperature, 0, temperature.Length);

            var userData = new UserData() {
                temperature = Convert.ToBase64String(temperature),
                selections = cell.GetAllSelectionInfo(),
                groupSelections = cell.GetAllSelectionGroupInfo()
            };
            var content = CompressUtils.Compress(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(userData)));
            var header = new byte[] { 0xAA, 0xBB, 0xCC, 0xDD };
            var length = BitConverter.GetBytes(content.Length);

            var sei = new byte[header.Length + length.Length + content.Length];
            Buffer.BlockCopy(header, 0, sei, 0, header.Length);
            Buffer.BlockCopy(length, 0, sei, header.Length, length.Length);
            Buffer.BlockCopy(content, 0, sei, header.Length + length.Length, content.Length);
            encoder.EncodeSEI(sei);
        }

        /// <summary>
        /// 创建视频数据
        /// </summary>
        /// <param name="size">原始数据大小</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
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
