using System.Threading;

namespace Miscs
{
    /// <summary>
    /// 线程局部存储工具类
    /// </summary>
    public static class Tls
    {
        /// <summary>
        /// 注册变量
        /// </summary>
        /// <param name="name">变量名称</param>
        public static void Register(string name)
        {
            Thread.AllocateNamedDataSlot(name);
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="name">变量名称</param>
        /// <returns>数据</returns>
        public static dynamic Get(string name)
        {
            return Thread.GetData(Thread.GetNamedDataSlot(name));
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="name">变量名称</param>
        /// <param name="value">数据</param>
        public static void Set(string name, dynamic value)
        {
            Thread.SetData(Thread.GetNamedDataSlot(name), value);
        }
    }
}
