using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.Domain.Entities;
using LTRegistratorApi.Model;

namespace LTRegistratorApi.Mappings
{
    public class DataMappingProfileWeb : Profile
    {
        public DataMappingProfileWeb()
        {
            #region Employee

            CreateMap<Employee, EmployeeDto>()
                .ForMember(e => e.Projects, opt => opt.MapFrom(src => src.ProjectEmployees));

            CreateMap<ProjectEmployee, ProjectDto>()
                .ForMember(pd => pd.Id, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(pd => pd.Name, opt => opt.MapFrom(src => src.Project.Name));

            CreateMap<LeaveDto, Leave>()
                .ForMember(l => l.EmployeeId, opt => opt.Ignore())
                .ForMember(l => l.Employee, opt => opt.Ignore());

            CreateMap<Leave, LeaveDto>();

            CreateMap<LeaveInputDto, Leave>();

            #endregion

        }
    }
}
