using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Query;

namespace Uoko.FireProj.WebSite.ControllerApi
{
    [RoutePrefix("api/ProjectApi")]
    public class ProjectApiController : BaseApiController
    {
        private IProjectSvc _projectSvc { get; set; }
        private IDomainResourceSvc DomainResourceSvc { get; set; }
        public ProjectApiController(IProjectSvc projectSvc, IDomainResourceSvc domainResourceSvc)
        {
            _projectSvc = projectSvc;
            DomainResourceSvc = domainResourceSvc;
        }

        /// <summary>
        /// 根据id获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/ById")]
        public IHttpActionResult GetById(int id)
        {
            var result = _projectSvc.GetProjectById(id);
            return Ok(result);
        }

        [Route("GetByTaskId/{taskId}")]
        public IHttpActionResult GetByTaskId(int taskId)
        {
            var result = _projectSvc.GetProjectByTaskId(taskId);
            return Ok(result);
        }
        
        /// <summary>
        /// 获取项目分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]ProjectQuery query)
        {
            var result = _projectSvc.GetProjectPage(query);
            return Ok(result);
        }
        [Route("GetAll")]
        public IHttpActionResult GetAll()
        {
            var result = _projectSvc.GetAllProject();
            return Ok(result);
        }
        /// <summary>
        /// 新增项目
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody] ProjectDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var projectId = _projectSvc.CreatProject(dto);
            if (projectId > 0)
            {
                //创建内部测试环境地址: 
                List<DomainResourceDto> resourceInfoList = new List<DomainResourceDto>();
                for (int i = 0; i < 10; i++)
                {
                    if (!string.IsNullOrEmpty(dto.DomainRule)&& dto.DomainRule.IndexOf("{编号}")>=0)
                    {
                        resourceInfoList.Add(new DomainResourceDto()
                        {
                            ProjectId = projectId,
                            Name = dto.DomainRule.Replace("{编号}", i == 0 ? "" : i.ToString())
                        });
                        
                    }
                    else
                    {
                        resourceInfoList.Add(new DomainResourceDto()
                        {
                            ProjectId = projectId,
                            Name = string.Format("{0}{1}.uoko.ioc", dto.ProjectGitlabName, i == 0 ? "" : i.ToString())
                        });
                    }
                    
                }
                DomainResourceSvc.CreatResource(resourceInfoList);
            }
            return Ok();
        }

        /// <summary>
        /// 根据id编辑项目信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, [FromBody] ProjectDto dto)
        {
            //参数验证
            if (id == 0)
            {
                ModelState.AddModelError("id", "id不允许为空");
                return BadRequest(ModelState);
            }
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            dto.Id = id;
            _projectSvc.EditProject(dto);
            return Ok();
        }

        /// <summary>
        /// 根据id删除项目信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IHttpActionResult Delete(int id)
        {
            //参数验证
            if (id == 0)
            {
                ModelState.AddModelError("id", "id不允许为空");
                return BadRequest(ModelState);
            }
            _projectSvc.DeleteProject(id);
            return Ok();
        }
    }
}
