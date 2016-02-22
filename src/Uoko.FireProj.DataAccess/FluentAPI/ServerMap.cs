using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Entity;

namespace Uoko.FireProj.DataAccess.FluentAPI
{
    class ServerMap : EntityTypeConfiguration<Server>
    {
        public ServerMap()
        {
            Property(r => r.IP).IsRequired();
            Property(r => r.Status).IsRequired();
            ToTable("Server");//指定生成表名
        }
    }
}
