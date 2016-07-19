using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        public StageEnum StageType { get; set; }

        /// <summary>
        /// 服务器描述 操作系统 配置等等
        /// </summary>
        public string ServerDesc { get; set; }

        /// <summary>
        /// 部署文件包服务器存储地址
        /// </summary>
        public string PackageDir { get; set; }

    }
}
