using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uoko.FireProj.DataAccess.Enum
{
    /// <summary>
    /// 任务状态
    /// </summary>
    public enum TaskEnum
    {
        /// <summary>
        /// 待部署
        /// </summary>
        [Description("待部署")]
        WaitingDeploy = 0,

        /// <summary>
        /// 部署失败
        /// </summary>
        [Description("部署失败")]
        DeployFails = 1,

        // <summary>
        /// 部署成功
        /// </summary>
        [Description("部署成功")]
        DeploySuccess = 2,


        // <summary>
        /// 测试中
        /// </summary>
        [Description("测试中")]
        Testing = 3,

        // <summary>
        /// 测试未通过
        /// </summary>
        [Description("测试未通过")]
        TestFails = 4,

        // <summary>
        /// 测试通过
        /// </summary>
        [Description("测试通过")]
        Tested = 5,
    }
}
