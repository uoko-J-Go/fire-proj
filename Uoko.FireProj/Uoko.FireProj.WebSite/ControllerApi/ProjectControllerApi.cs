﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    public class ProjectControllerApi : ApiController
    {
        // GET: api/ProjectControllerApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ProjectControllerApi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ProjectControllerApi
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ProjectControllerApi/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ProjectControllerApi/5
        public void Delete(int id)
        {
        }
    }
}
