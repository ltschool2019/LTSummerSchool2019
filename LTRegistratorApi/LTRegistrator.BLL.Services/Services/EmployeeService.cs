using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class EmployeeService : BaseService, IEmployeeService
    {
        public EmployeeService(DbContext db, IMapper mapper) : base(db, mapper) { }

        public async Task<Employee[]> GetAllAsync(int authEmployeeId)
        {
            var role = await GetRole(authEmployeeId);
            if (role == RoleType.Employee)
            {
                throw new ForbiddenException("Access denied");
            }

            var employees = DbContext.Set<Employee>();
            if (role == RoleType.Manager)
            {
                return await employees.Where(e => e.ManagerId == authEmployeeId).ToArrayAsync();
            }

            return await employees.ToArrayAsync();
        }

        public async Task<Employee[]> GetByProjectIdAsync(int authUserId, int projectId)
        {
            var project = await DbContext.Set<Project>().FirstOrDefaultAsync(p => p.Id == projectId);
            var role = await GetRole(authUserId);
            if (project == null || project.SoftDeleted && role != RoleType.Administrator)
            {
                throw new NotFoundException("Project was not found");
            }

            var employees = await DbContext.Set<ProjectEmployee>().Where(pe => pe.ProjectId == projectId)
                .Select(pe => pe.Employee)
                .Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Project)
                .ToArrayAsync();

            return employees.Select(e =>
            {
                e.ProjectEmployees = e.ProjectEmployees.Where(pe => !pe.Project.SoftDeleted).ToList();
                return e;
            }).ToArray();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            var currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var employee = await DbContext.Set<Employee>()
                .Include(e => e.Manager)
                .Include(e => e.Leaves)
                .Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Project)
                .Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Tasks).ThenInclude(t => t.TaskNotes)
                .SingleOrDefaultAsync(e => e.Id == id);
            if (employee == null)
            {
                throw new NotFoundException("Employee was not found");
            }

            employee.ProjectEmployees = employee.ProjectEmployees.Where(pe => !pe.Project.SoftDeleted).ToList();
            foreach (var projectEmployee in employee.ProjectEmployees)
            {
                foreach (var task in projectEmployee.Tasks)
                {
                    task.TaskNotes = task.TaskNotes.Where(tn => tn.Day >= currentMonth && tn.Day < currentMonth.AddMonths(1)).ToList();
                }
            }

            return employee;
        }

        public async Task<Leave> AddLeaveAsync(int userId, Leave leave)
        {
            var employee = await DbContext.Set<Employee>().Include(e => e.Leaves).FirstOrDefaultAsync(e => e.Id == userId);
            if (employee == null)
            {
                throw new NotFoundException("Employee was not found");
            }

            if (LeavesValidator.TryMergeLeaves(employee.Leaves.ToList(), new[] { leave }))
            {
                leave.EmployeeId = employee.Id;
                DbContext.Set<Leave>().Add(leave);
                await DbContext.SaveChangesAsync();
                return leave;
            }

            throw new BadRequestException("Transferred leave is not correct");
        }

        public async Task<Leave> UpdateLeaveAsync(int userId, Leave leave)
        {
            var employee = await DbContext.Set<Employee>().SingleOrDefaultAsync(e => e.Id == userId);
            if (employee == null)
            {
                throw new NotFoundException("Employee was not found");
            }

            var entity = await DbContext.Set<Leave>().FirstOrDefaultAsync(l => l.Id == leave.Id);
            if (entity == null)
            {
                throw new NotFoundException("Leave was not found");
            }

            if (entity.EmployeeId != userId)
            {
                throw new ForbiddenException("Access denied");
            }

            Mapper.Map(leave, entity);

            if (!LeavesValidator.ValidateLeaves(employee.Leaves.ToList()))
            {
                throw new BadRequestException("Transferred leave is not correct");
            }

            await DbContext.SaveChangesAsync();

            return leave;
        }

        public async Task DeleteLeaveAsync(int userId, int leaveId)
        {
            var employee = await DbContext.Set<Employee>().SingleOrDefaultAsync(e => e.Id == userId);
            if (employee == null)
            {
                throw new NotFoundException("Employee was not found");
            }

            var leave = await DbContext.Set<Leave>().FirstOrDefaultAsync(l => l.Id == leaveId);
            if (leave == null)
            {
                throw new NotFoundException("Leave was not found");
            }

            if (leave.EmployeeId != userId)
            {
                throw new ForbiddenException("You cannot change another employee’s leave");
            }

            DbContext.Set<Leave>().Remove(leave);

            await DbContext.SaveChangesAsync();
        }
    }
}
