using Common;
using Devices;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace IRMonitor.Worker
{
    /// <summary>
    /// 获取红外数据线程
    /// </summary>
    public class GetIrDataWorker : BaseWorker
    {
        #region 参数

        // 图像数据回调
        public event Delegates.DgOnImageCallback OnImageCallback;

        // 温度数据回调
        public event Delegates.DgOnTemperatureCallback OnTemperatureCallback;

        // 设备连接失败
        public event Delegates.DgOnIrDisconnected OnIrDisconnected;

        #endregion

        #region 

        // 设备
        private IDevice mDevice;

        // 缓存区
        private Byte[] mImageBuffer;
        private Single[] mTemperatureBuffer;
        private GCHandle mImageHandle, mTemperatureHandle;
        private IntPtr mImageAddr, mTemperatureAddr;

        // 宽度
        private Int32 mWidth;

        // 高度
        private Int32 mHeight;

        // 图像帧率
        private Int32 mVideoFrameRate;

        // 温度帧率
        private Int32 mTempertureFrameRate;

        // 图像睡眠时间
        private Int32 mVideoDuration;

        // 温度睡眠时间
        private Int32 mTempertureDuration;

        #endregion

        ~GetIrDataWorker()
        {
            if (mImageAddr != IntPtr.Zero)
                mImageHandle.Free();

            if (mTemperatureAddr != IntPtr.Zero)
                mTemperatureHandle.Free();
        }

        public ARESULT Init(
            Int32 width,
            Int32 height,
            String ipAddr,
            Int32 videoFrameRate,
            Int32 tempFrameRate,
            IDevice device)
        {
            mDevice = device;

            mDevice.Write(WriteMode.ConnectionString, ipAddr);
            mDevice.Write(WriteMode.FrameRate, tempFrameRate);

            mWidth = width;
            mHeight = height;

            mImageBuffer = new Byte[mWidth * mHeight];
            mImageHandle = GCHandle.Alloc(mImageBuffer, GCHandleType.Pinned);
            mImageAddr = mImageHandle.AddrOfPinnedObject();

            mTemperatureBuffer = new Single[mWidth * mHeight];
            mTemperatureHandle = GCHandle.Alloc(mTemperatureBuffer, GCHandleType.Pinned);
            mTemperatureAddr = mTemperatureHandle.AddrOfPinnedObject();

            mVideoFrameRate = videoFrameRate;
            mTempertureFrameRate = tempFrameRate;

            mVideoDuration = 1000 / videoFrameRate;
            mTempertureDuration = 1000 / tempFrameRate;

            return ARESULT.S_OK;
        }

        /// <summary>
        /// 设置图像获取频率
        /// </summary>
        public void SetVideoDuration(Int32 rate)
        {
            mVideoFrameRate = rate;
            mVideoDuration = 1000 / mVideoFrameRate;
            mDevice.Write(WriteMode.FrameRate, BitConverter.GetBytes(mVideoFrameRate));
        }

        /// <summary>
        /// 设置温度获取频率
        /// </summary>
        public void SetTemperatureDuration(Int32 rate)
        {
            mTempertureFrameRate = rate;
            mTempertureDuration = 1000 / mTempertureFrameRate;
        }

        protected override void Run()
        {
            // 开启设备
            mDevice.Open();

            Int32 sum = 0;
            Int32 used = 0;
            // 实时获取设备数据
            while (!IsTerminated()) {
                // 检查设备运行状态
                if (mDevice.GetDeviceStatus() != DeviceStatus.Running) {
                    // 尝试再次打开设备
                    mDevice.Open();
                    Thread.Sleep(1000);
                    continue;
                }

                DateTime begin = DateTime.Now;

                // 读取温度数据
                if (sum >= mTempertureDuration) {
                    sum = 0;
                    if (mDevice.Read(
                        ReadMode.TemperatureArray,
                        mTemperatureAddr,
                        mTemperatureBuffer.Length * sizeof(Single))) {
                        // 温度回调
                        Single[] buf = (Single[])mTemperatureBuffer.Clone();
                        OnTemperatureCallback?.Invoke(buf);
                    }
                    else if (mDevice.Read(
                        ReadMode.TemperatureArray,
                        mTemperatureBuffer,
                        out used)) {
                        // 温度回调
                        Single[] buf = (Single[])mTemperatureBuffer.Clone();
                        OnTemperatureCallback?.Invoke(buf);
                    }
                }

                // 读取图像数据
                if (mDevice.Read(
                    ReadMode.ImageArray,
                    mImageAddr,
                    mImageBuffer.Length)) {
                    // 灰度数据回调,加快视频帧转换速度不做保护
                    // Byte[] temp = (Byte[])mImageBuffer.Clone();
                    OnImageCallback?.Invoke(mImageBuffer);
                }
                else if (mDevice.Read(
                        ReadMode.ImageArray,
                        mImageBuffer,
                        out used)) {
                    // 灰度数据回调,加快视频帧转换速度不做保护
                    // Byte[] buf = (Byte[])mImageBuffer.Clone();
                    OnImageCallback?.Invoke(mImageBuffer);
                }

                DateTime end = DateTime.Now;
                TimeSpan timeUsed = end - begin;
                Int32 sleepTime = mVideoDuration - (Int32)timeUsed.TotalMilliseconds;
                if (sleepTime > 0)
                    Thread.Sleep(sleepTime);

                sum += mVideoDuration;
            }

            mDevice.Close();
        }

        public override void Discard()
        {
            OnImageCallback = null;
            OnTemperatureCallback = null;
            base.Discard();
        }
    }
}
