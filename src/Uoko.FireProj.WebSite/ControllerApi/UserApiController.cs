using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/UserApi")]
    public class UserApiController : ApiController
    {
        public IHttpActionResult Get()
        {
            var result = new WebApiProvider().Get<List<UserDto>>("http://api.systemset.uoko.pre/api/UserOld/10000/9/ByCompanyDepId");
            return Ok(result);
        }
    }
}
