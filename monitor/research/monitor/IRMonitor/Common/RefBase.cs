using System.Runtime.CompilerServices;

namespace Common
{
    /// <summary>
    /// 引用计数
    /// </summary>
    public class RefBase
    {
        /// <summary>
        /// 释放资源处理函数委托类型
        /// </summary>
        public delegate void DgOnRelease();

        /// <summary>
        /// 引用计数
        /// </summary>
        private long refs = 0;

        /// <summary>
        /// 释放资源处理函数
        /// </summary>
        private DgOnRelease onRelease;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="onRelease">释放资源处理函数</param>
        public RefBase(DgOnRelease onRelease)
        {
            this.onRelease = onRelease;
        }

        /// <summary>
        /// 添加引用
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public long AddRef()
        {
            return ++refs;
        }

        /// <summary>
        /// 释放引用
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public long Release()
        {
            if (refs > 0) {
                refs--;
            }

            if (refs == 0) {
                onRelease?.Invoke();
            }

            return refs;
        }
    }
}
