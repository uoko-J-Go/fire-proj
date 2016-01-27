using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Gitlab;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/WebHookApi")]
    public class WebHookApiController : BaseApiController
    {
        private ITaskSvc _taskSvc { get; set; }
        private ITaskLogsSvc _taskLogsSvc { get; set; }
        public WebHookApiController(ITaskSvc taskSvc, ITaskLogsSvc taskLogsSvc)
        {
            _taskSvc = taskSvc;
            _taskLogsSvc = taskLogsSvc;
        }

        /// <summary>
        /// 触发编译回调
        /// </summary>
        /// <param name="bhRequest"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult BuildCallback(BuildHookRequest bhRequest)
        {
            var taskLog = _taskLogsSvc.GetTaskLogByTriggerId(bhRequest.trigger_request_id);
            if (!"build".Equals(bhRequest.object_kind, StringComparison.OrdinalIgnoreCase)|| bhRequest.trigger_request_id==0|| taskLog==null)
            {
                return Ok();
            }
                      
            //部署失败
            if ("failed".Equals(bhRequest.build_status, StringComparison.OrdinalIgnoreCase))
            {
                _taskSvc.UpdateTaskStatus(new TaskDto() {Id = taskLog.TaskId, Status = TaskEnum.DeployFails.ToString()});
                taskLog.LogsDesc = string.Format("在执行{0} 时出错,详情gitlab  builds:{1}", bhRequest.build_name,bhRequest.build_id);
                _taskLogsSvc.UpdateTaskLogs(taskLog);
            }
            //部署成功
            if ("success".Equals(bhRequest.build_status, StringComparison.OrdinalIgnoreCase))
            {
                if ("deploy".Equals(bhRequest.build_name, StringComparison.OrdinalIgnoreCase))
                {
                    _taskSvc.UpdateTaskStatus(new TaskDto() { Id = taskLog.TaskId, Status = TaskEnum.DeploySuccess.ToString() });
                }
            }

            return Ok();
        }
    }
}
