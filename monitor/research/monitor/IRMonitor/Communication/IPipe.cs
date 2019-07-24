using System;

namespace Communication
{
    /// <summary>
    /// 通讯管道
    /// </summary>
    public abstract class IDevice : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public abstract void Connect();

        public abstract void Disconnect();

        public abstract void StartReceive();

        public abstract void Send();
    }
}
