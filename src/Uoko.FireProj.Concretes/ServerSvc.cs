using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Mehdime.Entity;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Extensions;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;
using Uoko.FireProj.Infrastructure.Exception;
using Uoko.FireProj.Model;

namespace Uoko.FireProj.Concretes
{
    public class ServerSvc : IServerSvc
    {
        private readonly IDbContextScopeFactory _dbScopeFactory;

        public ServerSvc(IDbContextScopeFactory dbScopeFactory)
        {
            _dbScopeFactory = dbScopeFactory;
        }
        public void CreateServer(ServerDto server)
        {
            try
            {
                var entity = Mapper.Map<ServerDto, Server>(server);
                entity.CreateBy = 1;
                entity.CreateDate = DateTime.Now;
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    db.Servers.Add(entity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public void DeleteServer(int serverId)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    Server entity = new Server() { Id = serverId };
                    db.Servers.Attach(entity);
                    db.Servers.Remove(entity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }
        public void UpdateServer(ServerDto server)
        {
            try
            {
                var entity = Mapper.Map<ServerDto, Server>(server);
                entity.ModifyBy = 1;
                entity.ModifyDate = DateTime.Now;
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    //根据实际情况修改
                    db.Update(entity, t => new { t.Name,t.IP,t.ServerDesc, t.Status, t.ModifyBy, t.ModifyDate });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public PageGridData<ServerDto> GetServerByPage(ServerQuery query)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.Servers.Select(t => new ServerDto
                {
                    Id = t.Id,
                    Name=t.Name,
                    IP=t.IP,
                    ServerDesc=t.ServerDesc,
                    Status = t.Status,
                });
                if (query.EnvironmentType.HasValue)
                {
                    data = data.Where(t => t.EnvironmentType==query.EnvironmentType.Value);
                }
                if (!string.IsNullOrEmpty(query.Search))
                {
                    data = data.Where(t => t.Name.Contains(query.Search)||t.IP.Contains(query.Search));
                }
                var result = data.OrderBy(t => t.Id).Skip(query.Offset).Take(query.Limit).ToList();
                var total = data.Count();
                return new PageGridData<ServerDto> { rows = result, total = total };
            }
        }

        public IList<ServerDto> GetAllServerOfEnvironment(EnvironmentEnum environmentEnum, bool needEnable = true)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.Servers.Where(t => t.EnvironmentType == environmentEnum);
                if (needEnable)
                {
                    data = data.Where(t => t.Status == GenericStatusEnum.Enable);
                }
                var result = data.Select(t => new ServerDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    IP = t.IP,
                    ServerDesc = t.ServerDesc,
                    Status = t.Status,
                }).ToList();

                return result;
            }
        }

        public ServerDto GetServerById(int serverId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.Servers.Select(t => new ServerDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    IP = t.IP,
                    ServerDesc = t.ServerDesc,
                    Status = t.Status,
                }).FirstOrDefault(t => t.Id == serverId);
                return data;
            }
        }
    
    }
}
