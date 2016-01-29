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
        /// 项目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 任务Id
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// 域名在哪台服务器下
        /// </summary>
        public int ServerId { get; set; }


        public DomainResourceStatusEnum Status { get; set; }


    }
}
