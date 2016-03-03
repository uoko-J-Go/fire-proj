using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Query;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/DomainResourceApi")]
    public class DomainResourceApiController : BaseApiController
    {
        private IDomainResourceSvc _domainResourceSvc { get; set; }
     
        public DomainResourceApiController(IDomainResourceSvc domainResourceSvc)
        {
            _domainResourceSvc = domainResourceSvc;
           
        }

        [Route("{projectId}/{serverId}/{taskId}")]
        public IHttpActionResult Get(int projectId, int serverId,int? taskId)
        {
            var result = _domainResourceSvc.GetResourceList(projectId, serverId, taskId);
            return Ok(result);
        }

        
        [Route("{serverId}")]
        public IHttpActionResult Get(int serverId)
        {
            var result = _domainResourceSvc.GetResourceList(serverId);
            return Ok(result);
        }

        /// <summary>
        /// 根据任务任务Id释放域名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("ReleaseDomain")]
        [HttpPost]
        public IHttpActionResult post(int id)
        {
            _domainResourceSvc.ReleaseDomain(id);
            return Ok();
        }

        /// <summary>
        /// 获取资源服务分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]DomainResourceQuery query)
        {
            var result = _domainResourceSvc.GetDomainPage(query);
            return Ok(result);
        }

        /// <summary>
        /// 删除域名信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("DeleteDomain/{id}")]
        [HttpPost]
        public IHttpActionResult DeleteDomain(int id)
        {
            _domainResourceSvc.DeleteDomain(id);
            return Ok();
        }
    }
}
