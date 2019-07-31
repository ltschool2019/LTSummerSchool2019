using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Dtos;
using LTRegistratorApi.Model.ResourceModels;

namespace LTRegistratorApi.Mappings
{
    public class DataMappingProfileWeb : Profile
    {
        public DataMappingProfileWeb()
        {
            #region Employee

            CreateMap<EmployeeDto, EmployeeResourceModel>()
                .ForMember(erm => erm.Projects, opt => opt.MapFrom(src => src.ProjectEmployees));

            CreateMap<ProjectEmployeeDto, ProjectResourceModel>()
                .ForMember(prm => prm.Id, opt => opt.MapFrom(src => src.Project.Id))
                .ForMember(prm => prm.Name, opt => opt.MapFrom(src => src.Project.Name));

            #endregion

            #region Leaves

            CreateMap<LeaveDto, LeaveResourceModel>();

            CreateMap<LeaveResourceModel, LeaveDto>()
                .ForMember(ld => ld.EmployeeId, opt => opt.Ignore());

            #endregion

        }
    }
}
