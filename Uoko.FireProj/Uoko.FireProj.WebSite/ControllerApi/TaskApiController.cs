using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Query;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/TaskApi")]
    public class TaskApiController : BaseApiController
    {
        private ITaskSvc _taskSvc { get; set; }
        public TaskApiController(ITaskSvc taskSvc)
        {
            _taskSvc = taskSvc;
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
    }
}
