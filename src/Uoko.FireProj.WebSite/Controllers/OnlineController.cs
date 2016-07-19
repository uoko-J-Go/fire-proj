using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uoko.FireProj.Abstracts;

namespace Uoko.FireProj.WebSite.Controllers
{
    public class OnlineController : BaseController
    {

        // GET: Online
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Detail(int taskId)
        {
            return View(taskId);
        }
        
    }
}