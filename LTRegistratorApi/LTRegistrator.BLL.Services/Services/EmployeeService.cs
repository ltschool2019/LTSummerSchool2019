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
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public class EmployeeService : BaseService, IEmployeeService
    {
        public EmployeeService(DbContext db, IMapper mapper) : base(db, mapper)
        {
        }

        public async Task<Response<Employee>> GetByIdAsync(int id)
        {
            var employee = await DbContext.Set<Employee>().Include(e => e.Manager).Include(e => e.Leaves).Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Project).SingleOrDefaultAsync(e => e.Id == id);
            return employee == null
                ? new Response<Employee>(HttpStatusCode.NotFound, $"Employee with id = {id} not found")
                : new Response<Employee>(employee);
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
                foreach (var leave in leaves)
                {
                    var currentLeave = employee.Leaves.SingleOrDefault(l => l.Id == leave.Id);
                    if (currentLeave == null)
                    {
                        return new Response<Employee>(HttpStatusCode.NotFound, $"Leave with id = {leave.Id} not found");
                    }

                    Mapper.Map(leave, currentLeave);
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
