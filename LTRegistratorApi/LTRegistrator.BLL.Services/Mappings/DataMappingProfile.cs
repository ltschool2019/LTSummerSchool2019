using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Dtos;
using LTRegistrator.Domain.Entities;

namespace LTRegistrator.BLL.Services.Mappings
{
    public class DataMappingProfile : Profile
    {
        public DataMappingProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(ed => ed.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(ed => ed.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(ed => ed.SecondName, opt => opt.MapFrom(src => src.SecondName))
                .ForMember(ed => ed.Mail, opt => opt.MapFrom(src => src.Mail))
                .ForMember(ed => ed.MaxRole, opt => opt.MapFrom(src => src.MaxRole))
                .ForMember(ed => ed.ProjectEmployees, opt => opt.MapFrom(src => src.ProjectEmployees))
                .ForMember(ed => ed.Leaves, opt => opt.MapFrom(src => src.Leaves))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<EmployeeDto, Employee>()
                .ForMember(ed => ed.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(ed => ed.SecondName, opt => opt.MapFrom(src => src.SecondName))
                .ForMember(ed => ed.Mail, opt => opt.MapFrom(src => src.Mail))
                .ForMember(ed => ed.MaxRole, opt => opt.MapFrom(src => src.MaxRole))
                .ForMember(ed => ed.ProjectEmployees, opt => opt.MapFrom(src => src.ProjectEmployees))
                .ForMember(ed => ed.Leaves, opt => opt.MapFrom(src => src.Leaves))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ProjectEmployee, ProjectEmployeeDto>().ReverseMap();

            CreateMap<Leave, LeaveDto>().ReverseMap();
        }
    }
}
