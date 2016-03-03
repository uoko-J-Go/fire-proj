using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Gitlab
{
    public class TriggerRequest
    {
        public string token { get; set; }

        public string @ref { get; set; }

        public object variables { get; set; }

    }
}
