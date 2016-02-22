using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Uoko.FireProj.WebSite.Models
{
    /// <summary>
    /// 菜单VM
    /// </summary>
    public class MenuTreeVM
    {
       

        public string MenuName { get; set; }

        public int MenuLevel { get; set; }

        public string IconCls { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public bool IsActive { get; set; }
        public ICollection<MenuTreeVM> Children { get; set; }
    }
}