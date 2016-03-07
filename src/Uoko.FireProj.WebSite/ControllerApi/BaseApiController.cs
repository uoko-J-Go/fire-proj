using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.Infrastructure.Extensions;
using Uoko.FireProj.WebSite.Models;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    public class BaseApiController : ApiController
    {
        public static ApplicationUser userInfo = new ApplicationUser();
        public static BaseDto baseDto = new BaseDto();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            try
            {
                var user = User as ClaimsPrincipal;
                baseDto.CreatorId = userInfo.UserId = int.Parse(user.FindFirst("userid").Value.ToString());
                baseDto.CreatorName = userInfo.NickName = user.FindFirst("NickName").Value;
            }
            catch (Exception ex)
            {

            }
            base.Initialize(controllerContext);
        }


    }
}
