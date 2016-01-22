using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Dto
{
    public class ProjectDto
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
        /// gitlab项目的id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 项目文件(csproj)相对路径
        /// </summary>
        public string ProjectFileName { get; set; }

        /// <summary>
        /// 项目描述
        /// </summary>
        public string ProjectDesc { get; set; }
    }
}
