using Common;
using Communication;
using IRMonitor2.Services.Cell;
using Miscs;
using System;
using System.Threading;

namespace IRMonitor2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            /*
            using (var db = new BloggingContext()) {
                db.Database.EnsureCreated();
                db.Query();

                // Create
                Console.WriteLine("Inserting a new blog");
                db.Add(new Blog { Url = "http://blogs.msdn.com/adonet" });
                db.SaveChanges();

                // Read
                Console.WriteLine("Querying for a blog");
                var blog = db.Blogs
                    .OrderBy(b => b.BlogId)
                    .First();

                // Update
                Console.WriteLine("Updating the blog and adding a post");
                blog.Url = "https://devblogs.microsoft.com/dotnet";
                blog.Posts.Add(
                    new Post {
                        Title = "Hello World",
                        Content = "I wrote an app using EF Core!"
                    });
                db.SaveChanges();

                // Delete
                Console.WriteLine("Delete the blog");
                db.Remove(blog);
                db.SaveChanges();
            }
            */

            // 初始化通讯会话管理器
            InitializeSessionManager();

            // 初始化设备单元服务管理器
            CellServiceManager.Instance.Initialize();
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
