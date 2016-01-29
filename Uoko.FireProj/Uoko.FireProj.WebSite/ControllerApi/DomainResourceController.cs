﻿using System;
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
        private IDomainResourceSvc DomainResourceSvc { get; set; }
     
        public DomainResourceController(IDomainResourceSvc domainResourceSvc)
        {
            DomainResourceSvc = domainResourceSvc;
           
        }

        [Route("{projectId}/{ipId}")]
        public IHttpActionResult Get(int projectId, int ipId)
        {
            var result = DomainResourceSvc.GetResourceList(projectId, ipId);
            return Ok(result);
        }
    }
}
