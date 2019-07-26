using Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Communication.Transport
{
    /// <summary>
    /// 传输通讯管道
    /// </summary>
    public class TransportPipe : Pipe
    {
        /// <summary>
        /// 最高优先级
        /// </summary>
        private const int PRIORITY_HIGHEST = 255;

        /// <summary>
        /// 最低优先级
        /// </summary>
        private const int PRIORITY_LOWEST = 0;

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

        public TransportPipe()
        {
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

        public override void Connect(Dictionary<string, object> arguments)
        {
            pipe?.Connect(arguments);
        }

        public override void Disconnect()
        {
            pipe?.Disconnect();
        }

        public override void Send(byte[] buffer, int offset, int length, object state)
        {
            Send(buffer, offset, length, PRIORITY_LOWEST, state);
        }

        public override void Receive(byte[] buffer, int length)
        {
            throw new NotImplementedException();
        }

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
                    pipe?.Send(data.buffer, data.offset, data.length, data.state);
                }
                else {
                    queue.Wait();
                }
            }
        }
    }
}
