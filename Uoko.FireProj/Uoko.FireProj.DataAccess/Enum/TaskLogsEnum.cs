using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Enum
{
    public enum TaskLogsEnum
    {
        /// <summary>
        /// 状态记录
        /// </summary>
        [Description("状态记录")]
        Status = 0,

        /// <summary>
        /// CI记录
        /// </summary>
        [Description("CI记录")]
        CI =1,

        /// <summary>
        /// YML记录
        /// </summary>
        [Description("YML记录")]
        YML = 2,
    }
}
