using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.Infrastructure.Data
{
    public class UserHelp
    {
        public static ApplicationUser GetUserInfo()
        {
            ApplicationUser user = new ApplicationUser();
            return user;
        }
    }
    public class ApplicationUser
    {
        public int UserId { get; set; }

        public string NickName { get; set; }
    }
}
