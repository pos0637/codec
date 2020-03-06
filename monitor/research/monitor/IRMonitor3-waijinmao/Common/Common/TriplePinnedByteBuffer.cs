using System.Runtime.CompilerServices;

namespace Common
{
    /// <summary>
    /// 三重缓存
    /// </summary>
    public sealed class TriplePinnedByteBuffer
    {
        /// <summary>
        /// 三重缓存
        /// </summary>
        private readonly PinnedBuffer<byte>[] byteBuffers;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="capacity">容量</param>
        public TriplePinnedByteBuffer(int capacity)
        {
            byteBuffers = new PinnedBuffer<byte>[3] {
                PinnedBuffer<byte>.Alloc(capacity),
                PinnedBuffer<byte>.Alloc(capacity),
                PinnedBuffer<byte>.Alloc(capacity)
            };
        }

        /// <summary>
        /// 获得读取缓存
        /// </summary>
        /// <returns>读取缓存</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public PinnedBuffer<byte> GetReadableBuffer()
        {
            return byteBuffers[0];
        }

        /// <summary>
        /// 获得写入缓存
        /// </summary>
        /// <returns>写入缓存</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public PinnedBuffer<byte> GetWritableBuffer()
        {
            return byteBuffers[2];
        }

        /// <summary>
        /// 交换读取缓存
        /// </summary>
        /// <returns>读取缓存</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public PinnedBuffer<byte> SwapReadableBuffer()
        {
            var byteBuffer = byteBuffers[1];
            byteBuffers[1] = byteBuffers[0];
            byteBuffers[0] = byteBuffer;

            return byteBuffers[0];
        }

        /// <summary>
        /// 交换写入缓存
        /// </summary>
        /// <returns>写入缓存</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public PinnedBuffer<byte> SwapWritableBuffer()
        {
            var byteBuffer = byteBuffers[1];
            byteBuffers[1] = byteBuffers[2];
            byteBuffers[2] = byteBuffer;
            byteBuffers[2].Reset();

            return byteBuffers[2];
        }
    }
}
