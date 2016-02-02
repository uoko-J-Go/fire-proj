using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Query;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/TaskLogsApi")]
    public class TaskLogsApiController : BaseApiController
    {
        private ITaskLogsSvc _taskLogsSvc { get; set; }
        public TaskLogsApiController(ITaskLogsSvc taskLogsSvc)
        {
            _taskLogsSvc = taskLogsSvc;
        }

        /// <summary>
        /// 根据任务id获取记录详细信息列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/ByTaskId")]
        public IHttpActionResult GetTaskId(int id)
        {
            var result = _taskLogsSvc.GetTaskLogsByTaskId(id);
            return Ok(result);
        }

        [Route("LogTotal/{taskId}")]
        public IHttpActionResult GetLogTotal(int taskId)
        {
            var result = new
            {
                IocTotal= _taskLogsSvc.GetLogTotalByEnvironment(taskId,EnvironmentEnum.IOC),
                PreTotal = _taskLogsSvc.GetLogTotalByEnvironment(taskId, EnvironmentEnum.PRE),
                ProductionTotal = _taskLogsSvc.GetLogTotalByEnvironment(taskId, EnvironmentEnum.PRODUCTION),
            };
            return Ok(result);
        }
        /// <summary>
        /// 获取任务记录分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]TaskLogsQuery query)
        {
            var result = _taskLogsSvc.GetTaskLogsPage(query);
            return Ok(result);
        }

        /// <summary>
        /// 新增任务记录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody]TaskLogsDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            _taskLogsSvc.CreatTaskLogs(dto);
            return Ok();
        }
    }
}
