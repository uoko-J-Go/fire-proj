using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Dto
{
   public class DictionaryDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 字典名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字典值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public DicEnum Status { get; set; }
    }
}
