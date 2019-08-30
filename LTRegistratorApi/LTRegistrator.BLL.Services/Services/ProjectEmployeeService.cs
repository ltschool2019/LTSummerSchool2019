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
    public class ProjectEmployeeService : BaseService, IProjectEmployeeService
    {
        public ProjectEmployeeService(DbContext db, IMapper mapper) : base(db, mapper)
        {
        }
        public async Task<Response<ProjectEmployee>> GetEmployeeIdAndProjectIdAsync(int employeeId, int projectId)
        {
            var employeeProject = await DbContext.Set<ProjectEmployee>().FirstOrDefaultAsync(pe => pe.ProjectId == projectId && pe.EmployeeId == employeeId);
            if (employeeProject == null)
            {
                return new Response<ProjectEmployee>(HttpStatusCode.NotFound, "EmployeeId and ProjectId not found");
            }
            return new Response<ProjectEmployee>(employeeProject);
        }
    }
}