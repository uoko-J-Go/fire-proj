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
        public DbSet<Project> Companies { get; set; }
       
        #endregion


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ProjectMap());
         
        }
    }
}
