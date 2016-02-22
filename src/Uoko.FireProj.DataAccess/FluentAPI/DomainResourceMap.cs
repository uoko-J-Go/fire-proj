using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Entity;

namespace Uoko.FireProj.DataAccess.FluentAPI
{
    class DomainResourceMap: EntityTypeConfiguration<DomainResource>
    {
        public DomainResourceMap()
        {
            Property(r => r.Name).IsRequired();
            Property(r => r.Status).IsRequired();
            ToTable("DomainResource");//指定生成表名
        }
    }
}
