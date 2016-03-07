using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Mvc;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.Infrastructure.Extensions;
using Uoko.FireProj.WebSite.Models;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    public class BaseApiController : ApiController
    {
        public ApplicationUser GetIdentity()
        {
            ApplicationUser userInfo = new ApplicationUser();
            try
            {
                var user = User as ClaimsPrincipal;
                userInfo.UserId = int.Parse(user.FindFirst("userid").Value.ToString());
                userInfo.NickName = user.FindFirst("NickName").Value;
            }
            catch (Exception ex)
            {
               
            }
            return userInfo;
        }
    }
}
