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

        public int? OnlineTaskId { get; set; }

    }

    public class OnlineTaskInfo:BaseEntity
    {
        /// <summary>
        /// 上线版本指定，作为一次上线任务的标志
        /// </summary>
        public string OnlineVersion { get; set; }


        /// <summary>
        /// 项目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public int ProjectName { get; set; }

        /// <summary>
        /// 部署服务器IP地址
        /// </summary>
        public string DeployIP { get; set; }

        /// <summary>
        /// 部署机器名称
        /// </summary>
        public string DeployServerName { get; set; }

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

    /// <summary>
    /// 部署信息
    /// </summary>
    public class DeployInfo
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
        /// 任务描述
        /// </summary>
        public string TaskDesc { get; set; }

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

    public class DeployInfoIoc : DeployInfo
    {

    }

    public class DeployInfoPre : DeployInfo
    {

    }

    public class DeployInfoOnline
    {
        /// <summary>
        /// 部署环境
        /// </summary>
        public StageEnum DeployStage { get; set; }

        public int? OnlineTaskId { get; set; }

        /// <summary>
        /// 验收人Id集合
        /// </summary>
        public string CheckUserId { get; set; }

        /// <summary>
        /// 任务相关通知人
        /// </summary>

        public string NoticeUserId { get; set; }
    }

}
