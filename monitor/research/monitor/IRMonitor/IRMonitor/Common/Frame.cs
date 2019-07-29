using System;
using Common;
using System.Collections.Generic;

namespace IRMonitor.Common
{
    /// <summary>
    /// 帧类型
    /// </summary>
    public enum FrameType
    {
        DiscoveryInfo,
        IdentityInfo,
        RealTimeInfo,
        Command,
        ReplayVideoStream,
        ReplaySelectionData,
        RealVideoStream,
        RealTemperatureStream,
        ReportData,
        TempCurve,
        RealTemperatureInfo
    }

    /// <summary>
    /// 帧优先级
    /// </summary>
    public enum FramePriority
    {
        DiscoveryInfo = 255,
        IdentityInfo = 255,
        RealTemperatureInfo = 5,
        RealTimeInfo = 5,
        Command = 4,
        ReplayVideoStream = 3,
        ReplaySelectionData = 3,
        RealVideoStream = 2,
        RealTemperatureStream = 2,
        ReportData = 1,
        TempCurve = 0
    }

    /// <summary>
    /// 帧结构
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private static List<Frame> sIdleFrameList = new List<Frame>();

        /// <summary>
        /// 自增帧号
        /// </summary>
        private static UInt32 sFrameID = 0;

        /// <summary>
        /// 包头MagicNumebr
        /// </summary>
        public const Int32 HeadMagicNumber = 0x72666E69; // "infr"

        /// <summary>
        /// 包尾MagicNumber
        /// </summary>
        public const Int32 TailMagicNumber = 0x64657261; // "ared"

        /// <summary>
        /// MagicNumber长度
        /// </summary>
        public const Int32 MagicNumberLength = 4;

        /// <summary>
        /// 数据包最大长度
        /// </summary>
        public const Int32 MaxDataLength =
            HeaderLength + 1024 * 1024 * 4 + TailLength;

        /// <summary>
        /// 包头字节数
        /// 包头格式：包头MagicNumebr + 帧序号 + 帧类型 + 数据长度
        /// 包头长度：MagicNumberLength + 4 + 1 + 4
        /// </summary>
        public const Int32 HeaderLength = MagicNumberLength + 4 + 1 + 4;

        /// <summary>
        /// 包尾字节数
        /// 包尾格式：CRC + 包尾MagicNumber
        /// 包尾长度：4 + MagicNumberLength
        /// </summary>
        public const Int32 TailLength = 4 + MagicNumberLength;

        /// <summary>
        /// 帧序号
        /// </summary>
        public UInt32 mFrameID;

        /// <summary>
        /// 帧类型
        /// </summary>
        public FrameType mFrameType;

        /// <summary>
        /// 帧数据
        /// </summary>
        public Byte[] mFrameData;

        /// <summary>
        /// 优先级
        /// </summary>
        public Int32 mPriority;

        public Int32 mLength;

        /// <summary>
        /// 反序列化成Frame
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Frame Create(
            Byte[] buffer,
            Int32 length)
        {
            Frame temp = null;
            lock (sIdleFrameList) {
                foreach (Frame frame in sIdleFrameList) {
                    if (frame.mFrameData.Length >= length) {
                        temp = frame;
                        sIdleFrameList.Remove(frame);
                        break;
                    }
                }
            }

            if (temp == null) {
                temp = new Frame();
                temp.mFrameData = new Byte[length];
            }

            Int32 index = MagicNumberLength;

            temp.mFrameID = BitConverter.ToUInt32(buffer, index);
            index += sizeof(UInt32);

            temp.mFrameType = (FrameType)buffer[index];
            index += 1;

            Array.Copy(buffer, temp.mFrameData, length);
            temp.mLength = length;

            return temp;
        }

        /// <summary>
        /// 序列化成Frame
        /// </summary>
        /// <param name="priority"></param>
        /// <param name="frameType"></param>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Frame Create(
            Int32 priority,
            FrameType frameType,
            Byte[] buffer,
            Int32 length)
        {
            Frame temp = null;
            lock (sIdleFrameList) {
                foreach (Frame frame in sIdleFrameList) {
                    if (frame.mFrameData.Length >= (HeaderLength + length + TailLength)) {
                        temp = frame;
                        sIdleFrameList.Remove(frame);
                        break;
                    }
                }
            }

            if (temp == null) {
                temp = new Frame();
                temp.mFrameData = new Byte[HeaderLength + length + TailLength];
            }

            temp.mFrameType = frameType;
            temp.mPriority = priority;
            temp.mFrameID = sFrameID++;
            temp.mLength = HeaderLength + length + TailLength;
            Int32 len = 0;

            // 头部
            Array.Copy(BitConverter.GetBytes(HeadMagicNumber), 0, temp.mFrameData, len, MagicNumberLength);
            len += sizeof(Int32);

            // 帧序号
            Array.Copy(BitConverter.GetBytes(temp.mFrameID), 0, temp.mFrameData, len, sizeof(Int32));
            len += sizeof(UInt32);

            // 帧类型
            Array.Copy(BitConverter.GetBytes((Int32)frameType), 0, temp.mFrameData, len, 1);
            len += 1;

            // 数据长度
            Array.Copy(BitConverter.GetBytes(length), 0, temp.mFrameData, len, sizeof(Int32));
            len += sizeof(Int32);

            // 数据
            Array.Copy(buffer, 0, temp.mFrameData, len, length);
            len += length;

            // CRC 
            Int32 crc = CRC32.Crc32(temp.mFrameData, 0, HeaderLength);
            Array.Copy(BitConverter.GetBytes(crc), 0, temp.mFrameData, len, sizeof(Int32));
            len += sizeof(Int32);

            // 尾部
            Array.Copy(BitConverter.GetBytes(TailMagicNumber), 0, temp.mFrameData, len, MagicNumberLength);

            return temp;
        }

        /// <summary>
        /// 回收到缓存
        /// </summary>
        public static void Recycle(Frame frame)
        {
            lock (sIdleFrameList) {
                sIdleFrameList.Add(frame);
            }
        }

        /// <summary>
        /// 回收到缓存
        /// </summary>
        public static void Recycle(List<Frame> frameList)
        {
            lock (sIdleFrameList) {
                foreach (Frame frame in frameList)
                    sIdleFrameList.Add(frame);
            }
        }

        private Frame() { }
    }
}
