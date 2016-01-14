using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Query
{
    /// <summary>
    /// 查询基类
    /// </summary>
    public abstract class BaseQuery
    {
        /// <summary>
        /// 数据偏移量,前端已经计算好分页数了
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// 每页显示大小
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// 按某个字段进行排序
        /// </summary>
        public string Sort { get; set; }

        /// <summary>
        /// 升序或者降序排序(asc/desc)
        /// </summary>
        public string Order { get; set; }

        /// <summary>
        /// 模糊查询值
        /// </summary>
        public string Search { get; set; }
    }
}
