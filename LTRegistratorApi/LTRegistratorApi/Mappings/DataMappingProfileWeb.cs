using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.Domain.Entities;
using LTRegistratorApi.Model;
using LTRegistratorApi.Model.CustomValues;
using LTRegistratorApi.Model.Projects;
using LTRegistratorApi.Model.Tasks;
using Task = LTRegistrator.Domain.Entities.Task;

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
                .ForMember(pd => pd.Name, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(pd => pd.TotalHours, opt => opt.MapFrom(src => src.Tasks.Sum(t => t.TaskNotes.Sum(tn => tn.Hours))));

            CreateMap<LeaveDto, Leave>()
                .ForMember(l => l.EmployeeId, opt => opt.Ignore())
                .ForMember(l => l.Employee, opt => opt.Ignore());

            CreateMap<Leave, Leave>();

            CreateMap<Leave, LeaveDto>();

            CreateMap<LeaveInputDto, Leave>();

            CreateMap<ProjectFullDto, Project>()
                .ForMember(p => p.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(p => p.CustomFieldProjects, opt => opt.MapFrom(src => src.CustomFields))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Project, ProjectFullDto>()
                .ForMember(pf => pf.CustomFields, opt => opt.MapFrom(src => src.CustomFieldProjects));

            CreateMap<CustomFieldProject, CustomFieldDto>()
                .ForMember(cf => cf.CustomFieldType, opt => opt.MapFrom(src => src.CustomField.Type))
                .ForMember(cf => cf.Name, opt => opt.MapFrom(src => src.CustomField.Name))
                .ForMember(cf => cf.Id, opt => opt.MapFrom(src => src.CustomField.Id))
                .ForMember(cf => cf.Description, opt => opt.MapFrom(src => src.CustomField.Description))
                .ForMember(cf => cf.IsRequired, opt => opt.MapFrom(src => src.CustomField.IsRequired))
                .ForMember(cf => cf.DefaultValue, opt => opt.MapFrom(src => src.CustomField.DefaultValue))
                .ForMember(cf => cf.MaxLength, opt => opt.MapFrom(src => src.CustomField.MaxLength));

            CreateMap<CustomFieldDto, CustomFieldProject>()
                .ForMember(cfp => cfp.CustomField, opt => opt.MapFrom(src => src));

            CreateMap<CustomFieldDto, CustomField>()
                .ForMember(cf => cf.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(cf => cf.Type, opt => opt.MapFrom(src => src.CustomFieldType))
                .ForMember(cf => cf.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(cf => cf.IsRequired, opt => opt.MapFrom(src => src.IsRequired))
                .ForMember(cf => cf.DefaultValue, opt => opt.MapFrom(src => src.DefaultValue))
                .ForMember(cf => cf.MaxLength, opt => opt.MapFrom(src => src.MaxLength))
                .ForMember(cf => cf.CustomFieldOptions, opt => opt.MapFrom(src => src.FieldOptions))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<CustomFieldOptionDto, CustomFieldOption>()
                .ForMember(cfo => cfo.Id, opt => opt.Ignore());

            CreateMap<Task, TaskDto>();

            CreateMap<TaskNote, TaskNoteDto>();

            CreateMap<CustomValue, CustomValueDto>();

            #endregion

        }
    }
}
