using System;
using System.Runtime.InteropServices;

namespace Common
{
    /// <summary>
    /// 高速缓冲区
    /// </summary>
    public class PinnedBuffer<T> : IDisposable
    {
        /// <summary>
        /// 缓冲区
        /// </summary>
        public T[] buffer;

        /// <summary>
        /// 缓冲区句柄
        /// </summary>
        public GCHandle handle;

        /// <summary>
        /// 缓冲区地址
        /// </summary>
        public IntPtr ptr;

        /// <summary>
        /// 缓冲区长度
        /// </summary>
        public int Length { get { return buffer?.Length ?? 0; } }

        /// <summary>
        /// 创建缓冲区
        /// </summary>
        /// <param name="size">缓冲区大小</param>
        /// <returns>缓冲区</returns>
        public static PinnedBuffer<T> Alloc(int size)
        {
            var buffer = new PinnedBuffer<T> {
                buffer = new T[size]
            };

            buffer.handle = GCHandle.Alloc(buffer.buffer, GCHandleType.Pinned);
            buffer.ptr = buffer.handle.AddrOfPinnedObject();

            return buffer;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (ptr != IntPtr.Zero) {
                handle.Free();
            }
        }

        private PinnedBuffer() { }
    }
}
