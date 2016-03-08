using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static WebApiProvider apiProvider = (new WebApiProvider(ConfigurationManager.AppSettings["system.api.url"]));
        public IHttpActionResult Get()
        {
            var result = apiProvider.Get<List<UserDto>>("api/UserOld/10000/9/ByCompanyDepId");
            return Ok(result);
        }
    }
}
