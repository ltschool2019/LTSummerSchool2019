using System;
using System.Collections.Generic;
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
    }
}
