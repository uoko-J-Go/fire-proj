using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Dto
{
    /// <summary>
    /// 任务详情Dto
    /// </summary>
    public class TaskDetailDto:BaseDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 项目信息
        /// </summary>
        public ProjectDto ProjectDto { get; set; }

        /// <summary>
        /// 代码分支
        /// </summary>
        public string Branch { get; set; }


        public string DeployInfoIocJson { get; set; }

        /// <summary>
        /// 验收人Id集合
        /// </summary>
        public string IocCheckUserId { get; set; }

        public string DeployInfoPreJson { get; set; }

        /// <summary>
        /// 验收人Id集合
        /// </summary>
        public string PreCheckUserId { get; set; }

        public string DeployInfoOnlineJson { get; set; }

        /// <summary>
        /// 验收人Id集合  12-0, 14-1, 20-2 代表 12 待测试， 14 测试未通过， 20 测试通过
        /// </summary>
        public string OnlineCheckUserId { get; set; }

        public bool HasOnlineDeployed { get; set; }

        public int? OnlineTaskId { get; set; }

        public  DeployInfoIocDto  DeployInfoIocDto  { get; set; }

        public  DeployInfoPreDto DeployInfoPreDto { get; set; }

        public DeployInfoOnlineDto DeployInfoOnlineDto { get; set; }
    }

    public class DeployInfoDto
    {
        /// <summary>
        /// 部署环境
        /// </summary>
        public StageEnum DeployStage { get; set; }

        /// <summary>
        /// 部署服务器IP地址
        /// </summary>
        public string DeployIP { get; set; }

        /// <summary>
        /// 部署地址
        /// </summary>
        public string DeployAddress { get; set; }

        /// <summary>
        /// 部署域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// IIS站点名称
        /// </summary>
        public string SiteName { get; set; }


        /// <summary>
        /// 验收人Id集合
        /// </summary>
        public string CheckUserId { get; set; }

        /// <summary>
        /// 任务相关通知人
        /// </summary>
        public string NoticeUserId { get; set; }

        /// <summary>
        /// 验收人名称-状态集合
        /// </summary>
        public List<UserDto> CheckUser { get; set; }

        /// <summary>
        /// 任务相关通知人-状态集合
        /// </summary>
        public List<UserDto> NoticeUser { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDesc { get; set; }

        /// <summary>
        /// 部署状态
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



    /// <summary>
    /// IOC部署dto
    /// </summary>
    public class DeployInfoIocDto: DeployInfoDto
    {
    }

    /// <summary>
    /// Pre部署dto
    /// </summary>
    public class DeployInfoPreDto: DeployInfoDto
    {
    }

    /// <summary>
    /// Online部署dto
    /// </summary>
    public class DeployInfoOnlineDto: DeployInfoDto
    {
        public int? OnlineTaskId { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string OnlineVersion { get; set; }
    }
}
