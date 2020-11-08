using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ShareIT.Models
{
    public class AppContext : DbContext
    {
        public AppContext() : base("DBConnectionString")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppContext,
            ShareIT.Migrations.Configuration>("DBConnectionString"));

        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}