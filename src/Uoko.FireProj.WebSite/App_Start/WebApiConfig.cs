using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Uoko.FireProj.WebSite.Filter;
using UOKO.WebAPI.Tools;

namespace Uoko.FireProj.WebSite.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // model state 校验
            config.Filters.Add(new ModelStateEnsureValidFilterAttribute());
            // 异常统一结果返回
            config.Filters.Add(new UnifyExceptionFilter());

            config.Filters.Add(new LogApiExceptionAttribute()); 

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
