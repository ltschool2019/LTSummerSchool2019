using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Services.Mappings.DbContext;
using LTRegistrator.Domain.Entities;
using LTRegistratorApi.Model;
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

            #endregion

            #region Task

            CreateMap<TaskInputDto, Task>()
                .ForMember(t => t.Id, opt => opt.Ignore());

            CreateMap<TaskNoteDto, TaskNote>()
                .ForMember(t => t.Id, opt => opt.Ignore());

            CreateMap<Task, TaskDto>();

            CreateMap<ICollection<Leave>, TaskDto>()
                .ForMember(td => td.Leave, opt => opt.MapFrom(src => src));

            CreateMap<Leave, LeaveDto>();

            CreateMap<TaskNote, TaskNoteDto>();
            #endregion

        }
    }
}
