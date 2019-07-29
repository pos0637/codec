using Communication.Base;
using Communication.Transport;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Communication.Session
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
            public MQTTSessionPipe(Pipe pipe, string targetClientId, string sessionId) : base(pipe, targetClientId, sessionId) { }

            public override void Connect(Dictionary<string, object> arguments) { }

            public override void Disconnect() { }
        }

        /// <summary>
        /// 通讯管道
        /// </summary>
        private MQTTPipe pipe;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Dispose()
        {
            pipe?.Dispose();
            base.Dispose();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Initialize(Dictionary<string, object> arguments)
        {
            base.Initialize(arguments);

            pipe = new MQTTPipe();
            pipe.Connect(arguments);
            pipe.OnConnectedCallback = OnConnected;
            pipe.OnDisconnectedCallback = OnDisconnected;
            pipe.OnSendCompletedCallback = OnSendCompleted;
            pipe.OnReceiveCallback = OnReceive;
            pipe.OnExceptionCallback = OnException;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override SessionPipe OnNewSession(string targetClientId, string sessionId)
        {
            return new TransportPipe(new MQTTSessionPipe(this.pipe, targetClientId, sessionId));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnReceive(Base.Pipe.Response response, byte[] buffer, int length)
        {
            base.OnReceive(response, buffer, length);
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
        private void OnException(Exception e)
        {
            sessionList.ForEach(pipe => pipe.OnExceptionCallback?.Invoke(e));
        }
    }
}
