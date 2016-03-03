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
                entity.CreatorId = 1;
                entity.CreateDate = DateTime.Now;
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    db.Servers.Add(entity);
                    db.SaveChanges();
                    //保存域名信息
                    foreach (var item in server.IISData)
                    {
                        var domainEntity = new DomainResource()
                        {
                            Name = item.Name,
                            SiteName = item.SiteName,
                            ProjectId = item.ProjectId,
                            ServerId = entity.Id,
                            CreatorId = 1,
                            CreateDate = DateTime.Now,
                        };
                        db.DomainResource.Add(domainEntity);
                    }
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
                entity.ModifyId = 1;
                entity.ModifyDate = DateTime.Now;
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    //根据实际情况修改
                    db.Update(entity, t => new { t.Name, t.IP, t.ServerDesc, ModifyBy = t.ModifyId, t.ModifyDate, t.PackageDir });

                    //修改域名有主键则修改,无主键新增
                    var domainList = Mapper.Map<List<DomainResourceDto>, List<DomainResource>>(server.IISData);

                    foreach (var item in domainList)
                    {
                        if (item.Id > 0)
                        {
                            db.Update(item, r => new { r.Name, r.SiteName, r.ProjectId });
                        }
                        else
                        {
                            item.ServerId = server.Id;
                            item.CreateDate = DateTime.Now;
                            db.DomainResource.Add(item);
                        }
                    }
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
                    Name = t.Name,
                    IP = t.IP,
                    ServerDesc = t.ServerDesc,
                    PackageDir = t.PackageDir,
                });
                if (query.StageType.HasValue)
                {
                    data = data.Where(t => t.StageType==query.StageType.Value);
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

        public IList<ServerDto> GetAllServerOfEnvironment(StageEnum stageEnum)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.Servers.Where(t => t.StageType == stageEnum);
                var result = data.Select(t => new ServerDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    IP = t.IP,
                    ServerDesc = t.ServerDesc,
                    PackageDir = t.PackageDir,
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
                    PackageDir = t.PackageDir,
                }).FirstOrDefault(t => t.Id == serverId);
                data.IISData = db.DomainResource.Where(r => r.ServerId == serverId).Select(r => new DomainResourceDto {
                    Id = r.Id,
                    Name = r.Name,
                    ProjectId = r.ProjectId,
                    TaskId = r.TaskId,
                    ServerId = r.ServerId,
                    ServerName = r.Name,
                }).ToList();
                return data;
            }
        }
    
    }
}
