using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using NLog;

namespace Uoko.FireProj.WebSite.Filter
{
    public class LogApiExceptionAttribute: ExceptionFilterAttribute
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            logger.Error(actionExecutedContext.Exception);
            base.OnException(actionExecutedContext);
        }
    }
}