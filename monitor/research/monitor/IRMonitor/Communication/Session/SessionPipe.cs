using Common;
using Communication.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

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
        /// 通讯管道
        /// </summary>
        private Pipe pipe;

        /// <summary>
        /// 最后激活时间
        /// </summary>
        private DateTime lastActiveTime;

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
            lastActiveTime = DateTime.Now;
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
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Send(byte[] buffer, int offset, int length, object state)
        {
            var newLength = SESSION_ID_LENGTH + length;
            var data = new byte[newLength];
            Array.Copy(Encoding.UTF8.GetBytes(SessionId), 0, data, 0, SESSION_ID_LENGTH);
            Array.Copy(buffer, offset, data, SESSION_ID_LENGTH, length);

            pipe.Send(TargetClientId, buffer, 0, buffer.Length, state);
        }

        /// <summary>
        /// 发送保活数据
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void KeepAlive()
        {
            pipe.Send(TargetClientId, Encoding.UTF8.GetBytes(SessionId), 0, SESSION_ID_LENGTH, null);
        }

        /// <summary>
        /// 获取最后激活时间
        /// </summary>
        /// <returns>最后激活时间</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual DateTime GetLastActiveTime()
        {
            return lastActiveTime;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void HandleReceiveData(Response response, byte[] buffer, int length)
        {
            if (length < SESSION_ID_LENGTH) {
                return;
            }

            response.sessionId = Encoding.UTF8.GetString(buffer, 0, SESSION_ID_LENGTH);
            var data = buffer.SubArray(SESSION_ID_LENGTH, length - SESSION_ID_LENGTH);
            lastActiveTime = DateTime.Now;
            OnReceiveCallback?.Invoke(response, data, data.Length);
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
        private void OnReceive(Response response, byte[] buffer, int length)
        {
            HandleReceiveData(response, buffer, length);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnException(Exception e)
        {
            OnExceptionCallback?.Invoke(e);
        }
    }
}
