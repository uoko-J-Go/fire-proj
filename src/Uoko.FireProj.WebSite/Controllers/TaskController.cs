﻿using System;
using System.Globalization;
using System.Web.Mvc;
using Mehdime.Entity;
using Newtonsoft.Json;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.Model;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Uoko.FireProj.WebSite.Controllers
{
    public class TaskController : BaseController
    {
        // GET: Task
        public ActionResult Index()
        {
            
            var userId = User.Identity.Name;
            var user = User as ClaimsPrincipal;
            //获取Claim信息
            var userid = user.FindFirst("userid").Value; //userid是服务端提供的Claim信息,获取之前需要跟服务端确认提供了哪些用户信息
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Edit(int taskId)
        {
            ViewBag.TaskId = taskId;
            return View();
        }

        public ActionResult Detail(int taskId)
        {
            ViewBag.TaskId = taskId;
            return View();
        }
       
        public ActionResult Logs()
        {
            return View();
        }

        public ActionResult Seed()
        {

            using (var dbScope = new DbContextScopeFactory().CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                db.SaveChanges();
            }

            return Json(null,JsonRequestBehavior.AllowGet);
        }


        public ActionResult OnlineList()
        {
            return View();
        }

        /// <summary>
        /// 上线任务详情，获取该上线任务的部署信息，以及该次上线对应的上线任务列表
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public ActionResult OnlineInfo(int? taskId)
        {


            return View();
        }


    }
}