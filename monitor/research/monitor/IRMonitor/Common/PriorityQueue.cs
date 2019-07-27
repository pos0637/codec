using System;
using System.Collections.Generic;
using System.Threading;

namespace Common
{
    /// <summary>
    /// 优先级队列
    /// </summary>
    /// <typeparam name="T">泛型对象</typeparam>
    public class PriorityQueue<T>
    {
        /// <summary>
        /// 消息优先级比较委托
        /// </summary>
        /// <param name="oldPriority">原有消息的优先级</param>
        /// <param name="newPriority">新消息的优先级</param>
        /// <returns>比较结果</returns>
        public delegate Int32 PriorityCompareHandler(T oldPriority, T newPriority);

        /// <summary>
        /// 事件
        /// </summary>
        private Object mEvent = new Object();

        /// <summary>
        /// 存储泛型链表
        /// </summary>
        private List<T> mPriorityList = new List<T>();

        /// <summary>
        /// 消息优先级比较器
        /// </summary>
        private PriorityCompareHandler mCompare;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="compare">接口函数</param>
        public PriorityQueue(PriorityCompareHandler compare)
        {
            mCompare = compare;
        }

        public PriorityQueue()
        {
        }

        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="item">消息对象</param>
        public void Enqueue(T item)
        {
            Boolean hasFind = false;
            lock (mPriorityList) {
                for (Int32 i = mPriorityList.Count - 1; i >= 0; i--) {
                    if (mCompare(mPriorityList[i], item) <= 0) {
                        mPriorityList.Insert(i + 1, item);
                        hasFind = true;
                        break;
                    }
                }

                if (!hasFind)
                    mPriorityList.Insert(0, item);
            }

            lock (mEvent) {
                Monitor.PulseAll(mEvent);
            }
        }

        /// <summary>
        /// 移除并返回第一个对象
        /// </summary>
        /// <returns>返回第一个对象结果</returns>
        public T Dequeue()
        {
            lock (mPriorityList) {
                if (mPriorityList.Count != 0) {
                    T firstObject = mPriorityList[0];
                    mPriorityList.Remove(firstObject);
                    return firstObject;
                }
            }

            return default(T);
        }

        /// <summary>
        /// 获取链表元素个数
        /// </summary>
        /// <returns>链表元素个数</returns>
        public Int32 Size()
        {
            return mPriorityList.Count;
        }

        /// <summary>
        /// 等待事件锁重新获取
        /// </summary>
        /// <returns></returns>
        public Boolean Wait()
        {
            lock (mEvent) {
                return Monitor.Wait(mEvent);
            }
        }

        /// <summary>
        /// 通知所有的等待线程对象状态的更改
        /// </summary>
        public void Notify()
        {
            lock (mEvent) {
                Monitor.PulseAll(mEvent);
            }
        }
    }
}
