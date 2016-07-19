﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using AutoMapper;
using Mehdime.Entity;
using Newtonsoft.Json;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Gitlab;
using Uoko.FireProj.DataAccess.Mail;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;
using Uoko.FireProj.Infrastructure.Exception;
using Uoko.FireProj.Infrastructure.Extensions;
using Uoko.FireProj.Infrastructure.Mail;
using Uoko.FireProj.Model;

namespace Uoko.FireProj.Concretes
{
    public class TaskSvc : ITaskSvc
    {

        #region 构造函数注册上下文

        private readonly IDbContextScopeFactory _dbScopeFactory;
        private readonly IProjectSvc _projectSvc;
        private readonly IServerSvc _serverSvc;
        private readonly string _domainUrl = ConfigurationManager.AppSettings["domain.url"];
        private string gitlabToken = UserHelper.GitLabToken;
        private WebApiProvider gitLabApi = new WebApiProvider(string.Format("{0}/api/v3/", UserHelper.GitLabUrl));

        public TaskSvc(IDbContextScopeFactory dbScopeFactory,IProjectSvc projectSvc,IServerSvc serverSvc)
        {
            _dbScopeFactory = dbScopeFactory;
            _projectSvc = projectSvc;
            _serverSvc = serverSvc;
        }

        #endregion
        #region 回滚任务

        public IEnumerable<RollbackTaskInfo> GetRollBackInfoByProjectId(int projectId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var result = db.RollbackTaskInfo.Where(t => t.ProjectId == projectId).OrderByDescending(t => t.Id).ToList();

                return result;
            }
        }
        public IEnumerable<OnlineTaskInfo> GetOnlineTaskRollbackAble(int projectId, int serverId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();

                var result = db.OnlineTaskInfos.Where(t => t.ProjectId == projectId && t.DeployStatus == DeployStatus.DeploySuccess && t.DeployServerId == serverId).OrderByDescending(t => t.Id).ToList();
                result = result.Where(t => t.CreateDate.AddHours(12).CompareTo(DateTime.Now) > 0).ToList();
                return result;
            }
        }
        public RollbackTaskInfo CreateRollbackTask(RollbackTaskInfo taskInfo)
        {
            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                taskInfo.CreatorId = UserHelper.CurrUserInfo.UserId;
                taskInfo.CreatorName = UserHelper.CurrUserInfo.NickName;
                taskInfo.CreateDate=DateTime.Now;
                db.RollbackTaskInfo.Add(taskInfo);
                db.SaveChanges();

                return taskInfo;
            }
        }

        public void DeployRollbackTask(RollbackTaskInfo taskInfo)
        {
            var project = _projectSvc.GetProjectById(taskInfo.ProjectId);
            if (project == null)
            {
                throw new TipInfoException("找不到 Project");
            }
            var deployServer = _serverSvc.GetServerById(taskInfo.DeployServerId);
            if (deployServer == null)
            {
                throw new TipInfoException("没有 server 信息");
            }
            var repoId = project.RepoId;

            var triggerUrl = string.Format("projects/{0}/triggers?private_token={1}", repoId, gitlabToken);
            var triggers = gitLabApi.Get<List<Trigger>>(triggerUrl)
                           ?? new List<Trigger>();
            var trigger = triggers.FirstOrDefault();
            if (trigger == null)
            {
                throw new TipInfoException("项目在GitLab上未配置 triggers");
            }

            var buildInfo = new Dictionary<string, string>
                            {
                                {"invokeTime", DateTime.Now.ToString("yy-MM-dd HH:mm:ss")},
                                {"rollbackVersion",taskInfo.FromVersion },
                                {"iisSiteName", taskInfo.SiteName},
                                {"pkgDir", deployServer.PackageDir},
                                {"msDeployUrl", "https://" + taskInfo.DeployServerIP + ":8172/msdeploy.axd"},
                                {"Target", "Rollback"}
                            };

            var buildRequst = new TriggerRequest()
            {
                token = trigger.token,
                @ref = "master",
                variables = buildInfo
            };

            var triggerUri = string.Format("projects/{0}/trigger/builds?private_token={1}", repoId, gitlabToken);
            var triggerId = gitLabApi.Post<TriggerRequest, TriggerResponse>(triggerUri, buildRequst).id;

            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var taskFromDb = db.RollbackTaskInfo.Find(taskInfo.Id);
                if (taskFromDb != null)
                {
                    taskFromDb.TriggeredId = triggerId;
                    taskFromDb.DeployStatus = DeployStatus.Deploying;
                    taskFromDb.ModifierName = UserHelper.CurrUserInfo.NickName;
                    taskFromDb.ModifyId = UserHelper.CurrUserInfo.UserId;
                    taskFromDb.ModifyDate = DateTime.Now;

                    #region 写日志
                    var log = new TaskLogs
                    {
                        TaskId = taskFromDb.Id,
                        LogType = LogType.RollBack,
                        Stage = StageEnum.PRODUCTION,
                        TriggeredId = triggerId,
                        CreateDate = DateTime.Now,
                        CreatorId = UserHelper.CurrUserInfo.UserId,
                        CreatorName = UserHelper.CurrUserInfo.NickName,
                        DeployInfo = JsonHelper.ToJson(taskFromDb)
                    };
                    db.TaskLogs.Add(log);
                    #endregion
                }

                dbScope.SaveChanges();
            }
        } 
        #endregion

        /// <summary>
        /// 创建 上线任务，并且关联 待上线任务, 写日志
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        public OnlineTaskInfo CreateOnlineTask(OnlineTaskInfo taskInfo)
        {
            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                //版本号唯一校验
                var num = db.OnlineTaskInfos.Where(r => r.OnlineVersion == taskInfo.OnlineVersion && r.ProjectId == taskInfo.ProjectId).Count();
                if (num > 0)
                {
                    throw new TipInfoException("上线版本号不允许重复!");
                }
                db.OnlineTaskInfos.Add(taskInfo);
                db.SaveChanges();

                var onlineTaskId = taskInfo.Id;

                // 已经上线过 pre，但未上线的任务
                var tasksToBeOnline = db.TaskInfo
                                        .Where(task => task.ProjectId == taskInfo.ProjectId
                                                       && task.OnlineTaskId == null
                                                       && task.DeployInfoPreJson != null)
                                        .ToList();

                foreach (var taskToBeOnline in tasksToBeOnline)
                {
                    taskToBeOnline.OnlineTaskId = onlineTaskId;
                    var deployOnlineInfo = new DeployInfoOnline();
                    if (!string.IsNullOrEmpty(taskToBeOnline.DeployInfoOnlineJson))
                    {
                        deployOnlineInfo =
                            JsonConvert.DeserializeObject<DeployInfoOnline>(taskToBeOnline.DeployInfoOnlineJson);
                    }
                    deployOnlineInfo.OnlineTaskId = onlineTaskId;
                    deployOnlineInfo.DeployStage = StageEnum.PRODUCTION;
                    deployOnlineInfo.CheckUserId = taskToBeOnline.OnlineCheckUserId;

                    taskToBeOnline.DeployInfoOnlineJson = JsonConvert.SerializeObject(deployOnlineInfo);
                }

                dbScope.SaveChanges();
                return taskInfo;
            }
        }


        public void DeployOnlineTask(OnlineTaskInfo onlineTaskInfo)
        {
            var project = _projectSvc.GetProjectById(onlineTaskInfo.ProjectId);
            if (project == null)
            {
                throw new TipInfoException("找不到 Project");
            }

            var deployServer = _serverSvc.GetServerById(onlineTaskInfo.DeployServerId);
            if (deployServer == null)
            {
                throw new TipInfoException("没有 server 信息");
            }

            var repoId = project.RepoId;

            var triggerUrl = string.Format("projects/{0}/triggers?private_token={1}", repoId, gitlabToken);
            var triggers = gitLabApi.Get<List<Trigger>>(triggerUrl)
                           ?? new List<Trigger>();
            var trigger = triggers.FirstOrDefault();
            if (trigger == null)
            {
                throw new TipInfoException("项目在GitLab上未配置 triggers");
            }

            var onlineTagName = string.Format("{0}", onlineTaskInfo.OnlineVersion);
            var buildInfo = new Dictionary<string, string>
                            {
                                {"slnFile", project.ProjectSlnName},
                                {"csProjFile", project.ProjectCsprojName},
                                {"iisSiteName", onlineTaskInfo.SiteName},
                                {"pkgDir", deployServer.PackageDir},
                                {"msDeployUrl", "https://" + onlineTaskInfo.DeployServerIP + ":8172/msdeploy.axd"},
                                {"useConfig", AnalysisObj.AnalysisDomain(onlineTaskInfo.Domain)},
                                {"Target", "Online"},
                                {"mergeFromBranch", "pre"},
                                {"onlineTagName", onlineTagName},
                                {"FireTaskId",onlineTaskInfo.Id.ToString() }
                            };

            var buildRequst = new TriggerRequest()
                              {
                                  token = trigger.token,
                                  @ref = "master",
                                  variables = buildInfo
                              };

            var triggerUri = string.Format("projects/{0}/trigger/builds?private_token={1}", repoId, gitlabToken);
            var triggerId = gitLabApi.Post<TriggerRequest, TriggerResponse>(triggerUri, buildRequst).id;

            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var taskFromDb = db.OnlineTaskInfos.Find(onlineTaskInfo.Id);
                if (taskFromDb != null)
                {
                    taskFromDb.TriggeredId = triggerId;
                    taskFromDb.DeployStatus = DeployStatus.Deploying;
                    taskFromDb.ModifierName =UserHelper.CurrUserInfo.NickName;
                    taskFromDb.ModifyId = UserHelper.CurrUserInfo.UserId;
                    taskFromDb.ModifyDate = DateTime.Now;

                    #region 写日志
                    var log = new TaskLogs
                    {
                        TaskId = taskFromDb.Id,
                        LogType = LogType.Online,
                        Stage = StageEnum.PRODUCTION,
                        TriggeredId = triggerId,
                        CreateDate = DateTime.Now,
                        CreatorId = UserHelper.CurrUserInfo.UserId,
                        CreatorName = UserHelper.CurrUserInfo.NickName,
                        DeployInfo=JsonHelper.ToJson(taskFromDb)
                    };
                    db.TaskLogs.Add(log);
                    #endregion
                    #region 释放域名占用
                    var domain = db.DomainResource.Where(r => r.ProjectId == taskFromDb.ProjectId);
                    foreach (var item in domain)
                    {
                        item.TaskId = null;
                    }
                    #endregion
                }

                dbScope.SaveChanges();
            }
        }

        public OnlineTaskDetailDto GetOnlineTaskDetail(int onlineTaskId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();

                var onlineTask = db.OnlineTaskInfos.FirstOrDefault(item => item.Id == onlineTaskId);
                if (onlineTask == null)
                {
                    return null;
                }


                var onlineTasksFromDb = db.TaskInfo
                                    .Where(task => task.OnlineTaskId == onlineTaskId)
                                    .OrderByDescending(item => item.Id)
                                    .ToList();
                var onlineTasks = TransferTask(onlineTasksFromDb);

                var onlineTaskDetail = new OnlineTaskDetailDto
                                       {
                                           OnlineTask = onlineTask,
                                           TaskBelongOnline = onlineTasks
                                       };
            
                return onlineTaskDetail;
            }
        }

        /// <summary>
        /// 先判断是否有更新的 上线任务
        /// </summary>
        /// <param name="onlineTaskId"></param>
        public void ReDeployOnlineTask(int onlineTaskId)
        {
            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var taskFromDb = db.OnlineTaskInfos.Find(onlineTaskId);
                if (taskFromDb == null)
                {
                    throw new TipInfoException("没有找到上线任务信息: " + onlineTaskId);
                }

                var hasNewOnlineTask = db.OnlineTaskInfos.Any(item => item.ProjectId == taskFromDb.ProjectId
                                                                      && item.Id > onlineTaskId);

                if (hasNewOnlineTask)
                {
                    throw new TipInfoException("已经存在新的上线任务,不能重试.");
                }

                DeployOnlineTask(taskFromDb);

                dbScope.SaveChanges();
            }
        }


        public int CreatTask(TaskWriteDto taskDto)
        {
            var taskInfo = new TaskInfo();
            taskInfo.ProjectId = taskDto.ProjectId;
            taskInfo.Branch = taskDto.Branch;
            taskInfo.TaskName = taskDto.TaskName;
            taskInfo.CreateDate = DateTime.Now;
            taskInfo.CreatorId = UserHelper.CurrUserInfo.UserId;
            taskInfo.CreatorName = UserHelper.CurrUserInfo.NickName;

            var domain = string.Empty;

            switch (taskDto.DeployStage)
            {
                case StageEnum.TEST:
                    if (!string.IsNullOrEmpty(taskDto.IocDeployInfo.CheckUserId))
                    {
                        var userIds = taskDto.IocDeployInfo.CheckUserId.Split(',');
                        var userIdsStatus =
                            userIds.Select(userId => string.Format("{0}-{1}", userId, (int) QAStatus.Waiting)).ToList();
                        taskInfo.IocCheckUserId = taskDto.IocDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                    }
                    domain = taskDto.IocDeployInfo.Domain;
                    taskDto.IocDeployInfo.DeployStage = taskDto.DeployStage;
                    taskDto.IocDeployInfo.DeployAddress = string.Format("https://{0}:8172/msdeploy.axd",
                        taskDto.IocDeployInfo.DeployIP);
                    taskDto.IocDeployInfo.DeployStatus = DeployStatus.Deploying;
                    taskInfo.DeployInfoIocJson = JsonHelper.ToJson(taskDto.IocDeployInfo);
                    break;
                case StageEnum.PRE:
                    if (!string.IsNullOrEmpty(taskDto.PreDeployInfo.CheckUserId))
                    {
                        var userIds = taskDto.PreDeployInfo.CheckUserId.Split(',');
                        var userIdsStatus =
                            userIds.Select(userId => string.Format("{0}-{1}", userId, (int) QAStatus.Waiting)).ToList();
                        taskInfo.PreCheckUserId = taskDto.PreDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                    }
                    domain = taskDto.PreDeployInfo.Domain;
                    taskDto.PreDeployInfo.DeployStage = taskDto.DeployStage;
                    taskDto.PreDeployInfo.DeployAddress = string.Format("https://{0}:8172/msdeploy.axd",
                        taskDto.PreDeployInfo.DeployIP);
                    taskDto.PreDeployInfo.DeployStatus = DeployStatus.Deploying;
                    taskInfo.DeployInfoPreJson = JsonHelper.ToJson(taskDto.PreDeployInfo);

                    //添加Online信息
                    if (!string.IsNullOrEmpty(taskDto.OnlineDeployInfo.CheckUserId))
                    {
                        var userIds = taskDto.OnlineDeployInfo.CheckUserId.Split(',');
                        var userIdsStatus =
                            userIds.Select(userId => string.Format("{0}-{1}", userId, (int) QAStatus.Waiting)).ToList();
                        taskInfo.OnlineCheckUserId =
                            taskDto.OnlineDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                    }
                    taskDto.OnlineDeployInfo.DeployStage = StageEnum.PRODUCTION;
                    taskInfo.DeployInfoOnlineJson = JsonHelper.ToJson(taskDto.OnlineDeployInfo);

                    break;
                default:
                    throw new NotSupportedException("暂不支持其他阶段");
            }

            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                db.TaskInfo.Add(taskInfo);
                db.SaveChanges();

                //更新域名资源使用(只针对TEST环境)
                if (taskDto.DeployStage == StageEnum.TEST)
                {
                    var resourceInfo = db.DomainResource.FirstOrDefault(r => r.ProjectId == taskDto.ProjectId && r.Name == domain);
                    resourceInfo.TaskId = taskInfo.Id;
                    db.SaveChanges();
                }
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
                taskInfo.ModifyDate = DateTime.Now;
                taskInfo.ModifyId = UserHelper.CurrUserInfo.UserId;
                taskInfo.ModifierName = UserHelper.CurrUserInfo.NickName;
                var domain = string.Empty;
                switch (taskDto.DeployStage)
                {
                    case StageEnum.TEST:
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
                            taskInfo.PreCheckUserId = taskDto.PreDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                        }
                        domain = taskDto.PreDeployInfo.Domain;
                        taskDto.PreDeployInfo.DeployStage = taskDto.DeployStage;
                        taskDto.PreDeployInfo.DeployAddress = string.Format("https://{0}:8172/msdeploy.axd", taskDto.PreDeployInfo.DeployIP);
                        taskDto.PreDeployInfo.DeployStatus = DeployStatus.Deploying;
                        taskInfo.DeployInfoPreJson = JsonHelper.ToJson(taskDto.PreDeployInfo);


                        //添加或修改Online信息
                        if (!string.IsNullOrEmpty(taskDto.OnlineDeployInfo.CheckUserId))
                        {
                            var userIds = taskDto.OnlineDeployInfo.CheckUserId.Split(',');
                            var userIdsStatus = userIds.Select(userId => string.Format("{0}-{1}", userId, (int)QAStatus.Waiting)).ToList();
                            taskInfo.OnlineCheckUserId = taskDto.OnlineDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                        }
                        taskDto.OnlineDeployInfo.DeployStage = StageEnum.PRODUCTION;
                        taskDto.OnlineDeployInfo.OnlineTaskId = null;
                        taskInfo.DeployInfoOnlineJson = JsonHelper.ToJson(taskDto.OnlineDeployInfo);
                        taskInfo.OnlineTaskId = null;
                        break;
                    default:
                        throw new NotSupportedException("暂不支持其他阶段");
                }


                //更新域名资源使用:根据环境直接更新本次占用,同时清空上次TEST环境占用的(只针对TEST环境)
                if (taskDto.DeployStage == StageEnum.TEST)
                {
                    //更新本次占用
                    var resourceInfo =
                        db.DomainResource.FirstOrDefault(r => r.ProjectId == taskDto.ProjectId && r.Name == domain);
                    resourceInfo.TaskId = taskDto.Id;

                    //清空上次占用
                    var lastTaskLogs =
                        db.TaskLogs.OrderByDescending(r => r.CreateDate)
                          .FirstOrDefault(r => r.TaskId == taskDto.Id && r.Stage == taskDto.DeployStage);
                    string lastDomain = string.Empty;
                    if (lastTaskLogs != null)
                    {
                        lastDomain = JsonHelper.FromJson<DeployInfoIocDto>(lastTaskLogs.DeployInfo).Domain;
                    }
                    var lastResourceInfo =
                        db.DomainResource.FirstOrDefault(r => r.ProjectId == taskDto.ProjectId && r.Name == lastDomain);
                    if (lastResourceInfo != null
                        && lastDomain != domain)
                    {
                        lastResourceInfo.TaskId = 0;
                    }
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
                    TaskInfo entity = new TaskInfo() {Id = taskId};
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
                taskDto.ProjectDto = Mapper.Map<Project, ProjectDto>(projectEntity);

             
                taskDto.DeployInfoIocDto = !taskDto.DeployInfoIocJson.IsNullOrEmpty()
                    ? JsonHelper.FromJson<DeployInfoIocDto>(taskDto.DeployInfoIocJson)
                    : new DeployInfoIocDto();
                taskDto.DeployInfoPreDto = !taskDto.DeployInfoPreJson.IsNullOrEmpty()
                    ? JsonHelper.FromJson<DeployInfoPreDto>(taskDto.DeployInfoPreJson)
                    : new DeployInfoPreDto();

                taskDto.DeployInfoOnlineDto = !taskDto.DeployInfoOnlineJson.IsNullOrEmpty()
                    ? JsonHelper.FromJson<DeployInfoOnlineDto>(taskDto.DeployInfoOnlineJson)
                    : new DeployInfoOnlineDto();

                if (taskDto.OnlineTaskId != null)
                {
                    var onlineTaskInfos = db.OnlineTaskInfos.FirstOrDefault(r => r.Id == taskDto.OnlineTaskId.Value);
                    taskDto.DeployInfoOnlineDto.OnlineVersion = onlineTaskInfos.OnlineVersion;
                    taskDto.DeployInfoOnlineDto.DeployIP = onlineTaskInfos.DeployServerIP;
                    taskDto.DeployInfoOnlineDto.Domain = onlineTaskInfos.Domain;
                    taskDto.DeployInfoOnlineDto.BuildId = onlineTaskInfos.BuildId;
                    taskDto.DeployInfoOnlineDto.TriggeredId = onlineTaskInfos.TriggeredId;
                    taskDto.DeployInfoOnlineDto.DeployStatus = onlineTaskInfos.DeployStatus;
                }

                //获取测试,通知人Id集合返回
                taskDto.DeployInfoIocDto.CheckUser = AnalysisObj.AnalysisCheckUser(taskDto.DeployInfoIocDto.CheckUserId);
                taskDto.DeployInfoIocDto.NoticeUser =
                    AnalysisObj.AnalysisNoticeUser(taskDto.DeployInfoIocDto.NoticeUserId);

                taskDto.DeployInfoPreDto.CheckUser = AnalysisObj.AnalysisCheckUser(taskDto.DeployInfoPreDto.CheckUserId);
                taskDto.DeployInfoPreDto.NoticeUser =
                    AnalysisObj.AnalysisNoticeUser(taskDto.DeployInfoPreDto.NoticeUserId);

                taskDto.DeployInfoOnlineDto.CheckUser =
                    AnalysisObj.AnalysisCheckUser(taskDto.DeployInfoOnlineDto.CheckUserId);
                taskDto.DeployInfoOnlineDto.NoticeUser =
                    AnalysisObj.AnalysisNoticeUser(taskDto.DeployInfoOnlineDto.NoticeUserId);

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

                switch (query.ShowType)
                {
                    case TaskQuery.QueryType.QaFocus:
                        var userIdForm = query.LoginUserId + "-";
                        data = data.Where(item => item.IocCheckUserId.Contains(userIdForm)
                                                  || item.PreCheckUserId.Contains(userIdForm)
                                                  || item.OnlineCheckUserId.Contains(userIdForm));
                        break;
                    case TaskQuery.QueryType.CreatorFocus:
                        data = data.Where(item => item.CreatorId == query.LoginUserId);
                        break;
                    case TaskQuery.QueryType.ShowAll:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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


        /// <summary>
        /// 获取需要上线的任务
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TaskInfoForList> GetTasksNeedOnline(int projectId)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();

                // 已经上线过 pre，但未上线的任务
                var tasksFromDb = db.TaskInfo
                                            .Where(task => task.ProjectId == projectId
                                                           && task.OnlineTaskId == null
                                                           && task.DeployInfoPreJson != null)
                                            .OrderByDescending(task => task.Id)
                                                     .ToList();
                var tasksToBeOnline = TransferTask(tasksFromDb);
                return tasksToBeOnline;
            }
        }


        private IEnumerable<TaskInfoForList> TransferTask(IEnumerable<TaskInfo> tasks)
        {
            Dictionary<int, string> projDic;
            var taskInfos = tasks as TaskInfo[] ?? tasks.ToArray();
            List<OnlineTaskInfo> onlineTaskInfos;
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {

                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var projectIds = taskInfos.Select(item => item.ProjectId).Distinct();
                projDic = db.Project.Where(item => projectIds.Contains(item.Id))
                            .ToDictionary(item => item.Id, item => item.ProjectName);

                var onlineTaskIds = taskInfos.Select(item => item.OnlineTaskId)
                                             .Where(item => item != null)
                                             .Distinct();
                onlineTaskInfos = db.OnlineTaskInfos
                          .Where(item => onlineTaskIds.Contains(item.Id))
                          .ToList();

            }

            var infoLists = taskInfos.Select(task =>
                                             {
                                                 string projectName;
                                                 projDic.TryGetValue(task.ProjectId, out projectName);
                                                 var onlineInfo = onlineTaskInfos.FirstOrDefault(item =>
                                                     item.Id == task.OnlineTaskId);
                                                 return new TaskInfoForList
                                                        {
                                                            TaskInfo = task,
                                                            OnlineTaskInfo = onlineInfo,
                                                            ProjectName = projectName
                                                        };
                                             });
            return infoLists;
        }

        
        /// <summary>
        /// 执行部署
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="deployStage"></param>
        /// <returns></returns>
        public TaskInfo BeginDeploy(int taskId, StageEnum deployStage)
        {
                var gitlabToken = "D3MR_rnRZK4xWS-CtVho";
                var gitLabApi = new WebApiProvider("http://gitlab.uoko.ioc:12015/api/v3/");
                var taskDto = this.GetTaskById(taskId);
                var triggers =
                    gitLabApi.Get<List<Trigger>>(string.Format("projects/{0}/triggers?private_token={1}",
                        taskDto.ProjectDto.RepoId, gitlabToken));
                if (triggers == null
                    || triggers.Count < 1)
                {
                    throw new Exception("项目在GitLab上为配置Trrigger");
                }
                var trigger = triggers[0];

                var target = "Deploy-To-IOC";
                var iisSiteName = string.Empty;
                var deployIP = string.Empty;
                var packagDir = string.Empty;
                var domain = string.Empty;
                var _ref = taskDto.Branch;
                switch (deployStage)
                {
                    case StageEnum.TEST:
                        target = "Deploy-To-IOC";
                        iisSiteName = taskDto.DeployInfoIocDto.SiteName;
                        deployIP = taskDto.DeployInfoIocDto.DeployIP;
                        domain = taskDto.DeployInfoIocDto.Domain;
                        using (var dbScope = _dbScopeFactory.CreateReadOnly())
                        {
                            var db = dbScope.DbContexts.Get<FireProjDbContext>();
                            var server = db.Servers.FirstOrDefault(r => r.IP.Equals(deployIP));
                            packagDir = server.PackageDir;
                        }
                        _ref = taskDto.Branch;
                        break;
                    case StageEnum.PRE:
                        target = "Deploy-To-PRE";
                        iisSiteName = taskDto.DeployInfoPreDto.SiteName;
                        deployIP = taskDto.DeployInfoPreDto.DeployIP;
                        domain = taskDto.DeployInfoPreDto.Domain;
                        using (var dbScope = _dbScopeFactory.CreateReadOnly())
                        {
                            var db = dbScope.DbContexts.Get<FireProjDbContext>();
                            var server = db.Servers.FirstOrDefault(r => r.IP.Equals(deployIP));
                            packagDir = server.PackageDir;
                        }
                        _ref = "pre";
                        break;
                    case StageEnum.PRODUCTION:
                        target = "Online";
                        iisSiteName = taskDto.DeployInfoOnlineDto.SiteName;
                        deployIP = taskDto.DeployInfoOnlineDto.DeployIP;
                        domain = taskDto.DeployInfoOnlineDto.Domain;
                        using (var dbScope = _dbScopeFactory.CreateReadOnly())
                        {
                            var db = dbScope.DbContexts.Get<FireProjDbContext>();
                            var server = db.Servers.FirstOrDefault(r => r.IP.Equals(deployIP));
                            packagDir = server.PackageDir;
                        }
                        _ref = "master";
                        break;
                }
  
                Dictionary<string, string> buildInfo = new Dictionary<string, string>();
                buildInfo.Add("slnFile", taskDto.ProjectDto.ProjectSlnName);
                buildInfo.Add("csProjFile", taskDto.ProjectDto.ProjectCsprojName);
                buildInfo.Add("iisSiteName", iisSiteName);
                buildInfo.Add("pkgDir", packagDir);
                buildInfo.Add("msDeployUrl", "https://" + deployIP + ":8172/msdeploy.axd");
                buildInfo.Add("useConfig", AnalysisObj.AnalysisDomain(domain));
                buildInfo.Add("Target", target);
                buildInfo.Add("mergeFromBranch", taskDto.Branch);
                buildInfo.Add("FireTaskId", taskId.ToString());
                var buildRequst = new TriggerRequest()
                {
                    token = trigger.token,
                    @ref = _ref,
                    variables = buildInfo
                };

                var triggerId = gitLabApi.Post<TriggerRequest, TriggerResponse>(
                    string.Format("projects/{0}/trigger/builds?private_token={1}", taskDto.ProjectDto.RepoId,
                        gitlabToken), buildRequst).id;


                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    var entity = db.TaskInfo.FirstOrDefault(r => r.Id == taskId);
                    //更改任务记录
                    switch (deployStage)
                    {
                        case StageEnum.TEST:
                            var iocDeployInfo = JsonHelper.FromJson<DeployInfoIoc>(entity.DeployInfoIocJson);
                            iocDeployInfo.TriggeredId = triggerId;
                            entity.DeployInfoIocJson = JsonHelper.ToJson(iocDeployInfo);
                            break;
                        case StageEnum.PRE:
                            var preDeployInfo = JsonHelper.FromJson<DeployInfoPre>(entity.DeployInfoPreJson);
                            preDeployInfo.TriggeredId = triggerId;
                            entity.DeployInfoPreJson = JsonHelper.ToJson(preDeployInfo);
                            break;
                        case StageEnum.PRODUCTION:
                            break;
                    }
                    entity.ModifyId = UserHelper.CurrUserInfo.UserId;
                    entity.ModifierName = UserHelper.CurrUserInfo.NickName;
                    entity.ModifyDate = DateTime.Now;

                    #region 写日志

                    var log = new TaskLogs
                    {
                        TaskId = entity.Id,
                        LogType = LogType.Deploy,
                        Stage = deployStage,
                        TriggeredId = triggerId,
                        CreateDate = DateTime.Now,
                        CreatorId = UserHelper.CurrUserInfo.UserId,
                        CreatorName = UserHelper.CurrUserInfo.NickName,
                    };
                    switch (deployStage)
                    {
                        case StageEnum.TEST:
                            log.DeployInfo = entity.DeployInfoIocJson;
                            break;
                        case StageEnum.PRE:
                            log.DeployInfo = entity.DeployInfoPreJson;
                            break;
                        case StageEnum.PRODUCTION:
                            log.LogType=LogType.Online;
                            log.DeployInfo = entity.DeployInfoOnlineJson;
                            break;
                    }
                    db.TaskLogs.Add(log); 

                    #endregion

                    db.SaveChanges();
                    return entity;
                }
            }

        public void DeployCallback(int triggerId, int buildId, DeployStatus deployStatus)
        {
            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();

                var taskLog = db.TaskLogs.Where(r => r.TriggeredId == triggerId).FirstOrDefault();
                if (taskLog == null)
                {
                    return;
                }
                //日志内容
                var log = new TaskLogs
                {
                    TaskId = taskLog.TaskId,
                    LogType = taskLog.LogType,
                    Stage = taskLog.Stage,
                    TriggeredId = triggerId,
                    CreateDate = DateTime.Now,
                    CreatorId = 0,
                    CreatorName = "系统",
                };

                //更改任务记录
                switch (taskLog.Stage)
                {
                    case StageEnum.TEST:
                        var entityIoc = db.TaskInfo.FirstOrDefault(r => r.Id == taskLog.TaskId);
                        if (entityIoc == null) return;
                        var iocDeployInfo = JsonHelper.FromJson<DeployInfoIoc>(entityIoc.DeployInfoIocJson);
                        iocDeployInfo.DeployStatus = deployStatus;
                        iocDeployInfo.BuildId = buildId;
                        entityIoc.DeployInfoIocJson = JsonHelper.ToJson(iocDeployInfo);
                        entityIoc.ModifyDate = DateTime.Now;
                        log.DeployInfo = entityIoc.DeployInfoIocJson;
                        break;
                    case StageEnum.PRE:
                        var entityPre = db.TaskInfo.FirstOrDefault(r => r.Id == taskLog.TaskId);
                        if (entityPre == null) return;
                        var preDeployInfo = JsonHelper.FromJson<DeployInfoPre>(entityPre.DeployInfoPreJson);
                        preDeployInfo.DeployStatus = deployStatus;
                        preDeployInfo.BuildId = buildId;
                        entityPre.DeployInfoPreJson = JsonHelper.ToJson(preDeployInfo);
                        entityPre.ModifyDate = DateTime.Now;
                        log.DeployInfo = entityPre.DeployInfoPreJson;
                        break;
                    case StageEnum.PRODUCTION:
                        if (taskLog.LogType == LogType.Online)
                        {
                            var entityOnline = db.OnlineTaskInfos.FirstOrDefault(r => r.Id == taskLog.TaskId);
                            if (entityOnline == null) return;

                            entityOnline.DeployStatus = deployStatus;
                            entityOnline.BuildId = buildId;
                            entityOnline.ModifyDate = DateTime.Now;
                            if (deployStatus == DeployStatus.DeploySuccess)
                            {
                                //更新任务当前的线上版本
                                var entityProject = db.Project.FirstOrDefault(t => t.Id == entityOnline.ProjectId);
                                entityProject.OnlineVersion = entityOnline.OnlineVersion;
                            }

                            log.DeployInfo = JsonHelper.ToJson(entityOnline);
                        }
                        else if (taskLog.LogType == LogType.RollBack)
                        {
                            var entityRollback = db.RollbackTaskInfo.FirstOrDefault(t => t.Id == taskLog.TaskId);
                            if (entityRollback == null) return;

                            entityRollback.DeployStatus = deployStatus;
                            entityRollback.BuildId = buildId;
                            entityRollback.ModifyDate = DateTime.Now;
                            if (deployStatus == DeployStatus.DeploySuccess)
                            {
                                //更新任务当前的线上版本
                                var entityProject = db.Project.FirstOrDefault(t => t.Id == entityRollback.ProjectId);
                                entityProject.OnlineVersion = entityRollback.ToVersion;
                            }


                            log.DeployInfo = JsonHelper.ToJson(entityRollback);
                        }

                        break;
                }

                #region 写日志
                var tTasklogs = db.TaskLogs.Count(t => t.TriggeredId == triggerId);
                if (tTasklogs < 2) //避免重复
                {
                    db.TaskLogs.Add(log);
                }
                #endregion
                db.SaveChanges();

            }
        }

        /// <summary>
        /// 更新测试任务
        /// </summary>
        /// <param name="testResult"></param>
        /// <returns></returns>
        public TaskInfo UpdateTestStatus(TestResultDto testResult)
        {
            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var entity = db.TaskInfo.FirstOrDefault(r => r.Id == testResult.TaskId);
                //日志内容
                var log = new TaskLogs
                          {
                              TaskId = entity.Id,
                              LogType = LogType.QA,
                              Stage = testResult.Stage,
                              Comments = testResult.Comments,
                              CreatorId = UserHelper.CurrUserInfo.UserId,
                              CreatorName = UserHelper.CurrUserInfo.NickName,
                              CreateDate = DateTime.Now
                          };

                //更改任务记录
                switch (testResult.Stage)
                {
                    case StageEnum.TEST:
                        var iocDeployInfo = JsonHelper.FromJson<DeployInfoIoc>(entity.DeployInfoIocJson);
                        entity.IocCheckUserId =
                            iocDeployInfo.CheckUserId =
                                GetTestedNewCheckUserId(iocDeployInfo.CheckUserId, testResult.QAStatus);
                        entity.DeployInfoIocJson = JsonHelper.ToJson(iocDeployInfo);

                        log.DeployInfo = entity.DeployInfoIocJson;
                        break;
                    case StageEnum.PRE:
                        var preDeployInfo = JsonHelper.FromJson<DeployInfoPre>(entity.DeployInfoPreJson);
                        entity.PreCheckUserId =
                            preDeployInfo.CheckUserId =
                                GetTestedNewCheckUserId(preDeployInfo.CheckUserId, testResult.QAStatus);
                        entity.DeployInfoPreJson = JsonHelper.ToJson(preDeployInfo);

                        log.DeployInfo = entity.DeployInfoPreJson;
                        break;
                    case StageEnum.PRODUCTION:
                        var onlineDeployInfo = JsonHelper.FromJson<DeployInfoOnline>(entity.DeployInfoOnlineJson);
                        entity.OnlineCheckUserId =
                            onlineDeployInfo.CheckUserId =
                                GetTestedNewCheckUserId(onlineDeployInfo.CheckUserId, testResult.QAStatus);
                        entity.DeployInfoOnlineJson = JsonHelper.ToJson(onlineDeployInfo);

                        log.DeployInfo = entity.DeployInfoOnlineJson;
                        break;
                }
                entity.ModifyId = UserHelper.CurrUserInfo.UserId;
                entity.ModifyDate = DateTime.Now;
                entity.ModifierName = UserHelper.CurrUserInfo.NickName;

                //写日志
                db.TaskLogs.Add(log);
                db.SaveChanges();
                return entity;
            }
        }


        public void NotifyTestResult(TestResultDto testResult)
        {
            var taskDto = this.GetTaskById(testResult.TaskId);
            var toIds = new List<int>();
            var ccIds = new List<int>();
            var notify = new QANotifyMail()
            {
                TaskName = taskDto.TaskName,
                TestUser = UserHelper.CurrUserInfo.NickName,
                StageName = testResult.Stage.ToDescription(),
                Coments = testResult.Comments,
                TaskUrl = string.Format("{0}/Task/Detail?taskId={1}", _domainUrl.TrimEnd('/'), taskDto.Id),
                IsAllPassed = false
            };

            toIds.Add(taskDto.CreatorId.Value);

            switch (testResult.Stage)
            {
                case StageEnum.TEST:
                    foreach (var user in taskDto.DeployInfoIocDto.CheckUser.Where(t => t.UserId != UserHelper.CurrUserInfo.UserId))
                    {
                        if (!toIds.Contains(user.UserId))
                        {
                            toIds.Add(user.UserId);
                        }
                    }
                    ccIds.AddRange(taskDto.DeployInfoIocDto.NoticeUser.Where(t => t.UserId != UserHelper.CurrUserInfo.UserId).Select(t => t.UserId));
                    notify.TestResult = taskDto.DeployInfoIocDto.CheckUser.FirstOrDefault(t => t.UserId == UserHelper.CurrUserInfo.UserId).QAStatus.ToDescription();
                    notify.TestUrl = taskDto.DeployInfoIocDto.Domain;
                    notify.IsAllPassed = taskDto.DeployInfoIocDto.CheckUser.Count == taskDto.DeployInfoIocDto.CheckUser.Count(t => t.QAStatus == QAStatus.Passed);
                    break;
                case StageEnum.PRE:
                    foreach (var user in taskDto.DeployInfoPreDto.CheckUser.Where(t => t.UserId != UserHelper.CurrUserInfo.UserId))
                    {
                        if (!toIds.Contains(user.UserId))
                        {
                            toIds.Add(user.UserId);
                        }
                    }
                    ccIds.AddRange(taskDto.DeployInfoPreDto.NoticeUser.Where(t => t.UserId != UserHelper.CurrUserInfo.UserId).Select(t => t.UserId));
                    notify.TestResult = taskDto.DeployInfoPreDto.CheckUser.FirstOrDefault(t => t.UserId == UserHelper.CurrUserInfo.UserId).QAStatus.ToDescription();
                    notify.TestUrl = taskDto.DeployInfoPreDto.Domain;
                    notify.IsAllPassed = taskDto.DeployInfoPreDto.CheckUser.Count == taskDto.DeployInfoPreDto.CheckUser.Count(t => t.QAStatus == QAStatus.Passed);
                    break;
                case StageEnum.PRODUCTION:
                    foreach (var user in taskDto.DeployInfoOnlineDto.CheckUser.Where(t => t.UserId != UserHelper.CurrUserInfo.UserId))
                    {
                        if (!toIds.Contains(user.UserId))
                        {
                            toIds.Add(user.UserId);
                        }
                    }
                    ccIds.AddRange(taskDto.DeployInfoOnlineDto.NoticeUser.Where(t => t.UserId != UserHelper.CurrUserInfo.UserId).Select(t => t.UserId));
                    notify.TestResult = taskDto.DeployInfoOnlineDto.CheckUser.FirstOrDefault(t => t.UserId == UserHelper.CurrUserInfo.UserId).QAStatus.ToDescription();

                    notify.IsAllPassed = taskDto.DeployInfoOnlineDto.CheckUser.Count == taskDto.DeployInfoOnlineDto.CheckUser.Count(t => t.QAStatus == QAStatus.Passed);
                    using (var dbScope = _dbScopeFactory.CreateReadOnly())
                    {
                        var db = dbScope.DbContexts.Get<FireProjDbContext>();
                        var onlineTask = db.OnlineTaskInfos.Find(taskDto.DeployInfoOnlineDto.OnlineTaskId.Value);
                        if (onlineTask != null)
                        {
                            notify.TestUrl = onlineTask.Domain;
                        }
                    }
                    break;
            }

            MailSendHelper.NotifyTestResult(toIds, ccIds, notify);

        }

        public void NotifyDeployResult(int triggerId, int buildId, DeployStatus deployStatus)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();

                var taskLog = db.TaskLogs.Where(r => r.TriggeredId == triggerId).FirstOrDefault();
                if (taskLog == null)
                {
                    return;
                }
                var toIds = new List<int>();
                var ccIds = new List<int>();
                var notify = new DeployNotifyMail()
                {
                    StageName = taskLog.Stage.ToDescription(),
                    DeployStatus = deployStatus.ToDescription()
                };
                switch (taskLog.Stage)
                {
                    case StageEnum.TEST:
                        var taskDtoIoc = this.GetTaskById(taskLog.TaskId);
                        notify.TaskName = taskDtoIoc.TaskName;
                        notify.TaskUrl = string.Format("{0}/Task/Detail?taskId={1}", _domainUrl.TrimEnd('/'), taskDtoIoc.Id);

                        if (deployStatus == DeployStatus.DeployFails)
                        {
                            notify.GitLabBuildPage = taskDtoIoc.ProjectDto.ProjectRepo.Replace(".git", "") + "/builds/" + buildId;
                            if (taskDtoIoc.CreatorId != null)
                            {
                                toIds.Add(taskDtoIoc.CreatorId.Value);
                            }
                        }
                        else if (deployStatus == DeployStatus.DeploySuccess)
                        {
                            notify.DeployUrl = taskDtoIoc.DeployInfoIocDto.Domain;
                            toIds.Add(taskDtoIoc.CreatorId.Value);
                            toIds.AddRange(taskDtoIoc.DeployInfoIocDto.CheckUser.Where(t => t.UserId != taskDtoIoc.CreatorId.Value).Select(t => t.UserId));
                            ccIds.AddRange(taskDtoIoc.DeployInfoIocDto.NoticeUser.Select(t => t.UserId));
                        }

                        break;
                    case StageEnum.PRE:
                        var taskDtoPre = this.GetTaskById(taskLog.TaskId);
                        notify.TaskName = taskDtoPre.TaskName;
                        notify.TaskUrl = string.Format("{0}/Task/Detail?taskId={1}", _domainUrl.TrimEnd('/'), taskDtoPre.Id);

                        if (deployStatus == DeployStatus.DeployFails)
                        {
                            notify.GitLabBuildPage = taskDtoPre.ProjectDto.ProjectRepo.Replace(".git", "") + "/builds/" + buildId;
                            if (taskDtoPre.CreatorId != null)
                            {
                                toIds.Add(taskDtoPre.CreatorId.Value);
                            }
                        }
                        else if (deployStatus == DeployStatus.DeploySuccess)
                        {
                            notify.DeployUrl = taskDtoPre.DeployInfoPreDto.Domain;
                            toIds.Add(taskDtoPre.CreatorId.Value);
                            toIds.AddRange(taskDtoPre.DeployInfoPreDto.CheckUser.Where(t => t.UserId != taskDtoPre.CreatorId.Value).Select(t => t.UserId));
                            ccIds.AddRange(taskDtoPre.DeployInfoPreDto.NoticeUser.Select(t => t.UserId));
                        }

                        break;
                    case StageEnum.PRODUCTION:
                        var onlineTask = db.OnlineTaskInfos.FirstOrDefault(t => t.Id == taskLog.TaskId);

                        var taskInfos = db.TaskInfo.Where(t => t.OnlineTaskId == onlineTask.Id).ToList();
                        notify.TaskUrl = string.Format("{0}/Online/Detail?taskId={1}", _domainUrl.TrimEnd('/'), onlineTask.Id);
                        notify.TaskName = onlineTask.ProjectName;

                        var project = db.Project.Find(onlineTask.ProjectId);
                        if (deployStatus == DeployStatus.DeployFails)
                        {
                            notify.GitLabBuildPage = project.ProjectRepo.Replace(".git", "") + "/builds/" + buildId;
                            toIds.Add(onlineTask.CreatorId);
                        }
                        else if (deployStatus == DeployStatus.DeploySuccess)
                        {
                            notify.DeployUrl = onlineTask.Domain;
                            toIds.Add(onlineTask.CreatorId);

                            foreach (var task in taskInfos)
                            {
                                var onlineDeployInfo = JsonHelper.FromJson<DeployInfoOnline>(task.DeployInfoOnlineJson);
                                var checkUsers = AnalysisObj.AnalysisCheckUser(onlineDeployInfo.CheckUserId);

                                foreach (var user in checkUsers)
                                {
                                    if (!toIds.Contains(user.UserId))
                                    {
                                        toIds.Add(user.UserId);
                                    }
                                }
                                var noticeUsers = AnalysisObj.AnalysisNoticeUser(onlineDeployInfo.NoticeUserId);
                                foreach (var user in noticeUsers)
                                {
                                    if (!ccIds.Contains(user.UserId))
                                    {
                                        ccIds.Add(user.UserId);
                                    }
                                }
                            }
                        }
                        break;
                }
                var tTasklogsCount = db.TaskLogs.Count(t => t.TriggeredId == triggerId);
                if (taskLog.LogType != LogType.RollBack && tTasklogsCount <= 2) //避免重复,回滚不发送邮件
                {
                    MailSendHelper.NotifyDeployResult(toIds, ccIds, notify);
                }

            }
        }
        /// <summary>
        /// 获取测试过后的用户状态结果
        /// </summary>
        /// <param name="oldCheckUserId"></param>
        /// <param name="qaStatus"></param>
        /// <returns></returns>
        private string GetTestedNewCheckUserId(string oldCheckUserId, QAStatus qaStatus)
        {
            if (string.IsNullOrEmpty(oldCheckUserId))
            {
                return oldCheckUserId;
            }

            var userStatusIds = oldCheckUserId.Split(',');
            var newUserStatusIds = new List<string>();
            foreach (var userStatus in userStatusIds)
            {
                var userandstate = userStatus.Split('-')
                                             .Select(int.Parse)
                                             .ToList();

                if (UserHelper.CurrUserInfo.UserId ==userandstate[0])
                {
                    userandstate[1] = ((int) qaStatus);
                }
                newUserStatusIds.Add(string.Join("-", userandstate));
            }

            return string.Join(",", newUserStatusIds);
        }

        public IEnumerable<TaskDetailDto> CheckOnlineByProjectId(int projectId)
        {
            using (var dbScope = _dbScopeFactory.Create())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = from online in db.OnlineTaskInfos.AsQueryable()
                           join task in db.TaskInfo.AsQueryable() on online.Id equals task.OnlineTaskId
                           where online.ProjectId == projectId && online.DeployStatus != DeployStatus.DeploySuccess
                           select new TaskDetailDto
                           {
                               TaskName = task.TaskName,
                               OnlineTaskId = online.Id
                           };
                return data.ToList();
            }
        }
    }
}
