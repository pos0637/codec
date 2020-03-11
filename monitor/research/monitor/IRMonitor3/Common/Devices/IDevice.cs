using System;

namespace Devices
{
    /// <summary>
    /// 设备抽象类
    /// </summary>
    public abstract class IDevice : IDisposable
    {
        /// <summary>
        /// 事件处理器
        /// </summary>
        /// <param name="deviceEvent">事件</param>
        /// <param name="arguments">参数</param>
        public delegate void EventHandler(DeviceEvent deviceEvent, params object[] arguments);

        /// <summary>
        /// 设备状态
        /// </summary>
        protected DeviceStatus status = DeviceStatus.Idle;

        /// <summary>
        /// 设备索引
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// 设备型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns>是否成功</returns>
        public abstract bool Initialize();

        /// <summary>
        /// 打开设备
        /// </summary>
        /// <returns>是否成功</returns>
        public abstract bool Open();

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns>是否成功</returns>
        public abstract bool Close();

        /// <summary>
        /// 从设备中读取数据
        /// </summary>
        /// <param name="mode">读取方式</param>
        /// <param name="dataAddr">内存地址</param>
        /// <param name="bufferLength">缓冲区长度</param>
        /// <returns>是否成功</returns>
        public abstract bool Read(ReadMode mode, IntPtr dataAddr, int bufferLength);

        /// <summary>
        /// 从设备中读取数据
        /// </summary>
        /// <param name="mode">读取方式</param>
        /// <param name="inData">输入数据</param>
        /// <param name="outData">输出数据</param>
        /// <param name="used">数据长度</param>
        /// <returns>是否成功</returns>        
        public abstract bool Read(ReadMode mode, in object inData, out object outData, out int used);

        /// <summary>
        /// 将数据写入设备
        /// </summary>
        /// <param name="mode">写入方式</param>
        /// <param name="data">数据</param>
        /// <returns>是否成功</returns>
        public abstract bool Write(WriteMode mode, object data);

        /// <summary>
        /// 控制设备
        /// </summary>
        /// <param name="mode">控制类型</param>
        /// <param name="data">数据</param>
        /// <returns>是否成功</returns>
        public abstract bool Control(ControlMode mode, object data);

        /// <summary>
        /// 获取设备类型
        /// </summary>
        /// <returns>设备类型</returns>
        public abstract DeviceCategory GetDeviceType();

        /// <summary>
        /// 获取设备状态
        /// </summary>
        /// <returns>设备状态</returns>
        public abstract DeviceStatus GetDeviceStatus();

        /// <summary>
        /// 释放资源
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// 事件
        /// </summary>
        public event EventHandler Handler;

        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="deviceEvent">事件</param>
        /// <param name="arguments">参数</param>
        protected void RaiseEvent(DeviceEvent deviceEvent, params object[] arguments)
        {
            Handler?.Invoke(deviceEvent, arguments);
        }
    }
}
