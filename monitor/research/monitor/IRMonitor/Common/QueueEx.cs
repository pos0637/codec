using System;
using System.Collections.Generic;
using System.Threading;

namespace Common
{
    /// <summary>
    /// 队列
    /// </summary>
    public class QueueEx<T> : Queue<T>
    {
        /// <summary>
        /// 事件
        /// </summary>
        public Object mEvent = new Object();

        public new void Enqueue(T item)
        {
            base.Enqueue(item);

            lock (mEvent) {
                Monitor.PulseAll(mEvent);
            }
        }

        public Boolean Wait()
        {
            lock (mEvent) {
                return Monitor.Wait(mEvent);
            }
        }

        public Boolean Wait(Int32 millisecondsTimeout)
        {
            lock (mEvent) {
                return Monitor.Wait(mEvent, millisecondsTimeout);
            }
        }

        public void Notify()
        {
            lock (mEvent) {
                Monitor.PulseAll(mEvent);
            }
        }
    }
}