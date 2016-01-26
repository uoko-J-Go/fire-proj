using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/WebHookApi")]
    public class WebHookApiController : BaseApiController
    {
        [HttpPost]
        public IHttpActionResult BuildCallback(HttpRequestMessage req)
        {
            var content = req.Content.ReadAsStringAsync().Result;
            var request = Request.GetRequestContext();
            return Ok();
        }
    }
}
