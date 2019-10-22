using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Communication
{
    /// <summary>
    /// 通讯管道工厂
    /// </summary>
    public abstract class PipeFactory : IDisposable
    {
        #region 管道管理

        /// <summary>
        /// 命名会话列表
        /// </summary>
        protected Hashtable namedPipes = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 会话列表
        /// </summary>
        protected List<Pipe> pipes = new List<Pipe>();

        /// <summary>
        /// 获取管道
        /// </summary>
        /// <param name="name">管道名称</param>
        /// <returns>管道</returns>        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual Pipe Get(string name = null)
        {
            var pipe = GetNamedPipe(name);
            if (pipe == null) {
                pipe = CreatePipe(this, name);
                if (string.IsNullOrEmpty(name)) {
                    pipes.Add(pipe);
                }
                else {
                    namedPipes.Add(name, pipe);
                }
            }

            return pipe;
        }

        /// <summary>
        /// 创建管道
        /// </summary>
        /// <param name="factory">通讯管道工厂</param>
        /// <param name="arguments">参数</param>
        /// <returns>管道</returns>
        protected abstract Pipe CreatePipe(PipeFactory factory, params object[] arguments);

        /// <summary>
        /// 获取命名管道
        /// </summary>
        /// <param name="name">管道名称</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private Pipe GetNamedPipe(string name)
        {
            return (!string.IsNullOrEmpty(name) && namedPipes.ContainsKey(name)) ? namedPipes[name] as Pipe : null;
        }

        #endregion

        #region 数据收发

        public abstract void Send(string dstId, byte[] data, int offset, int length, object state);

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="length">数据长度</param>
        public virtual void OnReceive(byte[] data, int length)
        {
            var protocol = Protocol.Parse(data, length);
            var name = protocol.SrcId;
            if (!string.IsNullOrEmpty(name) && namedPipes.ContainsKey(name)) {
                return namedPipes[name] as Pipe;
            }

            pipe.OnReceive(protocol);
        }

        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
