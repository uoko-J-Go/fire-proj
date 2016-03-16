using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Enum
{
    public enum LogType
    {
        /// <summary>
        /// 部署
        /// </summary>
        Deploy = 0,
        /// <summary>
        /// 测试
        /// </summary>
        QA = 1,
        /// <summary>
        /// 回滚
        /// </summary>
        RollBack=2
    }
}
