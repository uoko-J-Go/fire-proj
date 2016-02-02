using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Entity;

namespace Uoko.FireProj.DataAccess.FluentAPI
{
    public class ProjectMap : EntityTypeConfiguration<Project>
    {
        public ProjectMap()
        {
            Property(r => r.ProjectName).HasMaxLength(50).IsRequired();//长度50,必填
            Property(r => r.ProjectRepo).IsRequired();
            Property(r => r.ProjectSlnName).IsRequired();
            ToTable("ProjectInfo");//指定生成表名
        }
    }
}
