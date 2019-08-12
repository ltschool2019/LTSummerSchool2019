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

        public async Task<HourReportBllModel> GetHourReportAsync(int managerId)
        {
            var projects = await DbContext.Set<Project>().ToListAsync();
            var users = await DbContext.Set<Employee>().Where(e => e.ManagerId == managerId)
                .Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Tasks).ThenInclude(t => t.TaskNotes)
                .ThenInclude(tn => tn.Task).ThenInclude(t => t.ProjectEmployee).ThenInclude(pe => pe.Project)
                .ToListAsync();

            var report = Mapper.Map<HourReportBllModel>(projects);
            Mapper.Map(users, report);

            return report;
        }
    }
}
