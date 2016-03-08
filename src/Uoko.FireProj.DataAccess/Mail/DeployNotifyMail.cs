using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Mail
{
    public class DeployNotifyMail
    {
        public string ProjectName { get; set; }

        public string DeployStatus { get; set; }

        public string DeployUrl { get; set; }

        public string StageName { get; set; }

        public string GitLabBuildPage { get; set; }
    }
}
