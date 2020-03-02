using System;
using System.Runtime.InteropServices;

namespace Common
{
    /// <summary>
    /// 高速缓存
    /// </summary>
    public class PinnedBuffer<T> : IDisposable
    {
        /// <summary>
        /// 缓存
        /// </summary>
        public T[] buffer;

        /// <summary>
        /// 缓存句柄
        /// </summary>
        public GCHandle handle;

        /// <summary>
        /// 缓存地址
        /// </summary>
        public IntPtr ptr;

        /// <summary>
        /// 宽度
        /// </summary>
        public int width;

        /// <summary>
        /// 高度
        /// </summary>
        public int height;

        /// <summary>
        /// 缓存长度
        /// </summary>
        public int Length { get { return buffer?.Length ?? 0; } }

        /// <summary>
        /// 创建缓存
        /// </summary>
        /// <param name="size">缓存大小</param>
        /// <returns>缓存</returns>
        public static PinnedBuffer<T> Alloc(int size)
        {
            var buffer = new PinnedBuffer<T> {
                buffer = new T[size]
            };

            buffer.handle = GCHandle.Alloc(buffer.buffer, GCHandleType.Pinned);
            buffer.ptr = buffer.handle.AddrOfPinnedObject();
            buffer.width = size;
            buffer.height = 1;

            return buffer;
        }

        /// <summary>
        /// 创建缓存
        /// </summary>
        /// <param name="size">缓存大小</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns>缓存</returns>
        public static PinnedBuffer<T> Alloc(int size, int width, int height)
        {
            var buffer = Alloc(size);
            buffer.width = width;
            buffer.height = height;

            return buffer;
        }

        ~PinnedBuffer()
        {
            Dispose();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (ptr != IntPtr.Zero) {
                handle.Free();
                ptr = IntPtr.Zero;
            }
        }

        private PinnedBuffer() { }
    }
}
