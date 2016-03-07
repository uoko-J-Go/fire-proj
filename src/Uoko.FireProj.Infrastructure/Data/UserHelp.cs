using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.Infrastructure.Data
{
    public class UserHelp
    {
        public static ApplicationUser userInfo = new ApplicationUser();
        
    }
    public class ApplicationUser
    {
        public int UserId { get; set; }

        public string NickName { get; set; }
    }
}
