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
            task.ModifyId = UserHelp.userInfo.UserId;
            task.ModifierName = UserHelp.userInfo.NickName;
            _taskSvc.UpdateTask(task);
            //直接调用部署
            _taskSvc.BeginDeploy(task.Id, task.DeployStage);
            return Ok(task.Id);
        }

        [Route("Create")]
        [HttpPost]
        public IHttpActionResult Create([FromBody]TaskWriteDto task)
        {
            task.CreatorId = UserHelp.userInfo.UserId;
            task.CreatorName = UserHelp.userInfo.NickName;
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
            testResult.ModifyId = UserHelp.userInfo.UserId;
            testResult.CreatorName = UserHelp.userInfo.NickName;
            var taskInfo = _taskSvc.UpdateTestStatus(testResult);
            //创建日志
            var log = new TaskLogs
            {
                TaskId = taskInfo.Id,
                LogType = LogType.QA,
                Stage = testResult.Stage,
                Comments= testResult.Comments
            };
            switch (testResult.Stage)
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
            log.CreatorId = UserHelp.userInfo.UserId;
            log.CreatorName = UserHelp.userInfo.NickName;
            _taskLogsSvc.CreateTaskLogs(log);
            return Ok();
        }
    }
}
