using IRMonitor.Miscs;
using Miscs;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Communication.Base
{
    /// <summary>
    /// 通讯管道
    /// </summary>
    public abstract class Pipe : IDisposable
    {
        /// <summary>
        /// 请求信息
        /// </summary>
        public class Request
        {
            /// <summary>
            /// 用户索引
            /// </summary>
            public string clientId;

            /// <summary>
            /// 会话索引
            /// </summary>
            public string sessionId;
        }

        /// <summary>
        /// 用户索引长度
        /// </summary>
        public const int CLIENT_ID_LENGTH = 4;

        /// <summary>
        /// 定义完成回调函数代理
        /// </summary>
        public delegate void DgOnCompletedCallback();

        /// <summary>
        /// 定义发送数据回调函数代理
        /// </summary>
        /// <param name="state">状态信息</param>
        public delegate void DgOnSendCompletedCallback(object state);

        /// <summary>
        /// 定义接收数据回调函数代理
        /// </summary>
        /// <param name="request">请求信息</param>
        /// <param name="buffer">接收缓冲区</param>
        /// <param name="length">接收字节数</param>
        public delegate void DgOnReceiveCallback(Request request, byte[] buffer, int length);

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
        /// 用户索引
        /// </summary>
        public string ClientId { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int retry = 3;

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
        /// <param name="targetClientId">目标客户索引</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <param name="state">状态</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Send(string targetClientId, byte[] buffer, int offset, int length, object state)
        {
            var newLength = CLIENT_ID_LENGTH + CLIENT_ID_LENGTH + length;
            var data = new byte[newLength];
            Array.Copy(Encoding.UTF8.GetBytes(ClientId), 0, data, 0, CLIENT_ID_LENGTH);
            Array.Copy(Encoding.UTF8.GetBytes(targetClientId), 0, data, CLIENT_ID_LENGTH, CLIENT_ID_LENGTH);
            Array.Copy(buffer, offset, data, CLIENT_ID_LENGTH + CLIENT_ID_LENGTH, length);

            for (var i = 0; i < retry; ++i) {
                if (SendData(data, state)) {
                    return;
                }
            }
        }

        /// <summary>
        /// 注入接收数据
        /// </summary>
        /// <param name="request">请求信息</param>
        /// <param name="buffer">接收缓冲区</param>
        /// <param name="length">接收字节数</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void InjectReceiveData(Request request, byte[] buffer, int length)
        {
            HandleReceiveData(request, buffer, length);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="state">状态</param>
        /// <returns>是否成功</returns>
        protected virtual bool SendData(byte[] buffer, object state)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 处理接收数据
        /// </summary>
        /// <param name="request">请求信息</param>
        /// <param name="buffer">接收缓冲区</param>
        /// <param name="length">接收字节数</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual void HandleReceiveData(Request request, byte[] buffer, int length)
        {
            int headerLength = CLIENT_ID_LENGTH + CLIENT_ID_LENGTH;
            if (length < headerLength) {
                return;
            }

            var targetClientId = Encoding.UTF8.GetString(buffer, CLIENT_ID_LENGTH, CLIENT_ID_LENGTH);
            if (targetClientId != ClientId) {
                return;
            }

            var data = buffer.SubArray(headerLength, length - headerLength);
            using (new MethodUtils.Unlocker(this)) {
                OnReceiveCallback?.Invoke(new Request() { clientId = Encoding.UTF8.GetString(buffer, 0, CLIENT_ID_LENGTH) }, data, length - headerLength);
            }
        }
    }
}
