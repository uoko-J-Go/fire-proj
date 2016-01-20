using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.Abstracts
{
   public interface ITaskSvc
    {
        /// <summary>
        /// 新增任务
        /// </summary>
        /// <param name="task"></param>
        void CreatTask(TaskDto task);

        /// <summary>
        /// 任务编辑
        /// </summary>
        /// <param name="task"></param>
        void EditTask(TaskDto task);

        /// <summary>
        /// 根据任务id删除任务信息
        /// </summary>
        /// <param name="taskId"></param>
        void DeleteTask(int taskId);

        /// <summary>
        /// 获取任务分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        PageGridData<TaskDto> GetTaskPage(TaskQuery query);

        /// <summary>
        /// 根据任务Id获取任务信息详情
        /// </summary>
        /// <param name="taskId"></param>
        TaskDto GetTaskById(int taskId);
    }
}
