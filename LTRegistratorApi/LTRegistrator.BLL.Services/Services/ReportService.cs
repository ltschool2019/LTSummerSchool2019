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
    public class ReportService: BaseService, IReportService
    {
        public ReportService(DbContext db, IMapper mapper) : base(db, mapper)
        {
        }

        public async Task<HourReportBllModel> GetMonthlyReportAsync(int managerId, DateTime date)
        {
            date = new DateTime(date.Year, date.Month, 1);
            var projects = await DbContext.Set<Project>().ToListAsync();
            var taskNotes = await DbContext.Set<TaskNote>()
                .Where(t => t.Day >= date && t.Day < date.AddMonths(1) && t.Task.ProjectEmployee.Employee.ManagerId == managerId)
                .Include(tn => tn.Task).ThenInclude(t => t.ProjectEmployee).ThenInclude(pe => pe.Employee)
                .Include(tn => tn.Task).ThenInclude(t => t.ProjectEmployee).ThenInclude(pe => pe.Project)
                .ToListAsync();

            var report = Mapper.Map<HourReportBllModel>(projects);
            Mapper.Map(taskNotes.GroupBy(t => t.Task.ProjectEmployee.Employee).Select(g => g.Key).ToList(), report);

            return report;
        }
    }
}
