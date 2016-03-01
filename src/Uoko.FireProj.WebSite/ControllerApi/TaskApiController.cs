using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web.Http;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
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
        public IHttpActionResult Update([FromBody]TaskDto task)
        {
            _taskSvc.UpdateTask(task);
            return Ok();
        }

        [Route("Create")]
        [HttpPost]
        public IHttpActionResult Create([FromBody]TaskDto task)
        {
            _taskSvc.CreatTask(task);
            return Ok();
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
        public IHttpActionResult BeginDeploy(int taskId,int triggerId)
        {
            var task= _taskSvc.GetTaskById(taskId);
            _taskSvc.UpdateTaskStatus(new TaskDto() { Id = task.Id, Status = TaskStatusEnum.Deployment });
            _taskLogsSvc.CreatTaskLogs(new TaskLogsDto()
            {
                TaskId = taskId,
                TriggeredId = triggerId,
                CreateBy = 1,
                Stage= task.DeployStage,
                
            });
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
            task.Status = TaskEnum.TestFails;
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
            task.Status = TaskEnum.Tested;
            _taskSvc.UpdateTaskStatus(task);
            return Ok();
        }
    }
}
