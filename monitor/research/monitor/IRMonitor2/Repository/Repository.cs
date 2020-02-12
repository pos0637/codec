using Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Repository.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace Repository
{
    /// <summary>
    /// 数据仓库
    /// </summary>
    public static class Repository
    {
        private static readonly string AppConfigurationPath = AppDomain.CurrentDomain.BaseDirectory + @"\app.yml";
        private static readonly string SelectionsConfigurationPath = AppDomain.CurrentDomain.BaseDirectory + @"\selections.json";

        /// <summary>
        /// 数据仓库
        /// </summary>
        public class RepositoyContext : DbContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder options)
                => options.UseSqlite("Data Source=ir.db");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<Alarm>().ToTable("t_alarm");
            }

            /// <summary>
            /// 添加告警
            /// </summary>
            /// <param name="alarm">告警</param>
            public void AddAlarm(Alarm alarm)
            {
                Add(alarm);
                SaveChanges();
            }
        }

        public class BloggingContext : DbContext
        {
            public DbSet<Blog> Blogs { get; set; }
            public DbSet<Post> Posts { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder options)
                => options.UseSqlite("Data Source=blogging.db");

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<Blog>().ToTable("t_test_blog");
                modelBuilder.Entity<Post>().ToTable("t_test_post");
            }

            public void Query()
            {
                var parameters = new SqliteParameter[] {
                    new SqliteParameter("Title", "xxx"),
                    new SqliteParameter("Content", "123456")
                };

                Database.ExecuteSqlRaw("UPDATE t_test_post SET Title=@Title, Content=@Content WHERE PostId>0", parameters);
            }
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string Url { get; set; }

            public List<Post> Posts { get; } = new List<Post>();
        }

        public class Post
        {
            public int PostId { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }

            public int BlogId { get; set; }
            public Blog Blog { get; set; }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <returns>配置</returns>
        public static Configuration LoadConfiguation()
        {
            try {
                using var sr = new StreamReader(AppConfigurationPath, Encoding.UTF8);
                return new Deserializer().Deserialize<Configuration>(sr);
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="configuration">配置</param>
        public static void SaveConfiguation(Configuration configuration)
        {
            try {
                using var sw = new StreamWriter(AppConfigurationPath, false, Encoding.UTF8);
                var yaml = new Serializer().Serialize(configuration);
                sw.Write(yaml);
                sw.Flush();
            }
            catch (Exception e) {
                Tracker.LogE(e);
                throw e;
            }
        }

        /// <summary>
        /// 读取选区配置
        /// </summary>
        /// <returns>配置</returns>
        public static Selections LoadSelections()
        {
            try {
                using var sr = new StreamReader(SelectionsConfigurationPath, Encoding.UTF8);
                return JsonUtils.ObjectFromJson<Selections>(sr.ReadToEnd());
            }
            catch (Exception e) {
                Tracker.LogE(e);
                return null;
            }
        }

        /// <summary>
        /// 保存选区配置
        /// </summary>
        /// <param name="selections">选区配置</param>
        public static void SaveSelections(Selections selections)
        {
            try {
                using var sw = new StreamWriter(SelectionsConfigurationPath, false, Encoding.UTF8);
                var data = JsonUtils.ObjectToJson(selections);
                sw.Write(data);
                sw.Flush();
                sw.Close();
            }
            catch (Exception e) {
                Tracker.LogE(e);
                throw e;
            }
        }
    }
}
