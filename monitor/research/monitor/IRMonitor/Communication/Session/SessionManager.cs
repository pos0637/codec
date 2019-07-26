using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Communication
{
    /// <summary>
    /// 会话管理器
    /// </summary>
    public abstract class SessionManager : IDisposable
    {
        /// <summary>
        /// 定义创建新会话事件函数代理
        /// </summary>
        public delegate void DgOnNewSessionCallback(SessionPipe pipe);

        /// <summary>
        /// 定义关闭会话事件函数代理
        /// </summary>
        public delegate void DgOnSessionClosedCallback(SessionPipe pipe);

        /// <summary>
        /// 保活间隔
        /// </summary>
        private const int KEEP_ALIVE_DURATION = 3000;

        /// <summary>
        /// 保活时间
        /// </summary>
        private const int MAX_KEEP_ALIVE_DURATION = 10000;

        /// <summary>
        /// 创建新会话事件函数
        /// </summary>
        public DgOnNewSessionCallback OnNewSessionCallback { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>
        /// 关闭会话事件函数
        /// </summary>
        public DgOnSessionClosedCallback OnSessionClosedCallback { [MethodImpl(MethodImplOptions.Synchronized)] get; [MethodImpl(MethodImplOptions.Synchronized)] set; }

        /// <summary>
        /// 会话列表
        /// </summary>
        protected Hashtable sessions = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 会话列表
        /// </summary>
        protected List<SessionPipe> sessionList = new List<SessionPipe>();

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="arguments">参数列表</param>
        public virtual void Initialize(Dictionary<string, object> arguments)
        {
            KeepAlive();
        }

        /// <summary>
        /// 添加会话
        /// </summary>
        /// <param name="sessionId">会话索引</param>
        public void AddSession(string sessionId, SessionPipe pipe)
        {
            // 添加并同步SessionId
            sessions.Add(sessionId, pipe);
            sessionList.Add(pipe);
            pipe.KeepAlive();
            OnNewSessionCallback?.Invoke(pipe);
        }

        /// <summary>
        /// 获取会话
        /// </summary>
        /// <param name="sessionId">会话索引</param>
        /// <returns>会话通讯管道</returns>
        public SessionPipe GetSession(string sessionId)
        {
            return sessions[sessionId] as SessionPipe;
        }

        /// <summary>
        /// 保活线程
        /// </summary>
        private void KeepAlive()
        {
            Task.Run(async () => {
                while (true) {
                    DateTime now = DateTime.Now;
                    lock (this) {
                        var enumerator = sessions.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            var pipe = enumerator.Value as SessionPipe;
                            if ((now - pipe.LastActiveTime).Milliseconds >= MAX_KEEP_ALIVE_DURATION) {
                                pipe.Dispose();
                                sessions.Remove(enumerator.Key);
                                sessionList.Remove(pipe);
                                OnSessionClosedCallback?.Invoke(pipe);
                            }
                            else {
                                pipe.KeepAlive();
                            }
                        }
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(KEEP_ALIVE_DURATION));
                }
            });
        }
    }
}
