using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using Uoko.FireProj.Infrastructure.Data;
using Uoko.FireProj.WebSite.Models;

namespace Uoko.FireProj.WebSite.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            GetMenuData(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName);
            base.OnActionExecuted(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var user = User as ClaimsPrincipal;
            UserHelper.CurrUserInfo= UserHelper.GetUserById(int.Parse(user.FindFirst("userid").Value));
            ViewBag.UserId = UserHelper.CurrUserInfo.UserId;
            ViewBag.NickName = UserHelper.CurrUserInfo.NickName;
        }
        public void GetMenuData(string controller, string action)
        {
            List<MenuTreeVM> nodes = new List<MenuTreeVM>()
                                     {
                                         new MenuTreeVM()
                                         {
                                             MenuName = "任务列表",
                                             MenuLevel = 1,
                                             Controller = "Task",
                                             Action = "Index"
                                         },
                                         new MenuTreeVM()
                                         {
                                             MenuName = "上线",
                                             MenuLevel = 1,
                                             Controller = "Online",
                                             Action = "Index"
                                         },
                                          new MenuTreeVM()
                                         {
                                             MenuName = "紧急回滚",
                                             MenuLevel = 1,
                                             Controller = "Rollback",
                                             Action = "Index"
                                         },
                                         new MenuTreeVM()
                                         {
                                             MenuName = "基础信息",
                                             MenuLevel = 1,
                                             Controller = "SystemSet",
                                             Action = null,
                                             Children = new List<MenuTreeVM>()
                                                        {
                                                             new MenuTreeVM()
                                                             {
                                                                 MenuName = "项目",
                                                                 MenuLevel = 1,
                                                                 Controller = "Project",
                                                                 Action = "Index",
                                                             },
                                                            new MenuTreeVM()
                                                            {
                                                                MenuName = "服务器",
                                                                MenuLevel = 1,
                                                                Controller = "SystemSet",
                                                                Action = "ServerMgmt",
                                                            },
                                                            new MenuTreeVM()
                                                            {
                                                                MenuName = "域名",
                                                                MenuLevel = 1,
                                                                Controller = "SystemSet",
                                                                Action = "DomainResourceMgmt"
                                                            },
                                                        }
                                         }
                                     };
            var activeMenu = nodes.FirstOrDefault(t => t.MenuLevel == 1
                                                       && controller == t.Controller
                                                       );
            if (activeMenu != null)
            {
                activeMenu.IsActive = true;
            }
            ViewBag.MenuTreeData = nodes;
        }
    }
}
