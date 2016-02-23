using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.Infrastructure.Extensions;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/CommonApi")]
    public class CommonApiController : ApiController
    { 
        /// <summary>
        /// 获取环境枚举下拉框绑定
        /// </summary>
        /// <returns></returns>
        [Route("Environment")]
        public IHttpActionResult GetEnvironment()
        {
            List<object> data = new List<object>();
            foreach (Enum item in Enum.GetValues(typeof(EnvironmentEnum)))
            {
                data.Add(new { Id = item.GetHashCode(), Name = item.ToDescription() });
            }
            return Ok(data);
        }
    }
}
