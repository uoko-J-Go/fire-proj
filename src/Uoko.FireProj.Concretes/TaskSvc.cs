using System;
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
        public TaskSvc(IDbContextScopeFactory dbScopeFactory,IProjectSvc projectSvc,IServerSvc serverSvc)
        {
            _dbScopeFactory = dbScopeFactory;
            _projectSvc = projectSvc;
            _serverSvc = serverSvc;
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
            var gitlabToken = "D3MR_rnRZK4xWS-CtVho";
            var gitLabApi = new WebApiProvider("http://gitlab.uoko.ioc:12015/api/v3/");

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

            var onlineTagName = string.Format("{0}-{1}", onlineTaskInfo.OnlineVersion, onlineTaskInfo.Id);
            var buildInfo = new Dictionary<string, object>
                            {
                                {"slnFile", project.ProjectSlnName},
                                {"csProjFile", project.ProjectCsprojName},
                                {"iisSiteName", onlineTaskInfo.SiteName},
                                {"pkgDir", deployServer.PackageDir},
                                {"msDeployUrl", "https://" + onlineTaskInfo.DeployServerIP + ":8172/msdeploy.axd"},
                                {"useConfig", "Release"},
                                {"Target", "Online"},
                                {"mergeFromBranch", "pre"},
                                {"onlineTagName", onlineTagName}
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
                        LogType = LogType.Deploy,
                        Stage = StageEnum.PRODUCTION,
                        TriggeredId = triggerId,
                        CreateDate = DateTime.Now,
                        CreatorId = UserHelper.CurrUserInfo.UserId,
                        CreatorName = UserHelper.CurrUserInfo.NickName,
                        DeployInfo=JsonHelper.ToJson(taskFromDb)
                    };
                    db.TaskLogs.Add(log);
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
                case StageEnum.IOC:
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

                //更新域名资源使用
                var resourceInfo =
                    db.DomainResource.FirstOrDefault(r => r.ProjectId == taskDto.ProjectId && r.Name == domain);
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
                taskInfo.ModifyDate = DateTime.Now;
                taskInfo.ModifyId = UserHelper.CurrUserInfo.UserId;
                taskInfo.ModifierName = UserHelper.CurrUserInfo.NickName;
                var domain = string.Empty;
                switch (taskDto.DeployStage)
                {
                    case StageEnum.IOC:
                        if (!string.IsNullOrEmpty(taskDto.IocDeployInfo.CheckUserId))
                        {
                            var userIds = taskDto.IocDeployInfo.CheckUserId.Split(',');
                            var userIdsStatus =userIds.Select(userId => string.Format("{0}-{1}", userId, (int) QAStatus.Waiting)).ToList();
                            taskInfo.IocCheckUserId =taskDto.IocDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                        }
                        domain = taskDto.IocDeployInfo.Domain;
                        taskDto.IocDeployInfo.DeployStage = taskDto.DeployStage;
                        taskDto.IocDeployInfo.DeployAddress = string.Format("https://{0}:8172/msdeploy.axd",taskDto.IocDeployInfo.DeployIP);
                        taskDto.IocDeployInfo.DeployStatus = DeployStatus.Deploying;
                        taskInfo.DeployInfoIocJson  = JsonHelper.ToJson(taskDto.IocDeployInfo);
                        break;
                    case StageEnum.PRE:
                        if (!string.IsNullOrEmpty(taskDto.PreDeployInfo.CheckUserId))
                        {
                            var userIds = taskDto.PreDeployInfo.CheckUserId.Split(',');
                            var userIdsStatus =userIds.Select(userId => string.Format("{0}-{1}", userId, (int) QAStatus.Waiting)).ToList();
                            taskInfo.PreCheckUserId =taskDto.PreDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                        }
                        domain = taskDto.PreDeployInfo.Domain;
                        taskDto.PreDeployInfo.DeployStage = taskDto.DeployStage;
                        taskDto.PreDeployInfo.DeployAddress = string.Format("https://{0}:8172/msdeploy.axd",taskDto.PreDeployInfo.DeployIP);
                        taskDto.PreDeployInfo.DeployStatus = DeployStatus.Deploying;
                        taskInfo.DeployInfoPreJson  = JsonHelper.ToJson(taskDto.PreDeployInfo);


                        //添加或修改Online信息
                        if (!string.IsNullOrEmpty(taskDto.OnlineDeployInfo.CheckUserId))
                        {
                            var userIds = taskDto.OnlineDeployInfo.CheckUserId.Split(',');
                            var userIdsStatus =userIds.Select(userId => string.Format("{0}-{1}", userId, (int) QAStatus.Waiting)).ToList();
                            taskInfo.OnlineCheckUserId =taskDto.OnlineDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                        }
                        taskDto.OnlineDeployInfo.DeployStage = StageEnum.PRODUCTION;
                        taskDto.OnlineDeployInfo.OnlineTaskId = null;
                        taskInfo.DeployInfoOnlineJson = JsonHelper.ToJson(taskDto.OnlineDeployInfo);
                        taskInfo.OnlineTaskId = null;
                        break;
                    default:
                        throw new NotSupportedException("暂不支持其他阶段");
                }


                //更新域名资源使用:根据环境直接更新本次占用,同时清空上次IOC环境占用的
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
                var lastResourceInfo =
                    db.DomainResource.FirstOrDefault(r => r.ProjectId == taskDto.ProjectId && r.Name == lastDomain);
                if (lastResourceInfo != null
                    && lastDomain != domain)
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
                taskDto.DeployInfoIocDto.CheckUser = AnalysisUser.AnalysisCheckUser(taskDto.DeployInfoIocDto.CheckUserId);
                taskDto.DeployInfoIocDto.NoticeUser =
                    AnalysisUser.AnalysisNoticeUser(taskDto.DeployInfoIocDto.NoticeUserId);

                taskDto.DeployInfoPreDto.CheckUser = AnalysisUser.AnalysisCheckUser(taskDto.DeployInfoPreDto.CheckUserId);
                taskDto.DeployInfoPreDto.NoticeUser =
                    AnalysisUser.AnalysisNoticeUser(taskDto.DeployInfoPreDto.NoticeUserId);

                taskDto.DeployInfoOnlineDto.CheckUser =
                    AnalysisUser.AnalysisCheckUser(taskDto.DeployInfoOnlineDto.CheckUserId);
                taskDto.DeployInfoOnlineDto.NoticeUser =
                    AnalysisUser.AnalysisNoticeUser(taskDto.DeployInfoOnlineDto.NoticeUserId);

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
        public PageGridData<TaskInfoForList> GetTasksNeedOnline(TaskNeedOnlineQuery query)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();

                // 已经上线过 pre，但未上线的任务
                var taskToBeOnlineQuery = db.TaskInfo
                                            .Where(task => task.ProjectId == query.ProjectId
                                                           && task.OnlineTaskId == null
                                                           && task.DeployInfoPreJson != null);
                var total = taskToBeOnlineQuery.Count();

                var tasksFromDb = taskToBeOnlineQuery.OrderByDescending(task => task.Id)
                                                     .Skip(query.Offset)
                                                     .Take(query.Limit)
                                                     .ToList();

                var tasksToBeOnline = TransferTask(tasksFromDb);
                return new PageGridData<TaskInfoForList> {rows = tasksToBeOnline, total = total};
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
            try
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
                var _ref = taskDto.Branch;
                switch (deployStage)
                {
                    case StageEnum.IOC:
                        target = "Deploy-To-IOC";
                        iisSiteName = taskDto.DeployInfoIocDto.SiteName;
                        deployIP = taskDto.DeployInfoIocDto.DeployIP;
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
                        using (var dbScope = _dbScopeFactory.CreateReadOnly())
                        {
                            var db = dbScope.DbContexts.Get<FireProjDbContext>();
                            var server = db.Servers.FirstOrDefault(r => r.IP.Equals(deployIP));
                            packagDir = server.PackageDir;
                        }
                        _ref = "master";
                        break;
                }
  
                Hashtable buildInfo = new Hashtable();
                buildInfo.Add("slnFile", taskDto.ProjectDto.ProjectSlnName);
                buildInfo.Add("csProjFile", taskDto.ProjectDto.ProjectCsprojName);
                buildInfo.Add("iisSiteName", iisSiteName);
                buildInfo.Add("pkgDir", packagDir);
                buildInfo.Add("msDeployUrl", "https://" + deployIP + ":8172/msdeploy.axd");
                buildInfo.Add("useConfig", "Release");
                buildInfo.Add("Target", target);
                buildInfo.Add("mergeFromBranch", taskDto.Branch);
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
                        case StageEnum.IOC:
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

                    #endregion

                    db.SaveChanges();
                    return entity;
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public void DeployCallback(int triggerId, int buildId, DeployStatus deployStatus)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();

                    var taskLog = db.TaskLogs.Where(r => r.TriggeredId == triggerId).FirstOrDefault();
                    if (taskLog == null)
                    {
                        return ;
                    }
                    //日志内容
                    var log = new TaskLogs
                    {
                        TaskId = taskLog.TaskId,
                        LogType = LogType.Deploy,
                        Stage = taskLog.Stage,
                        TriggeredId = triggerId,
                        CreateDate = DateTime.Now,
                        CreatorId = 0,
                        CreatorName = "系统",
                    };

                    //更改任务记录
                    switch (taskLog.Stage)
                    {
                        case StageEnum.IOC:
                            var entityIoc = db.TaskInfo.FirstOrDefault(r => r.Id == taskLog.TaskId);
                            if (entityIoc == null) return;
                            var iocDeployInfo = JsonHelper.FromJson<DeployInfoIoc>(entityIoc.DeployInfoIocJson);
                            iocDeployInfo.DeployStatus = deployStatus;
                            iocDeployInfo.BuildId = buildId;
                            entityIoc.DeployInfoIocJson = JsonHelper.ToJson(iocDeployInfo);
                            entityIoc.ModifyId = 0;
                            entityIoc.ModifierName = "系统";
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
                            entityPre.ModifyId = 0;
                            entityPre.ModifierName = "系统";
                            entityPre.ModifyDate = DateTime.Now;
                            log.DeployInfo = entityPre.DeployInfoPreJson;
                            break;
                        case StageEnum.PRODUCTION:
                            var entityOnline = db.OnlineTaskInfos.FirstOrDefault(r => r.Id == taskLog.TaskId);
                            if (entityOnline == null) return;
                            entityOnline.DeployStatus = deployStatus;
                            entityOnline.BuildId = buildId;
                            entityOnline.ModifyId = 0;
                            entityOnline.ModifierName = "系统";
                            entityOnline.ModifyDate = DateTime.Now;
                            log.DeployInfo = JsonHelper.ToJson(entityOnline);
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
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
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
                    case StageEnum.IOC:
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
            try
            {
                var taskDto = this.GetTaskById(testResult.TaskId);
                var toIds = new List<int>();
                var ccIds = new List<int>();
                var notify = new QANotifyMail()
                {
                    TaskName=taskDto.TaskName,
                    TestUser=UserHelper.CurrUserInfo.NickName,
                    StageName = testResult.Stage.ToDescription(),
                    Coments = testResult.Comments,
                    TaskUrl=string.Format("{0}/Task/Detail?taskId={1}",_domainUrl.TrimEnd('/'),taskDto.Id),
                    IsAllPassed=false
                };
                if (UserHelper.CurrUserInfo.UserId != taskDto.CreatorId.Value)
                {
                    toIds.Add(taskDto.CreatorId.Value);
                }
                switch (testResult.Stage)
                {
                    case StageEnum.IOC:
                        foreach (var user in taskDto.DeployInfoIocDto.CheckUser.Where(t=>t.Id!= UserHelper.CurrUserInfo.UserId))
                        {
                            if (!toIds.Contains(user.Id))
                            {
                                toIds.Add(user.Id);
                            }
                        }
                        ccIds.AddRange(taskDto.DeployInfoIocDto.NoticeUser.Where(t => t.Id != UserHelper.CurrUserInfo.UserId).Select(t => t.Id));
                        notify.TestResult =taskDto.DeployInfoIocDto.CheckUser.FirstOrDefault(t => t.Id == UserHelper.CurrUserInfo.UserId).QAStatus.ToDescription();
                        notify.TestUrl = string.Format("http://{0}", taskDto.DeployInfoIocDto.Domain);
                        notify.IsAllPassed = taskDto.DeployInfoIocDto.CheckUser.Count ==taskDto.DeployInfoIocDto.CheckUser.Count(t => t.QAStatus == QAStatus.Passed);
                        break;
                    case StageEnum.PRE:
                        foreach (var user in taskDto.DeployInfoPreDto.CheckUser.Where(t => t.Id != UserHelper.CurrUserInfo.UserId))
                        {
                            if (!toIds.Contains(user.Id))
                            {
                                toIds.Add(user.Id);
                            }
                        }
                        ccIds.AddRange(taskDto.DeployInfoPreDto.NoticeUser.Where(t => t.Id != UserHelper.CurrUserInfo.UserId).Select(t => t.Id));
                        notify.TestResult = taskDto.DeployInfoPreDto.CheckUser.FirstOrDefault(t => t.Id == UserHelper.CurrUserInfo.UserId).QAStatus.ToDescription();
                        notify.TestUrl = string.Format("http://{0}", taskDto.DeployInfoPreDto.Domain);
                        notify.IsAllPassed = taskDto.DeployInfoPreDto.CheckUser.Count == taskDto.DeployInfoPreDto.CheckUser.Count(t => t.QAStatus == QAStatus.Passed);
                        break;
                    case StageEnum.PRODUCTION:
                        foreach (var user in taskDto.DeployInfoOnlineDto.CheckUser.Where(t => t.Id != UserHelper.CurrUserInfo.UserId))
                        {
                            if (!toIds.Contains(user.Id))
                            {
                                toIds.Add(user.Id);
                            }
                        }
                        ccIds.AddRange(taskDto.DeployInfoOnlineDto.NoticeUser.Where(t => t.Id != UserHelper.CurrUserInfo.UserId).Select(t => t.Id));
                        notify.TestResult = taskDto.DeployInfoOnlineDto.CheckUser.FirstOrDefault(t => t.Id == UserHelper.CurrUserInfo.UserId).QAStatus.ToDescription();
                       
                        notify.IsAllPassed = taskDto.DeployInfoOnlineDto.CheckUser.Count == taskDto.DeployInfoOnlineDto.CheckUser.Count(t => t.QAStatus == QAStatus.Passed);
                        using (var dbScope = _dbScopeFactory.CreateReadOnly())
                        {
                            var db = dbScope.DbContexts.Get<FireProjDbContext>();
                            var onlineTask = db.OnlineTaskInfos.Find(taskDto.DeployInfoOnlineDto.OnlineTaskId.Value);
                            if (onlineTask != null)
                            {
                                notify.TestUrl = string.Format("http://{0}", onlineTask.Domain);
                            }
                        }
                        break;
                }

                MailSendHelper.NotifyTestResult(toIds, ccIds, notify);
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public void NotifyDeployResult(int triggerId, int buildId, DeployStatus deployStatus)
        {
            try
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
                        case StageEnum.IOC:
                            var taskDtoIoc = this.GetTaskById(taskLog.TaskId);
                            notify.TaskName = taskDtoIoc.TaskName;
                            notify.TaskUrl = string.Format("{0}/Task/Detail?taskId={1}", _domainUrl.TrimEnd('/'),taskDtoIoc.Id);
                             
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
                                notify.DeployUrl = string.Format("http://{0}", taskDtoIoc.DeployInfoIocDto.Domain);
                                toIds.Add(taskDtoIoc.CreatorId.Value);
                                toIds.AddRange(taskDtoIoc.DeployInfoIocDto.CheckUser.Where(t => t.Id != taskDtoIoc.CreatorId.Value).Select(t=>t.Id));
                                ccIds.AddRange(taskDtoIoc.DeployInfoIocDto.NoticeUser.Select(t => t.Id));
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
                                notify.DeployUrl = string.Format("http://{0}", taskDtoPre.DeployInfoPreDto.Domain);
                                toIds.Add(taskDtoPre.CreatorId.Value);
                                toIds.AddRange(taskDtoPre.DeployInfoPreDto.CheckUser.Where(t => t.Id != taskDtoPre.CreatorId.Value).Select(t => t.Id));
                                ccIds.AddRange(taskDtoPre.DeployInfoPreDto.NoticeUser.Select(t => t.Id));
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
                                notify.DeployUrl = string.Format("http://{0}", onlineTask.Domain);
                                toIds.Add(onlineTask.CreatorId);

                                foreach (var task in taskInfos)
                                {
                                    var onlineDeployInfo =JsonHelper.FromJson<DeployInfoOnline>(task.DeployInfoOnlineJson);
                                    var checkUsers = AnalysisUser.AnalysisCheckUser(onlineDeployInfo.CheckUserId);

                                    foreach (var user in checkUsers)
                                    {
                                        if (!toIds.Contains(user.Id))
                                        {
                                            toIds.Add(user.Id);
                                        }
                                    }
                                    var noticeUsers = AnalysisUser.AnalysisCheckUser(onlineDeployInfo.NoticeUserId);
                                    foreach (var user in noticeUsers)
                                    {
                                        if (!toIds.Contains(user.Id))
                                        {
                                            ccIds.Add(user.Id);
                                        }
                                    }
                                }
                            }
                            break;
                    }
                    MailSendHelper.NotifyDeployResult(toIds, ccIds, notify);
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
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
    }
}
