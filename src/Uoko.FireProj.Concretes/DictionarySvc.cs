using AutoMapper;
using Mehdime.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.Abstracts;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Entity;
using Uoko.FireProj.DataAccess.Extensions;
using Uoko.FireProj.DataAccess.Query;
using Uoko.FireProj.Infrastructure.Data;
using Uoko.FireProj.Infrastructure.Exception;
using Uoko.FireProj.Model;

namespace Uoko.FireProj.Concretes
{
    public class DictionarySvc : IDictionarySvc
    {
        private readonly IDbContextScopeFactory _dbScopeFactory;

        public DictionarySvc(IDbContextScopeFactory dbScopeFactory)
        {
            _dbScopeFactory = dbScopeFactory;
        }

        public void CreatDictionary(DictionaryDto dto)
        {
            try
            {
                var entity = Mapper.Map<DictionaryDto, Dictionary>(dto);
                entity.CreateDate = DateTime.Now;
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    db.Dictionary.Add(entity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public void DeleteDictionary(int id)
        {
            try
            {
                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    var result = db.Dictionary.Where(r => r.ParentId == id).Count();
                    if (result > 0)
                    {
                        throw new TipInfoException("还有子集,无法删除!");
                    }
                    Dictionary entity = new Dictionary() { Id = id };
                    db.Dictionary.Attach(entity);
                    db.Dictionary.Remove(entity);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public void EditDictionary(DictionaryDto dto)
        {
            try
            {
                var entity = Mapper.Map<DictionaryDto, Dictionary>(dto);

                using (var dbScope = _dbScopeFactory.Create())
                {
                    var db = dbScope.DbContexts.Get<FireProjDbContext>();
                    db.Update(entity, r => new { r.Description, r.ModifyBy, r.ModifyDate, r.Name, r.ParentId, r.Value });
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new TipInfoException(ex.Message);
            }
        }

        public DictionaryDto GetDictionaryById(int id)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.Dictionary.Where(r => r.Id == id).Select(r => new DictionaryDto()
                {
                    Id = r.Id,
                    Name = r.Name,
                    Value = r.Value,
                    Description = r.Description,
                    Status = r.Status,
                    ParentId = r.ParentId,
                }).FirstOrDefault();

                return data;
            }
        }

        public PageGridData<DictionaryDto> GetDictionaryPage(DictionaryQuery query)
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.Dictionary.Select(r => new DictionaryDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Value = r.Value,
                    Description = r.Description,
                    Status = r.Status,
                    ParentId = r.ParentId,
                });
                if (!string.IsNullOrEmpty(query.Search))
                {
                    data = data.Where(r => r.Name.Contains(query.Search));
                }
                var result = data.OrderBy(r => r.Id).ToList();
                var total = data.Count();
                return new PageGridData<DictionaryDto> { rows = result, total = total };
            }
        }

        public List<DictionaryDto> GetDictionaryParent()
        {
            using (var dbScope = _dbScopeFactory.CreateReadOnly())
            {
                var db = dbScope.DbContexts.Get<FireProjDbContext>();
                var data = db.Dictionary.Where(r => r.ParentId == 0).Select(r => new DictionaryDto()
                {
                    Id = r.Id,
                    Name = r.Name,
                    Value = r.Value,
                    Description = r.Description,
                    Status = r.Status,
                  
                }).ToList();
                return data;
            }
        }
    }
}
