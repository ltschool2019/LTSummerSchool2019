using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public class LeaveService : BaseService, ILeaveService
    {
        public LeaveService(DbContext db, IMapper mapper) : base(db, mapper)
        {
        }

        public async Task<ICollection<Leave>> GetLeavesByEmployeeIdAsync(int employeeId, DateTime startDate, DateTime endDate)
        {
            var leaves = await DbContext.Set<Leave>()
                .Where(l => l.EmployeeId == employeeId &&
                            (l.StartDate >= startDate && l.StartDate <= endDate ||
                             l.EndDate >= startDate && l.EndDate <= endDate))
                .ToListAsync();

            return leaves;
        }
    }
}
