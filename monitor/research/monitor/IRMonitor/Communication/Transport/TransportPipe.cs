﻿using Common;
using Communication.Session;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Communication.Transport
{
    /// <summary>
    /// 传输通讯管道
    /// </summary>
    public class TransportPipe : SessionPipe
    {
        /// <summary>
        /// 最高优先级
        /// </summary>
        public const int PRIORITY_HIGHEST = 255;

        /// <summary>
        /// 最低优先级
        /// </summary>
        public const int PRIORITY_LOWEST = 0;

        /// <summary>
        /// 优先级数据
        /// </summary>
        private class PrioritizedData
        {
            public byte[] buffer;
            public int offset;
            public int length;
            public int priority;
            public bool ack;
            public object state;
        }

        /// <summary>
        /// 会话通讯管道
        /// </summary>
        private SessionPipe pipe;

        /// <summary>
        /// 优先队列
        /// </summary>
        private PriorityQueue<PrioritizedData> queue = new PriorityQueue<PrioritizedData>((data1, data2) => {
            return (data1.priority > data2.priority) ? 1 : (data1.priority == data2.priority) ? 0 : -1;
        });

        /// <summary>
        /// 运行标志
        /// </summary>
        private bool runningFlag = true;

        protected TransportPipe() { }

        public TransportPipe(SessionPipe pipe)
        {
            this.pipe = pipe;
            pipe.OnConnectedCallback = OnConnected;
            pipe.OnDisconnectedCallback = OnDisconnected;
            pipe.OnSendCompletedCallback = OnSendCompleted;
            pipe.OnReceiveCallback = OnReceive;
            pipe.OnExceptionCallback = OnException;

            if (!ThreadPool.QueueUserWorkItem(SendDataThread)) {
                throw new OverflowException();
            }
        }

        public override void Dispose()
        {
            runningFlag = false;
            queue.Notify();
            base.Dispose();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Connect(Dictionary<string, object> arguments)
        {
            pipe.Connect(arguments);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Disconnect()
        {
            pipe.Disconnect();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Send(byte[] buffer, int offset, int length, object state)
        {
            Send(buffer, offset, length, PRIORITY_LOWEST, state);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void Receive(string clientId, byte[] buffer, int length)
        {
            // TODO: 处理Frame
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void KeepAlive()
        {
            pipe.KeepAlive();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <param name="priority">优先级</param>
        /// <param name="state">状态</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Send(byte[] buffer, int offset, int length, int priority, object state)
        {
            if ((buffer == null) || (buffer.Length < length))
                throw new ArgumentException();

            queue.Enqueue(new PrioritizedData() {
                buffer = buffer,
                offset = offset,
                length = length,
                priority = priority,
                state = state
            });
        }

        /// <summary>
        /// 发送数据线程
        /// </summary>
        /// <param name="state">参数</param>
        private void SendDataThread(object state)
        {
            while (runningFlag) {
                var data = queue.Dequeue();
                if (data != null) {
                    pipe.Send(data.buffer, data.offset, data.length, data.state);
                }
                else {
                    queue.Wait();
                }
            }
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
