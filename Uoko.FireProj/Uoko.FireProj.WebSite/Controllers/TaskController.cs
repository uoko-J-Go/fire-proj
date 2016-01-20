using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            return View();
        }
        public ActionResult Detail(int taskId)
        {
            return View();
        }
    }
}