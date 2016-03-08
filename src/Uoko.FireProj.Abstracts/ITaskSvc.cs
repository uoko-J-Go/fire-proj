using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.Abstracts
{
   public interface ITaskSvc
   {

        OnlineTaskInfo CreateOnlineTask(OnlineTaskInfo taskInfo);


        /// <summary>
        /// 新增任务
        /// </summary>
        /// <param name="task"></param>
        /// <returns>创建任务生成的任务Id</returns>
        int CreatTask(TaskWriteDto task);

        /// <summary>
        /// 任务更新
        /// </summary>
        /// <param name="task"></param>
        void UpdateTask(TaskWriteDto task);

        /// <summary>
        /// 根据任务id删除任务信息
        /// </summary>
        /// <param name="taskId"></param>
        void DeleteTask(int taskId);

       /// <summary>
       /// 基本的查询分页功能，提供支持 按 任务名称进行 查询过滤
       /// 默认按照 提交日期 倒排序
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
       PageGridData<TaskInfoForList> GetTaskPage(TaskQuery query);

        /// <summary>
        /// 根据任务Id获取任务信息详情
        /// </summary>
        /// <param name="taskId"></param>
        TaskDetailDto GetTaskById(int taskId);

        /// <summary>
        /// 开始部署
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="deployStage"></param>

        /// <returns></returns>
        TaskInfo BeginDeploy(int taskId, StageEnum deployStage);
        /// <summary>
        /// 部署回调
        /// </summary>
        /// <param name="triggerId"></param>
        /// <param name="buildId"></param>
        /// <param name="deployStatus"></param>
        /// <returns></returns>
        void DeployCallback( int triggerId,int buildId, DeployStatus deployStatus);

        TaskInfo UpdateTestStatus(TestResultDto testResult);

       PageGridData<TaskInfoForList> GetTasksNeedOnline(TaskNeedOnlineQuery query);

       void DeployOnlineTask(OnlineTaskInfo taskFromDb);
        /// <summary>
        /// 邮件通知测试结果
        /// </summary>
        /// <param name="testResult"></param>
      void NotifyTestResult(TestResultDto testResult);
        /// <summary>
        /// 邮件通知部署结果
        /// </summary>
        /// <param name="triggerId"></param>
        /// <param name="buildId"></param>
        /// <param name="deployStatus"></param>
        void NotifyDeployResult(int triggerId, int buildId, DeployStatus deployStatus);
    }
}
