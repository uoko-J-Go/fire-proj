using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

        public string NickName { get; set; }

        public string LoginName { get; set; }

        public int UserId { get; set; }

        /// <summary>
        /// 测试状态
        /// </summary>
        public QAStatus QAStatus { get; set; }
    }
}
