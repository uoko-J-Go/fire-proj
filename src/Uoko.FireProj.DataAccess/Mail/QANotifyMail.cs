using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Mail
{
    public class QANotifyMail
    {
        public string TaskName { get; set; }

        public string TestUrl { get; set; }

        public string TestUser { get; set; }

        public string StageName { get; set; }

        public string TestResult { get; set; }

        public string Coments { get; set; }

        public bool IsAllPassed { get; set; }
    }
}
