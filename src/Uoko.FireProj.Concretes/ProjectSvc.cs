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

        public int CreatProject(ProjectDto dto)
        {
            try
            {
                var entity = Mapper.Map<ProjectDto, Project>(dto);
                entity.CreateDate = DateTime.Now;
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    var data = db.Project.Add(entity);
                    db.SaveChanges();
                }
                return entity.Id;
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
                    db.Project.Attach(entity);
                    db.Project.Remove(entity);
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
                entity.CreateDate = DateTime.Now;
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    db.Update(entity, r => new { r.ProjectDesc, r.ProjectName, r.ProjectRepo,ProjectId = r.RepoId, r.ProjectSlnName });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public IList<ProjectDto> GetAllProject()
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.Project.Select(r => new ProjectDto
                {
                    Id = r.Id,
                    ProjectName = r.ProjectName,
                    ProjectRepo = r.ProjectRepo,
                    ProjectDesc = r.ProjectDesc,
                    ProjectSlnName = r.ProjectSlnName,
                    ProjectCsprojName = r.ProjectCsprojName,
                    RepoId = r.RepoId,
                    DomainRule = r.DomainRule,
                    CreatorId = r.CreatorId,
                    CreatorName = r.CreatorName,
                    OnlineVersion = r.OnlineVersion
                });
                var result = data.ToList();
                return result;
            }
        }

        public ProjectDto GetProjectById(int projectId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.Project.Where(r => r.Id == projectId).Select(r => new ProjectDto()
                {
                    Id = r.Id,
                    ProjectName = r.ProjectName,
                    ProjectRepo = r.ProjectRepo,
                    ProjectDesc = r.ProjectDesc,
                    ProjectSlnName = r.ProjectSlnName,
                    RepoId = r.RepoId,
                    ProjectCsprojName=r.ProjectCsprojName,
                    DomainRule = r.DomainRule,
                    CreatorId = r.CreatorId,
                    CreatorName = r.CreatorName,
                    OnlineVersion = r.OnlineVersion
                }).FirstOrDefault();

                return data;
            }
        }

        public ProjectDto GetProjectByTaskId(int taskId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();

                var data = from p in db.Project
                    join t in db.TaskInfo on p.Id equals t.ProjectId
                    where t.Id == taskId
                    select new ProjectDto
                    {
                        Id = p.Id,
                        ProjectName = p.ProjectName,
                        ProjectRepo = p.ProjectRepo,
                        ProjectDesc = p.ProjectDesc,
                        ProjectSlnName = p.ProjectSlnName,
                        ProjectCsprojName = p.ProjectCsprojName,
                        RepoId = p.RepoId,
                        DomainRule = p.DomainRule,
                        CreatorId = p.CreatorId,
                        CreatorName = p.CreatorName,
                        OnlineVersion = p.OnlineVersion
                    };
                return data.FirstOrDefault();
            }
        }

        public PageGridData<ProjectDto> GetProjectPage(ProjectQuery query)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.Project.Select(r => new ProjectDto
                {
                    Id = r.Id,
                    ProjectName = r.ProjectName,
                    ProjectRepo = r.ProjectRepo,
                    ProjectDesc = r.ProjectDesc,
                    ProjectSlnName = r.ProjectSlnName,
                    ProjectCsprojName = r.ProjectCsprojName,
                    CreatorId = r.CreatorId,
                    CreatorName = r.CreatorName,
                    OnlineVersion = r.OnlineVersion,
                    RepoId = r.RepoId,
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
