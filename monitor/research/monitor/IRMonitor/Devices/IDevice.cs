using System;
using System.Runtime.CompilerServices;

namespace Devices
{
    /// <summary>
    /// 定义设备抽象类
    /// </summary>
    public abstract class IDevice : IDisposable
    {
        /// <summary>
        /// 设备索引
        /// </summary>
        public Int64 Id
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        /// <summary>
        /// 设备名称
        /// </summary>
        public String DeviceName
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns>是否成功,成功返回S_OK</returns>
        public abstract Boolean Initialize();

        /// <summary>
        /// 打开设备
        /// </summary>
        /// <returns>是否成功,成功返回S_OK</returns>
        public abstract Boolean Open();

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns>是否成功,成功返回S_OK</returns>
        public abstract Boolean Close();

        /// <summary>
        /// 从设备中读取数据
        /// </summary>
        /// <param name="mode">读取方式</param>
        /// <param name="dataAddr">内存地址</param>
        /// <param name="bufferLen">使用的长度</param>
        /// <returns></returns>
        public abstract Boolean Read(ReadMode mode, IntPtr dataAddr, Int32 bufferLen);

        /// <summary>
        /// 从设备中读取数据
        /// </summary>
        /// <param name="mode">读取方式</param>
        /// <param name="data">数据缓存</param>
        /// <param name="useLen">使用的长度</param>
        /// <returns></returns>        
        public abstract Boolean Read(ReadMode mode, Object data, out Int32 useLen);

        /// <summary>
        /// 将数据写入设备
        /// </summary>
        /// <param name="mode">写入方式</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public abstract Boolean Write(WriteMode mode, Object data);

        /// <summary>
        /// 控制设备
        /// </summary>
        /// <param name="mode">控制类型</param>
        /// <returns></returns>
        public abstract Boolean Control(ControlMode mode);

        /// <summary>
        /// 获取设备类型
        /// </summary>
        /// <returns></returns>
        public abstract DeviceType GetDeviceType();

        /// <summary>
        /// 获取设备状态
        /// </summary>
        /// <returns></returns>
        public abstract DeviceStatus GetDeviceStatus();

        /// <summary>
        /// 释放资源
        /// </summary>
        public abstract void Dispose();
    }
}
