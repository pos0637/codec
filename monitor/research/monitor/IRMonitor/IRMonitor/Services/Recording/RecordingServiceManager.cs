using Common;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace IRMonitor.Services.Recording
{
    /// <summary>
    /// 录像服务管理器
    /// </summary>
    public class _RecordingServiceManager : ServiceManager
    {
        /// <summary>
        /// 添加服务
        /// </summary>
        /// <param name="arguments">参数列表</param>
        /// <returns>服务索引</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual string AddService(Dictionary<string, object> arguments)
        {
            var service = new RecordingService();
            service.Initialize(arguments);

            string serviceId = GenerateServiceId();
            services.Add(serviceId, service);

            return serviceId;
        }
    }

    /// <summary>
    /// 录像服务管理器
    /// </summary>
    public class RecordingServiceManager : Singleton<_RecordingServiceManager> { }
}
