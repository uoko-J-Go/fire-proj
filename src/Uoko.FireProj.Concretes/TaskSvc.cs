using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Mehdime.Entity;
using Newtonsoft.Json;
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

        public void CreatTask(TaskDto taskDto)
        {
            var taskInfo = new TaskInfo();
            taskInfo.ProjectId = taskDto.Project.Id;
            taskInfo.Branch = "dev";
            taskInfo.TaskName = "邮件发送阻塞";

            switch (taskDto.DeployStage)
            {
                case StageEnum.IOC:
                    var iocInfo = new DeployInfoIoc
                    {
                        DeployStage = taskDto.DeployStage,
                        CheckUserId = string.Join(",", taskDto.CheckUsers.Select(t => t.Id)),
                        NoticeUseId = string.Join(",", taskDto.NoticeUses.Select(t => t.Id)),
                        DeployAddress = taskDto.DeployAddress,
                        DeployIP = taskDto.DeployIP,
                        Domain = taskDto.Domain,
                        SiteName = taskDto.SiteName,
                        Status = DeployStatus.WaitingDeploy,
                        TaskDesc = taskDto.TaskDesc,
                    };
                    taskInfo.DeployInfoIocJson = JsonConvert.SerializeObject(iocInfo);
                    break;
                case StageEnum.PRE:
                    var preInfo = new DeployInfoPre()
                    {
                        DeployStage = taskDto.DeployStage,
                        CheckUserId = string.Join(",", taskDto.CheckUsers.Select(t => t.Id)),
                        NoticeUseId = string.Join(",", taskDto.NoticeUses.Select(t => t.Id)),
                        DeployAddress = taskDto.DeployAddress,
                        DeployIP = taskDto.DeployIP,
                        Domain = taskDto.Domain,
                        SiteName = taskDto.SiteName,
                        Status = DeployStatus.WaitingDeploy,
                        TaskDesc = taskDto.TaskDesc,
                    };
                    taskInfo.DeployInfoPreJson = JsonConvert.SerializeObject(preInfo);
                    break;
                default:
                    throw new NotSupportedException("暂不支持其他阶段");
            }

            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                db.TaskInfo.Add(taskInfo);
                db.SaveChanges();
                taskDto.Id = taskInfo.Id;
                UpdateResourceInfo(taskDto);
            }


            //try
            //{
            //    var entity = Mapper.Map<TaskDto,TaskInfo>(taskDto);
            //    entity.ProjectId = taskDto.Project.Id;
            //    entity.CheckUserId = string.Join(",", taskDto.CheckUsers.Select(t => t.Id));
            //    entity.NoticeUseId= string.Join(",", taskDto.NoticeUses.Select(t => t.Id));
            //    entity.CreateBy = 1;
            //    entity.CreateDate = DateTime.Now;
            //    entity.Status = TaskEnum.WaitingDeploy;
            //    using (var dbScope = _dbScopeFactory.Create())
            //    {
            //        var db = dbScope.DbContexts.Get<FireProjDbContext>();
            //        db.TaskInfo.Add(entity);
            //        db.SaveChanges();
            //        taskDto.Id = entity.Id;
            //        UpdateResourceInfo(taskDto);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new TipInfoException(ex.Message);
            //}
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

        /// <summary>
        /// 应该是根据部署情况，更新部署信息
        /// </summary>
        /// <param name="task"></param>
        public void UpdateTask(TaskDto task)
        {
            //try
            //{
            //    var entity = Mapper.Map<TaskDto, TaskInfo>(task);
            //    entity.ProjectId = task.Project.Id;
            //    entity.CheckUserId = string.Join(",", task.CheckUsers.Select(t => t.Id));
            //    entity.NoticeUseId = string.Join(",", task.NoticeUses.Select(t => t.Id));
            //    entity.ModifyBy = 1;
            //    entity.ModifyDate = DateTime.Now;
            //    using (var dbScope = _dbScopeFactory.Create())
            //    {
            //        var db = dbScope.DbContexts.Get<FireProjDbContext>();
            //        //根据实际情况修改
            //        db.Update(entity, t => new { t.TaskName,t.ProjectId, t.Branch, t.DeployEnvironment, t.DeployIP,t.SiteName,t.DeployAddress,t.TaskDesc,t.CheckUserId,t.NoticeUseId,t.Status,t.ModifyBy,t.ModifyDate });
            //        db.SaveChanges();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new TipInfoException(ex.Message);
            //}
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

                data.DeployEnvironmentName = data.DeployStage.ToString();
               
                //获取任务的部署的站点名称
                var domainInfo = db.DomainResource.FirstOrDefault(r => r.TaskId == taskId);
                if (domainInfo != null)
                {
                    data.SiteName = domainInfo.SiteName;
                    //获取任务部署服务器的信息
                    var serverInfo = db.Servers.FirstOrDefault(r => r.Id == domainInfo.ServerId);
                    if (serverInfo != null)
                    {
                        data.PackageDir = serverInfo.PackageDir;
                        data.DeployIP = serverInfo.IP;
                    }
                }
                
                    

                var checkUsers = new List<UserDto>();
                taskInfo.CheckUserId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach((item)=>
                {
                    checkUsers.Add(new UserDto
                    {
                        Id = int.Parse(item)
                    });
                });
                data.CheckUsers = checkUsers;

                var noticeUsers = new List<UserDto>();
                taskInfo.NoticeUseId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach((item) =>
                {
                    noticeUsers.Add(new UserDto
                    {
                        Id = int.Parse(item)
                    });
                });
                data.NoticeUses = noticeUsers;
                //部署失败读取最新的build Id
                if (taskInfo.Status== TaskEnum.DeployFails)
                {
                    data.BuildId = db.TaskLogs.OrderByDescending(r => r.Id).FirstOrDefault(r => r.TaskId == taskId && r.BuildId > 0).BuildId;
                }
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
                    DeployStage = r.DeployEnvironment,
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
                        Stage = taskInfo.DeployEnvironment,
                        TaskId = task.Id,
                        TriggeredId = task.TriggeredId,
                        TaskLogsType = TaskLogsEnum.Status,
                        LogsText = task.LogsText,
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

        private void UpdateResourceInfo(TaskDto task)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    var resourceInfo = db.DomainResource.FirstOrDefault(r => r.ProjectId == task.Project.Id && r.Name == task.Domain);
                    resourceInfo.Status = DomainResourceStatusEnum.Enable;
                    resourceInfo.ServerId = task.ServerId;
                    resourceInfo.TaskId = task.Id;
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
