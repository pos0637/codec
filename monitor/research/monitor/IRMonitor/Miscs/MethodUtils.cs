using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IRMonitor.Miscs
{
    /// <summary>
    /// 方法调用工具
    /// </summary>
    public static class MethodUtils
    {
        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="instance">对象</param>
        /// <param name="method">方法名称</param>
        /// <param name="arguments">参数列表</param>
        /// <returns>返回值</returns>
        public static object Invoke(object instance, string method, string arguments)
        {
            return Invoke(instance, method, JsonConvert.DeserializeObject<Dictionary<string, object>>(arguments));
        }

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="instance">对象</param>
        /// <param name="method">方法名称</param>
        /// <param name="arguments">参数列表</param>
        /// <returns>返回值</returns>
        public static object Invoke(object instance, string method, Dictionary<string, object> arguments)
        {
            var type = instance.GetType();
            var methodInfo = type.GetMethods().First(info => info.Name == method);
            if (methodInfo == null) {
                throw new ArgumentException();
            }

            var parameters = methodInfo.GetParameters();
            var list = new List<object>();
            foreach (var parameter in parameters) {
                if (!arguments.ContainsKey(parameter.Name)) {
                    throw new ArgumentException();
                }

                list.Add(Convert.ChangeType(arguments[parameter.Name], parameter.ParameterType));
            }

            return methodInfo.Invoke(instance, list.ToArray());
        }

        /// <summary>
        /// 解锁器
        /// </summary>
        public sealed class Unlocker : IDisposable
        {
            private bool isLocked;
            private object locker;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="locker">锁</param>
            public Unlocker(object locker)
            {
                isLocked = Monitor.IsEntered(locker);
                if (isLocked) {
                    Monitor.Exit(locker);
                    this.locker = locker;
                }
            }

            private Unlocker() { }

            public void Dispose()
            {
                if (isLocked) {
                    Monitor.Enter(locker);
                }
            }
        }
    }
}
