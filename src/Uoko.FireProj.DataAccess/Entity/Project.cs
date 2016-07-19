using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Entity
{
    /// <summary>
    /// 项目实体
    /// </summary>
    public class Project : BaseEntity
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 站点对应的gitlab地址
        /// </summary>
        public string ProjectRepo { get; set; }

        /// <summary>
        /// gitlab项目的id
        /// </summary>
        public int RepoId { get; set; }

        /// <summary>
        /// 项目sln名称
        /// </summary>
        public string ProjectSlnName { get; set; }

        /// <summary>
        ///  项目Csproj相对路径
        /// </summary>
        public string ProjectCsprojName { get; set; }

        /// <summary>
        /// 项目描述
        /// </summary>
        public string ProjectDesc { get; set; }

        /// <summary>
        /// 域名生成规则
        /// </summary>
        public string DomainRule { get; set; }

        /// <summary>
        /// 项目线上最新版本
        /// </summary>
        public string OnlineVersion { get; set; }
    }
}
