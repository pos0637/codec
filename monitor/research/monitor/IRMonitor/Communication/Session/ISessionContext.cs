using System;
using static Communication.Base.Pipe;

namespace Communication.Session
{
    /// <summary>
    /// 会话上下文
    /// </summary>
    public interface ISessionContext : IDisposable
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize();

        /// <summary>
        /// 接收数据回调函数
        /// </summary>
        /// <param name="response">响应信息</param>
        /// <param name="buffer">接收缓冲区</param>
        /// <param name="length">接收字节数</param>
        void OnReceived(Response response, byte[] buffer, int length);

        /// <summary>
        /// 会话通讯管道连接
        /// </summary>
        /// <param name="pipe">会话通讯管道</param>
        void OnSessionConnected(SessionPipe pipe);

        /// <summary>
        /// 会话通讯管道关闭
        /// </summary>
        /// <param name="pipe">会话通讯管道</param>
        void OnSessionClosed(SessionPipe pipe);
    }
}
