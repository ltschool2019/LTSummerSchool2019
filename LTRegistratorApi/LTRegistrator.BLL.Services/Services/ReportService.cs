using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Contracts.Models;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public class ReportService : BaseService, IReportService
    {
        private readonly IWorkCalendarRepository _calendarRepository;
        private const int HoursInWorkingDay = 8;

        public ReportService(DbContext db, IMapper mapper, IWorkCalendarRepository calendarRepository) : base(db, mapper)
        {
            _calendarRepository = calendarRepository;
        }

        public async Task<HourReportBllModel> GetMonthlyReportAsync(int managerId, DateTime date)
        {
            date = new DateTime(date.Year, date.Month, 1);
            
            var workMonth = await _calendarRepository.GetWorkCalendarByMonth(date);
            var countWorkingDays = workMonth.Count(wm => wm.Value);

            var employees = (await DbContext.Set<Employee>()
                    .Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Tasks).ThenInclude(t => t.TaskNotes)
                    .Include(e => e.Leaves)
                    .Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Project)
                    .Include(e => e.Manager).ThenInclude(m => m.ProjectEmployees).ThenInclude(pe => pe.Project)
                    .Where(e => e.ManagerId == managerId)
                    .ToArrayAsync())
                .Select(e =>
                {
                    e.ProjectEmployees = e.ProjectEmployees.Where(pe => !pe.Project.SoftDeleted).Select(pe =>
                    {
                        pe.Tasks = pe.Tasks.Select(t =>
                        {
                            t.TaskNotes = t.TaskNotes.Where(tn => tn.Day >= date && tn.Day < date.AddMonths(1)).ToList();
                            return t;
                        }).ToList();
                        return pe;
                    }).ToList();
                    e.Leaves = e.Leaves.Where(
                            l => l.StartDate >= date && l.StartDate < date.AddMonths(1)
                                 || l.EndDate >= date && l.EndDate < date.AddMonths(1)
                                 || l.StartDate < date && l.EndDate > date.AddMonths(1))
                        .ToList();
                    return e;
                });

            var workingHoursInMonth = countWorkingDays * HoursInWorkingDay;
            //в качестве опции передается кол-во рабочих часов в месяце для вычисления нормы часов для каждого пользователя
            var report = Mapper.Map<HourReportBllModel>(employees, opt =>
            {
                opt.Items[nameof(workMonth)] = workMonth;
                opt.Items[nameof(workingHoursInMonth)] = workingHoursInMonth;
            });

            return report;
        }
    }
}
