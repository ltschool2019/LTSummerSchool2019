using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Models;
using LTRegistrator.Domain.Entities;

namespace LTRegistrator.BLL.Services.Mappings
{
    public class DataMappingProfile : Profile
    {
        public DataMappingProfile()
        {
            #region HourReport

            CreateMap<ICollection<Employee>, HourReportBllModel>()
                .ForMember(hr => hr.Users, opt => opt.MapFrom(src => src))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ICollection<Project>, HourReportBllModel>()
                .ForMember(hr => hr.Projects, opt => opt.MapFrom(src => src))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Employee, HourReportUserBllModel>()
                .ForMember(hru => hru.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(hru => hru.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(hru => hru.Surname, opt => opt.MapFrom(src => src.SecondName))
                .ForMember(hru => hru.Rate, opt => opt.MapFrom(src => src.Rate))
                .ForMember(hru => hru.Projects, opt => opt.MapFrom(src => src.ProjectEmployees))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<int, HourReportUserBllModel>()
                .ForMember(hru => hru.NormHours, opt => opt.MapFrom((src, crr) => crr.Rate * src))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ProjectEmployee, HourReportProjectBllModel>()
                .ForMember(hrp => hrp.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(hrp => hrp.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(hrp => hrp.Hours, opt => opt.MapFrom(src => src.Tasks.Sum(t => t.TaskNotes.Sum(tn => tn.Hours))))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Project, HourReportProjectBllModel>()
                .ForMember(hrp => hrp.ProjectId, opt => opt.MapFrom(src => src.Id))
                .ForMember(hrp => hrp.ProjectName, opt => opt.MapFrom(src => src.Name))
                .ForAllOtherMembers(opt => opt.Ignore());

            #endregion
        }
    }
}
