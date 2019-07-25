using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Communication
{
    /// <summary>
    /// 通讯管道
    /// </summary>
    public abstract class Pipe : IDisposable
    {
        /// <summary>
        /// 定义完成回调函数代理
        /// </summary>
        public delegate void DgOnCompletedCallback();

        /// <summary>
        /// 定义发送回调函数代理
        /// </summary>
        /// <param name="state">状态信息</param>
        public delegate void DgOnSendCompletedCallback(object state);

        /// <summary>
        /// 定义接收回调函数代理
        /// </summary>
        /// <param name="buffer">接收缓冲区</param>
        /// <param name="length">接收字节数</param>
        public delegate void DgOnReceiveCallback(byte[] buffer, int length);

        /// <summary>
        /// 定义异常回调函数代理
        /// </summary>
        /// <param name="e">异常</param>
        public delegate void DgOnExceptionCallback(Exception e);

        /// <summary>
        /// 连接成功回调函数
        /// </summary>
        public DgOnCompletedCallback OnConnectedCallback { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>
        /// 发送回调函数
        /// </summary>
        public DgOnSendCompletedCallback OnSendCompletedCallback { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>
        /// 接收回调函数
        /// </summary>
        public DgOnReceiveCallback OnReceiveCallback { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>
        /// 断开连接回调函数
        /// </summary>
        public DgOnCompletedCallback OnDisconnectedCallback { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>
        /// 异常回调函数
        /// </summary>
        public DgOnExceptionCallback OnExceptionCallback { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            Disconnect();
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="arguments">参数列表</param>
        public abstract void Connect(Dictionary<string, object> arguments);

        /// <summary>
        /// 断开连接
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <param name="state">状态</param>
        public abstract void Send(byte[] buffer, int offset, int length, object state);

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer">接收缓冲区</param>
        /// <param name="length">接收字节数</param>
        public abstract void Receive(byte[] buffer, int length);
    }
}
