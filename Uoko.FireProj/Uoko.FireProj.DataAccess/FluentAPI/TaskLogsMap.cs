using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Entity;

namespace Uoko.FireProj.DataAccess.FluentAPI
{
   public class TaskLogsMap : EntityTypeConfiguration<TaskLogs>
    {
        public TaskLogsMap()
        {
            Property(r => r.TaskId).IsRequired();
            ToTable("TaskLogs");//指定生成表名
        }
    }
}
