using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Dto;

namespace Uoko.FireProj.Abstracts
{
    public interface IResourceInfoSvc
    {
        /// <summary>
        /// 批量新增任务测试地址
        /// </summary>
        /// <param name="dto"></param>
        void CreatResource(List<ResourceInfoDto> dto);
    }
}
