using Common;
using IRMonitor.Common;
using IRMonitor.Miscs;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace IRMonitor.Services.Cell.Worker
{
    public class RecordingWorker : BaseWorker
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
        /// 添加录像文件记录事件
        /// </summary>
        public event Delegates.DgOnAddRecordInfo OnAddRecord;

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
        private Codec.Encoder encoder = new Codec.Encoder();

        /// <summary>
        /// 引用计数
        /// </summary>
        private RefBase refbase;

        ~RecordingWorker()
        {
            if (cell != null) {
                cell.OnImageCallback -= OnImageCallback;
                cell.OnTempertureCallback -= OnTemperatureCallback;
            }

            if (imageGCHandle.IsAllocated) {
                imageGCHandle.Free();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize(CellService cell)
        {
            this.cell = cell;
            cell.OnImageCallback += OnImageCallback;
            cell.OnTempertureCallback += OnTemperatureCallback;
            refbase = new RefBase(() => { base.Discard(); base.Join(); });

            CreateImageBuffer(cell.mCell.mIRCameraWidth * cell.mCell.mIRCameraHeight);
            encoder.Initialize(cell.mCell.mIRCameraWidth, cell.mCell.mIRCameraHeight, cell.mCell.mIRCameraVideoFrameRate);
        }

        /// <summary>
        /// 启动录像
        /// </summary>
        /// <returns>启动结果</returns>
        public override ARESULT Start()
        {
            var result = base.Start();
            if ((result == ARESULT.S_OK) || (result == ARESULT.E_ALREADY_EXISTS)) {
                refbase.AddRef();
                return ARESULT.S_OK;
            }

            return result;
        }

        /// <summary>
        /// 停止录像
        /// </summary>
        public void Stop()
        {
            refbase.Release();
        }

        protected override void Run()
        {
            try {
                // 创建目录
                var now = DateTime.Now;
                var folder = $"{Global.gRecordingsFolder}/{now.Year}-{now.Month}-{now.Day}";
                var filename = $"{folder}/{now.ToString("yyyyMMddHHmmss")}.h264";
                if (!Directory.Exists(folder)) {
                    Directory.CreateDirectory(folder);
                }

                encoder.Stop();
                encoder.Start(filename);
                OnAddRecord?.Invoke(filename);
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return;
            }

            while (!IsTerminated()) {
                try {
                    IntPtr addr = imageGCHandle.AddrOfPinnedObject();
                    encoder.Encode(addr, addr + imageSize, addr + imageSize + imageSize / 4);
                }
                catch (Exception e) {
                    Tracker.LogE(e);
                    encoder.Stop();
                    return;
                }

                Thread.Sleep(1000 / cell.mCell.mIRCameraVideoFrameRate);
            }

            try {
                encoder.Encode(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                encoder.Stop();
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
