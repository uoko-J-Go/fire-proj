﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uoko.FireProj.WebSite.Models;

namespace Uoko.FireProj.WebSite.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {           
            GetMenuData(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
            base.OnActionExecuted(filterContext);
        }
        public void GetMenuData(string currentController)
        {
            List<MenuTreeVM> nodes = new List<MenuTreeVM>()
            {
                 new MenuTreeVM()
                {
                    MenuName = "项目",
                    MenuLevel=1,
                    Controller="Project",
                    Action="Index",
                },
                 new MenuTreeVM()
                {
                    MenuName = "我的任务",
                    MenuLevel=1,
                    Controller="Task",
                    Action="Index"
                },
                  new MenuTreeVM()
                {
                    MenuName = "系统设置",
                    MenuLevel=1,
                    Controller="SystemSet",
                    Action=null,
                    Children=new List<MenuTreeVM>()
                    {
                         new MenuTreeVM()
                        {
                            MenuName = "服务器",
                            MenuLevel=1,
                            Controller="SystemSet",
                            Action="ServerMgmt",
                        },
                         new MenuTreeVM()
                        {
                            MenuName = "域名",
                            MenuLevel=1,
                            Controller="SystemSet",
                            Action="DomainResourceMgmt"
                        },
                          new MenuTreeVM()
                        {
                            MenuName = "数据字典",
                            MenuLevel=1,
                            Controller="SystemSet",
                            Action="DictionaryMgmt"
                        }
                    }
                }
            };
            var  activeMenu= nodes.FirstOrDefault(t=>t.MenuLevel==1&&t.Controller.Equals(currentController,StringComparison.CurrentCultureIgnoreCase));
            if (activeMenu != null)
            {
                activeMenu.IsActive = true;
            }
            ViewBag.MenuTreeData = nodes;
        }
    }
}