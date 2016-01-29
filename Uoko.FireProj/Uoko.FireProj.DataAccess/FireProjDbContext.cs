using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.FluentAPI;

namespace Uoko.FireProj.Model
{
    public class FireProjDbContext : DbContext
    {
        #region DbSet 
        public DbSet<Project> Project { get; set; }
        public DbSet<Dictionary> Dictionary { get; set; }
        public DbSet<DomainResource> ResourceInfo { get; set; }
        public DbSet<TaskInfo> TaskInfo { get; set; }
        public DbSet<TaskLogs> TaskLogs { get; set; }
        public DbSet<Server> Servers { get; set; }
        #endregion


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ProjectMap());
            modelBuilder.Configurations.Add(new DictionaryMap());
            modelBuilder.Configurations.Add(new DomainResourceMap());
            modelBuilder.Configurations.Add(new TaskInfoMap());
            modelBuilder.Configurations.Add(new TaskLogsMap());
            modelBuilder.Configurations.Add(new ServerMap());
        }
    }
}
