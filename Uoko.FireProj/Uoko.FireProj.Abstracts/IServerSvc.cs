using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.Abstracts
{
    public interface IServerSvc
    {
        /// <summary>
        /// 新增服务器
        /// </summary>
        /// <param name="server"></param>
        void CreatServer(ServerDto Server);

        /// <summary>
        /// 服务器编辑
        /// </summary>
        /// <param name="server"></param>
        void UpdateServer(ServerDto server);

        /// <summary>
        /// 根据服务器id删除服务器信息
        /// </summary>
        /// <param name="serverId"></param>
        void DeleteServer(int serverId);

        /// <summary>
        /// 分页获取服务器
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        PageGridData<ServerDto> GetServerByPage(ServerQuery query);
        /// <summary>
        /// 获取某个环境下所有服务器
        /// </summary>
        /// <param name="environmentEnum"></param>
        /// <param name="needEnable">是否要求是可用的</param>
        /// <returns></returns>
        IList<ServerDto> GetAllServerOfEnvironment(EnvironmentEnum environmentEnum,bool needEnable=true);
        /// <summary>
        /// 根据服务器Id获取服务器信息详情
        /// </summary>
        /// <param name="serverId"></param>
        ServerDto GetServerById(int serverId);
    }
}
