using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Communication
{
    /// <summary>
    /// 会话管理器
    /// </summary>
    public abstract class SessionManager
    {
        /// <summary>
        /// 保活间隔
        /// </summary>
        private const int KEEP_ALIVE_DURATION = 30 * 1000;

        /// <summary>
        /// 保活时间
        /// </summary>
        private const int MAX_KEEP_ALIVE_DURATION = 3 * KEEP_ALIVE_DURATION;

        /// <summary>
        /// 会话列表
        /// </summary>
        protected Hashtable sessions = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 会话列表
        /// </summary>
        protected List<Pipe> sessionList = new List<Pipe>();

        /// <summary>
        /// 会话计数器
        /// </summary>
        private int sessionCounter = 0;

        /// <summary>
        /// 添加会话
        /// </summary>
        /// <param name="sessionId">会话索引</param>
        /// <param name="pipe">会话</param>
        public virtual void AddSession(string sessionId, Pipe pipe)
        {
            // 添加并同步SessionId
            sessions.Add(sessionId, pipe);
            sessionList.Add(pipe);
            pipe.OnConnected();
        }

        /// <summary>
        /// 获取会话
        /// </summary>
        /// <param name="sessionId">会话索引</param>
        /// <returns>会话通讯管道</returns>
        public Pipe GetSession(string sessionId)
        {
            return sessions[sessionId] as Pipe;
        }

        /// <summary>
        /// 创建会话
        /// </summary>
        /// <param name="srcId">源客户端索引</param>
        /// <param name="dstId">目标客户端索引</param>
        /// <param name="sessionId">会话索引</param>
        /// <returns>会话</returns>
        protected abstract Pipe OnNewSession(string srcId, string dstId, string sessionId);

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="buffer">接收缓冲区</param>
        /// <param name="length">接收字节数</param>
        protected virtual void OnReceive(byte[] buffer, int length)
        {
            var protocol = Protocol.Parse(buffer, length);
            var pipe = GetSession(protocol.SessionId);
            if (pipe == null) {
                // 创建会话
                var sessionId = GenerateSessionId();
                pipe = OnNewSession(protocol.SrcId, protocol.DstId, sessionId);
                AddSession(sessionId, pipe);
            }

            pipe.OnReceive(protocol);
        }

        /// <summary>
        /// 生成会话索引
        /// </summary>
        /// <returns>会话索引</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private string GenerateSessionId()
        {
            while (true) {
                sessionCounter = (++sessionCounter) % 9999;
                var sessionId = $"{sessionCounter:D4}";
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
                        var list = new List<Pipe>();
                        sessionList.ForEach(pipe => {
                            if ((now - pipe.GetLastActiveTime()).Milliseconds > MAX_KEEP_ALIVE_DURATION) {
                                list.Add(pipe);
                            }
                            else {
                                pipe.KeepAlive();
                            }
                        });

                        foreach (var pipe in list) {
                            pipe.Dispose();
                            sessions.Remove(pipe.SessionId);
                            sessionList.Remove(pipe);
                            using (new MethodUtils.Unlocker(this)) {
                                OnSessionClosedCallback?.Invoke(pipe);
                            }
                        }
                    }

                    Thread.Sleep(KEEP_ALIVE_DURATION);
                }
            });
        }
    }
}
