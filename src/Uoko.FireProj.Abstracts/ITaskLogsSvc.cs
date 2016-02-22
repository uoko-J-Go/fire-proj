using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.Abstracts
{
    /// <summary>
    /// 任务记录
    /// </summary>
    public interface ITaskLogsSvc
    {
        /// <summary>
        /// 新增任务记录
        /// </summary>
        /// <param name="dto"></param>
        void CreatTaskLogs(TaskLogsDto dto);

        /// <summary>
        /// 更新任务记录
        /// </summary>
        /// <param name="dto"></param>
        void UpdateTaskLogs(TaskLogsDto dto);
        /// <summary>
        /// 根据TrriggerId获取任务记录
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        TaskLogsDto GetTaskLogByTriggerId(int triggerId);
        /// <summary>
        /// 获取任务记录分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        PageGridData<TaskLogsDto> GetTaskLogsPage(TaskLogsQuery query);

        /// <summary>
        /// 根据任务Id获取任务记录信息列表
        /// </summary>
        /// <param name="taskId"></param>
        List<TaskLogsDto> GetTaskLogsByTaskId(int taskId);
       /// <summary>
       /// 获取某个任务在某个环境下的任务记录数
       /// </summary>
       /// <param name="taskId"></param>
       /// <param name="environment"></param>
       /// <returns></returns>
        int GetLogTotalByEnvironment(int taskId,EnvironmentEnum environment);
    }
}
