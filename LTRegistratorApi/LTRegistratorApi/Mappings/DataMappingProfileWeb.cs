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
                .ForMember(erm => erm.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(erm => erm.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(erm => erm.SecondName, opt => opt.MapFrom(src => src.SecondName))
                .ForMember(erm => erm.Mail, opt => opt.MapFrom(src => src.Mail))
                .ForMember(erm => erm.MaxRole, opt => opt.MapFrom(src => src.MaxRole))
                .ForMember(erm => erm.Projects, opt => opt.MapFrom(src => src.ProjectEmployees))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ProjectEmployeeDto, ProjectResourceModel>()
                .ForMember(prm => prm.Id, opt => opt.MapFrom(src => src.Project.Id))
                .ForMember(prm => prm.Name, opt => opt.MapFrom(src => src.Project.Name));

            #endregion

            #region Leaves

            CreateMap<LeaveDto, LeaveResourceModel>();

            #endregion

        }
    }
}
