using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/UserApi")]
    public class UserApiController : ApiController
    {
        public IHttpActionResult Get()
        {
            var result = new WebApiProvider().GetAsync("http://api.systemset.uoko.pre/api/UserOld/10000/9/ByCompanyDepId").Result;
            if (result.IsSuccessStatusCode)
            {
                var user = result.Content.ReadAsAsync<dynamic>().Result;
                return Ok(user.dataList);
            }
            return Ok();
        }
    }
}
