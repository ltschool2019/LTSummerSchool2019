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
                .ForMember(p => p.CustomFieldProjects, opt => opt.MapFrom(src => src.CustomFields));

            CreateMap<Project, ProjectFullDto>()
                .ForMember(pf => pf.CustomFields, opt => opt.MapFrom(src => src.CustomFieldProjects))
                .ForMember(pf => pf.Employees, opt => opt.MapFrom(src => src.ProjectEmployees));

            CreateMap<ProjectEmployee, EmployeeDto>()
                .ForMember(ed => ed.Id, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(ed => ed.FirstName, opt => opt.MapFrom(src => src.Employee.FirstName))
                .ForMember(ed => ed.SecondName, opt => opt.MapFrom(src => src.Employee.SecondName))
                .ForMember(ed => ed.Mail, opt => opt.MapFrom(src => src.Employee.Mail))
                .ForMember(ed => ed.Rate, opt => opt.MapFrom(src => src.Employee.Rate))
                .ForMember(ed => ed.ManagerId, opt => opt.MapFrom(src => src.Employee.ManagerId));

            CreateMap<CustomFieldProject, CustomFieldDto>()
                .ForMember(cf => cf.Type, opt => opt.MapFrom(src => src.CustomField.Type))
                .ForMember(cf => cf.Name, opt => opt.MapFrom(src => src.CustomField.Name))
                .ForMember(cf => cf.Id, opt => opt.MapFrom(src => src.CustomField.Id))
                .ForMember(cf => cf.Description, opt => opt.MapFrom(src => src.CustomField.Description))
                .ForMember(cf => cf.IsRequired, opt => opt.MapFrom(src => src.CustomField.IsRequired))
                .ForMember(cf => cf.DefaultValue, opt => opt.MapFrom(src => src.CustomField.DefaultValue))
                .ForMember(cf => cf.MaxLength, opt => opt.MapFrom(src => src.CustomField.MaxLength))
                .ForMember(cf => cf.FieldOptions, opt => opt.MapFrom(src => src.CustomField.CustomFieldOptions));

            CreateMap<CustomFieldOption, CustomFieldOptionDto>();

            CreateMap<CustomFieldDto, CustomFieldProject>()
                .ForMember(cfp => cfp.CustomFieldId, opt => opt.MapFrom(src => src.Id))
                .ForMember(cfp => cfp.CustomField, opt => opt.MapFrom(src => src));

            CreateMap<CustomFieldDto, CustomField>()
                .ForMember(cf => cf.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(cf => cf.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(cf => cf.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(cf => cf.IsRequired, opt => opt.MapFrom(src => src.IsRequired))
                .ForMember(cf => cf.DefaultValue, opt => opt.MapFrom(src => src.DefaultValue))
                .ForMember(cf => cf.MaxLength, opt => opt.MapFrom(src => src.MaxLength))
                .ForMember(cf => cf.CustomFieldOptions, opt => opt.MapFrom(src => src.FieldOptions))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<CustomFieldOptionDto, CustomFieldOption>()
                .ForMember(cfo => cfo.Id, opt => opt.Ignore());

            

            CreateMap<CustomValue, CustomValueDto>();

            #endregion

            #region Task
            CreateMap<Task, TaskDto>();

            CreateMap<TaskNote, TaskNoteDto>();

            CreateMap<TaskDto, Task>()
                .ForMember(t => t.EmployeeId, opt => opt.MapFrom((src, dest, res, context) =>
                {
                    var employeeId = (int)context.Items["EmployeeId"];
                    return employeeId > 1 ? employeeId : 0;
                }))
                .ForMember(t => t.ProjectEmployee, opt => opt.Ignore())
                .ForMember(t => t.TaskNotes, opt => opt.Ignore());

            CreateMap<CustomValueDto, CustomValue>();


            #endregion
        }
    }
}
