using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Uoko.FireProj.WebSite.Controllers
{
    public class DictionaryController : BaseController
    {
        // GET: Dictionary
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Form()
        {
            return View();
        }
    }
}