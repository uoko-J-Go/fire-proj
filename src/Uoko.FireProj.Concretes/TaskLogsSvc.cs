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
using Uoko.FireProj.Infrastructure.Extensions;
using Uoko.FireProj.Model;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.Concretes
{
    public class TaskLogsSvc : ITaskLogsSvc
    {
        private readonly IDbContextScopeFactory _dbScopeFactory;

        public TaskLogsSvc(IDbContextScopeFactory dbScopeFactory)
        {
            _dbScopeFactory = dbScopeFactory;
        }


        public void CreateTaskLogs(TaskLogs entity)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    db.TaskLogs.Add(entity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }


        public List<TaskLogsDto> GetTaskLogsByTaskId(int taskId)
        {
          
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var taskInfo = db.TaskInfo.Find(taskId);
                var taskIds = new List<int>();
                taskIds.Add(taskId);
                if (taskInfo != null && taskInfo.OnlineTaskId.HasValue)
                {
                    taskIds.Add(taskInfo.OnlineTaskId.Value);
                }
                var entitys = db.TaskLogs.Where(r => taskIds.Contains(r.TaskId)).OrderByDescending(r => r.Id).ToList();
                List<TaskLogsDto> data = new List<TaskLogsDto>();
                foreach (var item in entitys)
                {
                    TaskLogsDto taskLogsDto = new TaskLogsDto();
                    taskLogsDto.TaskId = item.TaskId;
                    taskLogsDto.Stage = item.Stage;
                    taskLogsDto.Comments = item.Comments;
                    taskLogsDto.LogType = item.LogType;
                    taskLogsDto.CreatorId = item.CreatorId;
                    taskLogsDto.CreatorName = item.CreatorName;
                    taskLogsDto.CreateDate = item.CreateDate;
                    
                    switch (item.Stage)
                    {
                        case StageEnum.IOC:
                            taskLogsDto.DeployInfoIocDto = !item.DeployInfo.IsNullOrEmpty() ? JsonHelper.FromJson<DeployInfoIocDto>(item.DeployInfo) : new DeployInfoIocDto();
                            var checkUsers = AnalysisUser.AnalysisCheckUser(taskLogsDto.DeployInfoIocDto.CheckUserId);
                            var currUser = checkUsers.FirstOrDefault(t => t.UserId == item.CreatorId);
                            if (currUser != null)
                            {
                                taskLogsDto.QAStatus = currUser.QAStatus;
                            }
                            taskLogsDto.BuildId = taskLogsDto.DeployInfoIocDto.BuildId;
                            taskLogsDto.DeployStatus = taskLogsDto.DeployInfoIocDto.DeployStatus;
                            break;
                        case StageEnum.PRE:
                            taskLogsDto.DeployInfoPreDto = !item.DeployInfo.IsNullOrEmpty() ? JsonHelper.FromJson<DeployInfoPreDto>(item.DeployInfo) : new DeployInfoPreDto();
                            var checkUsers1 = AnalysisUser.AnalysisCheckUser(taskLogsDto.DeployInfoPreDto.CheckUserId);
                            var currUser1 = checkUsers1.FirstOrDefault(t => t.UserId == item.CreatorId);
                            if (currUser1 != null)
                            {
                                taskLogsDto.QAStatus = currUser1.QAStatus;
                            }
                            taskLogsDto.BuildId = taskLogsDto.DeployInfoPreDto.BuildId;
                            taskLogsDto.DeployStatus = taskLogsDto.DeployInfoPreDto.DeployStatus;
                            break;
                        case StageEnum.PRODUCTION:
                            taskLogsDto.DeployInfoOnlineDto = !item.DeployInfo.IsNullOrEmpty() ? JsonHelper.FromJson<DeployInfoOnlineDto>(item.DeployInfo) : new DeployInfoOnlineDto();
                            var checkUsers2 = AnalysisUser.AnalysisCheckUser(taskLogsDto.DeployInfoOnlineDto.CheckUserId);
                            var currUser2 = checkUsers2.FirstOrDefault(t => t.UserId == item.CreatorId);
                            if (currUser2 != null)
                            {
                                taskLogsDto.QAStatus = currUser2.QAStatus;
                            }
                            if (item.LogType == LogType.Deploy)
                            {
                                var onlineTask = JsonHelper.FromJson<OnlineTaskInfo>(item.DeployInfo);
                                if (onlineTask != null)
                                {
                                   taskLogsDto.BuildId = onlineTask.BuildId;
                                   taskLogsDto.DeployStatus = onlineTask.DeployStatus; 
                                }
                                
                            }
                           
                            break;
                        default:
                            break;
                    }
                    data.Add(taskLogsDto);
                }
                return data;
            }
        }


        public void UpdateTaskLogs(TaskLogsDto dto)
        {
            //try
            //{
            //    var entity = Mapper.Map<TaskLogsDto, TaskLogs>(dto);
            //    using (var dbScope = _dbScopeFactory.Create())
            //    {
            //        var db = dbScope.DbContexts.Get<FireProjDbContext>();
            //        db.Update(entity, r => new { r.ModifyBy,r.TaskLogsType,r.LogsDesc,r.BuildId});
            //        db.SaveChanges();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new TipInfoException(ex.Message);
            //}
        }
        public TaskLogs GetTaskLogByTriggerId(int triggerId)
        {

            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.TaskLogs.Where(r => r.TriggeredId == triggerId).FirstOrDefault();
                return data;
            }
        }
        public PageGridData<TaskLogsDto> GetTaskLogsPage(TaskLogsQuery query)
        {
            return null;
            //using (var dbScope = _dbScopeFactory.CreateReadOnly())
            //{
            //    var db = dbScope.DbContexts.Get<FireProjDbContext>();

            //    var data1 = db.TaskLogs.Where(r => r.TaskId == query.TaskId);
            //    if (query.Stage.HasValue)
            //    {
            //        data1 = data1.Where(r => r.Stage == query.Stage.Value);
            //    }
            //    var data = data1.Select(r => new TaskLogsDto
            //    {
            //        Id = r.Id,
            //        LogsDesc = r.LogsDesc,
            //        LogsText = r.LogsText,
            //        TriggeredId = r.TriggeredId,
            //        BuildId = r.BuildId,
            //        TaskLogsType = r.TaskLogsType,
            //        TaskId = r.TaskId,
            //        CreateBy = r.CreateBy,
            //        CreateDate = r.CreateDate
            //    });
            //    //分页和不分页情况
            //    if (query.Limit == 0)
            //    {
            //        var result = data.OrderByDescending(r => r.Id).ToList();
            //        var total = data.Count();
            //        return new PageGridData<TaskLogsDto> { rows = result, total = total };
            //    }
            //    else
            //    {
            //        var result = data.OrderByDescending(r => r.Id).Skip(query.Offset).Take(query.Limit).ToList();
            //        var total = data.Count();
            //        return new PageGridData<TaskLogsDto> { rows = result, total = total };
            //    }
            //}
        }
        public int GetLogTotalByEnvironment(int taskId, StageEnum stage)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.TaskLogs.Where(t=>t.TaskId==taskId&&t.Stage==stage);
                var total = data.Count();
                return total;
            }
        }
    }
}
