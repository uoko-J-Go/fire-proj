using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/TaskApi")]
    public class TaskApiController : BaseApiController
    {
        private ITaskSvc _taskSvc { get; set; }
        private ITaskLogsSvc _taskLogsSvc { get; set; }

        private IDomainResourceSvc _domainResourceSvc { get; set; }
        public TaskApiController(ITaskSvc taskSvc, ITaskLogsSvc taskLogsSvc, IDomainResourceSvc domainResourceSvc)
        {
            _taskSvc = taskSvc;
            _taskLogsSvc = taskLogsSvc;
            _domainResourceSvc = domainResourceSvc;
        }

        public IHttpActionResult Get([FromUri]TaskQuery query)
        {
            query.LoginUserId = UserHelper.CurrUserInfo.UserId;
            var result = _taskSvc.GetTaskPage(query);
            return Ok(result);
        }
        [Route("OnlineTasksRollbackAble/{projectId}/{serverId}")]
        public IHttpActionResult GetOnlineTaskRollbackAble(int projectId, int serverId)
        {
            var result = _taskSvc.GetOnlineTaskRollbackAble(projectId, serverId);
            return Ok(result);
        }
        [Route("Rollback")]
        [HttpPost]
        public IHttpActionResult Rollback([FromBody] RollbackTaskInfo rollbackTask)
        {
            // 创建完成回滚任务
            var taskFromDb = _taskSvc.CreateRollbackTask(rollbackTask);
            // 进行任务的部署
            _taskSvc.DeployRollbackTask(taskFromDb);

            return Ok();
        }
        [Route("tasksNeedOnline/{projectId}")]
        public IHttpActionResult GetTasksNeedToBeOnline(int projectId)
        {
            var result = _taskSvc.GetTasksNeedOnline(projectId);
            return Ok(result);
        }

        [Route("OnlineTaskDetail/{onlineTaskId}")]
        public IHttpActionResult GetOnlineTaskDetail(int onlineTaskId)
        {
            var result = _taskSvc.GetOnlineTaskDetail(onlineTaskId);
            return Ok(result);
        }



        [Route("RetryDeployOnline/{onlineTaskId}")]
        public IHttpActionResult RetryDeployOnline(int onlineTaskId)
        {
            _taskSvc.ReDeployOnlineTask(onlineTaskId);
            return Ok();
        }


        [Route("DeployOnline")]
        public IHttpActionResult DeployOnlineTask([FromBody] OnlineTaskInfo onlineTask)
        {
            onlineTask.CreateDate = DateTime.Now;
            onlineTask.CreatorId = UserHelper.CurrUserInfo.UserId;
            onlineTask.CreatorName = UserHelper.CurrUserInfo.NickName;

            // 创建完成上线任务以后
            var taskFromDb = _taskSvc.CreateOnlineTask(onlineTask);

            // 进行任务的部署
            _taskSvc.DeployOnlineTask(taskFromDb);

            return Ok();
        }

        [Route("{taskId}")]
        public IHttpActionResult Get(int taskId)
        {
            var result = _taskSvc.GetTaskById(taskId);
            return Ok(result);
        }
        [Route("Update")]
        [HttpPost]
        public IHttpActionResult Update([FromBody]TaskWriteDto task)
        {
            task.ModifyId = UserHelper.CurrUserInfo.UserId;
            task.ModifierName = UserHelper.CurrUserInfo.NickName;
            _taskSvc.UpdateTask(task);
            //直接调用部署
            _taskSvc.BeginDeploy(task.Id, task.DeployStage);
            return Ok(task.Id);
        }

        [Route("Create")]
        [HttpPost]
        public IHttpActionResult Create([FromBody]TaskWriteDto task)
        {
            task.CreatorId = UserHelper.CurrUserInfo.UserId;
            task.CreatorName = UserHelper.CurrUserInfo.NickName;
            var taskId=_taskSvc.CreatTask(task);
            //直接调用部署
            var taskInfo = _taskSvc.BeginDeploy(taskId, task.DeployStage);
            return Ok(taskId);
        }

        /// <summary>
        /// 根据id获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/ById")]
        public IHttpActionResult GetById(int id)
        {
            var result = _taskSvc.GetTaskById(id);
            return Ok(result);
        }

        /// <summary>
        /// 开始部署
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="triggerId"></param>
        /// <returns></returns>
        [Route("BeginDeploy")]
        [HttpPost]
        public IHttpActionResult BeginDeploy(int taskId, StageEnum deployStage)
        {
            var taskInfo = _taskSvc.BeginDeploy(taskId, deployStage);
            return Ok();
        }



        /// <summary>
        /// 更新测试结果
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("UpdateTestStatus")]
        [HttpPost]
        public IHttpActionResult UpdateTestStatus([FromBody]TestResultDto testResult)
        {
            var taskInfo = _taskSvc.UpdateTestStatus(testResult);
            _taskSvc.NotifyTestResult(testResult);
            return Ok();
        }

        /// <summary>
        /// 根据项目Id查询 同项目下已上线任务中未部署成功的
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [Route("{projectId}/CheckOnlineByProjectId")]
        [HttpGet]
        public IHttpActionResult CheckOnlineByProjectId(int projectId)
        {
            var taskInfo = _taskSvc.CheckOnlineByProjectId(projectId);
            return Ok(taskInfo);
        }
    }
}
