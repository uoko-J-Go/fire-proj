using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Query
{
    public class ServerQuery: BaseQuery
    {
        public EnvironmentEnum? EnvironmentType { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
    }
}
