using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Dto
{
    public class DomainResourceDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// IIS站点名称
        /// </summary>
        public string SiteName { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 任务Id
        /// </summary>
        public int? TaskId { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 域名在哪台服务器Id
        /// </summary>
        public int ServerId { get; set; }

        /// <summary>
        /// 域名在哪台服务器名称
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// 环境类型
        /// </summary>
        public StageEnum? StageType { get; set; }

        /// <summary>
        /// 任务创建人Id
        /// </summary>
        public int? TaskCreatorId { get; set; }
    }
}
