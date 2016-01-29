using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Dto
{
   public class ResourceInfoDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 域名
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// 任务Id
        /// </summary>
        public int TaskId { get; set; }

       
    }
}
