using System;
using System.Globalization;
using System.Web.Mvc;
using Mehdime.Entity;
using Newtonsoft.Json;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.Model;
using System.Security.Claims;
using Microsoft.AspNet.Identity.EntityFramework;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.WebSite.Controllers
{
    public class TaskController : BaseController
    {
        // GET: Task
        public ActionResult Index()
        {
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
            throw new Exception("WTF");
            using (var dbScope = new DbContextScopeFactory().CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                db.SaveChanges();
            }

            return Json(null,JsonRequestBehavior.AllowGet);
        }
    }
}