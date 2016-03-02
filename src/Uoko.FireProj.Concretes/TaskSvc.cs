using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Mehdime.Entity;
using Newtonsoft.Json;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;
using Uoko.FireProj.Infrastructure.Exception;
using Uoko.FireProj.Model;
using AutoMapper;
using System.Collections.Generic;
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

        public void CreatTask(TaskWriteDto taskDto)
        {
            var taskInfo = new TaskInfo();
            taskInfo.ProjectId = taskDto.ProjectId;
            taskInfo.Branch = taskDto.Branch;
            taskInfo.TaskName = taskDto.TaskName;
            taskInfo.CreatorId = 1;
            taskInfo.CreateDate = DateTime.Now;

            var domain = string.Empty;

            switch (taskDto.DeployStage)
            {
                case StageEnum.IOC:
                    if (!string.IsNullOrEmpty(taskDto.IocDeployInfo.CheckUserId))
                    {
                        var userIds = taskDto.IocDeployInfo.CheckUserId.Split(',');
                        var userIdsStatus = userIds.Select(userId => string.Format("{0}-{1}", userId, (int)QAStatus.Waiting)).ToList();
                        taskInfo.IocCheckUserId = string.Join(",", userIdsStatus);
                    }
                    domain = taskDto.IocDeployInfo.Domain;
                    taskDto.IocDeployInfo.DeployStage = taskDto.DeployStage;
                    taskInfo.DeployInfoIocJson = JsonConvert.SerializeObject(taskDto.IocDeployInfo);
                    break;
                case StageEnum.PRE:
                    if (!string.IsNullOrEmpty(taskDto.PreDeployInfo.CheckUserId))
                    {
                        var userIds = taskDto.PreDeployInfo.CheckUserId.Split(',');
                        var userIdsStatus = userIds.Select(userId => string.Format("{0}-{1}", userId, (int)QAStatus.Waiting)).ToList();
                        taskInfo.PreCheckUserId = string.Join(",", userIdsStatus);
                    }
                    domain = taskDto.PreDeployInfo.Domain;
                    taskDto.PreDeployInfo.DeployStage = taskDto.DeployStage;
                    taskInfo.DeployInfoPreJson = JsonConvert.SerializeObject(taskDto.PreDeployInfo);
                    break;
                default:
                    throw new NotSupportedException("暂不支持其他阶段");
            }

            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                db.TaskInfo.Add(taskInfo);
                db.SaveChanges();

                //更新域名资源使用
                var resourceInfo = db.DomainResource.FirstOrDefault(r => r.ProjectId == taskDto.ProjectId && r.Name == domain);
                resourceInfo.TaskId = taskInfo.Id;

                db.SaveChanges();
            }

        }

        /// <summary>
        /// 应该是根据部署情况，更新部署信息
        /// </summary>
        /// <param name="task"></param>
        public void UpdateTask(TaskWriteDto task)
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

       

        public TaskDetailDto GetTaskById(int taskId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var entity = db.TaskInfo.FirstOrDefault(r => r.Id == taskId);
                var taskDto = Mapper.Map<TaskInfo, TaskDetailDto>(entity);

                taskDto.ProjectName = db.Project.FirstOrDefault(r => r.Id == taskDto.ProjectId).ProjectName;

                taskDto.DeployInfoIocDto = !taskDto.DeployInfoIocJson.IsNullOrEmpty() ? JsonHelper.FromJson<DeployInfoIocDto>(taskDto.DeployInfoIocJson) : new DeployInfoIocDto();
                taskDto.DeployInfoPreDto = !taskDto.DeployInfoPreJson.IsNullOrEmpty() ? JsonHelper.FromJson<DeployInfoPreDto>(taskDto.DeployInfoPreJson) : new DeployInfoPreDto();
                taskDto.DeployInfoOnlineDto = !taskDto.DeployInfoOnlineJson.IsNullOrEmpty() ? JsonHelper.FromJson<DeployInfoOnlineDto>(taskDto.DeployInfoOnlineJson) : new DeployInfoOnlineDto();

                //获取测试,通知人Id集合返回
                taskDto.DeployInfoIocDto.CheckUser = AnalysisUser(taskDto.DeployInfoIocDto.CheckUserId);
                taskDto.DeployInfoIocDto.NoticeUser = AnalysisUser(taskDto.DeployInfoIocDto.NoticeUserId);

                taskDto.DeployInfoPreDto.CheckUser = AnalysisUser(taskDto.DeployInfoPreDto.CheckUserId);
                taskDto.DeployInfoPreDto.NoticeUser = AnalysisUser(taskDto.DeployInfoPreDto.NoticeUserId);

                taskDto.DeployInfoOnlineDto.CheckUser = AnalysisUser(taskDto.DeployInfoOnlineDto.CheckUserId);
                taskDto.DeployInfoOnlineDto.NoticeUser = AnalysisUser(taskDto.DeployInfoOnlineDto.NoticeUserId);

                return taskDto;
            }






            //using (var dbScope = _dbScopeFactory.CreateReadOnly())
            //{
            //    var db = dbScope.DbContexts.Get<FireProjDbContext>();
            //    var taskInfo = db.TaskInfo.FirstOrDefault(t => t.Id == taskId);
            //    var project=db.Project.FirstOrDefault(t => t.Id == taskInfo.ProjectId);
            //    var data = Mapper.Map<TaskInfo, TaskDto>(taskInfo);
            //    data.Project= Mapper.Map<Project, ProjectDto>(project);

            //    data.DeployEnvironmentName = data.DeployStage.ToString();

            //    //获取任务的部署的站点名称
            //    var domainInfo = db.DomainResource.FirstOrDefault(r => r.TaskId == taskId);
            //    if (domainInfo != null)
            //    {
            //        data.SiteName = domainInfo.SiteName;
            //        //获取任务部署服务器的信息
            //        var serverInfo = db.Servers.FirstOrDefault(r => r.Id == domainInfo.ServerId);
            //        if (serverInfo != null)
            //        {
            //            data.PackageDir = serverInfo.PackageDir;
            //            data.DeployIP = serverInfo.IP;
            //        }
            //    }



            //    var checkUsers = new List<UserDto>();
            //    taskInfo.CheckUserId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach((item)=>
            //    {
            //        checkUsers.Add(new UserDto
            //        {
            //            Id = int.Parse(item)
            //        });
            //    });
            //    data.CheckUsers = checkUsers;

            //    var noticeUsers = new List<UserDto>();
            //    taskInfo.NoticeUseId.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach((item) =>
            //    {
            //        noticeUsers.Add(new UserDto
            //        {
            //            Id = int.Parse(item)
            //        });
            //    });
            //    data.NoticeUses = noticeUsers;
            //    //部署失败读取最新的build Id
            //    if (taskInfo.Status== TaskEnum.DeployFails)
            //    {
            //        data.BuildId = db.TaskLogs.OrderByDescending(r => r.Id).FirstOrDefault(r => r.TaskId == taskId && r.BuildId > 0).BuildId;
            //    }
            //    return data;
            //}
        }

        public PageGridData<TaskInfoForList> GetTaskPage(TaskQuery query)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.TaskInfo.AsQueryable();
                if (!string.IsNullOrEmpty(query.Search))
                {
                    data = data.Where(r => r.TaskName.Contains(query.Search));
                }
                var total = data.Count();

                var tasksFromDb = data.OrderByDescending(r => r.Id)
                                      .Skip(query.Offset)
                                      .Take(query.Limit)
                                      .ToList();
                var tasksForList = TransferTask(tasksFromDb);
                return new PageGridData<TaskInfoForList> {rows = tasksForList, total = total};
            }
        }

        public IEnumerable<TaskInfoForList> TransferTask(IEnumerable<TaskInfo> tasks)
        {
            return tasks.Select(task => new TaskInfoForList
                                        {
                                            TaskInfo = task,
                                            OnlineTaskInfo = null
                                        });
        }

        public void UpdateTaskStatus(TaskDto task)
        {
            //try
            //{
            //    using (var dbScope = _dbScopeFactory.Create())
            //    {
            //        var db = dbScope.DbContexts.Get<FireProjDbContext>();
            //        //插入状态变更记录
            //        var taskInfo = db.TaskInfo.FirstOrDefault(r => r.Id == task.Id);
            //        TaskLogs taskinfo = new TaskLogs()
            //        {
            //            CreateBy = 0,
            //            CreateDate = DateTime.Now,
            //            Stage = taskInfo.DeployEnvironment,
            //            TaskId = task.Id,
            //            TriggeredId = task.TriggeredId,
            //            TaskLogsType = TaskLogsEnum.Status,
            //            LogsText = task.LogsText,
            //            LogsDesc = string.Format("{0}任务流程状态从{1}变更为{2}", taskInfo.TaskName, taskInfo.Status.ToDescription(), task.Status.ToDescription())
            //        };
            //        db.TaskLogs.Add(taskinfo);
                   
            //        ///修改任务表状态
            //        taskInfo.ModifyBy = 1;
            //        taskInfo.ModifyDate = DateTime.Now;
            //        taskInfo.Status = task.Status;

            //        db.SaveChanges();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new TipInfoException(ex.Message);
            //}
        }

        private List<UserDto> AnalysisUser(string userInfo)
        {
            List<UserDto> userDtoData = new List<UserDto>();
            var userList = userInfo.Split(',');
            foreach (var item in userList)
            {
                var user = item.Split('-');
                userDtoData.Add(new UserDto
                {
                    Id =int.Parse(user[0]),
                    QAStatus = (QAStatus) int.Parse(user[1])
                });
            }
            return userDtoData;
        }

    }
}
