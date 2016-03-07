using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Query
{
    public class TaskLogsQuery : BaseQuery
    {
        public int TaskId { get; set; }

        public StageEnum? Stage { get; set; }
    }
}
