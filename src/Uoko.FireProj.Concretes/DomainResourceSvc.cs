﻿using AutoMapper;
using Mehdime.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.Infrastructure.Exception;
using Uoko.FireProj.Model;
using System.Linq.Expressions;
using Uoko.FireProj.DataAccess.Extensions;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Data.Entity;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.Concretes
{
    public class DomainResourceSvc : IDomainResourceSvc
    {
        private readonly IDbContextScopeFactory _dbScopeFactory;

        public DomainResourceSvc(IDbContextScopeFactory dbScopeFactory)
        {
            _dbScopeFactory = dbScopeFactory;
        }

        public void CreatResource(List<DomainResourceDto> dto)
        {
            var entity = Mapper.Map<List<DomainResourceDto>, List<DomainResource>>(dto);
            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                foreach (var item in entity)
                {
                    item.CreateDate = DateTime.Now;
                    db.DomainResource.Add(item);
                }
                db.SaveChanges();
            }
        }

        public void DeleteDomain(int Id)
        {
            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                DomainResource entity = new DomainResource() { Id = Id };
                db.DomainResource.Attach(entity);
                db.DomainResource.Remove(entity);
                db.SaveChanges();
            }
        }

        public PageGridData<DomainResourceDto> GetDomainPage(DomainResourceQuery query)
        {

            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = from domainresource in db.DomainResource.AsQueryable()

                           join project in db.Project.AsQueryable() on domainresource.ProjectId equals project.Id

                           join task in db.TaskInfo.AsQueryable() on domainresource.TaskId equals task.Id
                           into tempTask
                           from parTask in tempTask.DefaultIfEmpty()

                           join server in db.Servers.AsQueryable() on domainresource.ServerId equals server.Id
                           into tempServer
                           from parServer in tempServer.DefaultIfEmpty()

                           select new DomainResourceDto
                           {
                               Id = domainresource.Id,
                               Name = domainresource.Name,
                               ProjectId = domainresource.ProjectId,
                               ProjectName = project.ProjectName,
                               TaskId = domainresource.TaskId,
                               TaskName = parTask.TaskName,
                               ServerId = domainresource.ServerId,
                               ServerName = parServer.Name,
                               SiteName = domainresource.SiteName,
                               StageType = parServer.StageType,
                               TaskCreatorId = parTask.CreatorId,
                           };
                var result = data.OrderBy(r => r.Id).Skip(query.Offset).Take(query.Limit).ToList();
                var total = data.Count();
                return new PageGridData<DomainResourceDto> { rows = result, total = total };
            }
        }

        public List<DomainResourceDto> GetResourceList(int serverId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = from domain in db.DomainResource.AsQueryable()
                           join project in db.Project.AsQueryable() on domain.ProjectId equals project.Id
                           where domain.ServerId == serverId
                           select new DomainResourceDto
                           {
                               Id = domain.Id,
                               Name = domain.Name,
                               ProjectId = domain.ProjectId,
                               TaskId = domain.TaskId,
                               ServerId = domain.ServerId,
                               ServerName = domain.Name,
                               SiteName = domain.SiteName,
                               ProjectName = project.ProjectName
                           };
                return data.ToList();
            }
        }

        public List<DomainResourceDto> GetResourceList(int projectId, int serverId, int? taskId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.DomainResource
                             .Where(r => r.ProjectId == projectId
                                         && r.ServerId == serverId
                                         && (r.TaskId == taskId || r.TaskId == null))
                             .Select(r => new DomainResourceDto
                             {
                                 Id = r.Id,
                                 Name = r.Name,
                                 SiteName = r.SiteName,
                             }).ToList();
                return data;
            }
        }

        public void ReleaseDomain(int taskId)
        {
            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var entity = db.DomainResource.FirstOrDefault(r => r.TaskId == taskId);
                entity.TaskId = null;
                db.SaveChanges();
            }
        }
    }
}
