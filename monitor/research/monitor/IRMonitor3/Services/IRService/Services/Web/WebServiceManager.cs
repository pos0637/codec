using Common;
using Repository.Entities;
using System.Collections.Generic;

namespace IRService.Services.Web
{
    /// <summary>
    /// 平台服务管理器
    /// </summary>
    public class _WebServiceManager : ServiceManager
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns>是否成功</returns>
        public bool Initialize()
        {
            // 读取所有设备单元信息
            Configuration configuration = Repository.Repository.LoadConfiguation();
            if (configuration == null) {
                Tracker.LogI("LoadConfiguration fail");
                return false;
            }

            // 创建服务
            var service = new WebService();

            // 初始化平台服务
            if (!service.Initialize(new Dictionary<string, object>() { ["configuration"] = configuration })) {
                Tracker.LogE($"WebService initialize fail");
                return false;
            }

            // 开启平台服务
            service.Start();

            AddService("WebService", service);
            Tracker.LogI($"WebService start succeed");

            return true;
        }
    }

    /// <summary>
    /// 平台服务管理器
    /// </summary>
    public class WebServiceManager : Singleton<_WebServiceManager> { }
}
