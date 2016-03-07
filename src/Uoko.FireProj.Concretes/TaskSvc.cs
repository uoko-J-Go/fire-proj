using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Mehdime.Entity;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Gitlab;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;
using Uoko.FireProj.Infrastructure.Exception;
using Uoko.FireProj.Infrastructure.Extensions;
using Uoko.FireProj.Model;

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
                            var userIdsStatus =
                                userIds.Select(userId => string.Format("{0}-{1}", userId, (int) QAStatus.Waiting))
                                       .ToList();
                            taskInfo.IocCheckUserId =
                                taskDto.IocDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                        }
                        domain = taskDto.IocDeployInfo.Domain;
                        taskDto.IocDeployInfo.DeployStage = taskDto.DeployStage;
                        taskDto.IocDeployInfo.DeployAddress = string.Format("https://{0}:8172/msdeploy.axd",
                            taskDto.IocDeployInfo.DeployIP);
                        taskDto.IocDeployInfo.DeployStatus = DeployStatus.Deploying;
                        taskInfo.DeployInfoIocJson = DeployInfo = JsonHelper.ToJson(taskDto.IocDeployInfo);
                        break;
                    case StageEnum.PRE:
                        if (!string.IsNullOrEmpty(taskDto.PreDeployInfo.CheckUserId))
                        {
                            var userIds = taskDto.PreDeployInfo.CheckUserId.Split(',');
                            var userIdsStatus =
                                userIds.Select(userId => string.Format("{0}-{1}", userId, (int) QAStatus.Waiting))
                                       .ToList();
                            taskInfo.PreCheckUserId =
                                taskDto.PreDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                        }
                        domain = taskDto.PreDeployInfo.Domain;
                        taskDto.PreDeployInfo.DeployStage = taskDto.DeployStage;
                        taskDto.PreDeployInfo.DeployAddress = string.Format("https://{0}:8172/msdeploy.axd",
                            taskDto.PreDeployInfo.DeployIP);
                        taskDto.PreDeployInfo.DeployStatus = DeployStatus.Deploying;
                        taskInfo.DeployInfoPreJson = DeployInfo = JsonHelper.ToJson(taskDto.PreDeployInfo);


                        //添加或修改Online信息
                        if (!string.IsNullOrEmpty(taskDto.OnlineDeployInfo.CheckUserId))
                        {
                            var userIds = taskDto.OnlineDeployInfo.CheckUserId.Split(',');
                            var userIdsStatus =
                                userIds.Select(userId => string.Format("{0}-{1}", userId, (int) QAStatus.Waiting))
                                       .ToList();
                            taskInfo.OnlineCheckUserId =
                                taskDto.OnlineDeployInfo.CheckUserId = string.Join(",", userIdsStatus);
                        }
                        taskDto.OnlineDeployInfo.DeployStage = StageEnum.PRODUCTION;
                        taskInfo.DeployInfoOnlineJson = JsonHelper.ToJson(taskDto.OnlineDeployInfo);

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
                    entity.ModifyId = 2;
                    entity.ModifyDate = DateTime.Now;

                    #region 写日志

                    var log = new TaskLogs
                              {
                                  TaskId = entity.Id,
                                  LogType = LogType.Deploy,
                                  Stage = deployStage,
                                  TriggeredId = triggerId,
                                  CreateDate = DateTime.Now,
                                  CreatorId = 0,
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

                    #region MyRegion

                    //创建日志
                    var log = new TaskLogs
                              {
                                  TaskId = taskLog.TaskId,
                                  LogType = LogType.Deploy,
                                  Stage = taskLog.Stage,
                                  TriggeredId = triggerId,
                                  CreateDate = DateTime.Now,
                                  CreatorId = 0,
                                  CreatorName = "系统"
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
                    var tTasklogs = db.TaskLogs.Count(t => t.TriggeredId == triggerId);
                    if (tTasklogs < 2) //避免重复
                    {
                        db.TaskLogs.Add(log);
                    }

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

        /// <summary>
        /// 更新测试任务
        /// </summary>
        /// <param name="testResult"></param>
        /// <returns></returns>
        public TaskInfo UpdateTestStatus(TestResultDto testResult)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    var entity = db.TaskInfo.FirstOrDefault(r => r.Id == testResult.TaskId);

                    var currentUserId = 2; //临时模拟一个当前用户Id
                    //更改任务记录
                    switch (testResult.Stage)
                    {
                        case StageEnum.IOC:
                            var iocDeployInfo = JsonHelper.FromJson<DeployInfoIoc>(entity.DeployInfoIocJson);
                            if (!string.IsNullOrEmpty(iocDeployInfo.CheckUserId))
                            {
                                var userStatusIds = iocDeployInfo.CheckUserId.Split(',');
                                var newUserStatusIds = new List<string>();
                                foreach (var userStatus in userStatusIds)
                                {
                                    var userandstate = userStatus.Split('-');
                                    //暂时注释判断 修改所有测试结果
                                    //if (currentUserId.Equals(userandstate[0]))
                                    //{
                                    userandstate[1] = ((int) testResult.QAStatus).ToString();
                                    //}

                                    newUserStatusIds.Add(string.Join("-", userandstate));
                                }
                                entity.IocCheckUserId = iocDeployInfo.CheckUserId = string.Join(",", newUserStatusIds);
                            }
                            entity.DeployInfoIocJson = JsonHelper.ToJson(iocDeployInfo);
                            break;
                        case StageEnum.PRE:
                            var preDeployInfo = JsonHelper.FromJson<DeployInfoPre>(entity.DeployInfoPreJson);
                            if (!string.IsNullOrEmpty(preDeployInfo.CheckUserId))
                            {
                                var userStatusIds = preDeployInfo.CheckUserId.Split(',');
                                var newUserStatusIds = new List<string>();
                                foreach (var userStatus in userStatusIds)
                                {
                                    var userandstate = userStatus.Split('-');
                                    //暂时注释判断 修改所有测试结果
                                    //if (currentUserId.Equals(userandstate[0]))
                                    //{
                                    userandstate[1] = ((int) testResult.QAStatus).ToString();
                                    //}

                                    newUserStatusIds.Add(string.Join("-", userandstate));
                                }
                                entity.PreCheckUserId = preDeployInfo.CheckUserId = string.Join(",", newUserStatusIds);
                            }
                            entity.DeployInfoPreJson = JsonHelper.ToJson(preDeployInfo);
                            break;
                        case StageEnum.PRODUCTION:
                            var onlineDeployInfo = JsonHelper.FromJson<DeployInfoOnline>(entity.DeployInfoOnlineJson);
                            if (!string.IsNullOrEmpty(onlineDeployInfo.CheckUserId))
                            {
                                var userStatusIds = onlineDeployInfo.CheckUserId.Split(',');
                                var newUserStatusIds = new List<string>();
                                foreach (var userStatus in userStatusIds)
                                {
                                    var userandstate = userStatus.Split('-');
                                    //暂时注释判断 修改所有测试结果
                                    //if (currentUserId.Equals(userandstate[0]))
                                    //{
                                    userandstate[1] = ((int) testResult.QAStatus).ToString();
                                    //}

                                    newUserStatusIds.Add(string.Join("-", userandstate));
                                }
                                entity.OnlineCheckUserId =
                                    onlineDeployInfo.CheckUserId = string.Join(",", newUserStatusIds);
                            }
                            entity.DeployInfoOnlineJson = JsonHelper.ToJson(onlineDeployInfo);
                            break;
                    }
                    entity.ModifyId = currentUserId;
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
    }
}