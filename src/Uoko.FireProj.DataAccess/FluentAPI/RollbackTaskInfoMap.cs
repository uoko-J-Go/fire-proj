using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Entity;

namespace Uoko.FireProj.DataAccess.FluentAPI
{
    public class RollbackTaskInfoMap : EntityTypeConfiguration<RollbackTaskInfo>
    {
        public RollbackTaskInfoMap()
        {
            Property(r => r.FromVersion).HasMaxLength(50);
            Property(r => r.ToVersion).HasMaxLength(50);
            Property(r => r.ProjectId).IsRequired();
            ToTable("RollbackTaskInfo");//指定生成表名
        }
    }
}
