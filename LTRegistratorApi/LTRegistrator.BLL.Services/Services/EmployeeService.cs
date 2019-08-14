using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.Domain.Entities;
using LTRegistrator.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public class EmployeeService : BaseService, IEmployeeService
    {
        private readonly UserManager<User> _userManager;
        private readonly HttpContext _httpContext;

        /// <summary>
        /// The method returns true if the user tries to change his data or he is a manager or administrator.
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns>Is it possible to change the data</returns>
        private async Task<bool> AccessAllowed(int id)
        {
            var thisUser = await _userManager.FindByIdAsync(
                _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)); //We are looking for an authorized user.
            var authorizedUser =
                await DbContext.Set<Employee>().SingleOrDefaultAsync(
                    e => e.Id ==
                         thisUser.EmployeeId); //We load Employee table.
            var maxRole = authorizedUser.MaxRole;


            return authorizedUser.Id == id ||
                   maxRole == RoleType.Manager ||
                   maxRole == RoleType.Administrator;
        }

        public EmployeeService(DbContext db, IMapper mapper, UserManager<User> userManager, HttpContext httpContext) : base(db, mapper)
        {
            _userManager = userManager;
            _httpContext = httpContext;
        }

        public async Task<Response<Employee>> GetByIdAsync(int id)
        {
            if (!this.AccessAllowed(id).Result)
            {
                return new Response<Employee>(HttpStatusCode.BadRequest, "You have not enough permissions to change data");
            }
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
            if (!this.AccessAllowed(userId).Result)
            {
                return new Response<Employee>(HttpStatusCode.BadRequest, "You have not enough permissions to change data");
            }
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
            if (!this.AccessAllowed(userId).Result)
            {
                return new Response<Employee>(HttpStatusCode.BadRequest, "You have not enough permissions to change data");
            }

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
            if (!this.AccessAllowed(userId).Result)
            {
                return new Response<Employee>(HttpStatusCode.BadRequest, "You have not enough permissions to change data");
            }
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
