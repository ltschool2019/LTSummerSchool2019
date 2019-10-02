using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Contracts.Exceptions;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public class TaskService : BaseService, ITaskService
    {
        public TaskService(DbContext db, IMapper mapper) : base(db, mapper)
        {
        }

        public async System.Threading.Tasks.Task<Task> GetByIdAsync(int authUserId, int taskId)
        {
            var task = await DbContext.Set<Task>()
                .Include(t => t.CustomValues)
                .Include(t => t.TaskNotes)
                .Include(t => t.ProjectEmployee).ThenInclude(pe => pe.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            var role = await GetRole(authUserId);
            if (task == null || task.ProjectEmployee.Project.SoftDeleted && role != RoleType.Administrator)
            {
                throw new NotFoundException("Task was not found");
            }

            if (task.ProjectEmployee.EmployeeId != authUserId)
            {
                if (role == RoleType.Employee) throw new ForbiddenException("Access denied");
            }

            return task;
        }

        public async System.Threading.Tasks.Task AddAsync(Task task)
        {
            var entity = await DbContext.Set<Task>().FirstOrDefaultAsync(t =>
                t.ProjectId == task.ProjectId && t.EmployeeId == task.EmployeeId && t.Name == task.Name);
            
            if (entity != null)
            {
                throw new ConflictException("Task already exist");
            }

            DbContext.Set<Task>().Add(task);
            await DbContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task UpdateAsync(Task task)
        {
            var entity = await DbContext.Set<Task>().Include(t => t.CustomValues).FirstOrDefaultAsync(t => t.Id == task.Id);
            if (entity == null)
            {
                throw new NotFoundException("Task was not found");
            }

            entity.Name = string.IsNullOrWhiteSpace(task.Name) ? entity.Name : task.Name;
            var unusedCustomValues = entity.CustomValues.Where(cv => !task.CustomValues.Select(tcv => tcv.Id).Contains(cv.Id));
            DbContext.Set<CustomValue>().RemoveRange(unusedCustomValues);
            foreach (var customValue in task.CustomValues)
            {
                var current = entity.CustomValues.FirstOrDefault(cv => cv.Id == customValue.Id);
                if (current == null)
                {
                    DbContext.Set<CustomValue>().Add(customValue);
                }
                else
                {
                    current.Type = customValue.Type;
                    current.Value = customValue.Value;
                    DbContext.Set<CustomValue>().Update(current);
                }
            }

            await DbContext.SaveChangesAsync();
        }
    }
}
