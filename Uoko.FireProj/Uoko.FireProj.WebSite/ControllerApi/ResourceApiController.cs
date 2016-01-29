using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Uoko.FireProj.Abstracts;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/ResourceApi")]
    public class ResourceApiController : BaseApiController
    {
        private IResourceInfoSvc _resourceInfoSvc { get; set; }
     
        public ResourceApiController(IResourceInfoSvc resourceInfoSvc)
        {
            _resourceInfoSvc = resourceInfoSvc;
           
        }

        [Route("{projectId}/{ipId}")]
        public IHttpActionResult Get(int projectId, string ip)
        {
            var result = _resourceInfoSvc.GetResourceList(projectId, ip);
            return Ok(result);
        }
    }
}
