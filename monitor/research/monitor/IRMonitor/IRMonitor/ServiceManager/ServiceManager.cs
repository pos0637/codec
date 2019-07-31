using IRMonitor.Services;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace IRMonitor.ServiceManager
{
    /// <summary>
    /// 服务管理器
    /// </summary>
    public abstract class ServiceManager : IDisposable
    {
        /// <summary>
        /// 服务列表
        /// </summary>
        protected Hashtable services = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 服务计数器
        /// </summary>
        private int serviceCounter = 0;

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            foreach (Service service in services.Values) {
                service.Dispose();
            }
        }

        /// <summary>
        /// 添加服务
        /// </summary>
        /// <param name="serviceId">服务索引</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void AddService(string serviceId, object service)
        {
            services.Add(serviceId, service);
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="serviceId">服务索引</param>
        /// <returns>服务</returns>
        public Service GetService(string serviceId)
        {
            return services[serviceId] as Service;
        }

        /// <summary>
        /// 删除服务
        /// </summary>
        /// <param name="serviceId">服务索引</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void RemoveService(string serviceId)
        {
            GetService(serviceId)?.Dispose();
            services.Remove(serviceId);
        }

        /// <summary>
        /// 生成服务索引
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected string GenerateServiceId()
        {
            while (true) {
                serviceCounter = (++serviceCounter) % 9999;
                var serviceId = $"{serviceCounter:D4}";
                if (GetService(serviceId) == null) {
                    return serviceId;
                }
            }
        }
    }
}
