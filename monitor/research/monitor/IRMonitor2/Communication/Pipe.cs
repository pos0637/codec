using System;

namespace Communication
{
    /// <summary>
    /// 通讯管道
    /// </summary>
    public abstract class Pipe : IDisposable
    {
        /// <summary>
        /// 源客户端索引
        /// </summary>
        public readonly string srcId;

        /// <summary>
        /// 目标客户端索引
        /// </summary>
        public readonly string dstId;

        /// <summary>
        /// 会话索引
        /// </summary>
        public readonly string sessionId;

        /// <summary>
        /// 最后激活时间
        /// </summary>
        private DateTime lastActiveTime;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="srcId">源客户端索引</param>
        /// <param name="dstId">目标客户端索引</param>
        /// <param name="sessionId">会话索引</param>
        public Pipe(string srcId, string dstId, string sessionId)
        {
            this.srcId = srcId;
            this.dstId = dstId;
            this.sessionId = sessionId;
            lastActiveTime = DateTime.Now;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// 连接成功事件处理函数
        /// </summary>
        public virtual void OnConnected() { }

        /// <summary>
        /// 连接断开事件处理函数
        /// </summary>
        public virtual void OnDisconnected() { }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="dstId">目标客户端索引</param>
        /// <param name="buffer">数据</param>
        /// <param name="offset">偏移</param>
        /// <param name="length">长度</param>
        /// <param name="state">状态</param>
        public abstract void Send(string dstId, byte[] buffer, int offset, int length, object state);

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="protocol">协议数据</param>
        public virtual void OnReceive(Protocol protocol) { }

        /// <summary>
        /// 发送保活数据
        /// </summary>
        public virtual void KeepAlive()
        {
            Send(dstId, null, 0, 0, null);
        }

        /// <summary>
        /// 获取最后激活时间
        /// </summary>
        /// <returns>最后激活时间</returns>
        public virtual DateTime GetLastActiveTime()
        {
            return lastActiveTime;
        }
    }
}
