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
    }
}
