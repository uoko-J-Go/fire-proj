﻿using System;
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
        /// 当Stage=PRODUCTION的时候 对应OnlineTaskInfo中的Id
        /// 当Stage=IOC,PRE的时候，对应TaskInfo中的Id
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// 历史当前部署信息
        /// 当Stage=PRODUCTION的时候对应OnlineTaskInfo的Json
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
        /// <summary>
        /// 部署日志对应的 TriggeredId
        /// </summary>
        public int? TriggeredId { get; set; }

        public LogType LogType { get; set; }
    }
}
