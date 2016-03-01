using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Entity
{
    /// <summary>
    /// 任务实体
    /// </summary>
    public class TaskInfo : BaseEntity
    {

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 代码分支
        /// </summary>
        public string Branch { get; set; }


        public string DeployInfoIocJson { get; set; }

        public string DeployInfoPreJson { get; set; }

        public string DeployInfoOnlineJson { get; set; }

        public bool HasOnlineDeployed { get; set; }

        public int? OnlineTaskId { get; set; }

    }

    /// <summary>
    /// 部署信息
    /// </summary>
    public class DeployInfoIoc
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
        public string NoticeUseId { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDesc { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public TaskStatusEnum Status { get; set; }

        /// <summary>
        /// gitlab triggered Id
        /// </summary>
        public int TriggeredId { get; set; }

        /// <summary>
        /// 用于记录失败时 GitLab的BuildId
        /// </summary>
        public int BuildId { get; set; }

    }

    public class DeployInfoPre
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
        public string NoticeUseId { get; set; }

        /// <summary>
        /// 任务描述
        /// </summary>
        public string TaskDesc { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public TaskStatusEnum Status { get; set; }

        /// <summary>
        /// gitlab triggered Id
        /// </summary>
        public int TriggeredId { get; set; }

        /// <summary>
        /// 用于记录失败时 GitLab的BuildId
        /// </summary>
        public int BuildId { get; set; }

    }

    public class DeployInfoOnline
    {

        /// <summary>
        /// 验收人Id集合
        /// </summary>
        public string CheckUserId { get; set; }

        /// <summary>
        /// 任务相关通知人
        /// </summary>
        public string NoticeUseId { get; set; }

   
    }

}
