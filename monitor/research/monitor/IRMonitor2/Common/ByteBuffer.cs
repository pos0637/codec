using System;
using System.Runtime.InteropServices;

namespace Common
{
    /// <summary>
    /// 缓存
    /// </summary>
    public sealed class ByteBuffer
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private byte[] buffer;

        /// <summary>
        /// 使用长度
        /// </summary>
        private int used;

        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity { get { return buffer?.Length ?? 0; } }

        /// <summary>
        /// 使用长度
        /// </summary>
        public int Used { get { return used; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ByteBuffer()
        {
            buffer = new byte[1024];
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="capacity">容量</param>
        public ByteBuffer(int capacity)
        {
            buffer = new byte[capacity];
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            used = 0;
        }

        /// <summary>
        /// 是否已满
        /// </summary>
        /// <returns>是否已满</returns>
        public bool IsFull()
        {
            return used == buffer.Length;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns>数据</returns>
        public byte[] ToArray()
        {
            return buffer;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">开始偏移</param>
        /// <param name="count">长度</param>
        /// <returns>是否成功</returns>
        public bool Push(byte[] data, int offset = 0, int count = -1)
        {
            count = count < 0 ? data.Length : count;
            if (used + count > buffer.Length) {
                return false;
            }

            Buffer.BlockCopy(data, offset, buffer, used, count);
            used += count;

            return true;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="ptr">数据</param>
        /// <param name="length">长度</param>
        /// <returns>是否成功</returns>
        public bool Push(IntPtr ptr, int length)
        {
            if (used + length > buffer.Length) {
                return false;
            }

            Marshal.Copy(ptr, buffer, used, length);
            used += length;

            return true;
        }
    }
}
