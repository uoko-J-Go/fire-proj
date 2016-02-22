using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Entity
{
    public class Server : BaseEntity
    {
        /// <summary>
        /// 服务器名称
        /// </summary>
        public string Name{ get; set; }
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 环境类型
        /// </summary>
        public EnvironmentEnum EnvironmentType { get; set; }
        /// <summary>
        /// 服务器描述 操作系统 配置等等
        /// </summary>
        public string ServerDesc { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public GenericStatusEnum Status { get; set; }
    }
}
