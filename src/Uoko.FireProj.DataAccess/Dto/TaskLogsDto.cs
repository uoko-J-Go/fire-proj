using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Dto
{
    public class TaskLogsDto
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

        public DeployStatus DeployStatus { get; set; }

        public QAStatus QAStatus { get; set; }

        public int? BuildId { get; set; }

        public DeployInfoIocDto DeployInfoIocDto { get; set; }

        public DeployInfoPreDto DeployInfoPreDto { get; set; }

        public DeployInfoOnlineDto DeployInfoOnlineDto { get; set; }

        public string CreatorName { get; set; }


        public DateTime CreateDate { get; set; }
    }
}
