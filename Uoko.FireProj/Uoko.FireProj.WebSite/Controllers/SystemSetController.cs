using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Uoko.FireProj.WebSite.Controllers
{
    public class SystemSetController : BaseController
    {
        // GET: SystemSet
        #region 数据字典
        public ActionResult DictionaryMgmt()
        {
            return View("~/Views/Dictionary/Index.cshtml");
        }

        public ActionResult DictionaryForm()
        {
            return View("~/Views/Dictionary/Form.cshtml");
        }
        #endregion

        #region 域名资源
        public ActionResult DomainResourceMgmt()
        {
            return View("~/Views/DomainResource/Index.cshtml");
        }
        #endregion

        #region 服务器管理
        public ActionResult ServerMgmt()
        {
            return View("~/Views/Server/Index.cshtml");
        }
        #endregion
    }
}