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
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;
using Uoko.FireProj.Infrastructure.Exception;
using Uoko.FireProj.Infrastructure.Extensions;
using Uoko.FireProj.Model;

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
                var data = db.TaskLogs.Where(r => r.TaskId == taskId).Select(r => new TaskLogsDto()
                {
                    Id = r.Id,
                    LogsDesc = r.LogsDesc,
                    LogsText = r.LogsText,
                    TriggeredId = r.TriggeredId,
                    TaskLogsType = r.TaskLogsType.ToDescription(),
                    TaskId = r.TaskId,
                    CreateBy = r.CreateBy,
                    CreateDate = r.CreateDate
                }).ToList();
                return data;
            }
        }

        public PageGridData<TaskLogsDto> GetTaskLogsPage(TaskLogsQuery query)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.TaskLogs.Where(r => r.TaskId == query.TaskId).Select(r => new TaskLogsDto
                {
                    Id = r.Id,
                    LogsDesc = r.LogsDesc,
                    LogsText = r.LogsText,
                    TriggeredId = r.TriggeredId,
                    TaskLogsType = r.TaskLogsType.ToDescription(),
                    TaskId = r.TaskId,
                    CreateBy = r.CreateBy,
                    CreateDate = r.CreateDate
                });
                var result = data.OrderBy(r => r.Id).Skip(query.Offset).Take(query.Limit).ToList();
                var total = data.Count();
                return new PageGridData<TaskLogsDto> { rows = result, total = total };
            }
        }
    }
}
