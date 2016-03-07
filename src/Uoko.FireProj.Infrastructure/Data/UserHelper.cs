using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.Infrastructure.Data
{
    public class UserHelper
    {
        public static ApplicationUser CurrUserInfo = new ApplicationUser();
        
    }
    public class ApplicationUser
    {
        public int UserId { get; set; }

        public string NickName { get; set; }
    }
}
