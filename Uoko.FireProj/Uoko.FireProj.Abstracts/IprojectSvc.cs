using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;

namespace Uoko.FireProj.Abstracts
{
    /// <summary>
    /// 项目管理
    /// </summary>
    public interface IProjectSvc
    {
        /// <summary>
        /// 新增项目
        /// </summary>
        /// <param name="dto"></param>
        int CreatProject(ProjectDto dto);

        /// <summary>
        /// 项目编辑
        /// </summary>
        /// <param name="dto"></param>
        void EditProject(ProjectDto dto);

        /// <summary>
        /// 根据项目id删除项目信息
        /// </summary>
        /// <param name="dto"></param>
        void DeleteProject(int projectId);

        /// <summary>
        /// 获取项目分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        PageGridData<ProjectDto> GetProjectPage(ProjectQuery query);

        /// <summary>
        /// 根据项目Id获取项目信息详情
        /// </summary>
        /// <param name="projectId"></param>
        ProjectDto GetProjectById(int projectId);
        /// <summary>
        /// 获取所有项目
        /// </summary>
        /// <returns></returns>
        IList<ProjectDto> GetAllProject();
    }
}
