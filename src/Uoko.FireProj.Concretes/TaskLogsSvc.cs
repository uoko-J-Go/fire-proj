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


        public void CreatTaskLogs(TaskLogsDto dto)
        {
            try
            {
                var entity = Mapper.Map<TaskLogsDto, TaskLogs>(dto);
                using (var dbScope = _dbScopeFactory.Create())
                {
                    entity.CreateBy = 1;
                    entity.CreateDate = DateTime.Now;
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
            return null;
            //using (var dbScope = _dbScopeFactory.CreateReadOnly())
            //{
            //    var db = dbScope.DbContexts.Get<FireProjDbContext>();
            //    var data = db.TaskLogs.Where(r => r.TaskId == taskId).Select(r => new TaskLogsDto()
            //    {
            //        Id = r.Id,
            //        LogsDesc = r.LogsDesc,
            //        LogsText = r.LogsText,
            //        TriggeredId = r.TriggeredId,
            //        BuildId=r.BuildId,
            //        TaskLogsType = r.TaskLogsType,
            //        TaskId = r.TaskId,
            //        CreateBy = r.CreateBy,
            //        CreateDate = r.CreateDate
            //    }).ToList();
            //    return data;
            //}
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
        public TaskLogsDto GetTaskLogByTriggerId(int triggerId)
        {
            return null;

            //using (var dbScope = _dbScopeFactory.CreateReadOnly())
            //{
            //    var db = dbScope.DbContexts.Get<FireProjDbContext>();
            //    var data = db.TaskLogs.Where(r => r.TriggeredId == triggerId).Select(r => new TaskLogsDto()
            //    {
            //        Id = r.Id,
            //        LogsDesc = r.LogsDesc,
            //        LogsText = r.LogsText,
            //        TriggeredId = r.TriggeredId,
            //        BuildId = r.BuildId,
            //        TaskId = r.TaskId,
            //        CreateBy = r.CreateBy,
            //        CreateDate = r.CreateDate,
            //        Stage = r.Stage,
            //        TaskLogsType = r.TaskLogsType,
            //    }).FirstOrDefault();
            //    return data;
            //}
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
