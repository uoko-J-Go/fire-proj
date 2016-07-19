using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Dto
{
    public class ProjectDto: BaseDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 站点对应的gitlab地址
        /// </summary>
        public string ProjectRepo { get; set; }

        /// <summary>
        /// 站点对应的gitlab的名称
        /// </summary>
        public string ProjectGitlabName { get; set; }

        /// <summary>
        /// gitlab项目的id
        /// </summary>
        public int RepoId { get; set; }

        /// <summary>
        /// 项目文件(csproj)相对路径
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
        /// 域名生成规则用
        /// </summary>
        public string DomainRule { get; set; }

        /// <summary>
        /// 项目线上最新版本
        /// </summary>
        public string OnlineVersion { get; set; }

    }
}
