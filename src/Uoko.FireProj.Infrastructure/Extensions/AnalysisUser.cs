using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.Infrastructure.Extensions
{
    public class AnalysisUser
    {
        public static List<UserDto> AnalysisCheckUser(string userInfo)
        {
            List<UserDto> userDtoData = new List<UserDto>();
            if (string.IsNullOrEmpty(userInfo))
            {
                return userDtoData;
            }
            var userList = userInfo.Split(',');
            foreach (var item in userList)
            {
                var user = item.Split('-');
                userDtoData.Add(new UserDto
                {
                    UserId = int.Parse(user[0]),
                    QAStatus = (QAStatus)int.Parse(user[1])
                });
            }
            return userDtoData;
        }
        public static List<UserDto> AnalysisNoticeUser(string userInfo)
        {
            List<UserDto> userDtoData = new List<UserDto>();
            if (string.IsNullOrEmpty(userInfo))
            {
                return userDtoData;
            }
            var userList = userInfo.Split(',');
            foreach (var item in userList)
            {
                userDtoData.Add(new UserDto
                {
                    UserId = int.Parse(item),
                });
            }
            return userDtoData;
        }
    }
}
