using Common;
using Communication;
using IRService.Services.Cell;
using IRService.Services.Web;
using Miscs;
using System;

namespace IRService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 初始化通讯会话管理器
            InitializeSessionManager();

            // 初始化设备单元服务管理器
            CellServiceManager.Instance.Initialize();

            // 初始化平台服务管理器
            WebServiceManager.Instance.Initialize();
        }

        /// <summary>
        /// 初始化通讯会话管理器
        /// </summary>
        private static void InitializeSessionManager()
        {
            // 创建通讯会话管理器
            var configuration = Repository.Repository.LoadConfiguation().information;
            var manager = new MQTTSessionManager(configuration.mqttServerIp, configuration.mqttServerPort, configuration.clientId);

            // 注册会话
            Tls.Register("Session");

            // 注册收到数据事件回调函数
            manager.OnReceiveEvent += (session, data) => {
                // 设置会话
                Tls.Set("Session", session);

                // 调用方法
                try {
                    var result = DynamicInvoker.JsonRpcInvoke(typeof(Controller), null, data);
                    if (result != null) {
                        session.Send(result, 0, -1);
                    }
                }
                catch (Exception e) {
                    Tracker.LogE(e);
                }
            };
        }
    }
}
