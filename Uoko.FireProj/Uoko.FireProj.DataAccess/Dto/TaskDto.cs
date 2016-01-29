using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Dto
{
    public class TaskDto
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
        public EnvironmentEnum DeployEnvironment { get; set; }

        /// <summary>
        /// 部署IP地址
        /// </summary>
        public string DeployIP { get; set; }

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
        public List<UserDto> CheckUsers{ get; set; }

        /// <summary>
        /// 任务相关通知人
        /// </summary>
        public List<UserDto> NoticeUses{ get; set; }

        /// <summary>
        /// 部署地址
        /// </summary>
        public string DeployAddress { get; set; }

        /// <summary>
        /// 任务状态值
        /// </summary>
        public TaskEnum Status { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDesc { get; set; }

        public int TriggeredId { get; set; }
    }
}
