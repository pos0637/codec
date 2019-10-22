using Common;
using Communication;
using IRMonitor2.Common;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using static Repository.Class1;

namespace IRMonitor2
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new MQTTSessionManager(Global.gCloudIP, Global.gCloudPort, "1234");

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

            while (true) {
                Thread.Sleep(3000);
                try {
                    manager.Get().Send("9999", Encoding.UTF8.GetBytes("Hello"), 0, -1);
                }
                catch (Exception e) {
                    Tracker.LogE(e);
                }
            }
        }
    }
}
