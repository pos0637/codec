using Common;
using IRService.Common;
using IRService.Miscs;
using Miscs;
using Repository.Entities;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using static Common.BaseWorker;

namespace IRService.Services.Web
{
    /// <summary>
    /// 平台服务
    /// </summary>
    public class WebService : Service, IExecutor
    {
        /// <summary>
        /// 配置信息
        /// </summary>
        private Configuration configuration;

        /// <summary>
        /// 工作线程
        /// </summary>
        private BaseWorker worker;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Initialize(Dictionary<string, object> arguments)
        {
            configuration = arguments["configuration"] as Configuration;

            return base.Initialize(arguments);
        }

        public override void Stop()
        {
            if (worker != null) {
                worker.Discard();
                worker.Join();
            }

            base.Stop();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnStart()
        {
            worker = new BaseWorker(this);
            worker.Start();

            base.OnStart();
        }

        public void Run(BaseWorker worker)
        {
            WebMethod.Device device = null;
            while (!worker.IsTerminated()) {
                var info = WebMethod.GetDevice(configuration.information.clientId);
                if (info == null) {
                    Tracker.LogE(" WebMethod: GetDevice fail");
                    Thread.Sleep(3000);
                    continue;
                }

                // 获取并检查设备信息
                if (info.Equals(device)) {
                    Thread.Sleep(1000 * 60);
                    continue;
                }

                device = info;
                EventEmitter.Instance.Publish(Constants.EVENT_SERVICE_START_STREAMING, new Dictionary<string, string>() {
                    { "0001", device.pushUrl },
                    { "0002", device.pushUrl }
                });

                Thread.Sleep(1000 * 60);
            }
        }
    }
}
