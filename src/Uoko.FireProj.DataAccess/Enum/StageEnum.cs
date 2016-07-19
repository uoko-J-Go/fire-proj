using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Enum
{
    /// <summary>
    /// 测试环境枚举
    /// </summary>
    public enum StageEnum
    {
        /// <summary>
        /// Test环境
        /// </summary>
        [Description("Test环境")]
        TEST = 0,

        /// <summary>
        /// Pre环境
        /// </summary>
        [Description("Pre环境")]
        PRE = 1,

        /// <summary>
        /// 正式环境
        /// </summary>
        [Description("正式环境")]
        PRODUCTION = 2,
    }
}
