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
            query.LoginUserId = 1;// this.User.Identity.GetUserId<int>();
            var result = _taskSvc.GetTaskPage(query);
            return Ok(result);
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
            _taskSvc.UpdateTask(task);
            return Ok();
        }

        [Route("Create")]
        [HttpPost]
        public IHttpActionResult Create([FromBody]TaskWriteDto task)
        {
            var taskId=_taskSvc.CreatTask(task);
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
        /// 根据id修改任务状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Put")]
        [HttpPost]
        public IHttpActionResult Put([FromBody]TaskDto task)
        {
            _taskSvc.UpdateTaskStatus(task);
            return Ok();
        }
        /// <summary>
        /// 开始部署
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="triggerId"></param>
        /// <returns></returns>
        [Route("BeginDeploy")]
        [HttpPost]
        public IHttpActionResult BeginDeploy(int taskId,StageEnum deployStage, int triggerId)
        {
           var taskInfo=_taskSvc.BeginDeploy(taskId, deployStage, triggerId);
            //创建日志
            var log = new TaskLogs
            {
                TaskId = taskInfo.Id,
                LogType = LogType.Deploy,
                Stage = deployStage,
                TriggeredId= triggerId
            };
            switch (deployStage)
            {
                case StageEnum.IOC:
                    log.DeployInfo = taskInfo.DeployInfoIocJson;
                    break;
                case StageEnum.PRE:
                    log.DeployInfo = taskInfo.DeployInfoPreJson;
                    break;
                case StageEnum.PRODUCTION:
                    log.DeployInfo = taskInfo.DeployInfoOnlineJson;
                    break;
            }
            _taskLogsSvc.CreateTaskLogs(log);
            return Ok();
        }

        /* 
        
            暂时去除提测步骤


        /// <summary>
        /// 提交测试动作
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("CommitToTest")]
        [HttpPost]
        public IHttpActionResult CommitToTest([FromBody]TaskDto task)
        {
            task.Status = TaskEnum.Testing;
            _taskSvc.UpdateTaskStatus(task);
            return Ok();
        }

        */

        /// <summary>
        /// 测试不通过动作
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("TestFails")]
        [HttpPost]
        public IHttpActionResult TestFails([FromBody]TaskDto task)
        {
            task.QAStatus = QAStatus.Refused;
            _taskSvc.UpdateTaskStatus(task);
            return Ok();
        }

        /// <summary>
        /// 测试通过动作
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        [Route("Tested")]
        [HttpPost]
        public IHttpActionResult Tested([FromBody]TaskDto task)
        {
            task.QAStatus = QAStatus.Passed;
            _taskSvc.UpdateTaskStatus(task);
            return Ok();
        }
    }
}
