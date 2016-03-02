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
        /// 历史当前部署信息
        /// </summary>
        public string DeployInfo { get; set; }

        /// <summary>
        /// 部署信息类型
        /// </summary>
        public StageEnum Stage { get; set; }

        /// <summary>
        /// 备注评论
        /// </summary>
        public string Comments { get; set; }

        public LogType LogType { get; set; }
    }
}
