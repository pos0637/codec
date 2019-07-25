using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static Common.Utils;

namespace Communication
{
    /// <summary>
    /// MQTT会话管理器
    /// </summary>
    public class MQTTSessionManager : SessionManager
    {
        /// <summary>
        /// MQTT会话通讯管道
        /// </summary>
        private class MQTTSessionPipe : SessionPipe
        {
            public MQTTSessionPipe(Pipe pipe, string sessionId) : base(pipe, sessionId) { }

            public override void Connect(Dictionary<string, object> arguments) { }

            public override void Disconnect() { }
        }

        /// <summary>
        /// 通讯管道
        /// </summary>
        private MQTTPipe pipe;

        public override void Initialize(Dictionary<string, object> arguments)
        {
            pipe = new MQTTPipe();
            pipe.Connect(arguments);
            pipe.OnConnectedCallback = OnConnected;
            pipe.OnDisconnectedCallback = OnDisconnected;
            pipe.OnSendCompletedCallback = OnSendCompleted;
            pipe.OnReceiveCallback = OnReceive;
            pipe.OnExceptionCallback = OnException;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnConnected()
        {
            sessionList.ForEach(pipe => pipe.OnConnectedCallback?.Invoke());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnDisconnected()
        {
            sessionList.ForEach(pipe => pipe.OnDisconnectedCallback?.Invoke());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnSendCompleted(object state)
        {
            sessionList.ForEach(pipe => pipe.OnSendCompletedCallback?.Invoke(state));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnReceive(byte[] buffer, int length)
        {
            int headerLength = SessionPipe.SESSION_ID_LENGTH + SessionPipe.SESSION_ID_LENGTH;
            if (length < headerLength) {
                return;
            }

            // 判断会话索引是否存在
            var sessionId = Encoding.UTF8.GetString(buffer.SubArray(0, SessionPipe.SESSION_ID_LENGTH));
            var pipe = GetSession(sessionId);
            if (pipe != null) {
                // 调用对应会话
                pipe.Receive(buffer, length);
            }
            else {
                // 添加会话
                var clientId = Encoding.UTF8.GetString(buffer.SubArray(SessionPipe.SESSION_ID_LENGTH, SessionPipe.SESSION_ID_LENGTH));
                AddSession(clientId, new MQTTSessionPipe(pipe, clientId));
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void OnException(Exception e)
        {
            sessionList.ForEach(pipe => pipe.OnExceptionCallback?.Invoke(e));
        }
    }
}
