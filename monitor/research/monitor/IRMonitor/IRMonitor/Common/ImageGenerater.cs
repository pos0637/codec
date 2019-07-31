using Common;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PixelType = SixLabors.ImageSharp.PixelFormats.Rgba32;

namespace IRMonitor.Common
{
    /// <summary>
    /// 图片生成器
    /// </summary>
    public class ImageGenerator
    {
        /// <summary>
        /// 创建温度图
        /// </summary>
        /// <param name="data">原始位图数据</param>
        /// <param name="selections">选区</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="type">调色板类型</param>
        /// <returns>温度图</returns>
        public static Image<PixelType> CreateImage(
            byte[] data,
            List<Selection> selections,
            int width,
            int height,
            IRPalette.PaletteType type = IRPalette.PaletteType.Gray)
        {
            try {
                Image<PixelType> result = CreateImage(data, width, height, type);
                if (result == null) {
                    return null;
                }

                #region 绘选区

                result.Mutate(image => {
                    var pen = new Pen<PixelType>(PixelType.White, 1);
                    var font = SystemFonts.CreateFont("Arial", 15, FontStyle.Bold);
                    var redBrush = new SolidBrush<PixelType>(PixelType.Red);
                    var blueBrush = new SolidBrush<PixelType>(PixelType.Blue);

                    foreach (Selection selection in selections) {
                        if (selection.mIsGlobalSelection) {
                            continue;
                        }

                        // 画最高温点和温度
                        var point1 = new Point(selection.mTemperatureData.mMaxPoint.X, selection.mTemperatureData.mMaxPoint.Y);
                        var point2 = new Point(selection.mTemperatureData.mMaxPoint.X + 8, selection.mTemperatureData.mMaxPoint.Y);
                        var point3 = new Point((point1.X + point2.X) / 2, selection.mTemperatureData.mMaxPoint.Y - 8);
                        image.FillPolygon<PixelType>(redBrush, point1, point2, point3);

                        var str = selection.mTemperatureData.mMaxTemperature.ToString("F2");
                        image.DrawText(str, font, redBrush, new PointF(selection.mTemperatureData.mMaxPoint.X + 4, selection.mTemperatureData.mMaxPoint.Y + 4));

                        // 画最低温点和温度
                        point1 = new Point(selection.mTemperatureData.mMinPoint.X, selection.mTemperatureData.mMinPoint.Y);
                        point2 = new Point(selection.mTemperatureData.mMinPoint.X + 8, selection.mTemperatureData.mMinPoint.Y);
                        point3 = new Point((point1.X + point2.X) / 2, selection.mTemperatureData.mMinPoint.Y - 8);
                        image.FillPolygon<PixelType>(blueBrush, point1, point2, point3);

                        str = selection.mTemperatureData.mMinTemperature.ToString("F2");
                        image.DrawText(str, font, blueBrush, new PointF(selection.mTemperatureData.mMinPoint.X + 4, selection.mTemperatureData.mMinPoint.Y + 4));

                        switch (selection.mType) {
                            case SelectionType.Rectangle:
                                if (selection is RectangleSelection) {
                                    image.Draw<PixelType>(pen, ((RectangleSelection)selection).mRectangle);
                                }
                                break;

                            case SelectionType.Ellipse:
                                // TODO: fix it
                                if (selection is EllipseSelection) {
                                    image.Draw<PixelType>(pen, ((EllipseSelection)selection).mRectangle);
                                }
                                break;

                            case SelectionType.Point:
                                if (selection is PointSelection) {
                                    var rect = new RectangleF(((PointSelection)selection).mPoint.X - 2, ((PointSelection)selection).mPoint.Y - 2, 4, 4);
                                    image.Draw<PixelType>(pen, rect);
                                }
                                break;

                            case SelectionType.Line:
                                if (selection is LineSelection) {
                                    image.DrawLines(pen, ((LineSelection)selection).mPoint[0], ((LineSelection)selection).mPoint[1]);
                                }
                                break;

                            default:
                                break;
                        }
                    }
                });

                #endregion

                return result;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 创建包含原始信息的温度图
        /// </summary>
        /// <param name="data">原始位图数据</param>
        /// <param name="selections">选区</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="maxTemperature">全局最高温度</param>
        /// <param name="minTemperature">全局最低温度</param>
        /// <param name="filename">文件名称</param>
        /// <param name="information">信息</param>
        /// <param name="type">调色板类型</param>
        /// <returns></returns>
        public static bool CreateImage(
            byte[] data,
            List<Selection> selections,
            int width,
            int height,
            float maxTemperature,
            float minTemperature,
            string filename,
            string information,
            IRPalette.PaletteType type = IRPalette.PaletteType.Gray)
        {
            MemoryStream stream = null;
            BinaryWriter writer = null;

            try {
                Image<PixelType> image = CreateImage(data, selections, width, height, type);
                if (image == null) {
                    return false;
                }

                // 序列化选区
                string selectionData = "";
                foreach (Selection selection in selections) {
                    if (selection.mIsGlobalSelection) {
                        continue;
                    }

                    string temp = selection.Serialize();
                    if (temp != null) {
                        selectionData += (temp + ",");
                    }
                }

                stream = new MemoryStream();
                image.SaveAsJpeg(stream);
                stream.Seek(0, SeekOrigin.End);
                writer = new BinaryWriter(stream);

                // 原始数据
                writer.Write(BitConverter.GetBytes(width), 0, 4);
                writer.Write(BitConverter.GetBytes(height), 0, 4);
                writer.Write(data, 0, width * height);

                // 高低温
                writer.Write(BitConverter.GetBytes(maxTemperature), 0, 4);
                writer.Write(BitConverter.GetBytes(minTemperature), 0, 4);

                // 选区
                byte[] buf = Encoding.UTF8.GetBytes(selectionData);
                writer.Write(BitConverter.GetBytes(buf.Length), 0, 4);
                writer.Write(buf, 0, buf.Length);

                // 设备信息
                information = information ?? "";
                buf = Encoding.UTF8.GetBytes(information);
                writer.Write(BitConverter.GetBytes(buf.Length));
                writer.Write(buf);

                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, System.IO.FileAccess.Write)) {
                    stream.CopyTo(fs);
                    fs.Flush();
                    fs.Close();
                }

                return true;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return false;
            }
            finally {
                if (writer != null) {
                    writer.Close();
                    writer.Dispose();
                }

                if (stream != null) {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }

        /// <summary>
        /// 创建调色后位图
        /// </summary>
        /// <param name="data">原始位图数据</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="type">调色板类型</param>
        /// <returns>调色后位图</returns>
        public static Image<PixelType> CreateImage(
            byte[] data,
            int width,
            int height,
            IRPalette.PaletteType type = IRPalette.PaletteType.Gray)
        {
            MemoryStream stream = null;

            try {
                stream = new MemoryStream();
                using (Image<PixelType> result = new Image<PixelType>(width, height)) {
                    result.SaveAsBmp(stream);
                }

                // 由于位图数据需要DWORD对齐（4byte倍数），计算需要补位的个数
                int padding = ((width * 8 + 31) / 32 * 4) - width;

                // 最终生成的位图数据大小
                int size = ((width * 8 + 31) / 32 * 4) * height;

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
                    byte[] color = BitConverter.GetBytes(palette[j]);
                    stream.Position = i;
                    stream.Write(color, 0, 4);
                }

                // 写入灰度值
                byte[] dest = new byte[size];
                int destWidth = width + padding;

                // 位图数据从左到右，从下到上
                for (int originalRowIndex = height - 1; originalRowIndex >= 0; originalRowIndex--) {
                    int destRowIndex = height - originalRowIndex - 1;
                    for (int dataIndex = 0; dataIndex < width; dataIndex++) {
                        dest[destRowIndex * destWidth + dataIndex] = data[originalRowIndex * width + dataIndex];
                    }
                }

                stream.Position = offset;
                stream.Write(dest, 0, size);
                stream.Flush();

                return Image.Load<PixelType>(stream, new BmpDecoder());
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
