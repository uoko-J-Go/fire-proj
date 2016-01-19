using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Entity
{
    /// <summary>
    /// 服务器资源实体
    /// </summary>
    public class ResourceInfo : BaseEntity
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string Url { get; set; }

        public ResourceEnum Status { get; set; }
    }
}
