using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Contracts.Exceptions;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public class EmployeeService : BaseService, IEmployeeService
    {
        public EmployeeService(DbContext db, IMapper mapper) : base(db, mapper) { }

        public async Task<IEnumerable<Employee>> GetAllAsync(int authEmployeeId)
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

        public async Task<Response<Employee>> GetByIdAsync(int id)
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
                return new Response<Employee>(HttpStatusCode.NotFound, $"Employee with id = {id} not found");
            }

            employee.ProjectEmployees = employee.ProjectEmployees.Where(pe => !pe.Project.SoftDeleted).ToList();
            foreach (var projectEmployee in employee.ProjectEmployees)
            {
                foreach (var task in projectEmployee.Tasks)
                {
                    task.TaskNotes = task.TaskNotes.Where(tn => tn.Day >= currentMonth && tn.Day < currentMonth.AddMonths(1)).ToList();
                }
            }

            return new Response<Employee>(employee);
        }

        public async Task<Response<Employee>> AddLeavesAsync(int userId, ICollection<Leave> leaves)
        {
            var employee = await DbContext.Set<Employee>().Include(e => e.Leaves).FirstOrDefaultAsync(e => e.Id == userId);
            if (employee == null)
            {
                return new Response<Employee>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }

            if (LeavesValidator.TryMergeLeaves(employee.Leaves.ToList(), leaves))
            {
                employee.Leaves = employee.Leaves.Concat(leaves).ToList();
                await DbContext.SaveChangesAsync();

                return new Response<Employee>(employee);
            }

            return new Response<Employee>(HttpStatusCode.BadRequest, "Transferred leave is not correct");
        }

        public async Task<Response<Employee>> UpdateLeavesAsync(int userId, ICollection<Leave> leaves)
        {
            var employee = await DbContext.Set<Employee>().Include(e => e.Leaves).SingleOrDefaultAsync(e => e.Id == userId);
            if (employee == null)
            {
                return new Response<Employee>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }

            try
            {
                foreach (Leave leave in leaves)
                {
                    leave.EmployeeId = employee.Id;
                    leave.Employee = employee;
                    var currentLeave = employee.Leaves.SingleOrDefault(l => l.Id == leave.Id);
                    if (currentLeave == null)
                    {
                        return new Response<Employee>(HttpStatusCode.NotFound, $"Leave with id = {leave.Id} not found");
                    }
                    Mapper.Map(leave, currentLeave);
                    DbContext.Entry(currentLeave).State = EntityState.Modified;
                    DbContext.Set<Leave>().Update(currentLeave);
                }

                if (!LeavesValidator.ValidateLeaves(employee.Leaves.ToList()))
                {
                    return new Response<Employee>(HttpStatusCode.BadRequest, "Transferred leave is not correct");
                }

                await DbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new Response<Employee>(HttpStatusCode.InternalServerError, "Internal server error");
            }

            return new Response<Employee>(Mapper.Map<Employee>(employee));
        }

        public async Task<Response<Employee>> DeleteLeavesAsync(int userId, ICollection<int> leaveIds)
        {

            if (!leaveIds.Any())
            {
                return new Response<Employee>(HttpStatusCode.BadRequest, "Leave ids not transferred");
            }

            var employee = await DbContext.Set<Employee>().Include(e => e.Leaves).SingleOrDefaultAsync(e => e.Id == userId);
            if (employee == null)
            {
                return new Response<Employee>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }

            var leaves = await DbContext.Set<Leave>().Where(l => leaveIds.Contains(l.Id)).ToListAsync();
            if (leaves.Count != leaveIds.Count)
            {
                return new Response<Employee>(HttpStatusCode.BadRequest, "transmitted id's are not correct");
            }

            if (leaves.Any(l => l.EmployeeId != userId))
            {
                return new Response<Employee>(HttpStatusCode.Forbidden, "You cannot change another employee’s leave");
            }

            foreach (var leave in leaves)
            {
                DbContext.Set<Leave>().Remove(leave);
            }
            await DbContext.SaveChangesAsync();
            return new Response<Employee>(employee);
        }
    }
}
