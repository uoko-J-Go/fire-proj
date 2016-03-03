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
using Uoko.FireProj.DataAccess.Extensions;
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

        public int CreatTask(TaskWriteDto taskDto)
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
                        taskInfo.IocCheckUserId = taskDto.IocDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                    }
                    domain = taskDto.IocDeployInfo.Domain;
                    taskDto.IocDeployInfo.DeployStage = taskDto.DeployStage;
                    taskDto.IocDeployInfo.DeployAddress = string.Format("https://{0}:8172/msdeploy.axd", taskDto.IocDeployInfo.DeployIP);
                    taskDto.IocDeployInfo.DeployStatus = DeployStatus.Deploying;
                    taskInfo.DeployInfoIocJson = JsonHelper.ToJson(taskDto.IocDeployInfo);
                    break;
                case StageEnum.PRE:
                    if (!string.IsNullOrEmpty(taskDto.PreDeployInfo.CheckUserId))
                    {
                        var userIds = taskDto.PreDeployInfo.CheckUserId.Split(',');
                        var userIdsStatus = userIds.Select(userId => string.Format("{0}-{1}", userId, (int)QAStatus.Waiting)).ToList();
                        taskInfo.PreCheckUserId= taskDto.PreDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                    }
                    domain = taskDto.PreDeployInfo.Domain;
                    taskDto.PreDeployInfo.DeployStage = taskDto.DeployStage;
                    taskDto.PreDeployInfo.DeployAddress = string.Format("https://{0}:8172/msdeploy.axd", taskDto.PreDeployInfo.DeployIP);
                    taskDto.PreDeployInfo.DeployStatus = DeployStatus.Deploying;
                    taskInfo.DeployInfoPreJson = JsonHelper.ToJson(taskDto.PreDeployInfo);
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
            //返回任务Id
            return taskInfo.Id;
        }

        /// <summary>
        /// 应该是根据部署情况，更新部署信息
        /// </summary>
        /// <param name="task"></param>
        public void UpdateTask(TaskWriteDto taskDto)
        {
            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();

                var taskInfo = db.TaskInfo.FirstOrDefault(r => r.Id == taskDto.Id);

                taskInfo.Branch = taskDto.Branch;

                taskInfo.ModifyId = 1;
                taskInfo.ModifyDate = DateTime.Now;

                var domain = string.Empty;
                var DeployInfo = string.Empty;
                switch (taskDto.DeployStage)
                {
                    case StageEnum.IOC:
                        if (!string.IsNullOrEmpty(taskDto.IocDeployInfo.CheckUserId))
                        {
                            var userIds = taskDto.IocDeployInfo.CheckUserId.Split(',');
                            var userIdsStatus = userIds.Select(userId => string.Format("{0}-{1}", userId, (int)QAStatus.Waiting)).ToList();
                            taskInfo.IocCheckUserId = taskDto.IocDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                        }
                        domain = taskDto.IocDeployInfo.Domain;
                        taskDto.IocDeployInfo.DeployStage = taskDto.DeployStage;
                        taskDto.IocDeployInfo.DeployAddress = string.Format("https://{0}:8172/msdeploy.axd", taskDto.IocDeployInfo.DeployIP);
                        taskDto.IocDeployInfo.DeployStatus = DeployStatus.Deploying;
                        taskInfo.DeployInfoIocJson = DeployInfo = JsonHelper.ToJson(taskDto.IocDeployInfo);
                        break;
                    case StageEnum.PRE:
                        if (!string.IsNullOrEmpty(taskDto.PreDeployInfo.CheckUserId))
                        {
                            var userIds = taskDto.PreDeployInfo.CheckUserId.Split(',');
                            var userIdsStatus = userIds.Select(userId => string.Format("{0}-{1}", userId, (int)QAStatus.Waiting)).ToList();
                            taskInfo.PreCheckUserId = taskDto.PreDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                        }
                        domain = taskDto.PreDeployInfo.Domain;
                        taskDto.PreDeployInfo.DeployStage = taskDto.DeployStage;
                        taskDto.PreDeployInfo.DeployAddress = string.Format("https://{0}:8172/msdeploy.axd", taskDto.PreDeployInfo.DeployIP);
                        taskDto.PreDeployInfo.DeployStatus = DeployStatus.Deploying;
                        taskInfo.DeployInfoPreJson = DeployInfo = JsonHelper.ToJson(taskDto.PreDeployInfo);
                        break;
                    default:
                        throw new NotSupportedException("暂不支持其他阶段");
                }


                //更新域名资源使用:根据环境直接更新本次占用,同时清空上次IOC环境占用的
                //更新本次占用
                var resourceInfo = db.DomainResource.FirstOrDefault(r => r.ProjectId == taskDto.ProjectId && r.Name == domain);
                resourceInfo.TaskId = taskDto.Id;

                //清空上次占用
                var lastTaskLogs = db.TaskLogs.OrderByDescending(r => r.CreateDate).FirstOrDefault(r => r.TaskId == taskDto.Id && r.Stage == taskDto.DeployStage);
                string lastDomain = string.Empty;
                if (lastTaskLogs != null)
                {
                    switch (taskDto.DeployStage)
                    {
                        case StageEnum.IOC:
                            lastDomain = JsonHelper.FromJson<DeployInfoIocDto>(lastTaskLogs.DeployInfo).Domain;
                            break;
                        case StageEnum.PRE:
                            lastDomain = JsonHelper.FromJson<DeployInfoPreDto>(lastTaskLogs.DeployInfo).Domain;
                            break;
                        default:
                            break;
                    }
                }
                var lastResourceInfo = db.DomainResource.FirstOrDefault(r => r.ProjectId == taskDto.ProjectId && r.Name == lastDomain);
                if (lastResourceInfo != null && lastDomain != domain)
                {
                    lastResourceInfo.TaskId = 0;
                }
                db.SaveChanges();
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

       

        public TaskDetailDto GetTaskById(int taskId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var entity = db.TaskInfo.FirstOrDefault(r => r.Id == taskId);
                var taskDto = Mapper.Map<TaskInfo, TaskDetailDto>(entity);

                //项目信息
                var projectEntity = db.Project.FirstOrDefault(r => r.Id == entity.ProjectId);
                taskDto.ProjectDto= Mapper.Map<Project, ProjectDto>(projectEntity);


                taskDto.DeployInfoIocDto = !taskDto.DeployInfoIocJson.IsNullOrEmpty() ? JsonHelper.FromJson<DeployInfoIocDto>(taskDto.DeployInfoIocJson) : new DeployInfoIocDto();
                taskDto.DeployInfoPreDto = !taskDto.DeployInfoPreJson.IsNullOrEmpty() ? JsonHelper.FromJson<DeployInfoPreDto>(taskDto.DeployInfoPreJson) : new DeployInfoPreDto();
                taskDto.DeployInfoOnlineDto = !taskDto.DeployInfoOnlineJson.IsNullOrEmpty() ? JsonHelper.FromJson<DeployInfoOnlineDto>(taskDto.DeployInfoOnlineJson) : new DeployInfoOnlineDto();

                //获取测试,通知人Id集合返回
                taskDto.DeployInfoIocDto.CheckUser = AnalysisCheckUser(taskDto.DeployInfoIocDto.CheckUserId);
                taskDto.DeployInfoIocDto.NoticeUser = AnalysisNoticeUser(taskDto.DeployInfoIocDto.NoticeUserId);

                taskDto.DeployInfoPreDto.CheckUser = AnalysisCheckUser(taskDto.DeployInfoPreDto.CheckUserId);
                taskDto.DeployInfoPreDto.NoticeUser = AnalysisNoticeUser(taskDto.DeployInfoPreDto.NoticeUserId);

                taskDto.DeployInfoOnlineDto.CheckUser = AnalysisCheckUser(taskDto.DeployInfoOnlineDto.CheckUserId);
                taskDto.DeployInfoOnlineDto.NoticeUser = AnalysisNoticeUser(taskDto.DeployInfoOnlineDto.NoticeUserId);

                return taskDto;
            }







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
            Dictionary<int, string> projDic;
            var taskInfos = tasks as TaskInfo[] ?? tasks.ToArray();

            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {

                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var projectIds = taskInfos.Select(item => item.ProjectId).Distinct();
                projDic = db.Project.Where(item => projectIds.Contains(item.Id))
                            .ToDictionary(item => item.Id, item => item.ProjectName);
            }

            var infoLists = taskInfos.Select(task =>
                                {
                                    string projectName;
                                    projDic.TryGetValue(task.ProjectId, out projectName);
                                    return new TaskInfoForList
                                           {
                                               TaskInfo = task,
                                               OnlineTaskInfo = null,
                                               ProjectName = projectName
                                           };
                                });
            return infoLists;
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

        private List<UserDto> AnalysisCheckUser(string userInfo)
        {
            List<UserDto> userDtoData = new List<UserDto>();
            if (string.IsNullOrEmpty(userInfo))
            {
                return userDtoData;
            }
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
        private List<UserDto> AnalysisNoticeUser(string userInfo)
        {
            List<UserDto> userDtoData = new List<UserDto>();
            if (string.IsNullOrEmpty(userInfo))
            {
                return userDtoData;
            }
            var userList = userInfo.Split(',');
            foreach (var item in userList)
            {
                userDtoData.Add(new UserDto
                {
                    Id = int.Parse(item),
                });
            }
            return userDtoData;
        }
        /// <summary>
        /// 执行部署
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="deployStage"></param>
        /// <param name="triggerId"></param>
        public TaskInfo BeginDeploy(int taskId, StageEnum deployStage, int triggerId)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    var entity = db.TaskInfo.FirstOrDefault(r => r.Id == taskId);
                   
                    //更改任务记录
                    switch (deployStage)
                    {
                        case StageEnum.IOC:
                            var iocDeployInfo = JsonHelper.FromJson<DeployInfoIoc>(entity.DeployInfoIocJson);
                            iocDeployInfo.TriggeredId = triggerId;
                            entity.DeployInfoIocJson = JsonHelper.ToJson(iocDeployInfo);
                            break;
                        case StageEnum.PRE:
                            var preDeployInfo = JsonHelper.FromJson<DeployInfoPre>(entity.DeployInfoPreJson);
                            preDeployInfo.TriggeredId = triggerId;
                            entity.DeployInfoPreJson= JsonHelper.ToJson(preDeployInfo);
                            break;
                        case StageEnum.PRODUCTION:
                            break;
                    }
                    entity.ModifyId = 2;
                    entity.ModifyDate = DateTime.Now;                   
                    db.SaveChanges();
                    return entity;
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public TaskInfo DeployCallback(int triggerId, int buildId, DeployStatus deployStatus)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();

                    var taskLog = db.TaskLogs.Where(r => r.TriggeredId == triggerId).FirstOrDefault();
                    if (taskLog == null)
                    {
                        return null;
                    }

                    var entity = db.TaskInfo.FirstOrDefault(r => r.Id == taskLog.TaskId);

                    //更改任务记录
                    switch (taskLog.Stage)
                    {
                        case StageEnum.IOC:
                            var iocDeployInfo = JsonHelper.FromJson<DeployInfoIoc>(entity.DeployInfoIocJson);
                            iocDeployInfo.DeployStatus = deployStatus;
                            iocDeployInfo.BuildId = buildId;
                            entity.DeployInfoIocJson = JsonHelper.ToJson(iocDeployInfo);
                            break;
                        case StageEnum.PRE:
                            var preDeployInfo = JsonHelper.FromJson<DeployInfoPre>(entity.DeployInfoPreJson);
                            preDeployInfo.DeployStatus = deployStatus;
                            preDeployInfo.BuildId = buildId;
                            entity.DeployInfoPreJson = JsonHelper.ToJson(preDeployInfo);
                            break;
                        case StageEnum.PRODUCTION:
                            break;
                    }
                    entity.ModifyId = 0;
                    entity.ModifierName = "系统";
                    entity.ModifyDate = DateTime.Now;
                    db.SaveChanges();

                    //创建日志
                    var log = new TaskLogs
                    {
                        TaskId = taskLog.TaskId,
                        LogType = LogType.Deploy,
                        Stage = taskLog.Stage,
                        TriggeredId = triggerId,
                        CreateDate= DateTime.Now,
                        CreatorId=0,
                        CreatorName="系统"
                    };
                    switch (taskLog.Stage)
                    {
                        case StageEnum.IOC:
                            log.DeployInfo = entity.DeployInfoIocJson;
                            break;
                        case StageEnum.PRE:
                            log.DeployInfo = entity.DeployInfoPreJson;
                            break;
                        case StageEnum.PRODUCTION:
                            log.DeployInfo = entity.DeployInfoOnlineJson;
                            break;
                    }
                    db.TaskLogs.Add(log);
                    db.SaveChanges();
                    return entity;
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }
    }
}
