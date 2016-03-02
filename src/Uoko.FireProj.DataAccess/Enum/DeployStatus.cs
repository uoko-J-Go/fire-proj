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
    public enum DeployStatus
    {
        /// <summary>
        /// 部署中
        /// </summary>
        [Description("部署中")]
        Deploying= 2,

        /// <summary>
        /// 部署失败
        /// </summary>
        [Description("部署失败")]
        DeployFails = 4,

        /// <summary>
        /// 部署成功
        /// </summary>
        [Description("部署成功")]
        DeploySuccess = 6,
    }

    /// <summary>
    /// 测试状态
    /// </summary>
    public enum QAStatus
    {
        /// <summary>
        /// 部署中
        /// </summary>
        [Description("待测试")]
        Waiting = 0,

        /// <summary>
        /// 部署失败
        /// </summary>
        [Description("测试未通过")]
        Refused = 1,

        /// <summary>
        /// 部署成功
        /// </summary>
        [Description("测试通过")]
        Passed = 2,
    }
}
