using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LTRegistrator.BLL.Contracts.Contracts;
using LTRegistrator.BLL.Contracts.Dtos;
using LTRegistrator.DAL.Contracts;
using LTRegistrator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LTRegistrator.BLL.Services.Services
{
    public class EmployeeService : BaseService, IEmployeeService
    {
        public EmployeeService(IUnitOfWork uow, IMapper mapper) : base(uow, mapper)
        {
        }

        public async Task<Response<EmployeeDto>> GetByIdAsync(Guid id)
        {
            var employee = await UnitOfWork.GetRepository<Employee>().AllIncluding(e => e.Manager, e => e.Leaves).Include(e => e.ProjectEmployees).ThenInclude(pe => pe.Project).SingleOrDefaultAsync(e => e.Id == id);
            return employee == null
                ? new Response<EmployeeDto>(HttpStatusCode.NotFound, $"Employee with id = {id} not found")
                : new Response<EmployeeDto>(Mapper.Map<EmployeeDto>(employee));
        }

        public async Task<Response<EmployeeDto>> AddLeavesAsync(Guid userId, ICollection<LeaveDto> leaves)
        {
            var employee = await UnitOfWork.GetRepository<Employee>().AllIncluding(e => e.Leaves).FirstOrDefaultAsync(e => e.Id == userId);
            if (employee == null)
            {
                return new Response<EmployeeDto>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }
            var newLeaves = Mapper.Map<ICollection<Leave>>(leaves).ToList();

            if (LeavesValidator.TryMergeLeaves(employee.Leaves.ToList(), newLeaves))
            {
                employee.Leaves = employee.Leaves.Concat(newLeaves).ToList();
                await UnitOfWork.CommitAsync();

                return new Response<EmployeeDto>(Mapper.Map<EmployeeDto>(employee));
            }

            return new Response<EmployeeDto>(HttpStatusCode.BadRequest, "");
        }

        public async Task<Response<EmployeeDto>> UpdateLeavesAsync(Guid userId, ICollection<LeaveDto> leaves)
        {
            var employee = await UnitOfWork.GetRepository<Employee>().AllIncluding(e => e.Leaves).SingleOrDefaultAsync(e => e.Id == userId);
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
                    UnitOfWork.GetRepository<Leave>().Update(currentLeave);
                }

                if (!LeavesValidator.ValidateLeaves(employee.Leaves.ToList()))
                {
                    return new Response<EmployeeDto>(HttpStatusCode.BadRequest, "Transferred leave is not correct");
                }

                await UnitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                return new Response<EmployeeDto>(HttpStatusCode.InternalServerError, "Internal server error");
            }

            return new Response<EmployeeDto>(Mapper.Map<EmployeeDto>(employee));
        }

        public async Task<Response<EmployeeDto>> DeleteLeavesAsync(Guid userId, ICollection<Guid> leaveIds)
        {
            if (!leaveIds.Any())
            {
                return new Response<EmployeeDto>(HttpStatusCode.BadRequest, "Leave ids not transferred");
            }

            var employee = await UnitOfWork.GetRepository<Employee>().AllIncluding(e => e.Leaves).SingleOrDefaultAsync(e => e.Id == userId);
            if (employee == null)
            {
                return new Response<EmployeeDto>(HttpStatusCode.NotFound, $"Employee with id = {userId} not found");
            }

            var leaves = employee.Leaves.Where(l => leaveIds.Contains(l.Id)).ToList();
            if (leaves.Count != leaveIds.Count)
            {
                return new Response<EmployeeDto>(HttpStatusCode.Forbidden, "You cannot change another employee’s leave");
            }

            foreach (var id in leaves.Select(l => l.Id))
            {
                UnitOfWork.GetRepository<Leave>().Remove(id);
            }
            await UnitOfWork.CommitAsync();
            return new Response<EmployeeDto>(Mapper.Map<EmployeeDto>(employee));
        }
    }
}
