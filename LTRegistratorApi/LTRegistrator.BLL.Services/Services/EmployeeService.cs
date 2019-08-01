using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Contracts.Dtos;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public class EmployeeService : BaseService, IEmployeeService
    {
        public EmployeeService(DbContext db, IMapper mapper) : base(db, mapper)
        {
        }

        public async Task<Response<EmployeeDto>> GetByIdAsync(int id)
        {
            var employee = await DbContext.Set<Employee>().Include(e => e.Manager).Include(e => e.Leaves).Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Project).SingleOrDefaultAsync(e => e.Id == id);
            return employee == null
                ? new Response<EmployeeDto>(HttpStatusCode.NotFound, $"Employee with id = {id} not found")
                : new Response<EmployeeDto>(Mapper.Map<EmployeeDto>(employee));
        }

        public async Task<Response<EmployeeDto>> AddLeavesAsync(int userId, ICollection<LeaveDto> leaves)
        {
            var employee = await DbContext.Set<Employee>().Include(e => e.Leaves).FirstOrDefaultAsync(e => e.Id == userId);
            if (employee == null)
            {
                return new Response<EmployeeDto>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }
            var newLeaves = Mapper.Map<ICollection<Leave>>(leaves).ToList();

            if (LeavesValidator.TryMergeLeaves(employee.Leaves.ToList(), newLeaves))
            {
                employee.Leaves = employee.Leaves.Concat(newLeaves).ToList();
                await DbContext.SaveChangesAsync();

                return new Response<EmployeeDto>(Mapper.Map<EmployeeDto>(employee));
            }

            return new Response<EmployeeDto>(HttpStatusCode.BadRequest, "Transferred leave is not correct");
        }

        public async Task<Response<EmployeeDto>> UpdateLeavesAsync(int userId, ICollection<LeaveDto> leaves)
        {
            var employee = await DbContext.Set<Employee>().Include(e => e.Leaves).SingleOrDefaultAsync(e => e.Id == userId);
            if (employee == null)
            {
                return new Response<EmployeeDto>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }

            try
            {
                foreach (var leave in leaves)
                {
                    var currentLeave = employee.Leaves.SingleOrDefault(l => l.Id == leave.Id);
                    if (currentLeave == null)
                    {
                        return new Response<EmployeeDto>(HttpStatusCode.NotFound, $"Leave with id = {leave.Id} not found");
                    }

                    Mapper.Map(leave, currentLeave);
                    DbContext.Set<Leave>().Update(currentLeave);
                }

                if (!LeavesValidator.ValidateLeaves(employee.Leaves.ToList()))
                {
                    return new Response<EmployeeDto>(HttpStatusCode.BadRequest, "Transferred leave is not correct");
                }

                await DbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return new Response<EmployeeDto>(HttpStatusCode.InternalServerError, "Internal server error");
            }

            return new Response<EmployeeDto>(Mapper.Map<EmployeeDto>(employee));
        }

        public async Task<Response<EmployeeDto>> DeleteLeavesAsync(int userId, ICollection<int> leaveIds)
        {
            if (!leaveIds.Any())
            {
                return new Response<EmployeeDto>(HttpStatusCode.BadRequest, "Leave ids not transferred");
            }

            var employee = await DbContext.Set<Employee>().Include(e => e.Leaves).SingleOrDefaultAsync(e => e.Id == userId);
            if (employee == null)
            {
                return new Response<EmployeeDto>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }

            var leaves = employee.Leaves.Where(l => leaveIds.Contains(l.Id)).ToList();
            if (leaves.Count != leaveIds.Count)
            {
                return new Response<EmployeeDto>(HttpStatusCode.Forbidden, "You cannot change another employee’s leave");
            }

            foreach (var leave in leaves)
            {
                DbContext.Set<Leave>().Remove(leave);
            }
            await DbContext.SaveChangesAsync();
            return new Response<EmployeeDto>(Mapper.Map<EmployeeDto>(employee));
        }
    }
}
