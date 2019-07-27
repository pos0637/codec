using Communication.Session;
using System;
using System.Collections.Generic;
using System.Threading;

namespace IRMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            MQTTSessionManager sessionManager = new MQTTSessionManager();
            sessionManager.OnNewSessionCallback = (pipe) => {
                Console.WriteLine($"OnNewSession: {pipe.SessionId}");
            };

            sessionManager.OnSessionClosedCallback = (pipe) => {
                Console.WriteLine($"OnSessionClosed: {pipe.SessionId}");
            };

            sessionManager.Initialize(new Dictionary<string, object>() {
                { "Server", "112.51.3.158" },
                { "Port", 1883 },
                { "Topic", "Topic1" },
                { "ClientId", "1234" }
            });

            while (true) {
                Thread.Sleep(1000);
            }

            Console.WriteLine("Hello World!");
        }
    }
}
