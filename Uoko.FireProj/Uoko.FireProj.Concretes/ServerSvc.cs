using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mehdime.Entity;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.Concretes
{
    public class ServerSvc : IServerSvc
    {
        private readonly IDbContextScopeFactory _dbScopeFactory;

        public ServerSvc(IDbContextScopeFactory dbScopeFactory)
        {
            _dbScopeFactory = dbScopeFactory;
        }
        public void CreatServer(ServerDto Server)
        {
            throw new NotImplementedException();
        }

        public void DeleteServer(int serverId)
        {
            throw new NotImplementedException();
        }
        public void UpdateServer(ServerDto server)
        {
            throw new NotImplementedException();
        }

        public PageGridData<ServerDto> GetServerByPage(ServerQuery query)
        {
            throw new NotImplementedException();
        }

        public IList<ServerDto> GetAllServerOfEnvironment(EnvironmentEnum environmentEnum, bool needEnable = true)
        {
            throw new NotImplementedException();
        }

        public ServerDto GetServerById(int serverId)
        {
            throw new NotImplementedException();
        }
    
    }
}
