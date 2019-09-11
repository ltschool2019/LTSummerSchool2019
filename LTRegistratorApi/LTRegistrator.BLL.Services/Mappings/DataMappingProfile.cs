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

            CreateMap<IEnumerable<Employee>, HourReportBllModel>()
                .AfterMap((src, dest, context) =>
                {

                    var leaves = src.SelectMany(e => e.Leaves).GroupBy(l => l.TypeLeave).Select(l => l.Key).Select(tl => new Event
                    {
                        Name = EnumAssociations.LeaveNames[tl],
                        EventType = EnumAssociations.LeaveEventTypes[tl]
                    });

                    var manager = src.FirstOrDefault()?.Manager;
                    var managerProjects = new List<int>();
                    if (manager != null)
                        managerProjects = manager.ProjectEmployees.Select(pem => pem.ProjectId).ToList();
                    var projects = src.SelectMany(e => e.ProjectEmployees).Select(pe => new Event
                    {
                        Name = pe.Project.Name,
                        EventType = managerProjects.Contains(pe.ProjectId)
                            ? EventType.ManagerProject
                            : EventType.NotManagerProject
                    }).GroupBy(p => new { p.EventType, p.Name }).Select(g => g.First());
                    var result = leaves.ToList();
                    result.AddRange(projects);
                    dest.Events = result.OrderBy(e => e.EventType).ToList();
                })
                .ForMember(hr => hr.Users, opt => opt.MapFrom(src => src))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Employee, HourReportUserBllModel>()
                .AfterMap((src, dest, context) =>
                {
                    var workMonth = context.Items["workMonth"] as IDictionary<DateTime, bool>;
                    var firstDayOfSelectedMonth = workMonth.FirstOrDefault().Key;
                    var le = src.Leaves.Select(l =>
                    {
                        if (l.StartDate < firstDayOfSelectedMonth)
                        {
                            l.StartDate = firstDayOfSelectedMonth;
                        }

                        if (l.EndDate > firstDayOfSelectedMonth.AddMonths(1))
                        {
                            l.EndDate = firstDayOfSelectedMonth.AddMonths(1);
                        }

                        return l;
                    });
                    var leaves = GetLeavesWithoutWeekends(le, workMonth).GroupBy(l => l.TypeLeave).Select(g => new HourReportLeaveBllModel
                    {
                        EventType = EnumAssociations.LeaveEventTypes[g.Key],
                        Name = EnumAssociations.LeaveNames[g.Key],
                        Hours = g.Sum(l => ((l.EndDate - l.StartDate).TotalDays + 1) * 8)
                    }).ToList();

                    
                    dest.Leaves = leaves;
                })
                .ForMember(hru => hru.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(hru => hru.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(hru => hru.Surname, opt => opt.MapFrom(src => src.SecondName))
                .ForMember(hru => hru.Rate, opt => opt.MapFrom(src => src.Rate))
                .ForMember(hru => hru.Projects, opt => opt.MapFrom(src => src.ProjectEmployees))
                .ForMember(hru => hru.NormHours, opt => opt.MapFrom((src, dist, res, context) =>
                {
                    var workingHoursInMonth = (int)context.Items["workingHoursInMonth"];
                    return workingHoursInMonth * src.Rate;
                }))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<int, HourReportUserBllModel>()
                .ForMember(hru => hru.NormHours, opt => opt.MapFrom((src, crr) => crr.Rate * src))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<ProjectEmployee, HourReportProjectBllModel>()
                .ForMember(hrp => hrp.ProjectId, opt => opt.MapFrom(src => src.ProjectId))
                .ForMember(hrp => hrp.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
                .ForMember(hrp => hrp.Hours, opt => opt.MapFrom(src => src.Tasks.Sum(t => t.TaskNotes.Sum(tn => tn.Hours))))
                .ForAllOtherMembers(opt => opt.Ignore());

            #endregion
        }

        private ICollection<Leave> GetLeavesWithoutWeekends(IEnumerable<Leave> leaves,
            IDictionary<DateTime, bool> workCalendar)
        {
            var result = new List<Leave>();
            foreach (var leave in leaves)
            {
                var currentDays = workCalendar.Where(wc => wc.Key >= leave.StartDate && wc.Key <= leave.EndDate);
                DateTime? tempStart = null;
                foreach (var item in currentDays)
                {
                    if (tempStart == null && item.Value)
                    {
                        tempStart = item.Key;
                        continue;
                    }

                    if (!item.Value && tempStart != null)
                    {
                        var newLeave = new Leave
                        {
                            TypeLeave = leave.TypeLeave,
                            EmployeeId = leave.EmployeeId,
                            StartDate = (DateTime)tempStart,
                            EndDate = item.Key.AddDays(-1)
                        };
                        result.Add(newLeave);
                        tempStart = null;
                    }
                }

                if (tempStart != null)
                {
                    result.Add(new Leave
                    {
                        TypeLeave = leave.TypeLeave,
                        EmployeeId = leave.EmployeeId,
                        StartDate = (DateTime)tempStart,
                        EndDate = currentDays.LastOrDefault().Key
                    });
                }
            }

            return result;
        }
    }
}
