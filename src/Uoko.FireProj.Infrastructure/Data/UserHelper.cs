using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Dto;

namespace Uoko.FireProj.Infrastructure.Data
{
    public class UserHelper
    {
        private static string systemSetApiUrl = ConfigurationManager.AppSettings["systemset.api.url"];
        private static WebApiProvider apiProvider;
        public static UserDto CurrUserInfo = new UserDto();
        static UserHelper()
        {
            apiProvider = new WebApiProvider(systemSetApiUrl);
        }

        /// <summary>
        /// 获取公司 部门下的所有员工
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="depId"></param>
        /// <returns></returns>
        public static List<UserDto> GetUserByCompanyDepId(int companyId, int depId)
        {
            return apiProvider.Get<List<UserDto>>(string.Format("api/UserOld/{0}/{1}/ByCompanyDepId", companyId, depId));
        }
        /// <summary>
        /// 根据用户Id集合获取用户
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        public static List<UserDto> GetUserByIds(List<int> userIds)
        {
            if (userIds == null || userIds.Count < 1)
            {
                return null;
            }
            return apiProvider.Get<List<UserDto>>(string.Format("api/UserOld?IdList={0}",string.Join(",",userIds)));
        }
        /// <summary>
        /// 获取用户详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static UserDto GetUserById(int id)
        {
            return apiProvider.Get<UserDto>(string.Format("api/UserOld/{0}",id));
        }
    }
}
