namespace Common
{
    /// <summary>
    /// 缓存
    /// </summary>
    public class Buffer<T>
    {
        /// <summary>
        /// 缓存
        /// </summary>
        public T[] buffer;

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
        /// <returns>缓冲区</returns>
        public static Buffer<T> Alloc(int size)
        {
            var buffer = new Buffer<T> {
                buffer = new T[size]
            };

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
        public static Buffer<T> Alloc(int size, int width, int height)
        {
            var buffer = Alloc(size);
            buffer.width = width;
            buffer.height = height;

            return buffer;
        }

        private Buffer() { }
    }
}
