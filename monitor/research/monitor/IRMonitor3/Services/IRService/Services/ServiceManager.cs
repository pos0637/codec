using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace IRService.Services
{
    /// <summary>
    /// 服务管理器
    /// </summary>
    public abstract class ServiceManager : BaseWorker, IDisposable
    {
        /// <summary>
        /// 检查服务时延
        /// </summary>
        private const int CHECK_SERVICE_DURATION = 3000;

        /// <summary>
        /// 服务列表
        /// </summary>
        protected Hashtable services = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 服务计数器
        /// </summary>
        private int serviceCounter = 0;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ServiceManager()
        {
            // 自动启动服务管理器
            Start();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            Discard();
            Join();

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
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Service GetService(string serviceId)
        {
            return services[serviceId] as Service;
        }

        /// <summary>
        /// 获取服务列表
        /// </summary>
        /// <returns>服务列表</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<Service> GetServices()
        {
            return services.Values.OfType<Service>().ToList();
        }

        /// <summary>
        /// 删除服务列表
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

        protected override void Run()
        {
            while (!IsTerminated()) {
                // 自动启动异常服务
                foreach (Service service in services.Values) {
                    service.Start();
                }

                Thread.Sleep(CHECK_SERVICE_DURATION);
            }
        }
    }
}
