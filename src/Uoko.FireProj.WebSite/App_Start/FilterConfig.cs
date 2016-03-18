using System.Web;
using System.Web.Mvc;
using Uoko.FireProj.WebSite.Filter;

namespace Uoko.FireProj.WebSite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogExceptionAttribute());
            filters.Add(new AuthorizeAttribute());
        }
    }
}
