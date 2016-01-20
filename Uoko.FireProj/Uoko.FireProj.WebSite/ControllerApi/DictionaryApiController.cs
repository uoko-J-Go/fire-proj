using System;
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
    [RoutePrefix("api/DictionaryApi")]
    public class DictionaryApiController : BaseApiController
    {
        private IDictionarySvc _dictionary { get; set; }
        public DictionaryApiController(IDictionarySvc dictionary)
        {
            _dictionary = dictionary;
        }

        /// <summary>
        /// 根据id获取详细信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}/ById")]
        public IHttpActionResult GetById(int id)
        {
            var result = _dictionary.GetDictionaryById(id);
            return Ok(result);
        }

        /// <summary>
        /// 获取所有父级字典
        /// </summary>
        /// <returns></returns>
        [Route("Parent")]
        public IHttpActionResult GetParent()
        {
            var result = _dictionary.GetDictionaryParent();
            return Ok(result);
        }

        /// <summary>
        /// 获取字典分页
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IHttpActionResult Get([FromUri]DictionaryQuery query)
        {
            var result = _dictionary.GetDictionaryPage(query);
            return Ok(result);
        }

        /// <summary>
        /// 新增字典
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IHttpActionResult Post([FromBody] DictionaryDto dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            _dictionary.CreatDictionary(dto);
            return Ok();
        }

        /// <summary>
        /// 根据id编辑字典信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int id, [FromBody] DictionaryDto dto)
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
            _dictionary.EditDictionary(dto);
            return Ok();
        }

        /// <summary>
        /// 根据id删除字典信息
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
            _dictionary.DeleteDictionary(id);
            return Ok();
        }
    }
}
