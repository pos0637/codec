using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using static Common.Utils;

namespace Communication.Session
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
        private const int KEEP_ALIVE_DURATION = 30 * 1000;

        /// <summary>
        /// 保活时间
        /// </summary>
        private const int MAX_KEEP_ALIVE_DURATION = 3 * KEEP_ALIVE_DURATION;

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
        /// 运行标志
        /// </summary>
        private bool runningFlag = true;

        /// <summary>
        /// 会话计数器
        /// </summary>
        private int sessionCounter = 0;

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            runningFlag = false;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="arguments">参数列表</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Initialize(Dictionary<string, object> arguments)
        {
            KeepAlive();
        }

        /// <summary>
        /// 添加会话
        /// </summary>
        /// <param name="sessionId">会话索引</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void AddSession(string sessionId, SessionPipe pipe)
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
        /// 创建新会话
        /// </summary>
        /// <param name="targetClientId">目标用户索引</param>
        /// <param name="sessionId">会话索引</param>
        /// <returns>新会话</returns>
        protected abstract SessionPipe OnNewSession(string targetClientId, string sessionId);

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="clientId">客户索引</param>
        /// <param name="buffer">接收缓冲区</param>
        /// <param name="length">接收字节数</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual void OnReceive(string clientId, byte[] buffer, int length)
        {
            const int headerLength = SessionPipe.SESSION_ID_LENGTH;
            if (length < headerLength) {
                return;
            }

            // 判断会话索引是否存在
            var sessionId = Encoding.UTF8.GetString(buffer.SubArray(0, SessionPipe.SESSION_ID_LENGTH));
            var pipe = GetSession(sessionId);
            if (pipe != null) {
                // 调用对应会话
                pipe.Receive(clientId, buffer, length);
            }
            else {
                // 添加会话
                sessionId = GenerateSessionId();
                AddSession(sessionId, OnNewSession(clientId, sessionId));
            }
        }

        /// <summary>
        /// 生成会话索引
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private string GenerateSessionId()
        {
            while (true) {
                sessionCounter = (++sessionCounter) % 9999;
                string sessionId = $"{sessionCounter:D4}";
                if (GetSession(sessionId) == null) {
                    return sessionId;
                }
            }
        }

        /// <summary>
        /// 保活线程
        /// </summary>
        private void KeepAlive()
        {
            ThreadPool.QueueUserWorkItem(state => {
                while (runningFlag) {
                    DateTime now = DateTime.Now;
                    lock (this) {
                        var enumerator = sessions.GetEnumerator();
                        while (enumerator.MoveNext()) {
                            var pipe = enumerator.Value as SessionPipe;
                            if ((now - pipe.LastActiveTime).Milliseconds > MAX_KEEP_ALIVE_DURATION) {
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

                    Thread.Sleep(KEEP_ALIVE_DURATION);
                }
            });
        }
    }
}
