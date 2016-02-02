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
            try
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
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public List<DomainResourceDto> GetResourceList(int projectId, int serverId)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.CreateReadOnly())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    var data = db.DomainResource.Where(r => r.ProjectId == projectId && r.Status == 0 && (r.ServerId == 0 || r.ServerId == serverId)).Select(r => new DomainResourceDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                    }).ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public void ReleaseDomain(int taskId)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    var entity = db.DomainResource.FirstOrDefault(r => r.TaskId == taskId);
                    entity.Status = DomainResourceStatusEnum.Unable;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }
    }
}
