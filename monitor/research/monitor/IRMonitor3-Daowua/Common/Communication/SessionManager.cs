using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Communication
{
    /// <summary>
    /// 通讯会话管理器
    /// </summary>
    public abstract class SessionManager : IDisposable
    {
        /// <summary>
        /// 客户端索引
        /// </summary>
        private readonly string clientId;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clientId">客户端索引</param>
        public SessionManager(string clientId)
        {
            this.clientId = clientId;
            KeepAlive();
        }

        private SessionManager() { }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            runningFlag = false;
        }

        #region 会话管理

        /// <summary>
        /// 最大会话数量
        /// </summary>
        private const int MAX_SESSIONS = 9999;

        /// <summary>
        /// 命名会话列表
        /// </summary>
        protected Hashtable namedSessions = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 会话列表
        /// </summary>
        protected List<Session> sessions = new List<Session>();

        /// <summary>
        /// 会话列表锁
        /// </summary>
        private object sessionsLock = new object();

        /// <summary>
        /// 会话计数器
        /// </summary>
        private int sessionCounter = 0;

        /// <summary>
        /// 获取会话
        /// </summary>
        /// <param name="name">会话名称</param>
        /// <returns>会话</returns>
        public virtual Session Get(string name = null)
        {
            lock (sessionsLock) {
                var session = GetNamedSession(name);
                if (session == null) {
                    session = CreateSession(name);
                    if (string.IsNullOrEmpty(name)) {
                        sessions.Add(session);
                    }
                    else {
                        namedSessions.Add(name, session);
                    }
                }

                return session;
            }
        }

        /// <summary>
        /// 创建会话
        /// </summary>
        /// <param name="name">会话名称</param>
        /// <returns>会话</returns>
        protected virtual Session CreateSession(string name)
        {
            return new Session(this, clientId, null, name);
        }

        /// <summary>
        /// 获取命名会话
        /// </summary>
        /// <param name="name">会话名称</param>
        /// <returns></returns>
        private Session GetNamedSession(string name)
        {
            lock (sessionsLock) {
                return (!string.IsNullOrEmpty(name) && namedSessions.ContainsKey(name)) ? namedSessions[name] as Session : null;
            }
        }

        /// <summary>
        /// 生成会话索引
        /// </summary>
        /// <returns>会话索引</returns>
        private string GenerateSessionId()
        {
            lock (sessionsLock) {
                for (int i = 0; i < MAX_SESSIONS; ++i) {
                    sessionCounter = (++sessionCounter) % 9999;
                    var sessionId = $"{sessionCounter:D4}";
                    if (GetNamedSession(sessionId) == null) {
                        return sessionId;
                    }
                }
            }

            return null;
        }

        #endregion

        #region 数据收发

        /// <summary>
        /// 接收数据事件
        /// </summary>
        public event Action<Session, byte[]> OnReceiveEvent;

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="session">会话</param>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        public virtual void Send(Session session, byte[] data, int offset, int length)
        {
            OnSend(Protocol.Build(session, data, offset, length), 0, -1);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        protected abstract void OnSend(byte[] data, int offset, int length);

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="length">数据长度</param>
        protected virtual void OnReceive(byte[] data, int length)
        {
            var protocol = Protocol.Parse(data, length);
            if (protocol == null) {
                return;
            }

            if (!protocol.DstId.Equals(clientId)) {
                return;
            }

            var sessionId = protocol.SessionId;
            if (string.IsNullOrEmpty(sessionId)) {
                sessionId = GenerateSessionId();
            }

            var session = Get(sessionId);
            if (session != null) {
                session.dstId = protocol.SrcId;
                session.LastActiveTime = DateTime.Now;
            }

            if ((protocol.Data == null) || (protocol.Data.Length == 0)) {
                return;
            }

            OnReceiveEvent.Invoke(session, protocol.Data);
        }

        #endregion

        #region 生命周期

        /// <summary>
        /// 保活间隔
        /// </summary>
        private const int KEEP_ALIVE_DURATION = 30 * 1000;

        /// <summary>
        /// 保活时间
        /// </summary>
        private const int MAX_KEEP_ALIVE_DURATION = 3 * KEEP_ALIVE_DURATION;

        /// <summary>
        /// 运行标志
        /// </summary>
        private bool runningFlag = true;

        /// <summary>
        /// 保活线程
        /// </summary>
        private void KeepAlive()
        {
            ThreadPool.QueueUserWorkItem(state => {
                while (runningFlag) {
                    var now = DateTime.Now;
                    var alive = new List<Session>();
                    var kill = new List<Session>();

                    lock (sessionsLock) {
                        // 检查命名会话
                        foreach (Session session in namedSessions.Values) {
                            if ((now - session.LastActiveTime).Milliseconds > MAX_KEEP_ALIVE_DURATION) {
                                kill.Add(session);
                            }
                            else {
                                alive.Add(session);
                            }
                        }

                        foreach (var session in kill) {
                            session.Dispose();
                            namedSessions.Remove(session.sessionId);
                        }

                        // 检查会话
                        foreach (Session session in sessions) {
                            session.Dispose();
                        }

                        sessions.Clear();
                    }

                    // 保活
                    foreach (var session in alive) {
                        session.KeepAlive();
                    }

                    Thread.Sleep(KEEP_ALIVE_DURATION);
                }
            });
        }

        #endregion
    }
}
