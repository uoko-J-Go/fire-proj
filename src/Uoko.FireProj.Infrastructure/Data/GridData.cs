using System.Collections.Generic;

namespace Uoko.FireProj.Infrastructure.Data
{
    /// <summary>
    /// 列表数据，封装列表的行数据与总记录数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageGridData<T>
    {
        /// <summary>
        /// 初始化一个<see cref="PageGridData{T}"/>类型的新实例
        /// </summary>
        public PageGridData()
            : this(new List<T>(), 0)
        { }

        /// <summary>
        /// 初始化一个<see cref="PageGridData{T}"/>类型的新实例
        /// </summary>
        public PageGridData(IEnumerable<T> rows, int total)
        {
            this.total = total;
            this.rows = rows;
        }
        /// <summary>
        /// 获取或设置 总记录数
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 获取或设置  分页数据
        /// </summary>
        public IEnumerable<T> rows { get; set; }
    }
}
