using System;

namespace Communication
{
    /// <summary>
    /// 通讯会话
    /// </summary>
    public class Session : IDisposable
    {
        /// <summary>
        /// 源索引
        /// </summary>
        public readonly string srcId;

        /// <summary>
        /// 目标索引
        /// </summary>
        public string dstId;

        /// <summary>
        /// 会话索引
        /// </summary>
        public readonly string sessionId;

        /// <summary>
        /// 最后激活时间
        /// </summary>
        public DateTime LastActiveTime { get; set; }

        /// <summary>
        /// 通讯会话管理器
        /// </summary>
        private readonly SessionManager manager;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="factory">通讯会话管理器</param>
        /// <param name="srcId">源索引</param>
        /// <param name="dstId">目标索引</param>
        /// <param name="sessionId">会话索引</param>
        public Session(SessionManager manager, string srcId, string dstId, string sessionId)
        {
            this.manager = manager;
            this.srcId = srcId;
            this.dstId = dstId;
            this.sessionId = sessionId;
            LastActiveTime = DateTime.Now;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="dstId">目标客户端索引</param>
        /// <param name="data">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        public virtual void Send(string dstId, byte[] data, int offset, int length)
        {
            if (string.IsNullOrEmpty(dstId)) {
                return;
            }

            if (string.IsNullOrEmpty(this.dstId)) {
                this.dstId = dstId;
            }

            if (!dstId.Equals(this.dstId)) {
                throw new ArgumentException();
            }

            manager?.Send(this, data, offset, length);
        }

        /// <summary>
        /// 保活
        /// </summary>
        public virtual void KeepAlive()
        {
            Send(dstId, null, 0, 0);
        }
    }
}
