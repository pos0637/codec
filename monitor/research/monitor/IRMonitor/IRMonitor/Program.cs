using Common;
using Communication.Session;
using IRMonitor.Common;
using IRMonitor.Controllers;
using IRMonitor.Services.Cell;
using System;
using System.Collections.Generic;
using System.Threading;

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

            var sessionManager = MQTTSessionManager.Instance;
            sessionManager.OnNewSessionCallback = (pipe) => {
                Tracker.LogNW($"OnNewSession: {pipe.SessionId}");
                pipe.Context = new SessionContext(pipe);
            };

            sessionManager.Initialize(new Dictionary<string, object>() {
                { "Server", Global.gCloudIP },
                { "Port", Global.gCloudPort },
                { "Topic", "Topic1" },
                { "ClientId", "1234" }
            });

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

            while (true) {
                Thread.Sleep(3000);
            }
        }
    }
}
