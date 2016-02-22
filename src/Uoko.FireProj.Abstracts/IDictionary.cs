using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.Abstracts
{
    /// <summary>
    /// 数据字典
    /// </summary>
    public interface IDictionarySvc
    {
        /// <summary>
        /// 新增字典
        /// </summary>
        /// <param name="dto"></param>
        void CreatDictionary(DictionaryDto dto);

        /// <summary>
        /// 字典编辑
        /// </summary>
        /// <param name="dto"></param>
        void EditDictionary(DictionaryDto dto);

        /// <summary>
        /// 根据字典id删除字典信息
        /// </summary>
        /// <param name="id"></param>
        void DeleteDictionary(int id);

        /// <summary>
        /// 获取字典分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        PageGridData<DictionaryDto> GetDictionaryPage(DictionaryQuery query);

        /// <summary>
        /// 根据字典Id获取字典信息详情
        /// </summary>
        /// <param name="id"></param>
        DictionaryDto GetDictionaryById(int id);

        /// <summary>
        /// 获取所有父节字典
        /// </summary>
        List<DictionaryDto> GetDictionaryParent();
    }
}
