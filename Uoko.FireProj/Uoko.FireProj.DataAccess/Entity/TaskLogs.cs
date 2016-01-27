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
        /// gitlab triggered Id
        /// </summary>
        public int TriggeredId { get; set; }

        /// <summary>
        /// 记录所处的环境
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// 任务记录描述
        /// 流程变更格式: XXX在XXX时间,把XXX任务流程状态从XXX变更为XXX.
        /// 部署记录格式: XXX在XXX时间,把XXX任务从XXX分支部署到XXX环境,部署服务器IP:XXX,站点名称:XXX
        /// 部署中记录格式: XXX任务在执行XXX Stages时出错,详情gitlab  builds
        /// </summary>
        public string LogsDesc { get; set; }

        /// <summary>
        /// 输出文件
        /// </summary>
        public string LogsText { get; set; }

        /// <summary>
        /// 记录类型
        /// </summary>
        public TaskLogsEnum TaskLogsType { get; set; }
    }
}
