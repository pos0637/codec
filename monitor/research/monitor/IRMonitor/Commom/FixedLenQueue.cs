using System;
using System.Collections.Generic;
using System.Threading;

namespace Common
{
    /// <summary>
    /// 固定长度的队列
    /// </summary>
    public class FixedLenQueue<T>
    {
        /// <summary>
        /// 事件
        /// </summary>
        private Object mEvent = new Object();

        /// <summary>
        /// 存储泛型链表
        /// </summary>
        private List<T> mList = new List<T>();

        /// <summary>
        /// 队列最大长度
        /// </summary>
        private Int32 mLen;

        public Int32 Count
        {
            get { return mList.Count; }
        }

        public FixedLenQueue(Int32 len)
        {
            mLen = len;
        }

        public T Dequeue()
        {
            T temp = default(T);

            lock (mList) {
                if (mList.Count <= 0)
                    return temp;
                else {
                    temp = mList[0];
                    mList.RemoveAt(0);
                    return temp;
                }
            }
        }

        public void Enqueue(T item)
        {
            lock (mList) {
                if (mList.Count >= mLen)
                    mList.RemoveAt(0);

                mList.Add(item);

                lock (mEvent) {
                    Monitor.PulseAll(mEvent);
                }
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
