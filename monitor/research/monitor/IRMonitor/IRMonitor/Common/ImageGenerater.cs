using Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace IRMonitor.Common
{
    /// <summary>
    /// 图片生成器
    /// </summary>
    public class ImageGenerator
    {
        /// <summary>
        /// 创建调色后位图
        /// </summary>
        /// <param name="data">原始位图数据</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="type">调色板类型</param>
        /// <returns>调色后位图</returns>
        public static Image<Rgba32> CreateBitmap(
            byte[] data,
            int width,
            int height,
            IRPalette.PaletteType type = IRPalette.PaletteType.Gray)
        {
            MemoryStream stream = null;

            try {
                stream = new MemoryStream();
                Image<Rgba32> result = new Image<Rgba32>(width, height);
                result.SaveAsBmp(stream);

                // 由于位图数据需要DWORD对齐（4byte倍数），计算需要补位的个数
                int curPadNum = ((width * 8 + 31) / 32 * 4) - width;

                // 最终生成的位图数据大小
                int bitmapDataSize = ((width * 8 + 31) / 32 * 4) * height;

                // 数据部分相对文件开始偏移
                int offset = ReadData(stream, 10, 4);
                if (offset == -1) {
                    return null;
                }

                // 取调色板
                int[] palette;
                switch (type) {
                    case IRPalette.PaletteType.Gray:
                        palette = IRPalette.sGrayPalette;
                        break;
                    case IRPalette.PaletteType.Red:
                        palette = IRPalette.sRedPalette;
                        break;
                    case IRPalette.PaletteType.Blue:
                        palette = IRPalette.sBluePalette;
                        break;
                    case IRPalette.PaletteType.Rainbow:
                        palette = IRPalette.sRainbowPalette;
                        break;
                    case IRPalette.PaletteType.Arctic:
                        palette = IRPalette.sArcticPalette;
                        break;
                    case IRPalette.PaletteType.Lava:
                        palette = IRPalette.sLavaPalette;
                        break;
                    default:
                        return null;
                }

                // 设置调色板
                byte[] tempData = new byte[4];
                stream.Position = 10;
                stream.Read(tempData, 0, 4);

                int paletteEnd = BitConverter.ToInt32(tempData, 0);
                for (int i = 54, j = 0; i < paletteEnd; i += 4, j++) {
                    byte[] tempColor = BitConverter.GetBytes(palette[j]);
                    stream.Position = i;
                    stream.Write(tempColor, 0, 4);
                }

                // 写入灰度值
                byte[] dest = new byte[bitmapDataSize];
                int destWidth = width + curPadNum;

                // 位图数据从左到右，从下到上
                for (int originalRowIndex = height - 1; originalRowIndex >= 0; originalRowIndex--) {
                    int destRowIndex = height - originalRowIndex - 1;
                    for (int dataIndex = 0; dataIndex < width; dataIndex++) {
                        dest[destRowIndex * destWidth + dataIndex] = data[originalRowIndex * width + dataIndex];
                    }
                }

                stream.Position = offset;
                stream.Write(dest, 0, bitmapDataSize);
                stream.Flush();

                return Image.Load<Rgba32>(stream, new BmpDecoder());
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
            finally {
                if (stream != null) {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// 从内存流中指定位置读取数据
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <returns>数据</returns>
        private static int ReadData(MemoryStream stream, int offset, int length)
        {
            try {
                byte[] tempData = new byte[length];
                stream.Position = offset;
                stream.Read(tempData, 0, length);

                return BitConverter.ToInt32(tempData, 0);
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return -1;
            }
        }
    }
}
