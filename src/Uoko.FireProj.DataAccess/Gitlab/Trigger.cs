using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Gitlab
{
    public class Trigger
    {
        public string token { get; set; }

        public string created_at { get; set; }

        public string updated_at { get; set; }

        public string deleted_at { get; set; }

        public string last_used { get; set; }
    }
}
