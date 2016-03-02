using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mehdime.Entity;
using Newtonsoft.Json;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.Model;

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
            var deployInfo = new DeployInfoIoc()
            {
                CheckUserId = "1-0,2-1,3-2",
                DeployAddress ="",
                DeployIP ="192.168.200.25",
                DeployStage = StageEnum.IOC,
                Domain = "fire.uoko.ioc",
                NoticeUseId =null,
                SiteName ="fire.uoko.ioc",
                DeployStatus = DeployStatus.Deploying,
                TaskDesc = "Seed data",
                TriggeredId = null,
                BuildId = null,
            };
            var taskInfo = new TaskInfo()
            {
                TaskName = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                ProjectId = 2025,
                Branch = "dev",
                CreateBy = 1,
                CreateDate = DateTime.Now,
                DeployInfoIocJson = JsonConvert.SerializeObject(deployInfo) ,
                DeployInfoOnlineJson = null,
                DeployInfoPreJson = null,
                HasOnlineDeployed = false,
                IocCheckUserId = "1-0,2-1,3-2",
                PreCheckUserId = "1-0,2-1,3-2",
                OnlineCheckUserId ="1-0,2-1,3-2" ,
                OnlineTaskId = null,
            };


            using (var dbScope = new DbContextScopeFactory().CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                db.TaskInfo.Add(taskInfo);
                db.SaveChanges();
            }


            return Json(taskInfo,JsonRequestBehavior.AllowGet);
        }
    }
}