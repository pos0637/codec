using System;
using System.IO;
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

        /// <summary>
        /// 工作线程等待超时时间
        /// </summary>
        /// <param name="worker">工作线程</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>用户是否中断等待</returns>
        public static bool WaitForTimeout(BaseWorker worker, long timeout)
        {
            DateTime startTime = DateTime.Now;

            while (true) {
                if (worker.IsTerminated())
                    return true;

                Thread.Sleep(10);

                if ((DateTime.Now - startTime).TotalMilliseconds >= timeout)
                    break;
            }

            return false;
        }

        /// <summary>
        /// 串行化对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>串行化对象结果,失败返回空</returns>
        public static byte[] Serialize(object obj)
        {
            MemoryStream ms = null;

            try {
                ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                byte[] bytes = new byte[ms.Length];
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
        public static object Deserialize(byte[] bytes)
        {
            MemoryStream ms = null;

            try {
                ms = new MemoryStream(bytes) {
                    Position = 0
                };

                BinaryFormatter bf = new BinaryFormatter();
                object obj = bf.Deserialize(ms);

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
        /// 由byte数组转换为结构体
        /// </summary>
        public static T ByteToStructure<T>(byte[] dataBuffer)
        {
            object structure = null;
            int size = Marshal.SizeOf(typeof(T));
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
        public static byte[] StructureToByte<T>(T structure)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
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
        /// 获取图片二进制流
        /// </summary>
        public static byte[] GetBytesByImagePath(string imagepath)
        {
            try {
                using (FileStream fs = new FileStream(imagepath, FileMode.Open, FileAccess.Read)) {
                    byte[] byData = new byte[fs.Length];
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
        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// 截取图像数据
        /// </summary>
        public static byte[] CutImageBytes(byte[] bytes)
        {
            try {
                int sos = 0, eoi = 0;

                // 找到jpeg图片sos段的结尾
                for (int i = 2; i < bytes.Length; i++) {
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
                        int count = 0;
                        count += bytes[i + 2];
                        count = count * 16 * 16 + bytes[i + 3];

                        // 偏移
                        i += (count + 1);
                        continue;
                    }
                }

                // 在sos段后遍历搜索EOI结束段
                for (int i = sos; i < bytes.Length; i++) {
                    // 结束符
                    if ((bytes[i] == 0xFF) && (bytes[i + 1] == 0xD9)) {
                        eoi = i + 2;
                        break;
                    }
                }

                if (eoi == 0)
                    return null;

                byte[] buf = new byte[eoi];
                Array.Copy(bytes, 0, buf, 0, buf.Length);
                return buf;
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }
    }
}
