using System;
using System.Collections.Generic;

namespace IRMonitor.Services
{
    /// <summary>
    /// 服务
    /// </summary>
    public abstract class Service : IDisposable
    {
        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="arguments">参数列表</param>
        public abstract void Initialize(Dictionary<string, object> arguments);

        /// <summary>
        /// 启动
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 停止
        /// </summary>
        public abstract void Stop();
    }
}
