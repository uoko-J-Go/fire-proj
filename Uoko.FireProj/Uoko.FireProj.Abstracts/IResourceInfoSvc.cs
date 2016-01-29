using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        /// <summary>
        /// 根据项目id,部署服务器ip的id获取未被使用的域名list
        /// </summary>
        /// <param name="iPId"></param>
        /// <returns></returns>
        List<ResourceInfoDto> GetResourceList(int projectId, string ip);

        /// <summary>
        /// 修改资源表
        /// </summary>
        /// <param name="iPId"></param>
        /// <returns></returns>
        void UpdateResource(ResourceInfoDto dto, Expression<Func<ResourceInfoDto, object>> propertyExpression);
    }
}
