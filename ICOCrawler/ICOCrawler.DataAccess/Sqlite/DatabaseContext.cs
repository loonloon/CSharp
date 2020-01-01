using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using ICOCrawler.DataAccess.Sqlite.Model;

namespace ICOCrawler.DataAccess.Sqlite
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() :
            base(new SQLiteConnection
            {
                ConnectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = $"{AppDomain.CurrentDomain.BaseDirectory}ICOCrawlerDB.db",
                    ForeignKeys = true
                }.ConnectionString
            }, true)
        {
            Database.SetInitializer<DatabaseContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<InitialCoinOffering> InitialCoinOfferingDbSet { get; set; }
    }
}
