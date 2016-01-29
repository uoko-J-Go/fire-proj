using AutoMapper;
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

namespace Uoko.FireProj.Concretes
{
    public class ResourceInfoSvc : IResourceInfoSvc
    {
        private readonly IDbContextScopeFactory _dbScopeFactory;

        public ResourceInfoSvc(IDbContextScopeFactory dbScopeFactory)
        {
            _dbScopeFactory = dbScopeFactory;
        }

        public void CreatResource(List<ResourceInfoDto> dto)
        {
            try
            {
                var entity = Mapper.Map<List<ResourceInfoDto>, List<ResourceInfo>>(dto);
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    foreach (var item in entity)
                    {
                        item.CreateDate = DateTime.Now;
                        db.ResourceInfo.Add(item);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public List<ResourceInfoDto> GetResourceList(int projectId, string ip)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.CreateReadOnly())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    var data = db.ResourceInfo.Where(r => r.ProjectId == projectId && r.Status == 0 && (r.DeployIP == ip )).Select(r => new ResourceInfoDto
                    {
                        Id = r.Id,
                        Url = r.Url,
                    }).ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public void UpdateResource(ResourceInfoDto dto, Expression<Func<ResourceInfoDto, object>> propertyExpression)
        {
            try
            {
                var entity = Mapper.Map<ResourceInfoDto, ResourceInfo>(dto);
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    db.Configuration.ValidateOnSaveEnabled = false;
                    var entry = db.Entry(entity);
                    entry.State = EntityState.Unchanged;
                    ReadOnlyCollection<MemberInfo> memberInfos = ((dynamic)propertyExpression.Body).Members;
                    foreach (MemberInfo memberInfo in memberInfos)
                    {
                        db.Entry(entity).Property(memberInfo.Name).IsModified = true;
                    }
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
