using Common;
using IRMonitor.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace IRMonitor.Services.Cell.Worker
{
    /// <summary>
    /// 帧类型
    /// </summary>
    public enum H264NALTYPE
    {
        H264NT_NAL = 0,
        H264NT_SLICE,
        H264NT_SLICE_DPA,
        H264NT_SLICE_DPB,
        H264NT_SLICE_DPC,
        H264NT_SLICE_IDR,
        H264NT_SEI,
        H264NT_SPS,
        H264NT_PPS,
    };

    /// <summary>
    /// 录像线程
    /// </summary>
    public class RecordingWorker : BaseWorker
    {
        public event Delegates.DgOnAddRecordInfo OnAddRecord; // 添加录像文件记录

        private FixedLenQueue<Byte[]> mVideoQueue = new FixedLenQueue<Byte[]>(1); // 图像队列
        private Thread mVideoThread; // 图像录制线程
        private Int32 mUserCount; // 计数
        private Int32 mSpace = 2 * 1024 * 1024; // 单文件空间
        private Int32 mWidth; // 宽度
        private Int32 mHeight; // 高度
        private Int32 mFrameNum; // 帧数
        private Int32 mImageFrameRate; // 图像帧率
        private Int32 mTempFramRate; // 温度帧率
        private Int32 mRateScale; // 帧率比值
        private String mRecordFolder; // 导出文件夹
        private List<Selection> mSelectionList; // 选区链表
        private Object mSyncEvent = new Object(); // 同步事件
        private const Int32 mThumbnailWidth = 90; // 缩略图宽度
        private const Int32 mThumbnailHeight = 60; // 缩略图高度

        // H264缓冲区，size为灰度图的1.5倍
        private Byte[] mImageBuf, mEncodeBuf;
        private GCHandle mImageBufHandle, mEncodeBufHandle;
        private IntPtr mImageBufAddr, mEncodeBufAddr;

        private BinaryWriter mVideoBw;
        private BinaryWriter mInfoBw;
        private IFrameInfo mFrameInfo;

        /// <summary>
        /// 是否有上一个I帧的数据
        /// </summary>
        private Boolean mHasLastIFrame;

        /// <summary>
        /// 上一个I帧的数据长度
        /// </summary>
        public Int32 mLastIFrameLen = 0; // 最近一个I帧的长度

        ~RecordingWorker()
        {
            if (mImageBufAddr != IntPtr.Zero)
                mImageBufHandle.Free();

            if (mEncodeBufAddr != IntPtr.Zero)
                mEncodeBufHandle.Free();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public ARESULT Init(
            Int32 width,
            Int32 height,
            Int32 imageFrameRate,
            Int32 tempFrameRate,
            String floder,
            Int32 space,
            List<Selection> selectionList)
        {
            Int32 len = width * height;
            mSpace = space;
            mRecordFolder = floder;
            mWidth = width;
            mHeight = height;
            mImageFrameRate = imageFrameRate;
            mTempFramRate = tempFrameRate;
            mRateScale = imageFrameRate / tempFrameRate;
            mImageBuf = Enumerable.Repeat((Byte)0x80, (Int32)(len * 1.5)).ToArray();
            mImageBufHandle = GCHandle.Alloc(mImageBuf, GCHandleType.Pinned);
            mImageBufAddr = mImageBufHandle.AddrOfPinnedObject();
            mEncodeBuf = new Byte[(Int32)(len * 1.5)];
            mEncodeBufHandle = GCHandle.Alloc(mEncodeBuf, GCHandleType.Pinned);
            mEncodeBufAddr = mEncodeBufHandle.AddrOfPinnedObject();
            mSelectionList = selectionList;
            return ARESULT.S_OK;
        }

        /// <summary>
        /// 接收图像数据
        /// </summary>
        public void ReceiveImageData(Byte[] buf)
        {
            mVideoQueue.Enqueue(buf);
        }

        /// <summary>
        /// 开始录像
        /// </summary>
        public void StartRecord()
        {
            Boolean first = false;
            lock (mLock) {
                if (mUserCount == 0) {
                    Start();
                    first = true;
                }
                mUserCount++;
            }

            // 同步
            if (first) {
                lock (mSyncEvent) {
                    Monitor.Wait(mSyncEvent);
                }
            }
        }

        /// <summary>
        /// 结束录像
        /// </summary>
        public void StopRecord()
        {
            lock (mLock) {
                mUserCount--;
                if (mUserCount <= 0) {
                    mUserCount = 0;
                    NotifyToStop(ARESULT.S_OK);
                }
            }
        }

        protected override void Run()
        {
            try {
                while (!IsTerminated()) {
                    DateTime time = DateTime.Now;
                    String floderPath = String.Format("{0}/{1}-{2}-{3}", mRecordFolder, time.Year.ToString(),
                        time.Month.ToString(), time.Day.ToString());

                    // 跨越零点,重置
                    DirectoryInfo folder = new DirectoryInfo(floderPath);
                    if (!folder.Exists)
                        folder.Create();

                    String videoFileName = String.Format("{0}/{1}.H264", floderPath, time.ToString("yyyyMMddHHmmss"));
                    FileStream vfs = new FileStream(videoFileName, FileMode.OpenOrCreate);
                    mVideoBw = new BinaryWriter(vfs);

                    String infoFileName = String.Format("{0}/{1}.Info", floderPath, time.ToString("yyyyMMddHHmmss"));
                    FileStream ifs = new FileStream(infoFileName, FileMode.OpenOrCreate);
                    mInfoBw = new BinaryWriter(ifs);

                    OnAddRecord?.Invoke(videoFileName, infoFileName);
                    lock (mSyncEvent) {
                        Monitor.PulseAll(mSyncEvent);
                    }

                    // 头部，16个字节
                    mFrameNum = 0;
                    mInfoBw.Write(BitConverter.GetBytes(mWidth));
                    mInfoBw.Write(BitConverter.GetBytes(mHeight));
                    mInfoBw.Write(BitConverter.GetBytes(mFrameNum));
                    mInfoBw.Write(BitConverter.GetBytes(mImageFrameRate));

                    // 无数据时,阻塞
                    if (mVideoQueue.Count <= 0)
                        mVideoQueue.Wait();

                    if (IsTerminated()) {
                        mInfoBw.BaseStream.Seek(8, SeekOrigin.Begin);
                        mInfoBw.Write(BitConverter.GetBytes(mFrameNum)); // 写入帧数
                        mInfoBw.Close();
                        return;
                    }

                    // 缩略图
                    Byte[] buf = mVideoQueue.Dequeue();
                    if (buf == null)
                        break;
                    /*
                    using (Image img = ImageGenerater.CreateBitmap(buf, mWidth, mHeight)) {
                        Image img2 = img.GetThumbnailImage(mThumbnailWidth, mThumbnailHeight, null, IntPtr.Zero);
                        Byte[] bytes = Utils.GetBytesByImage(img2);
                        img2.Dispose();

                        mInfoBw.Write(bytes.Length);
                        mInfoBw.Write(bytes);
                    }
                    */

                    // 只存带后缀的文件名
                    mFrameInfo.mVideoFileName = Path.GetFileName(videoFileName);

                    // 开启线程录制
                    mVideoThread = new Thread(RecordVideoThread);
                    mVideoThread.Start();
                    mVideoThread.Join();

                    mVideoBw.Close();
                    vfs.Close();

                    mInfoBw.BaseStream.Seek(8, SeekOrigin.Begin);
                    mInfoBw.Write(BitConverter.GetBytes(mFrameNum)); // 写入帧数
                    mInfoBw.Close();
                    ifs.Close();
                }
            }
            catch (Exception ex) {
                Tracker.LogE(ex);
            }

            lock (mSyncEvent) {
                Monitor.PulseAll(mSyncEvent);
            }
        }

        public override void Discard()
        {
            lock (mSyncEvent) {
                Monitor.PulseAll(mSyncEvent);
            }
            OnAddRecord = null;
            base.Discard();
            mVideoQueue.Notify();
        }

        /// <summary>
        /// 获取H264的帧类别
        /// </summary>
        /// <param name="buf">缓存</param>
        /// <param name="len">缓存长度</param>
        /// <returns></returns>
        private H264NALTYPE H264GetNALType(Byte[] buf, Int32 len)
        {
            // 不完整的NAL单元
            if (len < 5)
                return H264NALTYPE.H264NT_NAL;

            // NAL类型在固定的位置上 
            Int32 nType = buf[4] & 0x1F;
            if (nType <= (Int32)H264NALTYPE.H264NT_PPS)
                return (H264NALTYPE)nType;

            return H264NALTYPE.H264NT_NAL;
        }

        /// <summary>
        /// 图像录制线程
        /// </summary>
        /// <param name="vbw">二进制写入器</param>
        private void RecordVideoThread()
        {
            Int32 len = 0;
            H264NALTYPE type = H264NALTYPE.H264NT_NAL;
            Int32 count = 0;

            // 文件头添加I帧
            if (mHasLastIFrame) {
                mVideoBw.Write(mEncodeBuf, 0, mLastIFrameLen);
                mFrameInfo.mIsIFrame = true;
                mFrameInfo.mSelectionDatalength = 0;
                mFrameInfo.mVideoPosition = len;
                mFrameInfo.mVideoLength = mLastIFrameLen;

                Byte[] infoData = Utils.StructureToByte(mFrameInfo);
                mInfoBw.Write(infoData.Length);
                mInfoBw.Write(infoData, 0, infoData.Length);
                mFrameNum += 1;
                len += mLastIFrameLen;
            }

            // 添加mHasLastIFrame 和 hasLength 两个状态，保证每个头文件都是以I帧开头
            Boolean hasLength = false;
            while (((len < mSpace) || hasLength) && !IsTerminated()) {
                // 无数据时,阻塞
                if (mVideoQueue.Count <= 0)
                    mVideoQueue.Wait();

                if (IsTerminated())
                    return;

                mFrameInfo.mIsIFrame = false;
                mFrameInfo.mSelectionDatalength = 0;

                // 取数据编码
                Byte[] buf = mVideoQueue.Dequeue();
                if (buf == null)
                    break;

                buf.CopyTo(mImageBuf, 0);
                Int32 encoderLen = 0;
                if (encoderLen <= 0)
                    continue;

                // 判断是否I帧
                type = H264GetNALType(mEncodeBuf, encoderLen);
                if (type == H264NALTYPE.H264NT_SPS) {
                    mFrameInfo.mIsIFrame = true;
                    mLastIFrameLen = encoderLen;
                    if (hasLength) {
                        mHasLastIFrame = true;
                        break;
                    }
                }

                // 写入图像数据
                mVideoBw.Write(mEncodeBuf, 0, encoderLen);
                mVideoBw.Flush();

                mFrameInfo.mVideoPosition = len;
                mFrameInfo.mVideoLength = encoderLen;
                len += encoderLen;
                count++;

                if (count >= mRateScale) {
                    if (mSelectionList.Count > 0) {
                        String str = GetSelectionData();
                        Byte[] selectionData = Encoding.UTF8.GetBytes(str);
                        mFrameInfo.mSelectionDatalength = selectionData.Length;
                        Byte[] infoData = Utils.StructureToByte(mFrameInfo);
                        mInfoBw.Write(infoData.Length);
                        mInfoBw.Write(infoData, 0, infoData.Length);
                        mInfoBw.Write(selectionData, 0, selectionData.Length); // 写入选区数据
                        mFrameNum += 1;
                    }
                    else {
                        mFrameInfo.mSelectionDatalength = 0;
                        Byte[] infoData = Utils.StructureToByte(mFrameInfo);
                        mInfoBw.Write(infoData.Length);
                        mInfoBw.Write(infoData, 0, infoData.Length);
                        mFrameNum += 1;
                    }

                    count = 0;
                }
                else {
                    Byte[] infoData = Utils.StructureToByte(mFrameInfo);
                    mInfoBw.Write(infoData.Length);
                    mInfoBw.Write(infoData, 0, infoData.Length);
                    mFrameNum += 1;
                }

                if (len >= mSpace)
                    hasLength = true;
            }
        }

        /// <summary>
        /// 序列化选区数据
        /// </summary>
        private String GetSelectionData()
        {
            // 按帧率记录选区数据
            String str = "";
            Single maxTemp = 0.0f;
            Single minTemp = 0.0f;
            lock (mSelectionList) {
                foreach (Selection item in mSelectionList) {
                    if (item.mIsGlobalSelection) {
                        maxTemp = item.mTemperatureData.mMaxTemperature;
                        minTemp = item.mTemperatureData.mMinTemperature;
                    }
                    String var = item.Serialize();
                    if (var != null)
                        str += var + ",";
                }
            }

            str = "[" + str.Remove(str.Length - 1, 1) + "]";
            str = String.Format("{0},{1},{2}", maxTemp, minTemp, str);
            return str;
        }
    }
}