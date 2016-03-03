﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.Abstracts
{
    public interface IDomainResourceSvc
    {
        /// <summary>
        /// 批量新增任务测试地址
        /// </summary>
        /// <param name="dto"></param>
        void CreatResource(List<DomainResourceDto> dto);

        /// <summary>
        /// 根据项目id,部署服务器ip以及当前任务的id获取未被使用的域名list
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="serverId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        List<DomainResourceDto> GetResourceList(int projectId, int serverId,int? taskId);


        /// <summary>
        /// 根据任务Id释放域名
        /// </summary>
        /// <param name="taskId"></param>
        void ReleaseDomain(int taskId);


        /// <summary>
        /// 获取域名资源分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        PageGridData<DomainResourceDto> GetDomainPage(DomainResourceQuery query);

        /// <summary>
        /// 根据项部署服务器的id获取全部域名信息
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        List<DomainResourceDto> GetResourceList(int serverId);

        /// <summary>
        /// 根据主键删除信息
        /// </summary>
        /// <param name="Id"></param>
        void DeleteDomain(int Id);
    }
}
