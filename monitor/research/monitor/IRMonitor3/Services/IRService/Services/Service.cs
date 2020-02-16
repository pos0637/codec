using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace IRService.Services
{
    /// <summary>
    /// 服务
    /// </summary>
    public abstract class Service : IDisposable
    {
        /// <summary>
        /// 状态
        /// </summary>
        enum Status
        {
            /// <summary>
            /// 未初始化
            /// </summary>
            Idle,

            /// <summary>
            /// 初始化
            /// </summary>
            Initialized,

            /// <summary>
            /// 运行中
            /// </summary>
            Running,

            /// <summary>
            /// 故障
            /// </summary>
            Fault
        }

        /// <summary>
        /// 状态
        /// </summary>
        private Status status = Status.Idle;

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose() { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="arguments">参数列表</param>
        /// <returns>是否成功</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual bool Initialize(Dictionary<string, object> arguments)
        {
            status = Status.Initialized;
            return true;
        }

        /// <summary>
        /// 启动
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Start()
        {
            if ((status == Status.Initialized) || (status == Status.Fault)) {
                OnStart();
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Stop()
        {
            status = Status.Initialized;
        }

        /// <summary>
        /// 启动处理函数
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual void OnStart()
        {
            status = Status.Running;
        }

        /// <summary>
        /// 故障处理函数
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected virtual void OnFault()
        {
            status = Status.Fault;
        }
    }
}
