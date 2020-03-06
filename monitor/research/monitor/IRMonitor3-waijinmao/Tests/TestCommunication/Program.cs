using Communication;
using Miscs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TestCommunication
{
    class Program
    {
        private const string dstId = "0001";
        private static SessionManager manager;
        private static Session session;

        static void Main(string[] args)
        {
            bool isLogin = false;

            manager = new MQTTSessionManager("112.51.3.158", 1883, "0002");
            manager.OnReceiveEvent += (s, data) => {
                Console.WriteLine(Encoding.UTF8.GetString(data));
                session = s;
                isLogin = true;
            };

            login();

            while (true) {
                if (isLogin) {
                    logout();
                    break;
                }

                Thread.Sleep(3000);
            }

            Console.ReadLine();
        }

        private static void login()
        {
            try {
                var request = new JsonRpcRequest() {
                    version = "2.0",
                    method = "login",
                    parameters = new Dictionary<string, object>() { { "username", "admin" }, { "password", "123456" } },
                    id = "0"
                };

                manager.Get().Send(dstId, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request)), 0, -1);
            }
            catch {
            }
        }

        private static void logout()
        {
            try {
                var request = new JsonRpcRequest() {
                    version = "2.0",
                    method = "logout",
                    parameters = null,
                    id = "0"
                };

                session.Send(dstId, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request)), 0, -1);
            }
            catch {
            }
        }
    }
}
