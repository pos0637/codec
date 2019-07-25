using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using static Common.Utils;

namespace Communication
{
    /// <summary>
    /// 会话通讯管道
    /// </summary>
    public class SessionPipe : Pipe
    {
        /// <summary>
        /// 会话索引长度
        /// </summary>
        public const int SESSION_ID_LENGTH = 32;

        /// <summary>
        /// 最后激活时间
        /// </summary>
        public DateTime LastActiveTime { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>
        /// 通讯管道
        /// </summary>
        protected Pipe pipe;

        /// <summary>
        /// 会话索引
        /// </summary>
        protected string sessionIdStr;

        /// <summary>
        /// 会话索引
        /// </summary>
        protected byte[] sessionId;

        protected SessionPipe() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pipe">通讯管道</param>
        /// <param name="sessionId">会话索引</param>
        public SessionPipe(Pipe pipe, string sessionId)
        {
            this.pipe = pipe;
            this.sessionIdStr = sessionId;
            this.sessionId = Encoding.UTF8.GetBytes(sessionId);
            Debug.Assert(this.sessionId.Length == 32);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Connect(Dictionary<string, object> arguments)
        {
            pipe.Connect(arguments);
            pipe.OnConnectedCallback = OnConnected;
            pipe.OnDisconnectedCallback = OnDisconnected;
            pipe.OnSendCompletedCallback = OnSendCompleted;
            pipe.OnReceiveCallback = OnReceive;
            pipe.OnExceptionCallback = OnException;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Disconnect()
        {
            pipe.Disconnect();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Send(byte[] buffer, int offset, int length, object state)
        {
            int newLength = SESSION_ID_LENGTH + length;
            byte[] data = new byte[newLength];
            Array.Copy(sessionId, 0, data, 0, SESSION_ID_LENGTH);
            Array.Copy(buffer, 0, data, SESSION_ID_LENGTH, length);

            pipe.Send(data, 0, newLength, state);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Receive(byte[] buffer, int length)
        {
            int headerLength = SESSION_ID_LENGTH + SESSION_ID_LENGTH;
            if (length < headerLength) {
                return;
            }

            byte[] data = buffer.SubArray(headerLength, length - headerLength);
            LastActiveTime = DateTime.Now;
            OnReceiveCallback?.Invoke(data, data.Length);
        }

        /// <summary>
        /// 发送保活数据
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void KeepAlive()
        {
            pipe.Send(sessionId, 0, sessionId.Length, null);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnConnected()
        {
            OnConnectedCallback?.Invoke();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnDisconnected()
        {
            OnDisconnectedCallback?.Invoke();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnSendCompleted(object state)
        {
            OnSendCompletedCallback?.Invoke(state);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnReceive(byte[] buffer, int length)
        {
            Receive(buffer, length);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnException(Exception e)
        {
            OnExceptionCallback?.Invoke(e);
        }
    }
}
