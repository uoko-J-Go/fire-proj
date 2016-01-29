using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Uoko.FireProj.Abstracts;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/DomainResourceApi")]
    public class DomainResourceController : BaseApiController
    {
        private IDomainResourceSvc _domainResourceSvc { get; set; }
     
        public DomainResourceController(IDomainResourceSvc domainResourceSvc)
        {
            _domainResourceSvc = domainResourceSvc;
           
        }

        [Route("{projectId}/{serverId}")]
        public IHttpActionResult Get(int projectId, int serverId)
        {
            var result = _domainResourceSvc.GetResourceList(projectId, serverId);
            return Ok(result);
        }
    }
}
