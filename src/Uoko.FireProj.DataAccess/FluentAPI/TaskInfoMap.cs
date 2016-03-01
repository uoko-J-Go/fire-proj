using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Entity;

namespace Uoko.FireProj.DataAccess.FluentAPI
{
   public class TaskInfoMap : EntityTypeConfiguration<TaskInfo>
    {
        public TaskInfoMap()
        {
            Property(r => r.TaskName).HasMaxLength(50).IsRequired();//长度50,必填
            Property(r => r.ProjectId).IsRequired();
            Property(r => r.Branch).IsRequired();
            ToTable("TaskInfo");//指定生成表名
        }
    }
}
