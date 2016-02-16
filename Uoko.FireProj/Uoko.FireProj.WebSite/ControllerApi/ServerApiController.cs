﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Enum;
using Uoko.FireProj.DataAccess.Query;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/ServerApi")]
    public class ServerApiController : ApiController
    {
        private IServerSvc _serverSvc { get; set; }
        public ServerApiController(IServerSvc serverSvc)
        {
            _serverSvc = serverSvc;
        }

        public IHttpActionResult Get([FromUri]ServerQuery query)
        {
            var result = _serverSvc.GetServerByPage(query);
            return Ok(result);
        }
        [Route("Environment/{en}")]
        public IHttpActionResult Get(EnvironmentEnum en)
        {
            var result = _serverSvc.GetAllServerOfEnvironment(en);
            return Ok(result);
        }
       
        [Route("Update")]
        [HttpPost]
        public IHttpActionResult Update([FromBody]ServerDto server)
        {
            _serverSvc.UpdateServer(server);
            return Ok();
        }

        [Route("Create")]
        [HttpPost]
        public IHttpActionResult Create([FromBody]ServerDto server)
        {
            _serverSvc.CreateServer(server);
            return Ok();
        }
        [Route("Delete/{id}")]
        [HttpPost]
        public IHttpActionResult Delete(int id)
        {
            _serverSvc.DeleteServer(id);
            return Ok();
        }
        /// <summary>
        /// 根据id获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        public IHttpActionResult GetById(int id)
        {
            var result = _serverSvc.GetServerById(id);
            return Ok(result);
        }
    }
}
