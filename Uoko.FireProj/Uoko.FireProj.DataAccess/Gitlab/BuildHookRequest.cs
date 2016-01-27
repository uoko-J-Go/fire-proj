using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Gitlab
{
    public class BuildHookRequest
    {
        public string object_kind { get; set; }
        public string Ref { get; set; }
        public bool tag { get; set; }
        public string before_sha { get; set; }
        public string sha { get; set; }
        public string build_id { get; set; }
        public string build_name { get; set; }
        public string build_stage { get; set; }
        public string build_status { get; set; }
        public string build_started_at { get; set; }
        public string build_finished_at { get; set; }
        public string build_duration { get; set; }
        public bool build_allow_failure { get; set; }
        public int trigger_request_id { get; set; }
        public int project_id { get; set; }
    }
}
