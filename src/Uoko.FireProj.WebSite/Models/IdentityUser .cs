﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.WebSite.Models
{
    public class IdentityUser : IUser
    {
        public string Id { get; set; }
        
        public string UserName { get; set; }
        
    }
}
