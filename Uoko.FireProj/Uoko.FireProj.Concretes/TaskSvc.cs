using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Mehdime.Entity;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Extensions;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;
using Uoko.FireProj.Infrastructure.Exception;
using Uoko.FireProj.Model;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.Infrastructure.Extensions;

namespace Uoko.FireProj.Concretes
{
    public class TaskSvc : ITaskSvc
    {

        #region 构造函数注册上下文

        private readonly IDbContextScopeFactory _dbScopeFactory;

        public TaskSvc(IDbContextScopeFactory dbScopeFactory)
        {
            _dbScopeFactory = dbScopeFactory;
        }
        #endregion
        public void CreatTask(TaskDto task)
        {
            try
            {
                var entity = Mapper.Map<TaskDto,TaskInfo>(task);
                entity.ProjectId = task.Project.Id;
                entity.CheckUserId = string.Join(",", task.CheckUsers.Select(t => t.Id));
                entity.NoticeUseId= string.Join(",", task.NoticeUses.Select(t => t.Id));
                entity.CreateBy = 1;
                entity.CreateDate = DateTime.Now;
                entity.Status = TaskEnum.WaitingDeploy;
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    var data = db.TaskInfo.Add(entity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public void DeleteTask(int taskId)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    TaskInfo entity = new TaskInfo() { Id = taskId };
                    db.TaskInfo.Attach(entity);
                    db.TaskInfo.Remove(entity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public void UpdateTask(TaskDto task)
        {
            try
            {
                var entity = Mapper.Map<TaskDto, TaskInfo>(task);
                entity.ProjectId = task.Project.Id;
                entity.CheckUserId = string.Join(",", task.CheckUsers.Select(t => t.Id));
                entity.NoticeUseId = string.Join(",", task.NoticeUses.Select(t => t.Id));
                entity.ModifyBy = 1;
                entity.ModifyDate = DateTime.Now;
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    //根据实际情况修改
                    db.Update(entity, t => new { t.TaskName,t.ProjectId, t.Branch, t.DeployEnvironment, t.DeployIP,t.SiteName,t.DeployAddress,t.TaskDesc,t.CheckUserId,t.NoticeUseId,t.Status,t.ModifyBy,t.ModifyDate });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public TaskDto GetTaskById(int taskId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var taskInfo = db.TaskInfo.FirstOrDefault(t => t.Id == taskId);
                var project=db.Project.FirstOrDefault(t => t.Id == taskInfo.ProjectId);
                var data = Mapper.Map<TaskInfo, TaskDto>(taskInfo);
                data.Project= Mapper.Map<Project, ProjectDto>(project);
                
                var checkUsers = new List<UserDto>();
                taskInfo.CheckUserId.Split(',').ToList().ForEach((item)=>
                {
                    checkUsers.Add(new UserDto
                    {
                        Id = int.Parse(item)
                    });
                });
                data.CheckUsers = checkUsers;

                var noticeUsers = new List<UserDto>();
                taskInfo.NoticeUseId.Split(',').ToList().ForEach((item) =>
                {
                    noticeUsers.Add(new UserDto
                    {
                        Id = int.Parse(item)
                    });
                });
                data.NoticeUses = noticeUsers;
                return data;
            }
        }

        public PageGridData<TaskDto> GetTaskPage(TaskQuery query)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.TaskInfo.Select(r => new TaskDto
                {
                    Id = r.Id,
                    TaskName = r.TaskName,
                    DeployEnvironment = r.DeployEnvironment,
                    Branch = r.Branch,
                    TaskDesc = r.TaskDesc,
                    Status = r.Status,
                });
                if (!string.IsNullOrEmpty(query.Search))
                {
                    data = data.Where(r => r.TaskName.Contains(query.Search));
                }
                var result = data.OrderBy(r => r.Id).Skip(query.Offset).Take(query.Limit).ToList();
                var total = data.Count();
                return new PageGridData<TaskDto> { rows = result, total = total };
            }
        }

        public void UpdateTaskStatus(TaskDto task)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    //插入状态变更记录
                    var taskInfo = db.TaskInfo.FirstOrDefault(r => r.Id == task.Id);
                    TaskLogs taskinfo = new TaskLogs()
                    {
                        CreateBy = 0,
                        CreateDate = DateTime.Now,
                        Environment = taskInfo.DeployEnvironment,
                        TaskId = task.Id,
                        TriggeredId = task.TriggeredId,
                        TaskLogsType = TaskLogsEnum.Status,
                        LogsDesc = string.Format("{0}任务流程状态从{1}变更为{2}", taskInfo.TaskName, taskInfo.Status.ToDescription(), task.Status.ToDescription())
                    };
                    db.TaskLogs.Add(taskinfo);
                   
                    ///修改任务表状态
                    taskInfo.ModifyBy = 1;
                    taskInfo.ModifyDate = DateTime.Now;
                    taskInfo.Status = task.Status;

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
