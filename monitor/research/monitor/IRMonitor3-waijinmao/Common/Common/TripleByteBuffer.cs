using System.Runtime.CompilerServices;

namespace Common
{
    /// <summary>
    /// 三重缓存
    /// </summary>
    public sealed class TripleByteBuffer
    {
        /// <summary>
        /// 三重缓存
        /// </summary>
        private ByteBuffer[] byteBuffers;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TripleByteBuffer()
        {
            byteBuffers = new ByteBuffer[3] {
                new ByteBuffer(),
                new ByteBuffer(),
                new ByteBuffer()
            };
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="capacity">容量</param>
        public TripleByteBuffer(int capacity)
        {
            byteBuffers = new ByteBuffer[3] {
                new ByteBuffer(capacity),
                new ByteBuffer(capacity),
                new ByteBuffer(capacity)
            };
        }

        /// <summary>
        /// 获得读取缓存
        /// </summary>
        /// <returns>读取缓存</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ByteBuffer GetReadableBuffer()
        {
            return byteBuffers[0];
        }

        /// <summary>
        /// 获得写入缓存
        /// </summary>
        /// <returns>写入缓存</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ByteBuffer GetWritableBuffer()
        {
            return byteBuffers[2];
        }

        /// <summary>
        /// 交换读取缓存
        /// </summary>
        /// <returns>读取缓存</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ByteBuffer SwapReadableBuffer()
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
        public ByteBuffer SwapWritableBuffer()
        {
            var byteBuffer = byteBuffers[1];
            byteBuffers[1] = byteBuffers[2];
            byteBuffers[2] = byteBuffer;
            byteBuffers[2].Reset();

            return byteBuffers[2];
        }
    }
}
