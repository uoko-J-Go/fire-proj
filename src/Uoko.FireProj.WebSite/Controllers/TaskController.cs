using System;
using System.Globalization;
using System.Web.Mvc;
using Mehdime.Entity;
using Newtonsoft.Json;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.Model;
using System.Security.Claims;

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
            var deployInfo = new DeployInfoIoc()
            {
                CheckUserId = "1-0,2-1,3-2",
                DeployAddress ="",
                DeployIP ="192.168.200.25",
                DeployStage = StageEnum.IOC,
                Domain = "fire.uoko.ioc",
                NoticeUserId =null,
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
                CreatorId = 1,
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


        public ActionResult OnlineList()
        {
            return View();
        }

    }
}