using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public class LeaveService : BaseService, ILeaveService
    {
        private readonly HttpContext _httpContext;
        public LeaveService(DbContext db, IMapper mapper, HttpContext httpContext) : base(db, mapper)
        {
            _httpContext = httpContext;
        }
        public async Task<Response<List<Leave>>> GetLeavesByEmployeeIdAsync(int employeeId, DateTime startDate, DateTime endDate)
        {
            var leaves = await DbContext.Set<Leave>()
                .Where(l => l.EmployeeId == employeeId &&
                            (l.StartDate >= startDate && l.StartDate <= endDate ||
                             l.EndDate >= startDate && l.EndDate <= endDate))
                .ToListAsync();
            if (leaves == null)
            {
                return new Response<List<Leave>>(HttpStatusCode.NotFound, "Leaves not found");
            }
            return new Response<List<Leave>>(leaves);
        }
    }
}
