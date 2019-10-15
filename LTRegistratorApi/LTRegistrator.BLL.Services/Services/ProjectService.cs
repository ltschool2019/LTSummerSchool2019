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
using Task = System.Threading.Tasks.Task;

namespace LTRegistrator.BLL.Services.Services
{
    public class ProjectService : BaseService, IProjectService
    {
        public ProjectService(DbContext db, IMapper mapper) : base(db, mapper)
        {
        }

        public async Task<Project[]> GetProjects(int authUserId)
        {
            var role = await GetRole(authUserId);
            if (role == RoleType.Administrator)
            {
                return await DbContext.Set<Project>().Include(p => p.ProjectEmployees).ToArrayAsync();
            }

            return await DbContext.Set<ProjectEmployee>()
                .Include(pe => pe.Project)
                .Where(pe => pe.EmployeeId == authUserId && !pe.Project.SoftDeleted)
                .Select(pe => pe.Project).ToArrayAsync();
        }

        public async Task<Project> GetByIdAsync(int authUserId, int projectId)
        {
            var role = await GetRole(authUserId);
            var project = await DbContext.Set<Project>()
                .Include(p => p.CustomFieldProjects).ThenInclude(cf => cf.CustomField).ThenInclude(cf => cf.CustomFieldOptions)
                .Include(p => p.ProjectEmployees).ThenInclude(pe => pe.Employee)
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

        public async Task UpdateAsync(int authEmployeeId, Project project)
        {
            var role = await GetRole(authEmployeeId);
            if (role == RoleType.Employee)
            {
                throw new ForbiddenException("Access denied");
            }

            var entity = await DbContext.Set<Project>()
                .Include(p => p.ProjectEmployees)
                .Include(p => p.CustomFieldProjects).ThenInclude(cfp => cfp.CustomField)
                .FirstOrDefaultAsync(p => p.Id == project.Id);

            if (entity == null || role == RoleType.Manager && entity.SoftDeleted)
            {
                throw new NotFoundException("Project was not found");
            }

            entity.Name = project.Name;
            var unusedCustomFields = entity.CustomFieldProjects
                .Where(cfp => !project.CustomFieldProjects.Select(fp => fp.CustomFieldId).Contains(cfp.CustomFieldId));
            var customValues = await DbContext.Set<CustomValue>()
                .Where(cv => unusedCustomFields.Select(ucf => ucf.CustomFieldId).Contains(cv.CustomFieldId))
                .ToArrayAsync();
            DbContext.Set<CustomValue>().RemoveRange(customValues);
            DbContext.Set<CustomFieldProject>().RemoveRange(unusedCustomFields);
            foreach (var customFieldProject in project.CustomFieldProjects)
            {
                var current = entity.CustomFieldProjects.FirstOrDefault(cfp => cfp.CustomFieldId == customFieldProject.CustomFieldId);
                if (current == null)
                {
                    customFieldProject.ProjectId = project.Id;
                    DbContext.Set<CustomFieldProject>().Add(customFieldProject);
                }
                else
                {
                    current.CustomField.DefaultValue = customFieldProject.CustomField.DefaultValue;
                    current.CustomField.Description = customFieldProject.CustomField.Description;
                    current.CustomField.IsRequired = customFieldProject.CustomField.IsRequired;
                    current.CustomField.MaxLength = customFieldProject.CustomField.MaxLength;
                    current.CustomField.Name = customFieldProject.CustomField.Name;
                    if (current.CustomField.Type != customFieldProject.CustomField.Type)
                    {
                        DbContext.Set<CustomValue>().RemoveRange(DbContext.Set<CustomValue>().Where(cv => cv.CustomFieldId == current.CustomFieldId));
                        current.CustomField.Type = customFieldProject.CustomField.Type;
                    }
                    DbContext.Set<CustomFieldProject>().Update(current);
                }
            }

            await DbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(int authUserId, int projectId)
        {
            var role = await GetRole(authUserId);
            if (role == RoleType.Employee)
            {
                throw new ForbiddenException("Access denied");
            }

            var project = await DbContext.Set<Project>().FirstOrDefaultAsync(p => p.Id == projectId);
            if (project == null || project.SoftDeleted && role == RoleType.Manager)
            {
                throw new NotFoundException("Project was not found");
            }

            if (role == RoleType.Administrator)
            {
                DbContext.Set<Project>().Remove(project);
            }
            else
            {
                project.SoftDeleted = true;
            }

            await DbContext.SaveChangesAsync();
        }
    }
}
