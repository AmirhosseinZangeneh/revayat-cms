using System;
using System.Collections.Generic;
using System.Data.Entity;
using DataLayer.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class RevayatContext:DbContext
    {
        static RevayatContext()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<RevayatContext, Configuration>());
        }

        public DbSet<PageGroup> PageGroups { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<PageComment> PageComments { get; set; }
        public DbSet<AdminLogin> AdminLogins { get; set; }


    }
}
