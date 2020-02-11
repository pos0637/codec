using System.Collections.Generic;
using System.Threading;

namespace Common
{
    /// <summary>
    /// 固定长度的队列
    /// </summary>
    public class FixedLengthQueue<T>
    {
        /// <summary>
        /// 事件
        /// </summary>
        private readonly object mEvent = new object();

        /// <summary>
        /// 存储泛型链表
        /// </summary>
        private readonly List<T> mList = new List<T>();

        /// <summary>
        /// 队列最大长度
        /// </summary>
        private readonly int mLength;

        public int Count
        {
            get { return mList.Count; }
        }

        public FixedLengthQueue(int length)
        {
            mLength = length;
        }

        public T Dequeue()
        {
            T temp = default;

            lock (mList) {
                if (mList.Count <= 0) {
                    return temp;
                }
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
                if (mList.Count >= mLength)
                    mList.RemoveAt(0);

                mList.Add(item);

                lock (mEvent) {
                    Monitor.PulseAll(mEvent);
                }
            }
        }

        public bool Wait()
        {
            lock (mEvent) {
                return Monitor.Wait(mEvent);
            }
        }

        public bool Wait(int timeout)
        {
            lock (mEvent) {
                return Monitor.Wait(mEvent, timeout);
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
