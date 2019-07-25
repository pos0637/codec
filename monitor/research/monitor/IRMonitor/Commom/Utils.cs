using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Common
{
    /// <summary>
    /// 通用类
    /// </summary>
    public static class Utils
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(
            out long lpFrequency);

        /// <summary>
        /// 获取高精度性能计数
        /// </summary>
        /// <returns>高精度性能计数</returns>
        public static Int64 GetTickCount()
        {
            Int64 value = 0;

            if (!QueryPerformanceCounter(out value))
                return System.Environment.TickCount;
            else
                return value;
        }

#if FALSE
        /// <summary>
        /// 计算两个高精度性能计数差值(ms)
        /// </summary>
        /// <param name="stopTime">终止计数</param>
        /// <param name="startTime">开始计数</param>
        /// <returns>两个高精度性能计数差值(ms)</returns>
        public static Int64 CalcElapsed(Int64 stopTime, Int64 startTime)
        {
            Int64 freq;

            if (!QueryPerformanceFrequency(out freq) || (freq == 0))
                return stopTime - startTime; // 精度约为15ms
            else
                return (stopTime - startTime) * 1000 / freq;
        }
#endif

        /// <summary>
        /// 计算两个高精度性能计数差值(精度ms)
        /// </summary>
        /// <param name="stopTime">终止计数</param>
        /// <param name="startTime">开始计数</param>
        /// <returns>两个高精度性能计数差值(ms)</returns>
        public static Int64 CalcElapsedEx(Int64 stopTime, Int64 startTime)
        {
            Int64 freq;

            if (!QueryPerformanceFrequency(out freq) || (freq == 0))
                return (stopTime - startTime); // 精度约为15000us
            else
                return (stopTime - startTime) * 1000 / freq;
        }

        /// <summary>
        /// 工作线程等待超时时间
        /// </summary>
        /// <param name="worker">工作线程</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>用户是否中断等待</returns>
        public static Boolean WaitForTimeout(BaseWorker worker, Int64 timeout)
        {
            Int64 startTime = GetTickCount();

            while (true) {
                if (worker.IsTerminated())
                    return true;

                Thread.Sleep(10);

                if (CalcElapsedEx(GetTickCount(), startTime) >= timeout)
                    break;
            }

            return false;
        }

        /// <summary>
        /// 串行化对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>串行化对象结果,失败返回空</returns>
        public static Byte[] Serialize(Object obj)
        {
            MemoryStream ms = null;

            try {
                ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                Byte[] bytes = new Byte[ms.Length];
                ms.Read(bytes, 0, bytes.Length);

                return bytes;
            }
            catch {
                return null;
            }
            finally {
                if (ms != null)
                    ms.Close();
            }
        }

        /// <summary>
        /// 反串行化对象
        /// </summary>
        /// <param name="bytes">对象串行化数据</param>
        /// <returns>反串行化对象,失败返回空</returns>
        public static Object Deserialize(Byte[] bytes)
        {
            MemoryStream ms = null;

            try {
                ms = new MemoryStream(bytes);
                ms.Position = 0;

                BinaryFormatter bf = new BinaryFormatter();
                Object obj = bf.Deserialize(ms);

                return obj;
            }
            catch {
                return null;
            }
            finally {
                if (ms != null)
                    ms.Close();
            }
        }

        /// <summary>
        /// 获取PortName
        /// </summary>
        public static String GetPortName(String key)
        {
            String[] hwList = GetHardwareInfo(HardwareType.Win32_PnPEntity, "Name", key);
            if ((hwList == null) || (hwList.Length == 0))
                return null;

            String portName = null;
            foreach (String item in hwList) {
                if (!item.StartsWith(key))
                    continue;

                Int32 pos = item.IndexOf("(COM");
                if (pos < 0)
                    continue;

                Int32 end = item.IndexOf(")", pos + 4);
                if (end < 0)
                    continue;

                portName = item.Substring(pos + 1, end - pos - 1);
                break;
            }

            return portName;
        }

        /// <summary>
        /// 通过WMI获取查询符合条件的硬件列表
        /// </summary>
        /// <param name="hardType">硬件设备类型</param>
        /// <param name="propKey">属性字段</param>
        /// <param name="key">属性关键字</param>
        /// <returns>设备列表</returns>
        public static String[] GetHardwareInfo(HardwareType hardType, String propKey, String key)
        {
            try {
                List<String> strs = new List<String>();

                using (ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("SELECT * FROM " + hardType)) {
                    var hardInfos = searcher.Get();
                    foreach (var hardInfo in hardInfos) {
                        if ((hardInfo.Properties == null)
                            || (hardInfo.Properties[propKey] == null)
                            || (hardInfo.Properties[propKey].Value == null))
                            continue;

                        if (hardInfo.Properties[propKey].Value.ToString().Contains(key)) {
                            strs.Add(hardInfo.Properties[propKey].Value.ToString());
                        }
                    }
                    searcher.Dispose();
                }

                return strs.ToArray();
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// 枚举win32 api
        /// </summary>
        public enum HardwareType
        {
            // 硬件
            Win32_Processor, // CPU 处理器
            Win32_PhysicalMemory, // 物理内存条
            Win32_Keyboard, // 键盘
            Win32_PointingDevice, // 点输入设备，包括鼠标。
            Win32_FloppyDrive, // 软盘驱动器
            Win32_DiskDrive, // 硬盘驱动器
            Win32_CDROMDrive, // 光盘驱动器
            Win32_BaseBoard, // 主板
            Win32_BIOS, // BIOS 芯片
            Win32_ParallelPort, // 并口
            Win32_SerialPort, // 串口
            Win32_SerialPortConfiguration, // 串口配置
            Win32_SoundDevice, // 多媒体设置，一般指声卡。
            Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP)
            Win32_USBController, // USB 控制器
            Win32_NetworkAdapter, // 网络适配器
            Win32_NetworkAdapterConfiguration, // 网络适配器设置
            Win32_Printer, // 打印机
            Win32_PrinterConfiguration, // 打印机设置
            Win32_PrintJob, // 打印机任务
            Win32_TCPIPPrinterPort, // 打印机端口
            Win32_POTSModem, // MODEM
            Win32_POTSModemToSerialPort, // MODEM 端口
            Win32_DesktopMonitor, // 显示器
            Win32_DisplayConfiguration, // 显卡
            Win32_DisplayControllerConfiguration, // 显卡设置
            Win32_VideoController, // 显卡细节。
            Win32_VideoSettings, // 显卡支持的显示模式。

            // 操作系统
            Win32_TimeZone, // 时区
            Win32_SystemDriver, // 驱动程序
            Win32_DiskPartition, // 磁盘分区
            Win32_LogicalDisk, // 逻辑磁盘
            Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。
            Win32_LogicalMemoryConfiguration, // 逻辑内存配置
            Win32_PageFile, // 系统页文件信息
            Win32_PageFileSetting, // 页文件设置
            Win32_BootConfiguration, // 系统启动配置
            Win32_ComputerSystem, // 计算机信息简要
            Win32_OperatingSystem, // 操作系统信息
            Win32_StartupCommand, // 系统自动启动程序
            Win32_Service, // 系统安装的服务
            Win32_Group, // 系统管理组
            Win32_GroupUser, // 系统组帐号
            Win32_UserAccount, // 用户帐号
            Win32_Process, // 系统进程
            Win32_Thread, // 系统线程
            Win32_Share, // 共享
            Win32_NetworkClient, // 已安装的网络客户端
            Win32_NetworkProtocol, // 已安装的网络协议
            Win32_PnPEntity, // all device
        }

        /// <summary>
        /// 由byte数组转换为结构体
        /// </summary>
        public static T ByteToStructure<T>(Byte[] dataBuffer)
        {
            Object structure = null;
            Int32 size = Marshal.SizeOf(typeof(T));
            IntPtr allocIntPtr = Marshal.AllocHGlobal(size);
            try {
                Marshal.Copy(dataBuffer, 0, allocIntPtr, size);
                structure = Marshal.PtrToStructure(allocIntPtr, typeof(T));
            }
            finally {
                Marshal.FreeHGlobal(allocIntPtr);
            }
            return (T)structure;
        }

        /// <summary>
        /// 由结构体转换为byte数组
        /// </summary>
        public static Byte[] StructureToByte<T>(T structure)
        {
            Int32 size = Marshal.SizeOf(typeof(T));
            Byte[] buffer = new Byte[size];
            IntPtr bufferIntPtr = Marshal.AllocHGlobal(size);
            try {
                Marshal.StructureToPtr(structure, bufferIntPtr, true);
                Marshal.Copy(bufferIntPtr, buffer, 0, size);
            }
            finally {
                Marshal.FreeHGlobal(bufferIntPtr);
            }
            return buffer;
        }

        /// <summary>
        /// 将image转化为二进制
        /// </summary>
        public static Byte[] GetBytesByImage(Image image)
        {
            try {
                ImageFormat format = image.RawFormat;
                using (MemoryStream ms = new MemoryStream()) {
                    image.Save(ms, ImageFormat.Bmp);
                    return ms.ToArray();
                }
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// 读取byte[]并转化为图片
        /// </summary>
        public static Image GetImageByBytes(Byte[] bytes)
        {
            try {
                Image image = null;
                using (MemoryStream ms = new MemoryStream()) {
                    ms.Write(bytes, 0, bytes.Length);
                    image = Image.FromStream(ms);
                }
                return image;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// 获取图片二进制流
        /// </summary>
        public static Byte[] GetBytesByImagePath(String imagepath)
        {
            try {
                using (FileStream fs = new FileStream(imagepath, FileMode.Open, FileAccess.Read)) {
                    Byte[] byData = new Byte[fs.Length];
                    fs.Read(byData, 0, byData.Length);
                    return byData;
                }
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// 将 Stream 转成 byte[]
        /// </summary>
        public static Byte[] StreamToBytes(Stream stream)
        {
            Byte[] bytes = new Byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// 截取图像数据
        /// </summary>
        public static Byte[] CutImageBytes(Byte[] bytes)
        {
            try {
                Int32 sos = 0, eoi = 0;

                // 找到jpeg图片sos段的结尾
                for (Int32 i = 2; i < bytes.Length; i++) {
                    // 遇到sos段
                    if ((bytes[i] == 0xFF) && (bytes[i + 1] == 0xDA)) {
                        sos = i + 1;
                        break;
                    }
                    // 无意义的FF填充
                    if ((bytes[i] == 0xFF) && (bytes[i + 1] == 0xFF)) {
                        continue;
                    }
                    // 遇到其他段
                    if ((bytes[i] == 0xFF) && (bytes[i + 1] != 0xFF)) {
                        // 获取段数据长度
                        Int32 count = 0;
                        count += bytes[i + 2];
                        count = count * 16 * 16 + bytes[i + 3];

                        // 偏移
                        i += (count + 1);
                        continue;
                    }
                }

                // 在sos段后遍历搜索EOI结束段
                for (Int32 i = sos; i < bytes.Length; i++) {
                    // 结束符
                    if ((bytes[i] == 0xFF) && (bytes[i + 1] == 0xD9)) {
                        eoi = i + 2;
                        break;
                    }
                }

                if (eoi == 0)
                    return null;

                Byte[] buf = new Byte[eoi];
                Array.Copy(bytes, 0, buf, 0, buf.Length);
                return buf;
            }
            catch (Exception ex) {
                Tracker.LogE(ex);
                return null;
            }
        }

        public static T[] SubArray<T>(this T[] data, int index)
        {
            T[] result = new T[data.Length - index];
            Array.Copy(data, index, result, 0, data.Length - index);
            return result;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}