using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Entity;

namespace Uoko.FireProj.DataAccess.FluentAPI
{
    class ResourceInfoMap: EntityTypeConfiguration<ResourceInfo>
    {
        public ResourceInfoMap()
        {
            Property(r => r.Url).IsRequired();
            Property(r => r.Status).IsRequired();
            ToTable("ResourceInfo");//指定生成表名
        }
    }
}
