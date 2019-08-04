using System.IO;
using System.IO.Compression;

namespace IRMonitor.Miscs
{
    /// <summary>
    /// 压缩工具
    /// </summary>
    public static class CompressUtils
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="bytes">原始数据</param>
        /// <returns>压缩数据</returns>
        public static byte[] Compress(byte[] bytes)
        {
            using (MemoryStream compressStream = new MemoryStream()) {
                using (var zipStream = new GZipStream(compressStream, CompressionMode.Compress)) {
                    zipStream.Write(bytes, 0, bytes.Length);
                }
                return compressStream.ToArray();
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="bytes">压缩数据</param>
        /// <returns>原始数据</returns>
        public static byte[] Decompress(byte[] bytes)
        {
            using (var compressStream = new MemoryStream(bytes)) {
                using (var zipStream = new GZipStream(compressStream, CompressionMode.Decompress)) {
                    using (var resultStream = new MemoryStream()) {
                        zipStream.CopyTo(resultStream);
                        return resultStream.ToArray();
                    }
                }
            }
        }
    }
}
