using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Enum;

namespace Uoko.FireProj.DataAccess.Dto
{
    /// <summary>
    /// 任务写入 数据对象
    /// </summary>
    public class TaskWriteDto:BaseDto
    {

        public int Id { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 项目Id
        /// </summary>
        public int ProjectId { get; set; }
        /// <summary>
        /// 代码分支
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// 表示新增或修改任务 对应的环境
        /// </summary>
        public StageEnum DeployStage { get; set; }


        public DeployInfoIoc IocDeployInfo { get; set; }


        public DeployInfoPre PreDeployInfo { get; set; }


        public DeployInfoOnline OnlineDeployInfo { get; set; }


        public int? OnlineTaskId { get; set; }
    }

}
