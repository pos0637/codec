using Common;
using System.Collections.Generic;

namespace Miscs
{
    /// <summary>
    /// 事件订阅发布器
    /// </summary>
    public class EventEmitter : Singleton<EventEmitter>
    {
        /// <summary>
        /// 事件处理器
        /// </summary>
        /// <param name="arguments">参数</param>
        public delegate void EventHandler(params object[] arguments);

        /// <summary>
        /// 事件
        /// </summary>
        private class Event
        {
            /// <summary>
            /// 事件名称
            /// </summary>
            public string name;

            /// <summary>
            /// 事件处理器
            /// </summary>
            public event EventHandler handler;

            /// <summary>
            /// 处理事件
            /// </summary>
            /// <param name="arguments">参数</param>
            public void Handle(params object[] arguments)
            {
                handler?.Invoke(arguments);
            }
        }

        /// <summary>
        /// 事件列表
        /// </summary>
        private readonly Dictionary<string, Event> events = new Dictionary<string, Event>();

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <param name="handler">事件处理器</param>
        /// <returns>是否成功</returns>
        public bool Subscribe(string name, EventHandler handler)
        {
            if (string.IsNullOrEmpty(name)) {
                return false;
            }

            Event e = null;
            lock (events) {
                if (events.ContainsKey(name)) {
                    e = events[name];
                }
                else {
                    e = new Event() { name = name };
                    events.Add(name, e);
                }
            }

            lock (e) {
                e.handler += handler;
            }

            return true;
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <param name="handler">事件处理器</param>
        /// <returns>是否成功</returns>
        public bool Unsubscribe(string name, EventHandler handler)
        {
            if (string.IsNullOrEmpty(name)) {
                return false;
            }

            Event e = null;
            lock (events) {
                if (events.ContainsKey(name)) {
                    e = events[name];
                }
                else {
                    return false;
                }
            }

            lock (e) {
                e.handler -= handler;
            }

            return true;
        }

        /// <summary>
        /// 取消所有订阅事件
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <returns>是否成功</returns>
        public bool UnsubscribeAll(string name)
        {
            if (string.IsNullOrEmpty(name)) {
                return false;
            }

            lock (events) {
                events.Remove(name);
            }

            return true;
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="name">事件名称</param>
        /// <param name="arguments">参数</param>
        /// <returns>是否成功</returns>
        public bool Publish(string name, params object[] arguments)
        {
            if (string.IsNullOrEmpty(name)) {
                return false;
            }

            Event e = null;
            lock (events) {
                if (events.ContainsKey(name)) {
                    e = events[name];
                }
                else {
                    return false;
                }
            }

            lock (e) {
                e.Handle(arguments);
            }

            return true;
        }
    }
}
