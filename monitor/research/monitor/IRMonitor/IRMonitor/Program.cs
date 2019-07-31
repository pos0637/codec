using Common;
using IRMonitor.ServiceManager;
using IRMonitor.Services.LiveStreaming;
using System;
using System.Collections.Generic;

namespace IRMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (ARESULT.AFAILED(CellServiceManager.Instance.LoadConfig())) {
                Console.WriteLine("IRServiceManager初始化失败");
                Console.ReadLine();
                return;
            }

            string serviceId = LiveStreamingServiceManager.Instance.AddService(new Dictionary<string, object>() {
                { "Cell", CellServiceManager.gIRServiceList[0] }
            });
            LiveStreamingServiceManager.Instance.GetService(serviceId).Start();

            /*
            ServiceInstanceManager manager = new ServiceInstanceManager();
            if (ARESULT.AFAILED(manager.Start())) {
                Console.WriteLine("ServiceInstanceManager初始化失败");
                Console.ReadLine();
                return;
            }

            SmsServiceWorker.Instance.Start();
            SmsServiceWorker.Instance.Join();
            */
        }
    }
}
