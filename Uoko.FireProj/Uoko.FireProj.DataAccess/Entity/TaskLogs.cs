using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Entity
{
    /// <summary>
    /// 任务记录实体
    /// </summary>
    public class TaskLogs : BaseEntity
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// 代码分支
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// 部署环境
        /// </summary>
        public string DeployEnvironment { get; set; }

        /// <summary>
        /// 部署IP地址
        /// </summary>
        public string DeployIP { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public DeployEnum Status { get; set; }

        /// <summary>
        /// 输出文件
        /// </summary>
        public string LogsText { get; set; }
    }
}
