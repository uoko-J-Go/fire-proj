using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uoko.FireProj.DataAccess.Dto;
using Uoko.FireProj.DataAccess.Entity;

namespace Uoko.FireProj.DataAccess.AutoMapper
{
    public class AutoMapper
    {
        public static void MapperRegister()
        {
            Mapper.CreateMap<ProjectDto, Project>();
            Mapper.CreateMap<Project, ProjectDto>();
            Mapper.CreateMap<TaskLogsDto, TaskLogs>();
            Mapper.CreateMap<DictionaryDto, Dictionary>();
            Mapper.CreateMap<TaskDto, TaskInfo>();
            Mapper.CreateMap<TaskInfo, TaskDto>();
        }
    }
}
