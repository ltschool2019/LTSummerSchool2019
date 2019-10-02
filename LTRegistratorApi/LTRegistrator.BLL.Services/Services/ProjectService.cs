using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Contracts.Exceptions;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public class ProjectService: BaseService, IProjectService
    {
        public ProjectService(DbContext db, IMapper mapper) : base(db, mapper)
        {
        }

        public async Task<Project> GetByIdAsync(int authUserId, int projectId)
        {
            var role = await GetRole(authUserId);
            var project = await DbContext.Set<Project>()
                .Include(p => p.CustomFieldProjects).ThenInclude(cf => cf.CustomField).ThenInclude(cf => cf.CustomFieldOptions)
                .Include(p => p.ProjectEmployees)
                .FirstOrDefaultAsync(p => p.Id == projectId && (role == RoleType.Administrator || !p.SoftDeleted));
            
            if (project == null)
            {
                throw new NotFoundException("Project was not found");
            }

            if (role == RoleType.Employee && !project.ProjectEmployees.Select(pe => pe.EmployeeId).Contains(authUserId))
            {
                throw new ForbiddenException("Access denied");
            }

            return project;
        }

        public async Task<Project> AddAsync(int authUserId, Project project)
        {
            var role = await GetRole(authUserId);
            if (role == RoleType.Employee)
            {
                throw new ForbiddenException("Access denied");
            }

            var entity = await DbContext.Set<Project>()
                .FirstOrDefaultAsync(p => p.Name == project.Name && !p.SoftDeleted);
            if (entity != null)
            {
                throw new ConflictException($"Project with name {project.Name} already exist");
            }

            if (role == RoleType.Manager)
            {
                project.ProjectEmployees = new List<ProjectEmployee>
                {
                    new ProjectEmployee
                    {
                        EmployeeId = authUserId
                    }
                };
            }

            DbContext.Set<Project>().Add(project);
            await DbContext.SaveChangesAsync();

            return project;
        }
    }
}
