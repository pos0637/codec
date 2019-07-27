using Communication.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using static Common.Utils;

namespace Communication.Session
{
    /// <summary>
    /// 会话通讯管道
    /// </summary>
    public class SessionPipe : Pipe
    {
        /// <summary>
        /// 会话索引长度
        /// </summary>
        public const int SESSION_ID_LENGTH = 4;

        /// <summary>
        /// 目标用户索引
        /// </summary>
        public string TargetClientId { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>
        /// 会话索引
        /// </summary>
        public string SessionId { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>
        /// 最后激活时间
        /// </summary>
        public DateTime LastActiveTime { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>
        /// 通讯管道
        /// </summary>
        private Pipe pipe;

        protected SessionPipe() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pipe">通讯管道</param>
        /// <param name="targetClientId">目标用户索引</param>
        /// <param name="sessionId">会话索引</param>
        public SessionPipe(Pipe pipe, string targetClientId, string sessionId)
        {
            this.pipe = pipe;
            TargetClientId = targetClientId;
            Debug.Assert(TargetClientId.Length == CLIENT_ID_LENGTH);
            SessionId = sessionId;
            Debug.Assert(SessionId.Length == SESSION_ID_LENGTH);
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
        public override void Send(string targetClientId, byte[] buffer, int offset, int length, object state)
        {
            pipe.Send(targetClientId, buffer, offset, length, state);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Receive(string clientId, byte[] buffer, int length)
        {
            int headerLength = SESSION_ID_LENGTH;
            if (length < headerLength) {
                return;
            }

            byte[] data = buffer.SubArray(headerLength, length - headerLength);
            LastActiveTime = DateTime.Now;
            OnReceiveCallback?.Invoke(clientId, data, data.Length);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Send(byte[] buffer, int offset, int length, object state)
        {
            int newLength = SESSION_ID_LENGTH + length;
            byte[] data = new byte[newLength];
            Array.Copy(Encoding.UTF8.GetBytes(SessionId), 0, data, 0, SESSION_ID_LENGTH);
            Array.Copy(buffer, offset, data, SESSION_ID_LENGTH, length);

            Send(TargetClientId, data, 0, newLength, state);
        }

        /// <summary>
        /// 发送保活数据
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void KeepAlive()
        {
            pipe.Send(TargetClientId, Encoding.UTF8.GetBytes(SessionId), 0, SESSION_ID_LENGTH, null);
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
        private void OnReceive(string clientId, byte[] buffer, int length)
        {
            Receive(clientId, buffer, length);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnException(Exception e)
        {
            OnExceptionCallback?.Invoke(e);
        }
    }
}
