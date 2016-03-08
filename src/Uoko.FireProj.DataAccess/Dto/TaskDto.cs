using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Dto
{
    public class TaskDto: BaseDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public ProjectDto Project { get; set; }



        /// <summary>
        /// 代码分支
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// 部署环境
        /// </summary>
        public StageEnum DeployStage { get; set; }

        /// <summary>
        /// 部署环境枚举对应的名称
        /// </summary>
        public string DeployEnvironmentName { get; set; }

        /// <summary>
        /// 部署服务器IP地址
        /// </summary>
        public string DeployIP { get; set; }

        /// <summary>
        /// 部署服务器Id
        /// </summary>
        public int ServerId { get; set; }

        /// <summary>
        /// 部署域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// IIS站点名称
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// 验收人集合
        /// </summary>
        public List<UserDto> CheckUsers { get; set; }

        /// <summary>
        /// 任务相关通知人
        /// </summary>
        public List<UserDto> NoticeUsers { get; set; }

        /// <summary>
        /// 部署地址
        /// </summary>
        public string DeployAddress { get; set; }

        /// <summary>
        /// 部署文件包服务器存储地址
        /// </summary>
        public string PackageDir { get; set; }

        /// <summary>
        /// 任务状态值
        /// </summary>
        public QAStatus QAStatus { get; set; }

        /// <summary>
        /// 任务状态值
        /// </summary>
        public DeployStatus DeployStatus { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDesc { get; set; }

        /// <summary>
        /// 操作事件备注信息
        /// </summary>
        public string LogsText { get; set; }

        public int TriggeredId { get; set; }

        public int BuildId { get; set; }
    }

    public class TaskInfoForList
    {
        public string ProjectName { get; set; }

        public TaskInfo TaskInfo { get; set; }

        public OnlineTaskInfo OnlineTaskInfo { get; set; }
    }

    public class OnlineTaskDetailDto
    {
        public OnlineTaskInfo OnlineTask { get; set; }
        public IEnumerable<TaskInfoForList> TaskBelongOnline { get; set; }
    }
}
