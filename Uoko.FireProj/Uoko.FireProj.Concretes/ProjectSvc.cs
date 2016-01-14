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
using Uoko.FireProj.DataAccess.Extensions;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;
using Uoko.FireProj.Infrastructure.Exception;
using Uoko.FireProj.Model;

namespace Uoko.FireProj.Concretes
{
    public class ProjectSvc : IProjectSvc
    {
        #region 构造函数注册上下文

        private readonly IDbContextScopeFactory _dbScopeFactory;

        public ProjectSvc(IDbContextScopeFactory dbScopeFactory)
        {
            _dbScopeFactory = dbScopeFactory;
        }
        #endregion

        public void CreatProject(ProjectDto dto)
        {
            try
            {
                var entity = Mapper.Map<ProjectDto, Project>(dto);
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    var data = db.Companies.Add(entity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public void DeleteProject(int projectId)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    Project entity = new Project() { Id = projectId };
                    db.Companies.Attach(entity);
                    db.Companies.Remove(entity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public void EditProject(ProjectDto dto)
        {
            try
            {
                var entity = Mapper.Map<ProjectDto, Project>(dto);
               
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    db.Update(entity, r => new { r.ProjectDesc,r.ProjectName,r.ProjectRepo,r.SiteNmae });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public ProjectDto GetProjectById(int projectId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.Companies.Where(r => r.Id == projectId).Select(r => new ProjectDto()
                {
                    Id = r.Id,
                    ProjectName = r.ProjectName,
                    SiteNmae = r.SiteNmae,
                    ProjectRepo = r.ProjectRepo,
                    ProjectDesc = r.ProjectDesc,
                }).FirstOrDefault();

                return data;
            }
        }

        public PageGridData<ProjectDto> GetProjectPage(ProjectQuery query)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.Companies.Select(r => new ProjectDto
                {
                    Id = r.Id,
                    ProjectName = r.ProjectName,
                    SiteNmae = r.SiteNmae,
                    ProjectRepo = r.ProjectRepo,
                    ProjectDesc = r.ProjectDesc,
                });
                if (!string.IsNullOrEmpty(query.Search))
                {
                    data = data.Where(r => r.ProjectName.Contains(query.Search));
                }
                var result = data.OrderBy(r => r.Id).Skip(query.Offset).Take(query.Limit).ToList();
                var total = data.Count();
                return new PageGridData<ProjectDto> { rows = result, total = total };
            }
        }
    }
}
