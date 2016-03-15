using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Entity
{
    public class RollbackTaskInfo : BaseEntity
    {
   
        /// <summary>
        /// 项目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 源版本
        /// </summary>
        public string FromVersion { get; set; }

        /// <summary>
        /// 目标版本
        /// </summary>
        public string ToVersion { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public DeployStatus DeployStatus { get; set; }

        /// <summary>
        /// gitlab triggered Id
        /// </summary>
        public int? TriggeredId { get; set; }

        /// <summary>
        /// 用于记录失败时 GitLab的BuildId
        /// </summary>
        public int? BuildId { get; set; }
    }
}
