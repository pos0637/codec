using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Repository
{
    public class Class1
    {
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

                Database.ExecuteSqlCommand("UPDATE t_test_post SET Title=@Title, Content=@Content WHERE PostId>0", parameters);
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
    }
}
